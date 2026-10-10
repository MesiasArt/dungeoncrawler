# Dungeon Crawler

Prototipo en Unity 6000.5.4f1 con URP. Abre el proyecto en Unity y pulsa Play en SampleScene. El prototipo se genera al iniciar la escena.

- W/S o flechas arriba/abajo: avanzar y retroceder por casillas.
- A/D o flechas izquierda/derecha: girar 90 grados.
- Acercarte a un enemigo inicia combate por turnos. Usa los botones para atacar, habilidad, poción o huir.
- Tres enemigos, 30 HP y tres pociones. Al derrotar todos completas el nivel.

Las paredes usan `Assets/Resources/Textures/textura azul luffy2.jpg`; el piso usa `textura rosa luffy2.jpg`. Las referencias visuales están en `References` y el PSD original en `ArtSource`.

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
