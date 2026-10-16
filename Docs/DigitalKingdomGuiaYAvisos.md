> Actualizacion vigente: la opcion Explorar dungeons se retiro y su guia/NEW se integraron en Fast Travel Station. Ver Docs/AtmosferaViajesYMenu.md para el flujo dungeon → nodos. Los IDs y la persistencia de avisos descritos aqui se conservan.

# Digital Kingdom: guia, avisos NEW y Reino del Pixel

## Secuencia implementada

1. Debajo de RNG Sewers aparece **Explorar dungeons**. El guia pregunta adonde quieres ir; Gamerboy admite que no sabe y el guia le recomienda hablar con la gente de la ciudad. Todavia no hay un destino disponible.
2. Visitar los siete destinos visibles, terminar sus conversaciones y volver al menu de ciudad registra cada visita. No exige activar cada servicio, comprar ni usar el punto de guardado. Exit City permanece oculto y no cuenta. Las partidas anteriores no inventan visitas: hay que recorrer las opciones.
3. Tras la ultima visita, aparece **NEW** en la esquina inferior derecha del boton de **The Last Life**.
4. Al entrar, el posadero cuenta que escucho a alguien en el bar hablar de problemas en **Reino del Pixel**: las tuberias sueltan criaturas y algunos caminos ya no son seguros. Recomienda hablar con el guia.
5. Solo al terminar ese dialogo se registra `pixel.rumor.heard`. El aviso del bar desaparece y aparece NEW en **Explorar dungeons**.
6. El guia reconoce que Gamerboy ya tiene un destino, advierte sobre las tuberias y habilita **Viajar a Reino del Pixel** al terminar la conversacion (`pixel.route.unlocked`). El aviso se consume una sola vez.
7. El viaje carga `Assets/Scenes/RetroDungeonExperiment.unity` con la transicion blanca existente. El reino conserva su mapa propio. En pausa aparece **VOLVER A DIGITAL KINGDOM** para regresar. La posicion de retorno al dungeon original se conserva.

Nombre provisional confirmado por el usuario: **Reino del Pixel**. El nombre visible se cambia en Story.asset y sus dialogos.

## Dialogos nuevos

Primera visita al guia:

- GUIA: Llevo exploradores a otros reinos. ¿A donde quieres ir?
- GAMERBOY: Esa es una buena pregunta... No se a donde debo ir.
- GUIA: Entonces date una vuelta por la ciudad. A veces una conversacion basta para encontrar el camino.

Aviso del bar:

- POSADERO: Antes de que te vayas... Escuche a alguien en el bar hablar de problemas en el Reino del Pixel.
- GAMERBOY: ¿Que clase de problemas?
- POSADERO: Decia que las tuberias estan soltando criaturas y que algunos caminos ya no son seguros.
- GAMERBOY: Por fin un lugar donde empezar a buscar respuestas.
- POSADERO: Habla con el guia de dungeons. El conoce el camino.

Segundo dialogo con el guia:

- GUIA: Veo que ya tienes algo en mente.
- GAMERBOY: Me hablaron de problemas en el Reino del Pixel. Quiero ir alli.
- GUIA: Conozco la ruta. No te fies de las tuberias: algunas tienen visitas.
- GAMERBOY: Claro. Hasta las tuberias tienen mas vida social que yo.
- GUIA: Cuando estes listo, elige el reino. Podras regresar a Digital Kingdom desde el menu de pausa.

Los assets de Unity contienen la version en español con tildes y signos de apertura.

## Reutilizar los avisos

Editar `Assets/Resources/DigitalKingdom/Story.asset` (DigitalKingdomStorySettings). Cada entrada Notices tiene:

- `id`: identificador unico estable; usar un ID nuevo para una noticia posterior.
- `locationId`: destino cuyo boton muestra NEW.
- `requireAllLocationsVisited` y `requiredVisits`: requisitos de visitas.
- `requiredFlags` / `blockedFlags`: condiciones de historia.
- `dialogue`: conversacion que se abre al seleccionar el destino.
- `grantFlags`: estados que se conceden solo al terminar de leer.

`CityNoticeSystem` evalua estas reglas de forma generica. Para otro aviso basta agregar datos; no hace falta agregar una condicion especial en el controlador. Si hay varios avisos para el mismo lugar, se presenta el primero disponible por entrada. Al completar uno se puede activar el siguiente. Cancelar o abandonar sin completar no lo marca como leido.

Destinations contiene el nombre, escena independiente, mapa y flag requerida de cada ruta. La nueva opcion de ciudad se edita en `Assets/Resources/DigitalKingdom/Locations/08-city.dungeons.asset`.

## Persistencia y compatibilidad

Se reutilizan DigitalKingdomProgress, DungeonSaveStore y los snapshots por mapId existentes. Los nuevos campos son visitedLocations, storyFlags y readNotices. No se creo un segundo guardado. Los campos ausentes en partidas antiguas se inicializan vacios; no se concede el rumor ni el viaje automaticamente.

El viaje utiliza DungeonMapPersistence.ForTravel. La salida guarda el snapshot del piso original; el regreso conserva los cambios del reino y la ruta de retorno original. La escena retro se agrego a Build Settings por ser ahora un destino real. CodeboundTitle sigue primero. No se modifico la geometria principal, el combate ni el balance.

## Pruebas

`Tests/KingdomFollowupResults.txt`: 53 comprobaciones correctas: todas las visitas, aparicion y consumo de NEW, bloqueo previo, reglas futuras, datos antiguos, guardado/carga, transiciones, ida y vuelta dos veces, snapshots y retorno, minimapa oculto en combate, musica y normales asignadas. Las escrituras de prueba se desviaron a Tests/KingdomFollowupSave.json. No se sobrescribieron los seis slots personales.

Archivos principales: DigitalKingdomStorySettings.cs, DungeonPrototype.CityStory.cs, DigitalKingdomSettings.cs, DungeonPrototype.DigitalKingdom.cs, DigitalKingdomTransit.cs, DungeonWhiteTransition.cs y DungeonPrototype.Pause.cs. Los assets Story y 08-city.dungeons contienen la narrativa configurable. La Biblia V1.0 y Docs/Narrativa/Fuentes no se alteraron.

## Comprobacion adicional de Continuar

Tests/KingdomTitleResults.txt: 6 comprobaciones correctas. Se cargo una partida ficticia guardada dentro del reino desde CONTINUAR del titulo. Se restauraron su escena, estado, decoraciones, CRT propio y listas de musica; la opcion de regresar a la ciudad siguio disponible. Tambien se verifico Virtual Worlds Main Title en una unica fuente con loop.

Tests/KingdomFollowupIntegrity.txt confirma que los archivos personales existentes, la escena principal, CombatMath, el controlador original de movimiento y el PSD no cambiaron. Respaldo previo: Backups/RetroKingdomFollowup/20261015-194803.

Resultados finales: 53 comprobaciones de ciudad, 17 de profundidad/tuberias y 6 de titulo/continuar: 76 correctas, sin errores de consola. La verificacion final de integridad compara dos archivos personales existentes y quince archivos protegidos: todos sin cambios. Captura del aviso: Captures/RetroDungeonExperiment/CityNew.png.
