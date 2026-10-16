# Dungeon retro luminoso experimental

Escena independiente: `Assets/Scenes/RetroDungeonExperiment.unity`. Abrirla y pulsar Play. Ahora forma parte de Build Settings como destino independiente del guia de Digital Kingdom; CodeboundTitle sigue siendo el inicio del juego.

## Ajustes

Seleccionar `Assets/Resources/RetroDungeonExperiment/Settings.asset`: semilla, texturas, densidades, rangos de alturas, parallax, niebla blanca, tuberias y Normal Strength. El componente RetroDungeonWallComposer del objeto raiz referencia este asset.

**CRT propio:** `Assets/Resources/RetroDungeonExperiment/CRT.asset`, tambien enlazado en el campo Crt Profile de Settings. Cambiar scanlines, RGB split, curvatura, brillo y glitches aqui. No modifica el perfil del dungeon principal. Los ajustes globales del jugador pueden desactivar CRT.

La composicion actual tiene 299 elementos decorativos, 18 tramos de pared y tuberias con volumen. Se aumentaron densidades a 1.2 nubes, 1.35 montanas y 1.65 arbustos por casilla de pared. Las posiciones estratificadas y alturas variables reducen huecos; la misma semilla reproduce la distribucion. Las capas se limitan a cada tramo para respetar esquinas y entradas. El techo conserva la cuadricula original.

Las capas PNG se extrajeron del PSD original sin repintarlo ni redimensionarlo. Para sustituirlas, asignar nuevas texturas en Settings; usar Point y transparencia para las capas. El manifiesto de origen esta en Tests/RetroPsdLayers.json.

## Relieve y reflejos

El shader calcula una normal tangente desde diferencias de luminancia de pixeles vecinos. Normal Strength controla el relieve (0 lo aplana). Es una aproximacion de relieve, no un normal map artistico dibujado ni una textura normal exportada. Conserva el color base y una iluminacion suave.

El suelo reutiliza DungeonFloorReflection y su RenderTexture existente. SetSurfaceMaterial reconecta el material nuevo sin crear otra camara. El shader proyecta la captura y evita realimentacion durante el render reflejado. Paredes, tuberias y techo entran en esa captura. No cambia el material del primer nivel.

## Recorrido y tuberia

Mapa 21x20: inicio (5,1), pasillo recto, sala 7x7 en X2-8/Y6-12, curva y puerta normal (10,11), segunda sala 7x7 en X11-17/Y10-16. Tuberia interactiva (17,14), aproximarse a (17,12). La tuberia usa Hongo existente, factory/director y estado persistente actuales. No cambia combate ni movimiento.

Probabilidad, distancia Manhattan, cooldown, una vez/repetible y maximo son configurables. Hay tres placementIds preparados. Las restricciones existentes de poblacion y distancia siguen vigentes. Los intentos se guardan en completedEvents y los enemigos en su snapshot; no hay un sistema paralelo. Cooldown es temporal de la sesion. Los tubos decorativos no bloquean; para un obstaculo logico utilizar la geometria del mapa. No hay pooling nuevo.

## Verificacion

28 comprobaciones pasaron tras aumentar la decoracion y conectar el reflejo: BFS, salas, movimiento original, giros, puerta, determinismo, parallax, colisiones decorativas, tuberia, combate existente, probabilidad cero y guardado/carga sin duplicados. Tests/RetroDungeonResults.txt contiene el detalle. Shader sin errores; perfil CRT propio confirmado por referencia; captura planar lista y enlazada al suelo.

En Editor la escena redirige las escrituras de guardado a Tests/RetroExperimentSave.json y restaura la ruta al salir. Los seis slots personales no se sobrescribieron. Tests/RetroDungeonIntegrity.txt confirma 15 archivos protegidos sin cambios respecto al respaldo previo: escenas principales, geometria/assets de zonas, controlador de movimiento, CombatMath y PSD. Otros cambios previos del proyecto se conservaron.

Tests/RetroDungeonPerformance.txt registra 180 frames en Play Mode del Editor, resolucion y percentiles. No representa una garantia de rendimiento de una build ni una comparacion controlada con el juego principal. Se comparten materiales, no se regeneran las decoraciones por frame y el parallax es limitado en UV. Referencia tecnica: https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl

Capturas: Captures/RetroDungeonExperiment/Corridor.png, Room.png y PipeRoom.png.

## Actualizacion: guia y profundidad

Ver Docs/DigitalKingdomGuiaYAvisos.md y Docs/ReinoPixelProfundidad.md. La prueba inicial de 28 comprobaciones corresponde a la version anterior sin enemigos normales; los informes nuevos verifican la version actual. Se incorporaron normales importados por Unity, musica propia, tubos frontales y profundidad reversible.
