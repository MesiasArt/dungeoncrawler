# Digital Kingdom: llegada, idioma y CRT

Implementado el 15 de octubre de 2026. Respaldo previo: `Backups/DigitalKingdomTransition/20261015-133201`.

## Transicion

La puerta de ciudad conserva su ubicacion real (X11/Y66), IDs y destino. Una superficie blanca permanece dentro del marco cuando se abre. El cruce se detecta antes de interpolar el paso siguiente, evitando avanzar hacia el espacio posterior a la puerta.

El controlador existente `DigitalKingdomTransit` captura los mismos datos y carga la misma escena de ilustracion (`DigitalKingdom.unity`), ahora de forma asincrona bajo una cobertura persistente. Fundido hacia blanco: 1 segundo; revelado: 0.8 segundos. Ambos son configurables en `Assets/Resources/DigitalKingdom/City.asset`. La cobertura permanece opaca hasta que la camara, el modelo y la ilustracion estan preparados. Los controles se restauran tras terminar el revelado. La cobertura se aplica despues del compositor CRT, evitando que este la tape o distorsione.

La musica anterior se desvanece antes de cargar. La ciudad usa `Welcome to the Town` con entrada suave. Se conserva una unica fuente de musica y la fuente de efectos existente. Al salir se recupera la musica del dungeon y el punto de retorno, conservando inventario, enemigos, entidades y snapshots.

## Textos y CRT propios

Los dialogos de llegada de GamerBoy, NPC, descripciones y servicios estan en espanol. Los siete nombres propios permanecen exactamente en ingles. Los textos de servicios estan configurados en `City.asset > Texts`; cada establecimiento conserva sus dialogos, descripcion y nombres traducidos opcionales de existencias en su asset. No habia un paquete de localizacion que integrar.

El CRT de ciudad se edita en **`Assets/Resources/DigitalKingdom/CityCRT.asset`**, vinculado desde `City.asset > Crt Settings`. Es independiente del perfil del dungeon. Reutiliza el compositor, shader NTSC y controlador existentes; la ilustracion se procesa como fondo, sin crear una escena 3D ni cambiar la ilustracion. El menu se conserva limpio por defecto (`Affect UI` desactivado). El interruptor general de CRT de las opciones se sigue respetando. El perfil de ciudad permite modificar brillo, gamma, intensidad, scanlines, curvatura, RGB y glitches sin alterar el dungeon.

## Otros ajustes solicitados

- Los comandos y objetivos de combate se ocultan durante los dialogos posteriores a la batalla, tambien si el submenu HACK habia quedado seleccionado. No se adelanta el cierre del combate ni la entrega de recompensas.
- Sonido dedicado de aterrizaje de Makong, sincronizado con `jump_3`: `MakongLanding.wav`, original y provisional, 0.72 segundos. Se puede reemplazar en `MakongSpecialSettings > Landing Sound` y ajustar `Landing Volume`. No cambia el dano ni la sacudida.
- `Assets/Resources/Audio/DungeonAudioSettings.asset`: `Exploration Playlist`, `Normal Combat Playlist`, `Shuffle Playlist`, `City Music` y `City Music Fade Seconds`. Exploracion incluye Cyber Labyrinth y Cyber Labyrinth 2. Combate normal conserva Arcade Showdown y admite mas pistas. Listas vacias usan las referencias anteriores; nulos y duplicados se ignoran; se evita la repeticion inmediata; las pistas avanzan al terminar. Las actualizaciones de contexto no reinician una pista en curso. La musica especial de Makong conserva prioridad.

## Validacion

Resultado final: **103 comprobaciones aprobadas, 0 fallidas** (38 de ciudad, 11 de listas/presentacion, 30 narrativas y 24 audiovisuales). Compilacion sin errores; consola final sin errores.

Pruebas de Play Mode ejecutadas mediante Unity MCP, con datos aislados en `Tests/`, sin escribir partidas personales:

- `Tests/DigitalKingdomSceneMcpResults.txt`: puerta bloqueada antes de Makong, cruce real, blanco opaco durante carga, controles bloqueados/restaurados, musica propia sin fuentes duplicadas, siete destinos, dialogos espanoles, teclado/mando, templo, guardado/carga, retorno exacto y conservacion del progreso, revisita, viaje a nodo descubierto y respawn.
- `Tests/DigitalKingdomPresentationResults.txt`: listas vacias, nulos/duplicados, orden secuencial, rotacion automatica, mezcla sin repeticion inmediata, continuidad de musica, lista independiente de combate, nivel del audio de aterrizaje y reutilizacion de fuentes.
- `Tests/MakongNarrativeResults.txt`: regresion de narrativa, final de combate, recompensas unicas y guardado.
- `Tests/MakongAudiovisualResults.txt`: regresion de las pistas de dialogo/jefe, silencios, CRT contextual y retorno musical.
- `Tests/DigitalKingdomTransitionIntegrity.txt`: hashes de las dos partidas personales y cinco backups sin cambios. CombatMath, JUMP, OVERCLOCK, narrativa de Makong y logica de progresion sin modificaciones en esta tarea.

Capturas revisadas en `Captures/DigitalKingdom/`: `WhiteCover.png`, `FullScreenCity.png`, `CurrentCityCRT.png` y `MakongFinalWithoutCommands.png`.

Los servicios que ya estaban en desarrollo siguen en desarrollo (posada, compras, investigacion y alcantarillas); esta tarea traduce sus interfaces sin implementar servicios nuevos. La prueba del encuentro usa un fixture de derrota del jefe para el cruce y la prueba narrativa verifica la derrota real por el flujo de combate existente. No se realizo un build standalone. La consola puede mostrar el aviso preexistente de resolucion del atlas de sombras del dungeon.

## Archivos

- `Assets/Scripts/DigitalKingdomController.cs`
- `Assets/Scripts/DigitalKingdomLocation.cs`
- `Assets/Scripts/DigitalKingdomSceneValidation.cs`
- `Assets/Scripts/DigitalKingdomSettings.cs`
- `Assets/Scripts/DigitalKingdomTransit.cs`
- `Assets/Scripts/DungeonAudio.EncounterMusic.cs`
- `Assets/Scripts/DungeonAudioSettings.cs`
- `Assets/Scripts/DungeonDoor.cs`
- `Assets/Scripts/DungeonPrototype.cs`
- `Assets/Scripts/DungeonPrototype.DigitalKingdom.cs`
- `Assets/Scripts/DungeonPrototype.Makong.cs`
- `Assets/Scripts/DungeonPrototype.MapEntities.cs`
- `Assets/Scripts/DungeonPrototype.Movement.cs`
- `Assets/Scripts/DungeonPrototype.Save.cs`
- `Assets/Scripts/DungeonPrototype.SaveSlots.cs`
- `Assets/Scripts/HelmetCRTWholeFrame.cs`
- `Assets/Scripts/MakongNarrativeValidation.cs`
- `Assets/Scripts/MakongSpecialSettings.cs`
- `Assets/Resources/Audio/DungeonAudioSettings.asset`
- `Assets/Resources/Combat/MakongSpecialSettings.asset`
- `Assets/Resources/DigitalKingdom/City.asset`
- `Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset`
- `Assets/Resources/DigitalKingdom/Locations/02-city.shop.asset`
- `Assets/Resources/DigitalKingdom/Locations/03-city.temple.asset`
- `Assets/Resources/DigitalKingdom/Locations/04-city.lab.asset`
- `Assets/Resources/DigitalKingdom/Locations/05-city.travel.asset`
- `Assets/Resources/DigitalKingdom/Locations/06-city.sewers.asset`
- `Assets/Resources/DigitalKingdom/Locations/07-city.exit.asset`
- `Assets/Scripts/DungeonWhiteTransition.cs`
- `Assets/Scripts/DungeonPrototype.CityPortal.cs`
- `Assets/Scripts/DungeonAudio.Playlists.cs`
- `Assets/Scripts/DigitalKingdomTexts.cs`
- `Assets/Scripts/DigitalKingdomPresentationValidation.cs`
- `Assets/Resources/Shaders/WhiteTransition.shader`
- `Assets/Resources/Audio/Temporary/MakongLanding.wav`
- `Assets/Resources/DigitalKingdom/CityCRT.asset`

## Revision RGB Split

Se detecto que Chromatic Fringe del asset NTSC solamente participa en los modos Crosstalk de los glitches. Se incorporo una separacion RGB permanente explicita al compositor existente, sin otro pase de postprocesado.

En CityCRT.asset: `RGB Split Pixels` = 2 y `Glitch RGB Split Pixels` = 8 adicionales al alcanzar la maxima intensidad del glitch (total 10). El refuerzo decae con el glitch y vuelve a 2. Cero desactiva cada ajuste. Se mantienen los demas valores editados por el usuario. El dungeon conserva ambos valores nuevos en cero.

Prueba Unity MCP: material real 2 -> 10 -> 2; comparacion GPU con imagen fija, 49 415 pixeles cambiaron sus canales rojo/azul y ninguno cambio el verde. Registro: `Tests/CityRGBSplitResults.txt`. Una advertencia de liberacion de RenderTexture durante la limpieza de la prueba fue provocada por el script de comprobacion; se restablecio el target activo. No corresponde al flujo de renderizado del juego.

El RGB Split tambien queda activado en el perfil general `Assets/Resources/HelmetCRT/Settings.asset`: 2 pixeles permanentes y hasta 8 adicionales durante glitches. Exploracion y combate comparten este perfil. Los elementos siguen respetando Background/Enemy/UI Intensity y Affect UI. La ciudad conserva su perfil independiente.
