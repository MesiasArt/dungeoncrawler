# Prueba independiente: Codebound pseudo-3D dibujado

Escena: `Assets/MapTests/Codebound_RetroStyle_Test.unity`. Todos los recursos nuevos están en `Assets/MapTests/RetroStyle`. El juego principal no utiliza este renderizador.

## Abrir y recorrer
Abrir la escena de prueba desde el Project o `Editor → Pruebas → Pseudo-3D dibujado`, después Play. W/S o flechas arriba/abajo: una casilla; A/D o flechas izquierda/derecha: giro90°; E: abrir/cerrar puerta enfrente. Mando: cruceta y botón A. TAB cambia A/C. El plano pequeño es exclusivo de prueba y muestra toda su geometría.

Seleccionar `EXPERIMENTO — seleccionar para ajustar` en Hierarchy: Inspector en español con colores, contornos, resolución256×192 o426×240, profundidad, contraste y transición. Cero segundos = cambio instantáneo; valores mayores producen un fundido breve de piezas2D. Mantener anchos de perspectiva positivos y decrecientes.

## Aislamiento y corrección del arranque
El launcher original tiene un callback global: al cargar cualquier escena sin DungeonPrototype, crea el juego completo. La primera prueba reveló que también lo creaba en esta escena, mezclando geometrías y desactivando cámaras experimentales. Ese fallo de aislamiento se corrigió antes de entregar.

La escena contiene ahora un DungeonPrototype **desactivado y oculto**, que sirve únicamente como marcador para impedir ese autoarranque. Su Start/Update/OnGUI no se ejecutan, no crea mapas, cámaras ni combate. Las pruebas verifican que no exista ningún controlador de juego activo ni cámara/geométría generada del juego antiguo. No se modificó el launcher ni ningún script original.

Al abrir la escena principal este marcador deja de existir. Git no registra diferencias en escenas originales, mapas, Resources, Scripts del juego, Packages o ProjectSettings. El arranque original se comprobó con GamerBoy LV1 y37/9/7/10. Además, sus871 paredes coinciden con las871 casillas de pared del mapa, sin paredes generadas sobre casillas transitables y con cero renderizadores experimentales. MainSceneVerification.json conserva esa comprobación. No se tocaron guardados, IA, XP, equipo, Director, JUMP ni OVERCLOCK. Al terminar se deja abierta SampleScene, para evitar confusión entre las escenas.

## Arquitectura
- `RetroDungeonExperiment.cs`: pose, controles, puertas experimentales, presentación Canvas, selector y resolución.
- `RetroPerspectiveCanvas.cs`: composición2D desde la cuadrícula. Consulta suelo mediante el DungeonMapSettings existente, varias columnas y10 profundidades; proyecta polígonos con anchos de perspectiva dibujados, contornos de ladrillos y bandas de color fijas. No hay mallas3D, iluminación, reflejos, normal maps ni filtro3D en modo C.
- Buffer de profundidad y recorte cercano: oculta superficies y sprites detrás de paredes/puertas. Las piezas comparten bordes. Composición actualizada al cambiar pose, puerta o parámetros; buffers y textura se reutilizan en reposo.
- Map.asset/Objects.asset: mismo formato de mapas/entidades que Codebound, pero assets nuevos24×24. Incluye corredor, esquina, cruce, habitación pequeña, amplia y puerta.
- El controlador completo de exploración original contiene combate, narrativa y cámaras; no se ejecuta. La prueba usa IsFloor y las convenciones de casillas/orientación originales con controles mínimos locales. No introduce otro formato de mapa ni sistemas de combate/guardado.
- Copias recortadas de Tapaku y cofre, Point, sin mipmaps ni compresión; no se modificaron sus PNG/PSD originales. No tienen combate ni IA.

## Modos disponibles
C: pseudo-3D dibujado, principal experimento.
A: copia independiente de geometría de prueba con materiales que reutilizan en lectura los shaders/texturas actuales. Misma pose, orientación y mapa que C; no es una captura del nivel original ni incluye su narrativa, enemigos o reflejos dinámicos. La cámara de referencia se presenta mediante RenderTexture independiente.
B no existe: el encargo anterior se canceló antes de crearlo. No se implementó silenciosamente un tercer estilo.

Las capturas A/C de pasillo, esquina, intersección, habitación amplia y puerta abierta/cerrada están en `Captures`. A utiliza resolución3× la interna de C para representar la variante3D habitual con más detalle; por tanto la comparación de rendimiento no iguala la cantidad de píxeles renderizados.

## Limitaciones
Perspectiva por piezas, discretizada a casillas y cuatro orientaciones; no permite cámara libre. Su horizonte visual termina en la distancia configurada; por debajo de10 puede ocultar el fondo de pasillos largos. Las habitaciones anchas se rasterizan en varias columnas y respetan profundidad, pero las superficies muy lejanas se simplifican a tonos planos para evitar ruido y las paredes lejanas pueden ocupar pocos píxeles. Son decisiones artísticas de esta prueba, no restricciones añadidas a los mapas del juego.

No hay texturas nuevas pesadas para paredes: los patrones se dibujan directamente en el buffer. No se crearon shaders nuevos; A usa materiales copiados y shaders existentes en lectura. El cambio de resolución recrea buffers una vez; la transición opcional copia la composición solo al cambiarla.

## Pruebas Unity MCP
26PASS/0FAIL, después de corregir el aislamiento: apertura, bloqueo del autoarranque original, movimiento, retroceso, giros90, paredes, puertas, cambio visual de estado, crecimiento de sprite con distancia, ocultación detrás de pared/puerta, cofre visible, Point, sin mipmaps, composición en reposo sin regeneración y seis vistas sin huecos. `McpResults.txt` conserva los resultados. Consola final sin errores.

## Rendimiento medido en Play Mode
120 frames por modo/resolución,30 de calentamiento; cámara y pose equivalentes en habitación amplia. Mediciones del Editor, sin limitar FPS ni modificar ajustes globales. GPU disponible mediante FrameTimingManager. CPU = marcador PlayerLoop. Draw calls/SetPass son estadísticas del Editor en esas condiciones, no garantía de rendimiento en un build.

| Modo | Interna | FPS | Frame ms | CPU ms | GPU ms | Draw calls | SetPass | GC bytes/frame | Recomposición C ms |
|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| C | 256x192 | 271.67 | 3.681 | 0.908 | 0.901 | 8.00 | 9.00 | 14356 | 12.448 |
| A | 256x192 | 190.02 | 5.263 | 1.680 | 1.212 | 204.00 | 17.00 | 14322 | — |
| C | 426x240 | 298.32 | 3.352 | 0.733 | 0.901 | 8.00 | 9.00 | 14320 | 25.211 |
| A | 426x240 | 207.66 | 4.816 | 1.488 | 1.482 | 260.00 | 17.00 | 14321 | — |

GC incluye Editor, interfaz y verificador, no solamente el compositor. La prueba confirma cero recomposiciones adicionales estando quieto; no afirma cero GC de toda la aplicación. La recomposición consume aproximadamente12.4ms a256×192 y25.2ms a426×240 en este equipo: puede notarse al moverse, aunque no se paga cada frame. Optimizar antes de considerar integrarlo al juego.

Memoria parcial de buffers (CPU y RenderTexture retenida para alternar):4.13MB a256×192 y8.59MB a426×240. La textura de composición RGBA32 ocupa196,608 y408,960bytes respectivamente; hay otra textura para transición y copias de sprites. La referencia3D utiliza RT768×576 o1278×720 con profundidad24. Los assets actuales leídos por A no son memoria de texturas nuevas y se comparten sin modificarlos. Performance.csv conserva medidas sin resumir.

## Revertir
Eliminar únicamente Codebound_RetroStyle_Test.unity, su meta y RetroStyle/ con su meta. Los archivos originales permanecen intactos; el experimento no tiene referencias desde escenas principales. No se aplicó al juego ni se hizo commit/push de esta nueva prueba.

## Acabado neón añadido
El Inspector ofrece Acabado 2D: NeonTextures o DrawnBlocks. N alterna sin cambiar la geometría ni la navegación. La escena se guarda inicialmente con neón. Se copiaron exactamente las regiones de cuadrícula de los JPG originales: azul(266,260,144,144) y rosa(266,850,146,143), iguales a las utilizadas por los materiales actuales. Copias en Neon/Blue-Neon.asset y Pink-Neon.asset, Point, sin mipmaps; los JPG/importadores originales permanecen intactos. Las superficies se dibujan en polígonos2D con muestreo Point y tonos de distancia, sin reflejos ni iluminación3D. Cuadrículas de neón por casilla controla su densidad.

Captures/C-Neon-*.png y C-Blocks-*.png muestran las mismas seis posiciones con ambos acabados. Se repitieron las26 comprobaciones con neón:26PASS/0FAIL, sin errores. Performance.csv ahora corresponde al neón; la tabla anterior conserva la medición previa con bloques. Caché adicional de píxeles de ambas copias:166,456bytes, cargada una vez por textura.
- Neón 256x192: 263.64FPS en Editor, CPU 0.861ms, GPU 0.903ms, recomposición 16.351ms.
- Neón 426x240: 261.25FPS en Editor, CPU 0.886ms, GPU 0.887ms, recomposición 33.230ms.
