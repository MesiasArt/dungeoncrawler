# Auditoría de Insert Coin or Die
Fecha: 11 de octubre de 2026. Proyecto: `E:/UNITY PRUEBA/DCG/dungeoncrawler`. Motor Unity 6000.5.4f1 / URP. Auditoría de código y conexiones reales mediante Unity MCP. Identidad visual y controles conservados.

## A. Estado general del juego

| Sistema | Estado y evidencia | Límite actual |
|---|---|---|
| Movimiento/exploración | Movimiento por casillas, giros de 90°, interpolación y bloqueo de paredes en DungeonPrototype.Move. Verificado en Play Mode. | No movimiento libre; mapa fijo. |
| Niveles | DungeonPrototype.Start construye geometría desde un arreglo de 11 × 9 casillas; corredor de combate independiente de 9 m. | No generador de varios niveles; solo SampleScene en build. EnemySizeGallery es una herramienta de comparación. |
| Mapa | RevealVisitedCell descubre la casilla y paredes contiguas; minimapa y mapa ampliado usan explored. Verificado el descubrimiento. | No persiste entre partidas; no mapa global de varios pisos. |
| IA | EnemyTurn se ejecuta tras mover/girar al jugador; deambula, detecta en línea de visión a dos casillas, persigue mediante BFS y respeta puertas/paredes. Comprobada la ejecución del turno. | El estado alerted no se pierde hasta reaparecer; no memoria/retirada de persecución. |
| Apariciones | Reaparece usando un índice muerto; listas acotadas y SpawnRandom selecciona cuatro especies. Respawn configurable, predeterminado 30 s. | La IA queda quieta si el jugador queda quieto. La reaparición se difiere durante combate, intro y movimiento. |
| Combate | Turnos; encuentro de 1–3, selección independiente de objetivo; cada rival vivo responde al turno. Ruta de resolución analizada; golpes normales y especial multi-hit probados en runtime. | Formación global y encuentros pueden reclutar enemigos alejados del contacto inicial. |
| Fight / ATTACK | Usa PlayerStrength, acierto, crítico y defensa del objetivo. | No habilidades de arma adicionales. |
| Hack | Selecciona objetivo y consulta GetComponents<CombatHack>; el único HACK concreto conectado es EnemyJumpHack. | No existen otros minijuegos HACK concretos. Clases abstractas no cuentan como implementación. |
| Item | Comparte itemCounts entre combate y pausa; cura, descuenta cantidad y da turno a los enemigos en combate. | Solo consumibles de curación; no adquisición ni drops conectados. |
| Run / HUIR | Tirada configurable; fracaso consume el turno, éxito restaura el dungeon y cámara. Código conectado a Action. | No se verificó estadísticamente la acción real de huida con miles de pulsaciones; fórmula cubierta por tests. |
| JUMP | Audio DSP/PlayScheduled, salto manual, ventanas PERFECT/GOOD/MISS, animación de fracaso, sprites por rival. Física del modelo probada a 30/60/240 FPS y 18 combinaciones. | La canción no tiene un análisis automático de BPM; calibración manual. El escenario estático es intencional. |
| HP/derrota | HP, muerte del rival, cadáver mantenido durante bark, derrota de jugador y reinicio están conectados. Daño y muerte probados. | Cadáver espera fin del bark de muerte según requisito previo; no interrumpe los golpes normales. |
| Estadísticas | Catálogo por personaje, equipo, tres ranuras y fórmulas comunes. Estadísticas de los cuatro enemigos y jugador comprobadas contra objetos instanciados. | Los cambios de HP/ATK/DEF del enemigo requieren creación/reaparición; algunos parámetros de habilidad consultan catálogo vivo. |
| Inventario/equipo | Pociones, catálogos editables, iconos Texture2D, selección de tres ranuras y equipo inicial. Conservados tus objetos. | Equipo disponible desde configuración; aún no es botín recogido. Sin sistema de venta/compra. |
| UI/pausa | UI IMGUI con tus sprites PSD, panel de personaje, inventario 18 casillas, mapa ampliado; bloqueo de tiempo/audio verificado. | Ajustes sigue siendo marcador “Próximamente”; navegación sin pestañas de opciones reales. |
| Narrativa | Intro 20 líneas, teclas y typewriter; barks contextuales con probabilidad/prioridad/historial de cinco frases. | Solo español conectado; las claves de intro preparan localización futura, no existe un selector/traductor de idiomas. |
| Animación/efectos/audio | Quads animados por frames, luz rosa, horizonte, cámara de impacto, granada, reflejos planos y captura del dungeon durante HACK. Las dos pistas y 22 SFX existen en Resources (0 faltantes). | Presencia de clips no verifica calidad ni sincronía perceptiva en todos los eventos. |
| Guardado/progresión | GameStartContext tiene bandera SavedGame y la puerta dispara pantalla de fin/reintentar. | No hay serialización/carga de partida, EXP, niveles de personaje, drops ni siguiente nivel conectado. Preparar SavedGame solo omite intro. |

## B. Problemas detectados y gravedad

### Confirmados y corregidos

1. **Alta: la defensa anulaba ataques.** Antes `max(0, ATK − DEF)`. Tu equipo inicial lleva DEF 7; enemigos ATK 2/3/4/6. Todos podían hacer 0 incluso acertando. También bloqueaba buena parte de sus críticos y el especial de Big Tapaku. No era un fallo de conversión a entero ni una aplicación duplicada de defensa.
2. **Alta: JUMP ignoraba ATK.** El callback reportaba `Combo × 2` como daño. Se aplicaba DEF como resta; no importaba la fuerza del protagonista. Ahora reporta desempeño y el combate calcula ATK/DEF/calidad; HUD y callback usan la misma fórmula.
3. **Media: la suerte podía producir probabilidades extremas.** Antes 1 punto de LUK sumaba 1% de crítico/huida y restaba 1% de fallo sin cap específico. Ahora críticos, huida y reducción de fallo tienen límites independientes.
4. **Media: HP inicial incompleto con equipo de vida.** Máximo 37, pero Start mantenía 30. Ahora una partida nueva inicia con su máximo real, 37 con tu equipo.
5. **Media: varias pruebas antiguas dependen de estadísticas antiguas.** Algunos scripts externos de Tests esperan Hongo16/Soldado13 y daño fijo20 de JUMP. Sus expectativas ya no describen tus valores editados ni el nuevo daño. Se añadieron suites de balance y runtime; los tests físicos de JUMP conservan los 20 como puntos de desempeño, no como daño final.

### Confirmados y pendientes (fuera del cambio de estadísticas)

- **Alta para una entrega completa:** guardar/cargar y siguiente nivel no existen conectados. No se implementaron durante esta auditoría.
- **Media:** nombres de equipo se usan como identificadores de equipamiento inicial. Renombrar un objeto sin actualizar la asignación puede dejar una ranura inicial vacía; usar GUID/ID estable es la siguiente mejora recomendada.
- **Media:** los barks cortos con ≤5 frases únicas pueden agotar todos los candidatos y quedarse silenciosos indefinidamente, pues el historial solo avanza al hablar. Los bancos actuales amplios evitan este caso; el fallo existe al reducir el banco.
- **Media:** catálogo y componentes de enemigos tienen diferente momento de actualización: HP/ATK/DEF se copian al nacer; suerte/especiales consultan catálogo durante el ataque. Conviene unificar las instantáneas antes de permitir balance en caliente.
- **Baja:** la intro no es un cargador de partidas; el nombre SavedGame podría crear una expectativa falsa de guardado.

### Riesgos y recomendaciones, no fallos de rendimiento demostrados

- Dos reflejos de 1024² ARGBHalf, cámara de sombras y luces por antorcha pueden ser costosos en GPUs modestas. Se actualizan a 2 Hz inmóvil y cada frame al mover; el HACK captura el dungeon y evita esos renders durante el minijuego.
- UI crea GUIStyle/strings en OnGUI; BFS y turnos crean diccionarios/listas. Hay oportunidades de caché/pooling, pero no hay medición de build que permita atribuir lag a una de ellas.
- Materiales wall/floor/ceiling/pink/blue se crean por partida; OnDestroy del prototipo no libera todos expresamente. Revisar ciclos de reinicio con Memory Profiler. No se midió una fuga creciente en build.
- Snapshot de Editor: 343 GameObjects, ~856 MiB de memoria administrada informada por el Editor. Incluye herramientas/MCP y no representa memoria del juego compilado. No se declara FPS de producción.

## C. Estadísticas reales conservadas

| Personaje | Categoría | HP máximo | ATK | DEF | LUK | Especial por golpe × golpes |
|---|---|---:|---:|---:|---:|---|
| GamerBoy sin equipo | Jugador | 30 | 6 | 3 | 4 | JUMP por calidad |
| GamerBoy equipo inicial | Jugador | 37 | 9 | 7 | 10 | JUMP por calidad |
| TAPAKU | Enemigo | 10 | 3 | 2 | 5 | 3 × 1 |
| HONGO | Enemigo | 15 | 2 | 3 | 3 | 2 × 1 |
| SOLDADO | Enemigo | 20 | 4 | 5 | 3 | 4 × 1 |
| BIG TAPAKU | Fuerte | 24 | 6 | 9 | 4 | 3 × 3 |

Equipo conservado: **Power Glove de Jugete** (+3 ATK, +2 LUK), **Tshirt de Pacas** (+5 HP, +3 DEF), **Medias de la suerte** (+2 HP, +1 DEF, +4 LUK). No se corrigieron sus nombres ni se reemplazaron sus valores.
No hay subjefe/jefe instanciable actual; pestañas vacías preparadas para futuras fichas. Big Tapaku pertenece a Fuertes, no se lo presenta como jefe.

## D. Sistema anterior y nuevo

Antes: `daño = max(0, ataque − defensa)`. Crítico se redondeaba antes de restar DEF. JUMP usaba directamente puntos de combo.

Ahora:

```
daño sin redondear = ATK × calidad × crítico / (1 + DEF × K / 100)
daño de un impacto = max(1, redondeo al entero más cercano)
```

**K = 10** inicialmente. DEF 3 reduce ~23.1%; DEF 7 ~41.2%; DEF 9 ~47.4%. Esta escala hace que tu DEF 7 tenga valor sin neutralizar los ataques de 2 HP. No se modificó ninguna estadística del catálogo.

Crítico multiplíca el valor sin redondear; por defecto máximo de multiplicador 2×, usando el 1.5× que ya tienen tus personajes. Se redondea una sola vez con mitades hacia arriba. Se usa double internamente para evitar división entera/overflow, clamp de DEF y parámetros inválidos, y saturación al máximo de int.

**Cero sigue siendo válido** para fallo de acierto, ATK 0, calidad 0 o inmunidad explícita. El mínimo 1 solo se aplica a ataques positivos que impactan. La inmunidad es un parámetro previsto en CombatMath; no existe una especie inmune conectada en este proyecto.

### Suerte acotada

- Crítico: base + `0.5 × LUK`, cap 30%.
- Fallo: base − `min(10, 0.25 × LUK)`, límite inferior 0%.
- Huida del jugador: base + `0.5 × LUK`, cap 85%.
- Con tu equipo: crítico 10%, fallo normal 7.5%, huida 55% (base de huida 50%).
- Enemigos: crítico Tapaku 2.5%, Hongo/Soldado 1.5%, Big Tapaku 2%.
- Drops/eventos: sin sistema conectado, no se inventó una tirada de recompensa. La futura conexión debe usar la misma configuración acotada.

### JUMP

Calidad = `(PERFECT + 0.85 × GOOD) / objetivo × 2.5`, acotada al máximo. MISS no suma un nuevo impacto; se conserva el desempeño conseguido antes del fracaso. Fallar antes del primero da calidad 0 y daño 0. No usa acierto/critico aleatorios de LUK.

Una cadena completa PERFECT hace, con equipo inicial, 19 a Tapaku, 17 a Hongo, 15 a Soldado y 12 a Big Tapaku. Todos GOOD: 16, 15, 13 y 10. El premio al buen desempeño sigue siendo importante, pero aumentar ATK o DEF cambia el resultado. Un PERFECT aislado contra Big Tapaku no equivale a 12: la calidad depende del progreso de la cadena completa.

Configuración: **Editor → Balance de combate**, recurso `Assets/Resources/Combat/CombatBalanceSettings.asset`. Todos los coeficientes y topes se pueden editar. Personajes muestra los valores finales; probabilidades base se conservan en su desplegable.

## E. Simulaciones reproducibles

10,000 duelos por combinación, semilla 110102026. Un jugador realiza ATTACK y, si el rival sobrevive, recibe una respuesta. Sin pociones, huida ni HACK. Soldado usa granada cada tercer turno; Big Tapaku usa tres golpes especiales cada tercero. Críticos y fallos independientes por golpe. Los turnos son acciones del jugador, no segundos; el tiempo real depende del input, animaciones y barks.

“Turnos para morir” simula al jugador sin atacar, con los especiales activos. No es el tiempo que se espera que dure un duelo ganado. “Daño promedio” incluye fallos (0) y críticos.

| Equipo | Enemigo | Impacto normal / crítico | Daño medio por ATTACK | Impacto enemigo normal | Acciones medias del duelo | Turnos enemigos para matar jugador | Victoria |
|---|---|---:|---:|---:|---:|---:|---:|
| Sin equipo | TAPAKU | 5 / 8 | 4.741 | 2 | 2.21 | 16.38 | 100.0% |
| Sin equipo | HONGO | 5 / 7 | 4.677 | 2 | 3.3 | 16.52 | 100.0% |
| Sin equipo | SOLDADO | 4 / 6 | 3.767 | 3 | 5.46 | 11.77 | 100.0% |
| Sin equipo | BIG TAPAKU | 3 / 5 | 2.857 | 5 | 7.05 | 7.16 | 22.07% |
| Equipo inicial | TAPAKU | 8 / 11 | 7.678 | 2 | 2.06 | 20.4 | 100.0% |
| Equipo inicial | HONGO | 7 / 10 | 6.753 | 1 | 3.04 | 40.18 | 100.0% |
| Equipo inicial | SOLDADO | 6 / 9 | 5.828 | 2 | 4.03 | 22.14 | 100.0% |
| Equipo inicial | BIG TAPAKU | 5 / 7 | 4.81 | 4 | 5.35 | 10.25 | 99.98% |

La debilidad de los enemigos normales en 1 contra 1 se conserva; su amenaza aumenta en grupos y por desgaste sin recuperación infinita. Big Tapaku es peligroso sin equipo: victoria ~22%, frente a ~99.98% con equipo. No se aumentaron estadísticas para forzar un equilibrio más duro sin tu decisión.

Sensibilidad con equipo inicial:

| Cambio | Tapaku | Hongo | Soldado | Big Tapaku |
|---|---:|---:|---:|---:|
| Daño normal que recibes con DEF 7 | 2 | 1 | 2 | 4 |
| Con DEF 10 (+3) | 2 | 1 | 2 | 3 |
| Daño normal que haces con ATK 9 | 8 | 7 | 6 | 5 |
| Con ATK 11 (+2) | 9 | 8 | 7 | 6 |

El entero pequeño produce escalones: DEF no cambia cada golpe cada vez que sube un punto, pero la reducción continua se conserva antes de redondear. No es un error de conversión.

Jefes: no se simularon enemigos ficticios. Datos reproducibles: `Tests/SimulateCombat.py`, snapshot `Docs/AuditData/audit-*.json`, resultados `Docs/AuditData/combat-simulations.json`.

## F. Cambios realizados y validación

- **CombatMath.cs**: fuente única de daño, calidad y probabilidades acotadas.
- **CombatBalanceSettings.cs / CombatBalanceEditor.cs**: coeficientes, límites e Inspector accesible desde Editor.
- **DungeonPrototype.cs / .Accuracy.cs**: ambas rutas usan CombatMath; HP inicial máximo real y huida con su propio cap.
- **CombatAccuracySettings.cs / .Pause.cs / CharacterStatisticsEditor.cs**: cálculo compartido de probabilidades y texto actualizado.
- **CombatHack.cs / EnemyJumpHack.cs / EnemyJumpModel.cs / .Enemies.cs**: contratos de calidad, contadores de grades, ATK/DEF del objetivo y preview/HUD coherentes.
- No se alteraron mapas, arte PSD, música, amplitud de antorchas, movimiento, física de salto ni ventanas rítmicas.

Pruebas:

- `Tests/CombatBalanceMcpTests.cs`: **226 assertions PASS** (mínimo, alta DEF, monotonicidad ATK/DEF, críticos, topes, inmunidad, NaN/overflow y calidad de JUMP).
- `Tests/JumpRhythmMcpTests.cs`: PASS 18 cadenas a 30/60/240 FPS y offsets; PERFECT/GOOD/MISS, fallo sin input y colisiones reales del modelo. Los 20 puntos de desempeño no se describen como 20 HP finales.
- `Tests/AuditPlayModeMcpTests.cs`: pruebas de integración mediante MCP; ver evidencia final anexa. Dos primeras ejecuciones del controlador de prueba JUMP fallaron porque seleccionaban NextArrival (siguiente spawn) en lugar del enemigo pendiente; se corrigió el controlador, no la física del juego.
- Compilación y consola revisadas mediante Unity MCP; resultado final anexo.

## G. Próximas prioridades

1. Diseñar e implementar guardado/carga real y transición al siguiente piso. No anunciar continuidad de partidas hasta verificar persistencia completa.
2. Definir objetivos de balance para grupos de 2–3 y para jefes futuros; ajustar solo tras pruebas de juego humanas. El enemigo fuerte sin equipo ya es exigente.
3. Introducir identificadores estables de objetos/enemigos, adquisición y drops antes de economía/equipos amplios.
4. Unificar snapshots de estadísticas enemigas y retirar campos legacy de balance que aún pueden confundir al editar assets directamente.
5. Corregir agotamiento de bancos pequeños de barks y completar localización.
6. Perfilar build en el equipo donde se observa lag (CPU/GPU/GC); optimizar reflejos/luces/UI según evidencia, no rehacerlos por suposición.
7. Convertir los tests MCP existentes a una suite Unity Test Framework con datos configurables, sin expectativas antiguas fijas.

## Evidencia final de Play Mode y consola

La prueba de integración terminó en **PASS**. El registro conservado en `Docs/AuditData/audit-playmode-results.txt` confirma movimiento y paredes, mapa descubierto, pausa/reanudación, los cuatro enemigos conectados al catálogo, daño normal, tres impactos especiales, JUMP con música programada y daño mostrado/final de 17 HP, derrota, reaparición y salida del nivel.

Consulta final de consola mediante Unity MCP: **0 errores**. Se registraron cuatro avisos de URP indicando que reduce a la mitad la resolución de sombras puntuales para encajar seis mapas en el atlas de 2048×2048. Es una degradación visual confirmada; no demuestra por sí sola la causa del lag. No se cambió el atlas sin medir CPU/GPU en una build.

Los snapshots de estadísticas y resultados de simulación se conservaron en `Docs/AuditData`, fuera de Temp. Ejecutar `Tests/SimulateCombat.py` reproduce las simulaciones a partir de esos datos con semilla fija. Las pruebas no equivalen a una medición de FPS en el equipo del jugador ni a una revisión auditiva humana.
