# CODEBOUND — HACK OVERCLOCK

Implementado el 11 de octubre de 2026. OVERCLOCK aparece junto a JUMP a partir de GamerBoy **nivel8**. JUMP conserva nivel1.

## Arquitectura reutilizada

Se revisaron CombatHack, EnemyJumpHack, PlayerLevelSettings, PlayerHackCatalog, menú de combate, selección de objetivos, Input System, CombatMath, DungeonAudio y WristHackScreen/HackDungeonBackdrop. OVERCLOCK deriva de CombatHack y comunica una calidad de ataque mediante AttackQuality, igual que la ruta existente de JUMP. No tiene un controlador de turnos ni una fórmula de daño propia.

El catálogo utiliza ID estable **hack.overclock**, nombre OVERCLOCK, descripción, nivel8 y referencia al script. El desbloqueo y sus avisos ya existentes se reconstruyen desde el nivel, también al cargar o viajar entre mapas. No se guarda un segundo estado de disponibilidad.

## Parámetros editables

Abrir **Editor → OVERCLOCK** para seleccionar `Assets/Resources/Combat/OverclockSettings.asset`:

| Campo | Inicial | Función |
|---|---:|---|
| Duration Seconds | 5 | Duración máxima de la sesión |
| Required Presses | 20 | Pulsaciones para llegar al100% |
| Success Quality | 2.5 | Calidad del ataque al completar |
| Failure Quality | 0.5 | Calidad al agotar tiempo |
| Keyboard Key | Space | Tecla de carga configurable |
| Gamepad Button | South | Botón de mando configurable |
| Result Display Seconds | 0.5 | Feedback visual posterior, sin retrasar daño |
| Feedback Strength | 1 | Intensidad de destello, escala y vibración del panel |
| Play Sounds | true | Activar sonidos existentes |
| Pulse/Success/Failure Sound | Confirm/HackWin/HackFail | Cues existentes, sin añadir recursos de audio |

El nivel de desbloqueo se edita en **Editor → Catálogo de HACKS**, no se duplica en los settings del minijuego. El componente OverclockHack tiene UnityEvents onPulse/onSuccess/onFailure para conectar efectos adicionales. Si se añade el componente a un DungeonPrototype de escena, el catálogo reutiliza ese componente y sus referencias; de otro modo lo crea al desbloquearlo y usa los settings de Resources.

## Reloj y controles

`OverclockModel` recibe segundos de `Stopwatch.GetTimestamp()/Stopwatch.Frequency`, un reloj monotónico. Cuenta pulsaciones y tiempo real, no frames. La sesión toma duración, número de pulsaciones y calidades al comenzar. Una pulsación recibida al alcanzar el límite de tiempo ya no gana.

Dos InputActions tipo Button, teclado y mando, procesan eventos de pulsación. Cada control necesita una transición nueva tras soltarlo. Se ignora una tecla/botón ya mantenido al abrir, incluida la confirmación que abrió el HACK. Mantenerlos no repite carga. Teclado y mando se pueden alternar; una pulsación real de mando puede contar aunque el teclado esté sostenido. Se activan comprobaciones de estado inicial para detectar correctamente la liberación de un control mantenido al entrar.

La interfaz indica ESPACIO o la tecla configurada y el nombre del botón del mando; sin mando conectado utiliza equivalencias A/×, B/○, etc. El menú de combate recibe d-pad y botónSouth por HandleCombatInput existente; botónEast vuelve cuando las reglas actuales lo permiten. El minijuego no ofrece cancelación por ESC o botón de regreso, igual que JUMP: busy y Active bloquean otros comandos.

## Daño y flujo

Al completar20 pulsaciones antes de5 segundos, termina inmediatamente con MAXIMUM POWER y calidad2.5. Al expirar, termina con OVERLOAD FAILED y calidad0.5. El callback se entrega una sola vez con un marcador de rendimiento; ese marcador no es daño fijo.

La ruta existente calcula:

`CombatMath.Damage(PlayerStrength, DEF del objetivo, calidad, sin crítico de ataque normal, multiplicador/configuración existentes)`.

Por ello el daño usa estadísticas actuales y defensa; el cociente final respecto a un ataque normal puede variar ligeramente por redondeo. Se conserva la política de precisión de los HACKS existentes: OVERCLOCK hereda UsesAttackLuck; el asset actual no aplica fallo a HACKS sin regla específica. No se cambiaron esas reglas.

BeginTargetHack captura la instancia objetivo al empezar y vuelve a validarla al terminar. No puede alcanzar otro enemigo si cambia activeEnemy, ni golpear una instancia de respawn que ocupó el mismo índice. Descarta callbacks repetidos o de un objetivo desaparecido. Solo DungeonPrototype aplica daño y continúa respuestas enemigas, muerte, victoria, XP y drops.

El resultado se dibuja durante0.5 segundos mientras el daño ya se está resolviendo; no espera confirmación ni retrasa el ataque. El fondo compartido se restaura antes de devolver el callback. OnDisable libera InputActions y la captura, sin otorgar daño por una interrupción externa. La desactivación del componente es limpieza de carga/escena; no se añadió una opción de cancelación del jugador.

## HUD

OVERCLOCK.exe, panel oscuro, temporizador grande, barra segmentada de azul a rosa, porcentaje y pulsaciones. Cada pulsación provoca escala breve, vibración y destello; la intensidad aumenta con la carga. Se reutilizan el marco de muñeca, colores neón, textura blanca de Unity y fondo congelado existentes. No se añadieron texturas pesadas ni se alteró arte existente. Captura revisada: `Captures/OverclockHUD.png`.

## Pruebas Unity MCP

El registro final contiene **53 comprobaciones aprobadas**, incluidas comprobaciones individuales de las primeras19 pulsaciones.

- Bloqueado enLV7, ejecución bloqueada, disponible enLV8; JUMP disponible enLV1.
- 19 pulsaciones no completan;20 antes del límite completan. No acepta más después de terminar.
- Expiración exacta a5 segundos, pulsación en el límite rechazada y reloj que no retrocede.
- Callback único de éxito/calidad2.5 y fallo/calidad0.5. CombatMath aplica defensa y ambas calidades.
- Dispositivos virtuales reales del Input System: tecla sostenida al abrir no cuenta; una pulsación cuenta; repetir estados mantenidos no carga; soltar y pulsar vuelve a contar. Mando y alternancia entre dispositivos verificados. Veinte pulsaciones por eventos completan sin callback doble, incluso procesadas dentro de la misma ejecución de herramientas.
- Prueba adicional de configuración: teclaR y botónWest funcionaron; Space no contó con R configurada. Se usaron settings temporales y se restauraron los originales.
- Selección por el menú HACK y confirmación de objetivo en un grupo. No permite JUMP simultáneo, otros comandos o cancelación mientras OVERCLOCK está activo.
- Daño real por ResolveCombatDamage: solo se reduce la vida del objetivo elegido. En la última ejecución, calidad2.5 produjo19 y calidad0.5 produjo4 con las estadísticas/defensa de las instancias existentes. Se contrastó el resultado con CombatMath, no con daño fijo esperado.
- Cambio artificial de activeEnemy durante la sesión: el ataque conserva la instancia elegida inicialmente.
- Regreso al turno normal tras la respuesta de los enemigos, manteniendo el objetivo.
- Objetivo retirado temporalmente de la lista: callback descartado sin daño ni excepción. Se restauró la referencia después del test.
- Victoria real con un enemigo cuya vida actual se redujo a1 en memoria: muerte, fin del combate y7XP en la última ejecución, según su nivel y diferencia con GamerBoyLV8. La ruta de drops siguió siendo la existente. Pulsaciones tardías no duplicaron XP ni monedas.
- JUMP real iniciado después: modelo y reloj de ritmo activos, exclusión de suerte conservada. Se cerró por su limpieza existente.
- Compilación y consola final sin errores. Validación del catálogo sin problemas. Unity quedó fuera de Play Mode.

Las pruebas alteraron únicamente estado temporal en Play Mode (nivel del jugador, vida actual y lista de encuentro para crear fixtures), no assets de estadísticas enemigas, pesos, niveles de aparición o mapas. En pruebas no letales se dio vida actual100 al objetivo y HP2000 al jugador en memoria para medir daño sin interrumpir la comprobación. No se guardaron estos fixtures en slots personales. Se usaron dispositivos virtuales, no una sesión humana con mando físico, y no se generó un build standalone. No se jugó una sesión completa de JUMP; se verificó su inicio e integración.

Scripts reproducibles principales: `Tests/OverclockMcpTest.txt`, `Tests/OverclockIntegrationMcpTest.txt`. Registro: `Tests/OverclockResults.txt`. Ejecutar en Play Mode con un dungeon y al menos3 enemigos vivos, Editor sin pausa; los scripts desactivan el Update del dungeon para aislar la prueba. El segundo utiliza las primeras3 instancias vivas, no índices de enemigos que podrían estar muertos en una partida cargada. Las comprobaciones posteriores de turnos/victoria se realizaron en llamadas MCP sucesivas, dejando correr las coroutines reales.

## Archivos

Nuevos:

- `Assets/Scripts/OverclockModel.cs` — contador y límite temporal.
- `Assets/Scripts/OverclockHack.cs` — InputActions, callback de calidad, HUD, efectos y hooks. Preserve mantiene la clase resuelta desde el catálogo en builds con stripping.
- `Assets/Scripts/OverclockSettings.cs` y `Assets/Resources/Combat/OverclockSettings.asset` — parámetros.
- `Assets/Editor/OverclockSettingsMenu.cs` — acceso desde Editor.

Ampliados:

- `Assets/Resources/Combat/PlayerHackCatalog.asset` — OVERCLOCK LV8, conservando JUMP LV1.
- `Assets/Scripts/DungeonPrototype.cs` — mando por la misma ruta de entrada del menú existente.
- `Assets/Scripts/DungeonPrototype.Enemies.cs` — bloqueo de HACK simultáneo, objetivo capturado/validado y callback único. La configuración y ejecución de JUMP se conservan.

Sin cambios: CombatMath, PlayerLevelSettings, curva de experiencia, estadísticas, escalado y aparición de enemigos, Director de Encuentros, zonas, mapas, código de JUMP, sprites y música.


## Presentación con sprites — 12 de octubre de 2026

El HUD inicial fue sustituido por una escena dentro de la pantalla del brazo, usando el mismo JumpParallaxBackdrop estático de JUMP. GamerBoy utiliza los ocho frames de `Assets/sprites/gamerboy overclock.psd`: frames1–7 según porcentaje durante la carga; frame8 al disparar, tanto con carga completa como parcial. El PSD original permanece intacto. Los PNG exportados están en `Assets/Resources/UI/Overclock`, con recorte transparente común597×480, filtro Point y sin deformación por ajuste a potencia de2.

La especie objetivo se transmite desde BeginTargetHack y se dibuja con su primer frame de JUMP, quieta frente a GamerBoy. Conserva la referencia de tamaño utilizada por JUMP. La barra superior muestra únicamente porcentaje, junto al temporizador. Carga y rayo temporales se dibujan mediante OverclockEnergyVisual, con formas pixeladas azul/cian/blanco, brillo y chispas; diámetro y longitud crecen con el porcentaje. No hay texturas grandes nuevas para los efectos.

El disparo visual dura1.1 segundos por defecto (Result Display Seconds, editable0.2–2). La entrega de daño permanece inmediata y única; no se cambian5 segundos,20 pulsaciones, calidad2.5/0.5 ni desbloqueoLV8. Una presentación terminada no tapa otro HACK activo. JUMP mantiene su código y presentación; ambos comparten el marco del brazo y el fondo.

Verificación: regresión del modelo y Input System aprobada; secuencia1,2,3,4,5,6,7,8 comprobada con cargas0/15/30/45/60/75/90/100%; carga parcial40% termina en frame8 con calidad0.5; callback único. Selección de objetivo, ataque real solo al objetivo original y retorno de turnos comprobados mediante las pruebas de integración existentes. Consola sin errores. Capturas revisadas de carga65%, disparo100% y disparo40% en `Captures/OverclockSpriteCharge.png`, `OverclockSpriteBeam.png` y `OverclockSpriteBeamLow.png`. Las capturas detienen temporalmente el reloj visual mediante fixtures del Editor; eso no forma parte del gameplay.

Modificados para esta presentación: OverclockHack.cs, OverclockSettings.cs, OverclockSettings.asset y la referencia de objetivo en DungeonPrototype.Enemies.cs. Nuevo OverclockEnergyVisual.cs y los ocho PNG. Respaldos en Backups/OverclockSprites. No se cambiaron mapas, director, estadísticas, XP ni el PSD original.
