# Makong: presentación audiovisual

## Música
- **Corrupted Memories.mp3**: introducción y despedida. La exploración se atenúa durante 1,5 segundos antes de cambiar a esta pista.
- **Boss Battle Groove.mp3**: pelea de Makong, tras una pausa dramática de 0,4 segundos.
- La despedida utiliza el ambiente a volumen muy bajo; después se restaura la exploración.

Se extendió DungeonAudio: sigue usando una fuente musical y su fuente de efectos anterior. Las canciones no se superponen y SetCombat no reemplaza accidentalmente la música del jefe. Los MP3 se importan como streaming. No se agregaron paquetes externos.

## Efectos
Se reutiliza HelmetCRTController.TriggerGlitch, sin shaders, luces ni movimientos de cámara nuevos. Los glitches duran 0,10 / 0,18 / 0,26 segundos y tienen intensidades 0,06 / 0,14 / 0,26. Se respeta la preferencia de desactivar CRT y la intensidad de UI existente.

CorruptedDialoguePresentation observa LineStarted de WorldDialogue. Los disparos se sincronizan con el comienzo de la escritura y tienen un cooldown de 0,65 segundos. No hay efectos por cada letra ni acumulación al avanzar rápidamente. Los glitches automáticos aleatorios quedan suspendidos durante el diálogo y recuperan su contexto anterior al terminar.

Metadatos especiales, sin cambiar los textos:
- Ella puede verte: música al 25% y efecto leve.
- No... otra vez no...: efecto moderado.
- N0 D3BES... PASAR...: efecto fuerte.
- Mis manos / Se están moviendo solas: música al 12%, sin efectos audiovisuales adicionales.
- Parece que debo matarte: silencio y pausa breve antes del combate.
- Despedida: efectos al 45% y pequeña interferencia al desaparecer, respetando el cooldown.

## Configuración
Seleccionar **Assets/Resources/Combat/MakongPresentation.asset**, referenciado en MakongSpecialSettings > Presentation.

Parámetros: pistas, duraciones de fundido, pausa dramática, ganancias de ambiente/jefe/despedida, Audio Effects, Screen Effects, cooldown, sincronización y los tres niveles con clip, volumen, intensidad y duración. Un clip ambiental nulo permite silencio. Cada línea tiene un Audiovisual Cue opcional: Default, Observed, Distress, Quiet o DramaticCut.

Las dos canciones proporcionadas están enlazadas. Los tres sonidos de interferencia son placeholders originales breves en Assets/Resources/Audio/Temporary: CorruptionLight.wav, CorruptionModerate.wav y CorruptionStrong.wav (0,05 / 0,11 / 0,19 s). Se pueden sustituir desde los campos Clip; no falta ninguna pista musical.

## Pruebas Unity MCP
- Tests/MakongAudiovisualResults.txt: **24 aprobadas, 0 fallos**. Fundidos, pistas, una fuente musical, momentos especiales, cooldown, cámara inmóvil, efectos temporales, pausa, pista protegida frente a SetCombat, ambiente final, restauración y silencio sin clip.
- Tests/MakongNarrativeResults.txt: **30 aprobadas, 0 fallos**. Narrativa completa, pausa, despedida, desaparición, recompensas únicas, guardado/carga y combate en grupo.
- Un combate normal recupera Arcade Showdown y conserva dos AudioSource totales.
- Tests/MakongAudiovisualIntegrity.txt: las 75 líneas, CombatMath, IA/visual de Makong, JUMP, OVERCLOCK y configuración musical general sin cambios. Guardado únicamente en rutas temporales de prueba.
- Consola sin errores en las ejecuciones finales. Unity mostró el aviso de ajuste de sombras puntuales al atlas existente; esta tarea no añade luces. No se hizo una comparativa controlada de GPU/FPS; se reutiliza el renderizado CRT anterior.

## Archivos
Nuevos: DungeonAudio.EncounterMusic.cs, CorruptedEncounterPresentationSettings.cs, CorruptedDialoguePresentation.cs, MakongPresentationInstaller.cs, MakongAudiovisualValidation.cs, el asset y tres WAV. Integración en DungeonAudio, WorldDialogue, HelmetCRTController, MakongSpecialSettings, DungeonPrototype.MakongNarrative/Enemies/Save y limpieza de DungeonPrototype. MovementInterfaceBlocked solo añade el bloqueo durante la pausa narrativa; las duraciones y la navegación se conservan.

Respaldo: **Backups/MakongAudiovisual/20261015-124453**. No se cambiaron ataques, dificultad, sprites, geometría ni habilidades. Otros personajes no reciben estos efectos sin vincular explícitamente un perfil.
