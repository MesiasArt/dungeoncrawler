# Dungeon Crawler

Prototipo en Unity 6000.5.4f1 con URP. Abre el proyecto en Unity y pulsa Play en SampleScene. El prototipo se genera al iniciar la escena.

- W/S o flechas arriba/abajo: avanzar y retroceder por casillas.
- A/D o flechas izquierda/derecha: girar 90 grados.
- Acercarte a un enemigo inicia combate por turnos. Usa los botones para atacar, habilidad, poción o huir.
- Tres enemigos, 30 HP y tres pociones. Al derrotar todos completas el nivel.

Las paredes usan `Assets/Resources/Textures/textura azul luffy2.jpg`; el piso usa `textura rosa luffy2.jpg`. Las referencias visuales están en `References` y el PSD original en `ArtSource`.

El techo también usa la textura rosa. Las superficies usan el shader URP
`Dungeon/Metal Toon`, con sombras en tres bandas, reflejos metálicos y emisión
suave. El piso refleja las paredes mediante una cámara plana debajo del suelo
(`DungeonFloorReflection`), con perspectiva actualizada al moverse y girar.
La captura HDR de 1024 píxeles se actualiza también a 2 Hz cuando el jugador
está quieto. Las paredes y el techo no reciben esa reflexión. Los parámetros
Metallic, Smoothness, Neon, Anime shadow y Floor reflection strength controlan
el acabado. Los paneles azules del techo se quitaron.

Esta primera versión usa sentinelas geométricos provisionales y una interfaz básica. Faltan sprites definitivos, audio, balance, guardado y acabado visual. La habilidad hace 10 de daño sin coste por ahora; el ataque hace 7.

Git excluye los directorios temporales de Unity. Conserva y versiona los archivos `.meta` junto a sus recursos.

## Introducción de Virtual Dungeon

Al entrar en Play o reiniciar una partida nueva aparece la introducción en el mismo cuadro de mensajes. E (`Player/Interact`) o Enter (`UI/Submit`) avanza una línea; en mando se usan las acciones equivalentes configuradas en el proyecto. Mantener el botón no salta líneas. El movimiento y las acciones quedan bloqueados hasta confirmar el último diálogo. Las pausas duran 0,8 s antes de la primera voz desconocida, 0,75 s de interferencia antes de CONNECTION LOST y 0,4 s después.

El guion español está en `Assets/Scripts/IntroSequence.cs`, con claves `intro.01`–`intro.20` para una futura tabla de localización. El proyecto todavía no tiene sistema de idiomas ni cargador de partidas.

Integración del futuro menú/cargador, antes de cargar la escena:

- Partida nueva: `VirtualDungeon.GameStartContext.PrepareNewGame()`.
- Partida guardada: `VirtualDungeon.GameStartContext.PrepareSavedGame()`; el cargador restaura sus datos por su propia vía. Esta entrada únicamente omite la introducción, no implementa la lectura de un archivo guardado.
- La entrada se consume una sola vez; cambiar de escena no repite la secuencia.

Pruebas de regresión: ejecuta `Tests/RunIntroTests.ps1` con PowerShell. Usa el SDK que incluye Unity y deja los resultados temporales en `Temp/IntroTests`. Comprueba partida nueva/cargada, continuación de escena, avance manual, pausas, interferencia y cierre/liberación al confirmar la última línea.

Prueba de integración Play Mode: `Tests/IntroPlayModeTests.cs` se ejecutó en una copia temporal aislada con Unity Test Framework. Verifica el pasillo inicial, bloqueo de movimiento/combate, avance por interacción, cierre del cuadro, recuperación del movimiento y omisión al cargar. Resultado: 1 prueba aprobada, 0 fallos. La ejecución sin ventana usa ajustes de foco solo en el test y los restaura al finalizar.


## Combat / Enemy Jump

Advance introduction dialogue with Space, or skip the full introduction with Enter, then approach a sentinel. From the initial
position, the route W W D W W A W W A W reaches the sentinel near the left corridor. Combat stays
in the current scene. Arrow keys select ATTACK/HACK/ITEM/HUIR and submenu entries; Enter or Space confirms. Escape/Backspace or VOLVER returns to the main menu. ATTACK deals 7 damage and triggers the existing enemy response; HACK opens the hack list; selecting Jump starts Enemy Jump. Space or
the JUMP button jumps from the floor. Land on red enemies while descending to
receive a small bounce. Press Space/JUMP within 0.09 seconds of the landing to
boost into a high bounce and reach the more widely spaced next enemy. Holding
the key does not count; every landing requires a fresh press. Ten consecutive
stomps confirm ten damage; a side collision
or missed enemy ends the attempt and confirms one damage per defeated enemy. The result returns to combat
automatically after 1.8 seconds (or press VOLVER AL COMBATE). The sentinel then
responds using the existing five-HP counterattack.

Select Assets/Resources/Combat/EnemyJumpSettings.asset in Unity to configure the
target count, speed, gravity, jump/bounce force, spacing, spacing variation and
speed increase per combo, weak bounce force and landing press tolerance. Values use a logical 900x360 arena independent of screen
resolution. Spawn spacing is clamped to the bounce flight time to keep chains
reachable. The UI uses the project's existing IMGUI system and primitive colored
rectangles, with no new sprites or scene changes. CombatHack is the extension
point for future minigames; only confirmed damage is returned to combat.

Regression snippets in Tests/EnemyJumpMcpTests.cs and
Tests/EnemyJumpReachabilityMcpTests.cs can be run with Unity MCP execute_code.
They check gravity, jump gating, stomp/bounce, miss/side collision, partial damage
on failure, ten-hit victory, and reachable timing across 30 random seeds.

Normal attacks have a short camera shake on impact and a stronger shake when the
sentinel retaliates. CombatImpactFeedback on Player Camera controls their
amplitude and duration. The offset is removed when each shake ends.

Combat inventory is configured in Assets/Resources/Combat/CombatInventorySettings.asset.
Small, medium and large potions heal 25/50/100 HP and start with 3/2/1 units.
Healing is capped at maximumHP (30 by default); full health does not consume an
item. Using an item consumes one unit and the existing enemy response turn.
HUIR exits combat without spending a turn and restores the exploration heading.
New hacks derive from CombatHack, override DisplayName and Begin, and are added
as components on Dungeon Prototype. The HACK menu discovers these components.
Enemy Jump mechanics are unchanged by the submenu/inventory integration.


## Exploration and level exit

The existing minimap now remembers visited cells and their immediately adjacent
walls. Unvisited corridors, rooms and enemy markers remain hidden. Exploration
is updated only after a completed grid step and resets on retry.

The exit is at cell (9,8), behind a door reached from (9,7), far from spawn (1,1).
The outer-right corridor provides access. Near the closed door, E starts its
sliding opening animation. Its collider and grid traversal block passage until
fully open. Crossing toward the exit fades the screen to NIVEL COMPLETADO with
Reintentar and Volver al menu. Arrow keys and Enter/Space operate those options.
The temporary main menu stays in the same scene and provides JUGAR.

Configure Assets/Resources/Exploration/ExitDoorSettings.asset in the Inspector:
initiallyOpen, openingSpeed, completesLevel, fadeDuration. DungeonDoor can be
reused with an assigned panel, collider and settings; its local +Z is the exit
side. Retry rebuilds the generated dungeon in the current scene, preserves
configuration references, and resets the map, door, camera and completion state.
