# Digital Kingdom — Hub inicial

## Presentación y escenas
Assets/Scenes/DigitalKingdom.unity es la escena independiente de la ciudad. Se carga con LoadSceneMode.Single: el dungeon se descarga. La escena solo contiene el controlador de hub, una cámara sin geometría y una luz desactivada. DungeonPrototype permanece desactivado como adaptador de datos/UI; no ejecuta Start, movimiento, generación de casillas, enemigos ni minijuegos. Se reutiliza la restauración del jugador, fórmulas, inventario, diálogos y selector de guardado existentes.

La ilustración proporcionada está en Resources/DigitalKingdom/Panorama.png. Cubre toda la pantalla con ScaleAndCrop, manteniendo proporciones y recortando bordes cuando es necesario. Las opciones están superpuestas en un panel. El HUD de exploración y minimapa no se dibujan aquí.

## Acceso
Makong actual: X11/Y53. Se añadió únicamente una prolongación al norte: pasillo seguro X11/Y55–67, puerta blanca iluminada en X11/Y66. Requiere la derrota persistente de makong.first-sub-boss. El mapa anterior se respaldó en Backups/DigitalKingdom. Las casillas anteriores al pasillo se conservaron.

La entrada es una DungeonMapEntity Door/Transition con City Entrance activado. La orientación y el ID siguen usando el editor actual. Exit City vuelve a X11/Y66 mirando hacia el pasillo. El retorno guarda escena, mapa, zona, casilla y orientación, junto con el snapshot de enemigos, objetos y exploración. Los mismos datos son usados por slots. El puente de ciudad funciona con el dungeon actual; no sustituye una futura persistencia general de todos los pisos.

## Funcional
- Siete ubicaciones exactas, teclado (flechas/Enter/E), mando (D-pad/A/B) y botones de ratón.
- Diálogo inicial de descubrimiento una vez; diálogos de establecimientos una vez por configuración.
- Temple of Continue cura HP/Bits, registra Continue y usa los seis slots existentes. Tras derrota devuelve a la ciudad conservando el estado de progreso existente, sin pérdidas añadidas.
- Fast Travel muestra únicamente nodos registrados. Usa las coordenadas y eventos existentes al reconstruir el dungeon en el nodo seleccionado.
- Salir/cargar/revisitar conserva los estados y no recrea aleatoriamente la población al restaurar el snapshot.
- Música conservada entre las escenas y sonidos de UI activos.

## Provisional / futuro
- Posada: diálogo, interior provisional y evento futureService preparado; sin alojamiento/bonificaciones.
- Tienda: catálogo de pociones del inventario y consulta de cantidades; sin compras ni nueva economía.
- Bug Lab: cuatro servicios en desarrollo; sin mejoras reales de HACKS o ataques.
- RNG Sewers: entrada y diálogo; sin generador ni piso procedural.
- Interiores y retratos: campos configurables; no se generaron ilustraciones definitivas ni NPC secundarios.
- No se añadió un nodo de ciudad a la red automáticamente.

## Inspector
Editor → Digital Kingdom → Configuración de ciudad abre City.asset. Panorama cambia el fondo exterior. Locations contiene los siete assets en orden bajo Resources/DigitalKingdom/Locations/01…07. En cada establecimiento se editan Background, NPC Portrait, Npc Name, Dialogue, disponibilidad y conversación única. No cambies un ID que ya esté en guardados.

La escena usa DigitalKingdomController y el mismo modelo del dungeon en modo de datos. Para abrir la escena desde Editor y verla sin entrar desde el dungeon existe una vista de prueba; no simula que Makong haya sido derrotado en una partida personal.

## Verificación
Tests/DigitalKingdomSceneMcpResults.txt documenta pruebas MCP/Play Mode con escenas reales: bloqueo antes de Makong, descarga de actores/geometría, ausencia de minijuegos en ciudad, controles de teclado/mando, aperturas de establecimientos, Temple, save/load, viajes bloqueados/desbloqueados, retorno, revisita y Continue. Se usaron archivos de prueba separados de los slots personales.

Captura revisada: Captures/DigitalKingdom/FullScreenCity.png. La vista se comprobó en el tamaño actual del Game View. Los botones de ratón están implementados; la automatización de ratón mediante Game View no produjo una interacción reproducible, por lo que no se declara esa prueba como aprobada. Tampoco se declara una medición comparativa de memoria ni una prueba exhaustiva de todas las resoluciones.

La verificación final contiene 30 PASS y cero FAIL. Las entradas simuladas se probaron con el encaminamiento de foco del Editor temporalmente controlado; los ajustes originales se restauraron al terminar. Tests/DigitalKingdomMcpResults.txt y DigitalKingdomInputResults.txt pertenecen al prototipo previo de overlay; el resultado definitivo es DigitalKingdomSceneMcpResults.txt.
