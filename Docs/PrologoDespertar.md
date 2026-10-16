# Despertar en la Red — 2026-10-15

Implementado exclusivamente el encargo de reescritura del prólogo. Respaldo: `Backups/PrologueWake/20261015-164620`.

## Presentación

- 1 s inicial completamente negro y sin música. Terminal SYSTEM BOOT, recovery.exe y Restaurando sesión... con cursor parpadeante y escritura progresiva. Esta última cadena conserva su fuente completa, pero su presentación se corta en 14 caracteres.
- Transmisión de emisor ??? sin identidad. Los 26 parlamentos coinciden literalmente con el adjunto, incluidos acentos y la raya de interrupción final.
- Vistazo de 0,34 s al refugio con la cámara existente. Fundido suave de 2 s y pausa de contemplación de 1,1 s. No hay desplazamiento, giro, manos, brazos, modelo ni animación de levantarse.
- Interferencias de 0,10–0,12 s mediante el mismo `CorruptedDialoguePresentation` usado por Makong, con intensidad 0,025–0,04 y volumen reducido. Texto afectado de forma mínima solo en la frase interrumpida; sin blancos intensos. No se modifican los perfiles de Makong.
- Zumbido portador muy suave generado temporalmente en memoria y reproducido por la fuente musical ya existente. No hay voces ni otra fuente de música. Se corta la portadora al perder la conexión y se restaura la música normal con la envolvente de fundido existente al finalizar.
- El HUD se oculta durante el prólogo. La composición CRT deja limpio el texto y el negro mediante una bandera temporal, sin cambiar los valores guardados del efecto. Se restaura al finalizar, omitir o cargar.
- ESPACIO o botón A/sur completa el texto y una nueva pulsación lo avanza. ENTER conserva la opción de omitir. Las pausas no almacenan pulsaciones y la última confirmación no ejecuta movimiento en el mismo frame.

## Compatibilidad

Se reutilizan `IntroSequence`, `DialogueTypewriter`, `GameStartContext`, el bloqueo de `IntroductionActive`, el reproductor de audio, el compositor CRT y el guardado existentes. No hay otro controlador de combate o guardado.

Una partida guardada siempre se trata como continuación: no repite el prólogo aunque su antiguo campo `introductionCompleted` sea falso o falte. El cambio es de interpretación al cargar; los archivos personales no se modifican automáticamente. Se conservan posición, inventario y progresión cargadas.

## Archivos

- `Assets/Scripts/IntroSequence.cs`: guion exacto, etapas automáticas, pausas y oscuridad interpolada.
- `Assets/Scripts/DungeonPrototype.Prologue.cs` (nuevo): presentación del arranque, audio y efectos de transmisión; no crea geometría ni modelos.
- `Assets/Scripts/DungeonPrototype.cs`: arranque/finalización y dibujo de la secuencia antes del HUD.
- `Assets/Scripts/DungeonPrototype.Save.cs`: las partidas cargadas omiten la introducción.
- `Assets/Scripts/CorruptedDialoguePresentation.cs`: punto de entrada Pulse para reutilizar sus efectos existentes.
- `Assets/Scripts/HelmetCRTWholeFrame.cs`: bandera temporal CinematicCleanUI, restaurada al terminar.
- Pruebas: `Tests/IntroSequenceTests.cs`, `Tests/IntroPlayModeTests.cs`, `Assets/Scripts/PrologueValidation.cs`, `Tests/PrologueExpectedDialogue.json`.
- Documentación vigente de narrativa: archivos 08, 09 y 10. Biblia V1.0 y fuentes históricas protegidas sin editar.

## Pruebas

- `Tests/RunIntroTests.ps1`: **436 aserciones aprobadas** del modelo, incluyendo ejecución con pasos de tiempo de 60/120 FPS. Esto verifica temporización; no es una medición del rendimiento gráfico a esos FPS.
- `Tests/PrologueResults.txt`: **145 comprobaciones aprobadas en Play Mode mediante Unity MCP**. Negro en la salida CRT renderizada, silencio real, guion exacto y orden, cámara fija, bloqueo en cada etapa, sesión truncada, vistazo y fundido, desconexión española, omisión, devolución de controles, carga antigua y conversaciones/terminal/nodo del refugio.
- `Tests/PrologueAudioResults.txt`: **3 aprobadas**: vuelve Cyber Labyrinth mediante la fuente normal, no se añade fuente musical y la transmisión libera la propiedad del audio.
- Capturas revisadas: `Captures/PrologueWake/01-Black.png`, `02-Boot.png` y `03-Reveal.png`. Se comprobó la lectura del terminal y la ausencia de modelo del protagonista en el refugio.
- Consola sin errores en la ejecución final; scripts compilados correctamente. Las pruebas guardan exclusivamente en `Tests/PrologueSave.json`, sin sobrescribir los seis slots personales.
- `Tests/PrologueIntegrity.txt`: huellas del mapa, sus objetos, movimiento, NPC, Biblia y Fuentes comparadas con el respaldo.

## Alcance conservado

No se cambia la geometría, los cinco refugiados, sus diálogos aprobados, terminal, teleport, conexiones ni eventos posteriores. No se inventan personajes, lore, objetivos ni marcadores nuevos. La rotación durante los diálogos se mantiene bloqueada para conservar los controles existentes; era opcional en el encargo.


## Ajuste solicitado: avatar y vistazo con glitch

El retrato habitual de GamerBoy vuelve a mostrarse junto al diálogo cuando habla. El emisor ??? conserva su identidad oculta. El vistazo al refugio dura 0,34 s con entrada y salida mediante una envolvente negra suave, sincronización horizontal interrumpida y un pulso del CRT existente; se evita el corte limpio repentino. No se altera el guion ni se mueve la cámara. Verificación: 145 comprobaciones MCP aprobadas y 436 aserciones del modelo; consola sin errores. Captura del retrato: `Captures/PrologueWake/04-Avatar.png`.


### Cambio de cierre del prototipo

A petición posterior se retira la omisión del prólogo: ENTER ya no lo salta y se elimina su indicación. La referencia anterior a ENTER describe la versión inicial, sustituida por este ajuste. Ver Docs/BienvenidaYCierrePrototipo.md.
