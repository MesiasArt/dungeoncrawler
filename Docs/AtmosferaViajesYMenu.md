# Reino del Píxel: atmósfera, viajes y edición del título

## Editar el menú de inicio

En Unity: **Codebound → Menu de inicio → Editar diseño**. Selecciona `Assets/Resources/UI/Title/Settings.asset`.

El Inspector permite cambiar fondo, logo, canción, textos, color, tamaño de botones, separación, margen inferior y alineación horizontal. Logo Rect utiliza coordenadas normalizadas (0–1); los botones usan una referencia 1280×720 y se escalan a la ventana. Hay vista previa en el Inspector y un botón para abrir `Assets/Scenes/CodeboundTitle.unity`. En esa escena, el componente CodeboundTitleScreen también tiene **Editar arte, musica y botones**.

Los ajustes de diseño cambian durante Play. Al editar el asset quedan guardados como configuración del proyecto; no dependen de los cambios temporales del objeto de escena. Se conservan el fondo y logo existentes, los botones pequeños sin panel exterior y Virtual Worlds Main Title.

## Auditoría visual previa

El dungeon original usa DungeonAtmosphereSettings, DungeonCorridorLighting y DungeonAtmosphere.hlsl. La función ApplyDungeonHorizon oscurece por distancia, mezcla el color del fondo y añade una línea y brillo de horizonte. DungeonMetalToon usa iluminación en tonos diferenciados; los materiales de neón comparten la atmósfera. El CRT añade scanlines, separación RGB y procesamiento a menor resolución. No encontré una niebla cuantizada explícita en ese recorrido original: su aspecto procede de la combinación de iluminación, contraste, horizonte y CRT.

El reino usaba un shader de recortes con iluminación sencilla y un desvanecimiento blanco continuo de 16 a 40 unidades. Sus materiales copiaban Parallax y demás valores solo al construirse: esa era la causa de que editar Layered Depth, Depth Strength y los factores no cambiara nada durante Play.

Ahora el compositor detecta cambios de parámetros y actualiza los materiales existentes. No reconstruye planos, tuberías ni enemigos para esos cambios. La geometría, las colisiones y el controlador original se conservaron.

## Comparación A/B/C

Abrir **Codebound → Reino del Pixel → Editar atmosfera y profundidad**, o seleccionar `Assets/Resources/RetroDungeonExperiment/Settings.asset`.

| Opción | Resultado |
|---|---|
| A - Actual | Conserva el aspecto anterior, su desvanecimiento blanco y el CRT configurado. |
| B - Suave | Niebla azul clara continua, sin oscurecimiento añadido por distancia. |
| C - Retro | La niebla luminosa pasa por 6 bandas con bordes suavizados. Usa el mismo CRT configurado. |

Los tres botones del Inspector funcionan en vivo sin reconstruir la escena. B es la transición más suave; C da un matiz digital más marcado y quedó seleccionada como prueba. El control global del jugador para desactivar CRT sigue teniendo prioridad: elegir C no fuerza una opción gráfica desactivada.

**Escala medida:** una casilla = aproximadamente 5,18 unidades. Fog Start Cells está en 4,5 casillas (23,31 unidades) y Fog End Cells en 11 (56,98 unidades). Los valores se convierten desde casillas a la escala real al ejecutar.

- **Fog Start Cells:** desde dónde comienza a reducirse la visibilidad.
- **Fog End Cells:** dónde el escenario se funde con el color de niebla. Bajarlo oculta antes las estructuras.
- **Luminous Fog Color:** azul cielo/blanco azulado.
- **Fog Response:** curva de mezcla; valores menores hacen que la niebla gane presencia antes dentro del intervalo.
- **Quantized Fog:** permite quitar las bandas de C.
- **Fog Levels:** de 5 a 8; 6 por defecto.
- **Band Softness:** suaviza los escalones. No se utiliza ruido temporal para crear las bandas.

Las salas próximas siguen siendo visibles: empezar a 4–5 casillas no oculta una pared situada a 2 o 3 casillas. Para un resultado más cerrado en salas amplias, bajar Fog Start Cells desde el Inspector. No se añadió geometría para tapar la vista.

La niebla se aplica en shaders del mundo, antes de la composición del HUD. No altera los textos o barras de interfaz. El tinte de montañas sigue siendo configurable y los arbustos conservan su saturación. El techo digital, las texturas, el relieve y los reflejos actuales se conservan. La salida tras el jugador conserva un brillo blanco.

El shader compartido original incluye una rama que solo se activa mientras existe el compositor del reino. Al salir se desactiva; se verificó el aspecto original fuera de este mapa.

## Viajes: dungeon → nodos

Se quitó el destino visible Explorar dungeons. Todo está en **Fast Travel Station**:

1. Elegir un dungeon: actualmente **Dungeon original** y, cuando se desbloquea, **Reino del Píxel**.
2. Dentro aparecen únicamente sus nodos desbloqueados. El reino tiene **Entrada del reino**, que se habilita con la ruta del guía.
3. Los nuevos teleports que se configuren en el mapa se incorporan a su grupo al desbloquear su ID. Los nodos bloqueados no se muestran.
4. Volver desde la lista de nodos regresa a la lista de dungeons; volver desde esa lista regresa a la ciudad. Funciona con ratón, teclado y mando; hay desplazamiento para listas largas.

El nombre Dungeon original agrupa el plano actual, que contiene varias zonas. Puede cambiarse por Zona 0 u otro nombre sin cambiar su mapId.

Configuración: `Assets/Resources/DigitalKingdom/Story.asset`, lista **Destinations**. Cada grupo tiene nombre visible, map, scenePath y requiredFlag. Include Legacy Nodes se utiliza para los nodos anteriores del mapa principal. Allow Entry Without Node permite una entrada inicial al desbloquear la ruta; Entry Node Name define su etiqueta. Los teleports del propio mapa se buscan automáticamente.

Se mantiene la historia del bar. Tras visitar los seis destinos visibles aparece NEW en The Last Life; al terminar el rumor, NEW aparece en Fast Travel Station. Su guía desbloquea el reino. Se conservan los IDs `city.rumor.pixel.v1` y `city.guide.pixel.v1`, los flags y los avisos ya leídos. No se crea otro sistema de guardado. Una ubicación antigua guardada como city.dungeons se redirige a city.travel.

## Puerta de regreso

En la casilla inicial (5,1), mirando hacia el norte, la puerta está detrás, orientada al sur. Usa la pared que ya existía; no se añadió otra casilla ni se alteró la geometría del mapa. Girar y pulsar **E — Regresar a Digital Kingdom** inicia la transición blanca. El punto de retorno al dungeon principal y el snapshot del reino se conservan. El regreso de pausa también sigue disponible.

## Combate del Reino del Píxel

Al combatir en ese mapa, el escenario ancho existente recibe su suelo de ladrillos, cielo, montañas, nubes, arbustos y tuberías decorativas. Conserva la cámara, formación, techo digital, reflejos, música Bit-Battle y reglas de combate existentes. No añade enemigos ni mecánicas. El escenario se oculta al terminar. En los otros mapas no se instala este fondo: se verificó el material original de combate.

Implementación principal: RetroDungeonWallComposer.Battle.cs; el controlador llama al componente solo cuando el mapa posee la presentación retro.

## Pruebas y límites

`Tests/AtmosphereTravelTitleResults.txt`: 58 comprobaciones correctas. Incluye visitas y avisos, agrupación de mapas, nodos desbloqueados, un nodo de mapa futuro en fixture independiente, guardado existente, snapshots, edición en vivo, A/B/C en tres ubicaciones, salida, combate propio, retorno al mundo original y edición del título.

Se comparó el buffer de interfaz entre A y C y se comprobó que la niebla no lo modifica. Se capturaron pasillo, esquina, sala amplia, estación, puerta, combate y título. No se garantiza que todos los gustos visuales queden resueltos: la fuerza del CRT y las distancias se mantienen editables. No se modificaron partidas personales ni las filas de los mapas. El respaldo anterior a esta iteración está en Backups/AtmosphereTravelTitle/20261015-205821.

[Comparación visual A/B/C](ReinoPixelComparacion.html). Capturas originales en Captures/AtmosphereTravelTitle.

## Integridad de la sesión

Tests/AtmosphereTravelTitleIntegrity.txt compara escenas originales, controlador de movimiento, CombatMath, assets de zonas, filas del mapa retro y partidas personales con el respaldo. Todos esos archivos permanecen iguales. El PSD fuente tiene un hash distinto al registrado antes del trabajo y fecha de edición posterior; esta implementación no escribe el PSD y se conservó su versión actual. Las capas usadas por el experimento siguen siendo los PNG extraídos previamente.

Validación final repetida desde Digital Kingdom: 58/58. El fixture debe iniciarse desde esa escena para evitar referencias estáticas del mapa previamente abierto. Los valores 4,5/11 son el preset inicial; se conservan los cambios posteriores del usuario en el Inspector. La prueba comprueba la escala usando los valores configurados, sin restaurar el preset.
