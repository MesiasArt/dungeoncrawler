# Decoración de dungeons — Codebound

## Resultado
18 prefabs reutilizables y 35 decoraciones automáticas aplicadas al mapa principal. Se probaron primero en la escena independiente `Assets/MapTests/DecorationShowcase.unity` con la cámara y el controlador originales. No se modificaron filas del mapa, estadísticas, apariciones, combate ni inventario.

## Recursos
Todos los recursos están en `Assets/Resources/Decorations`: catálogo `Catalog.asset`, modelos `Prefabs`, materiales compartidos `Materials` y texturas `Art`. La configuración del mapa principal es `MainDungeonDecorations.asset`.

- Híbridos: monitor CRT, máquina arcade y ventana holográfica, combinando pantalla 2D y estructura 3D.
- Paneles 2D con marcos: ventilación, control, Memory Core y memoria corrupta.
- Geometría 3D sencilla: conductos, cables, placas de suelo, marcas digitales, lámpara, ventilador animado, servidores, generador, columna energética y marco de puerta.

Se reutilizó una copia del atlas de referencia y se generó una pantalla CRT original. Los archivos originales de referencia y las texturas de neón del juego se conservan. Los objetos decorativos no añaden colisiones ni luces dinámicas. Los materiales se comparten y los modelos estáticos se agrupan; el ventilador queda separado para animarse. La intensidad disminuye a distancia.

## Colocar y editar
1. Seleccionar el asset del mapa en el Inspector.
2. Elegir **Decoración** y un modelo del desplegable del catálogo.
3. Pulsar una casilla válida. El símbolo **DC** identifica la decoración.
4. Seleccionarla para editar orientación, desplazamiento local, escala y giro adicional.
5. Usar **Mover a otra casilla** o eliminarla conservando el suelo. Undo/Redo conserva sus identificadores.

Las decoraciones de pared necesitan una pared real adyacente; los marcos necesitan una puerta existente. La validación rechaza ubicaciones protegidas o un volumen que invada el paso central. Diferentes tipos compatibles pueden compartir casilla.

## Distribución automática
Abrir **Editor → Decoración y ambientación**. Cada mapa referencia su propia configuración. Se pueden editar semilla, densidad, máximo, separación, preferencia por máquinas de habitación y reglas por zona con tema y activación.

La configuración principal usa semilla 1987, máximo 60 y separación de tres casillas. Las zonas seguras usan densidad 0,12 y las demás 0,20; Zona 2 usa tema corrupto. El resultado actual es de 35 objetos. El algoritmo utiliza geometría, zona y semilla; no altera Unity.Random ni cambia la distribución al recargar. Favorece paneles en pasillos, cables en esquinas y máquinas en habitaciones. Respeta inicio, entidades interactivas, enemigos manuales, puertas, antorchas y vacío.

**Convertir distribución automática a colocaciones editables** conserva la distribución como objetos manuales y desactiva la generación automática, permitiendo moverlos o eliminarlos individualmente. **Previsualizar ubicación en SceneView** muestra guías de posiciones; la representación final se ve en Play Mode.

## Pruebas
**Editor → Pruebas → Decoración del dungeon** abre la escena independiente. En Play: 1 = pasillo corto, 2 = pasillo largo, 3 = habitación amplia. Usa el controlador real del juego.

Unity MCP: **63 comprobaciones correctas y 0 fallos**. Incluyen catálogo, referencias, identificadores, Undo/Redo, guardado y reimportación de assets, movimiento real junto a conductos y sobre placas, carga sin duplicados, agrupación estática y ventilador animado. Se comprobaron además 100 semillas reproducibles con límites y ubicaciones válidas. El mapa principal conserva su geometría, tiene 35 IDs únicos y no añade colisiones ni luces. Consola sin errores al finalizar.

Resultados: `Tests/DecorationMcpResults.txt`, `Tests/DecorationMainResults.txt` y `Tests/DecorationPerformance.csv`.

## Rendimiento
Medición en el Editor, misma cámara, 180 frames por condición:

| Decoraciones | FPS | CPU PlayerLoop ms | GPU ms | Draw calls | SetPass |
|---|---:|---:|---:|---:|---:|
| 0 | 125,90 | 3,386 | 1,895 | 474,80 | 22,75 |
| 35 | 125,42 | 3,706 | 1,736 | 478,93 | 24,80 |

El incremento de CPU observado es de aproximadamente 0,32 ms. Las diferencias de GPU están sujetas a variación de la medición. Estos resultados corresponden a una vista en el Editor, no a un benchmark de una build completa.

## Pendiente
La corrupción es visual, sin un nuevo sistema de eventos o partículas. Los prefabs personalizados deben mantener un volumen compatible con los pasillos. La vista previa del Editor muestra ubicación, mientras que la escena de prueba permite evaluar el aspecto final. Se pueden añadir más variantes al mismo catálogo sin crear listas paralelas.

Capturas finales en `Captures/Decorations`: `ShortCorridor-1.png`, `LongCorridor.png`, `WideRoomFinal-1.png` y `MainDungeonDecorated.png`. El pasillo largo fue capturado antes del último ajuste de color oscuro de los materiales.
