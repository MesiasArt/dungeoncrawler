# Makong: narrativa y fuente española

## Resultado
Se instalaron las 51 líneas introductorias, 21 de derrota y tres de epílogo suministradas por el usuario. Se conserva el sistema WorldDialogue, la primera persona y el combate existente, sin voces, cinemáticas ni sprites nuevos. Los nombres de la presencia que lo controla no se revelan.

La introducción termina iniciando el encuentro existente. Su marca de visto se guarda por mapa mediante los eventos persistentes de la Fase 4. El jefe derrotado no vuelve a reproducirla. El laboratorio conserva su flujo de pruebas de combate sin introducción narrativa.

La derrota registra al jefe con la lógica existente y conserva su sprite muerto en pantalla. Al terminar el encuentro se muestran sus últimas palabras, se ejecuta la desaparición habitual y se presenta el epílogo de Gamerboy; después se entregan las recompensas normales una sola vez y se restaura la exploración. Si quedan enemigos vivos, mantienen sus turnos y el texto espera a que termine ese encuentro. Si se huye de esos enemigos después de haber derrotado al jefe, se cierra su secuencia narrativa antes de completar la huida; la regla existente de XP al huir no cambia.

## Fuente
- Nueva: `Assets/Resources/UI/Fonts/CodeboundPixelSpanish.ttf`.
- Original conservada: `ThaleahFat_TTF.ttf`.
- Añadidos: ÁÉÍÓÚÜÑ / áéíóúüñ, ¿ y puntuación habitual de UI. El signo ¡ existente se conserva.
- Acentos, diéresis y tilde construidos en la cuadrícula de 64 unidades del TTF. Los glifos ASCII y sus avances no cambian.
- La UI carga esta variante mediante NeonDungeonUI y mantiene la fuente original como respaldo.
- Generador reproducible: `Tools/generate_spanish_font.py`. Requiere fonttools; la dependencia de desarrollo se instaló solo en `Library/CodeboundFontTools`, fuera del contenido del juego.

## Configuración
`Assets/Resources/Combat/MakongSpecialSettings.asset`: `Encounter Dialogue`, `Defeat Dialogue`, `Defeat Epilogue` y `Narrative Enabled`. No se cambiaron sus valores de IA, especiales o dificultad.

Cada WorldDialogueLine admite:
- `Corruption`: Normal, Light, Moderate, Controlled.
- `Writing Speed`: 0 utiliza la velocidad anterior del juego.
- `Pause Before`: pausa opcional antes de escribir; confirmar permite saltarla.
- `Character Chance` / `Flicker Strength`: -1 utiliza los valores generales.
- `Recovery Seconds`: 0 utiliza el valor general.
- `Interrupt After Characters`: 0 muestra toda la línea; un valor positivo limita su presentación sin modificar el texto fuente.

`Assets/Resources/UI/DialogueCorruptionSettings.asset`: permite desactivar efectos, reducir intensidad, ajustar probabilidades por nivel, recuperación, parpadeo y pequeñas pausas durante la escritura. El efecto solo afecta al texto y una franja del cuadro; no dispara CRT ni glitches de pantalla completa.

Las líneas anteriores son normales por defecto. Los metadatos se copian al paginar; el texto original y los nombres no se corrompen. La presentación no consume el generador aleatorio del gameplay. Confirmar revela inmediatamente el texto limpio, y otra confirmación avanza. Los errores escritos explícitamente en el guión, como N0 o D3BE, forman parte del texto suministrado y se conservan.

## Pruebas Unity MCP
`Tests/MakongNarrativeResults.txt`: **29 aprobadas, 0 fallos**. Incluyen fuente española, texto y etiquetas intactos, efectos desactivables, RNG sin cambios, NPC anterior, revelado inmediato, avance rápido, inicio del combate, estado de introducción, sprite visible durante la despedida, desaparición antes del epílogo, XP y premios únicos, guardado/carga con jefe derrotado, controles y cámara restaurados. También se probó la derrota de Makong dentro de un grupo sin alterar los turnos de los enemigos restantes.

Regresión adicional: **58/58** de Fase 4, incluyendo diálogos encadenados, persistencia, seis slots temporales, viajes y Digital Kingdom. Consola sin errores durante las ejecuciones finales. Las pruebas escribieron únicamente `Tests/MakongNarrativeSave.json` y los archivos temporales del fixture de Fase 4.

Comprobaciones de integridad en `Tests/MakongNarrativeIntegrity.txt`: CombatMath, IA de Makong, su visual, JUMP, OVERCLOCK, fuente original y los parámetros existentes de especiales intactos. Captura real revisada: `Captures/MakongNarrative/SpanishDialogue-Unity.png`; lámina de glifos: `SpanishFont.png`.

## Archivos y respaldo
Nuevos: `DialogueCorruptionSettings.cs`, `DungeonPrototype.MakongNarrative.cs`, `MakongNarrativeValidation.cs`, `Editor/MakongNarrativeInstaller.cs`, fuente y configuración de efectos. Integración en WorldDialogue, NeonDungeonUI, DungeonPrototype.Enemies, DungeonPrototype.Refuge, DungeonPrototype, DungeonPrototype.Save y MakongSpecialSettings. El JSON `Tools/MakongNarrative.json` conserva el texto entregado; el menú de instalación es explícito y no sobrescribe automáticamente las ediciones del Inspector.

Respaldo previo: `Backups/MakongNarrative/20261015-120747`.

Las partidas antiguas sin marca de introducción podrán verla si el jefe sigue vivo. Si ya estaba derrotado, se respeta esa derrota y no se vuelve a crear ni se conceden recompensas retrospectivas. No se guarda la letra exacta de un diálogo en curso; se conservan los estados persistentes del encuentro.
