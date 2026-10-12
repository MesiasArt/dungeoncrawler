# CODEBOUND — Progresión automática de GamerBoy

Implementado el 11 de octubre de 2026. Configuración: **Editor → Progresión de GamerBoy**. Asset: `Assets/Resources/Combat/PlayerLevelSettings.asset`.

## Auditoría e integración

- `PlayerCharacterSettings` conserva ATK6/DEF3/LUK4 y los mismos IDs, iconos y bonificaciones del equipo inicial. `CombatInventorySettings.maximumHP` conserva la base HP30. No se ha creado una segunda lista de estadísticas base.
- Los accesos existentes `MaximumHP`, `PlayerStrength`, `PlayerDefense` y `PlayerLuck` incorporan crecimiento. Ataque, hacks, JUMP, defensa, crítico, fallo, huida, pociones y equipo continúan consumiendo esos accesos.
- `EnemyLevelSettings.Evaluate`, `CombatMath`, pesos de `EnemyZoneSettings`, mapas, regiones, respawn y Director de Encuentros no se modificaron para esta fase.
- Se reutiliza `EnemyCombatStats` (nivel, categoría y especie) y `EnemyCharacterCatalog` para categorías. La instancia manual Élite/Jefe se respeta; el catálogo eleva Normal a Fuerte/Subjefe = Élite, o Jefe = Jefe. Un campo no serializado marca la XP registrada por vida, sin tocar estadísticas ni escalado enemigo.
- Nuevos: `PlayerLevelSettings`, parcial `DungeonPrototype.PlayerLevels.cs` y `PlayerProgressionWindow`. Sin distribución manual de puntos.

## Fórmulas y comportamiento

`R(x) = floor(x + 0.5)` para valores positivos, con productos decimales antes de redondear para evitar desviaciones de precisión.

Para nivel L, se redondea el crecimiento acumulado una sola vez:

- HP = base Inventario + R(2 × (L−1)) + equipo.
- ATK = base Personaje + R(0.45 × (L−1)) + equipo.
- DEF = base Personaje + R(0.30 × (L−1)) + equipo.
- LUK = base Personaje + R(0.15 × (L−1)) + equipo.
- XP para pasar de L a L+1 = R(12 × L^1.6). Nivel inicial 1; máximo 50.
- XP enemiga = R(5 × nivel enemigo × multiplicador categoría × multiplicador diferencia).

Categorías: Normal 1; Fuerte/Élite/Subjefe 1.5; Jefe 3. Diferencia enemigo−jugador: ≥5:1.2; 1–4:1.1; 0:1; −1/−2:0.7; −3/−4:0.35; −5/−7:0.1; ≤−8:0. Todos los coeficientes y límites de bandas se editan en el asset de progresión.

Las derrotas acumulan XP pendiente. Se entrega al cerrar una victoria, antes de las recompensas existentes. Huir o perder no entrega esa XP. Todas las recompensas de un grupo usan el nivel del jugador al empezar el encuentro: el orden de las bajas no afecta al resultado. Cada vida enemiga se registra una vez; un respawn crea un componente nuevo y puede volver a dar XP aunque reutilice el identificador persistente. Los encuentros siguientes limpian la recompensa pendiente anterior.

La XP sobrante procesa tantas subidas como corresponda. Al máximo la barra muestra NIVEL MÁXIMO y no acumula más XP. Por defecto no se cura: HP actual permanece igual. Recuperación configurable por subida = HP plano + round(HP máximo final × fracción); nunca supera HP máximo. Una subida múltiple aplica esa cantidad por nivel ganado. Mantiene las bonificaciones de equipo. El aviso discreto LEVEL UP dura cuatro segundos.

## Guardado y compatibilidad

Se amplía `PlayerProgressionRecord` con `level` y `experience` (XP disponible en la barra del nivel actual). Conserva HP actual, inventario, monedas y IDs equipados ya existentes. No se guardan estadísticas derivadas ni XP restante: se reconstruyen con la configuración vigente para evitar datos redundantes. `PlayerTotalExperience` reconstruye XP acumulada total; `XPRemaining` calcula lo que falta. Las partidas anteriores sin los nuevos campos cargan nivel 1 y XP0. Se conserva el formato y slots v1; no se sobrescribieron partidas personales.

La transición entre mapas ya usa Capture/Restore del mismo registro y conserva los nuevos campos. Su flujo fue revisado en código; no se construyó un nuevo mapa para probar una transición física. Cambiar fórmulas después de guardar recalcula estadísticas y normaliza sobrantes con la curva vigente; no se congelan estadísticas históricas.

## Editor

1. Abrir **Editor → Progresión de GamerBoy**.
2. Editar crecimiento, máximo, curva, categorías, bandas de diferencia y recuperación parcial. Los valores base siguen en sus assets existentes.
3. Marcar incluir equipo inicial para comparar la previsualización. Se calculan todas las filas hasta el máximo sin tablas manuales.
4. Seleccionar nivel del jugador, nivel enemigo y categoría para simular XP.
5. Pulsar Guardar configuración. Undo/Redo del Inspector usa SerializedObject. Verificados guardado e importación del asset y persistencia tras recarga de scripts; no se cerró y reabrió toda la aplicación Unity.

## Estadísticas y encuentros por nivel

Equipo inicial: bonificaciones HP+7/ATK+3/DEF+4/LUK+6. La columna normal indica encuentros de dos enemigos normales equivalentes desde XP cero, redondeados hacia arriba. La media encadenada usa grupos de 1–3, categorías y pesos reales de especies de Zona 1, arrastrando XP sobrante entre niveles; enemigos hipotéticamente del mismo nivel que GamerBoy. No se cambiaron los niveles reales de la zona.

| LV | Base HP/ATK/DEF/LUK | Con equipo | XP siguiente | Encuentros 2 normales | Media encadenada |
|---:|---|---|---:|---:|---:|
| 1 | 30/6/3/4 | 37/9/7/10 | 12 | 2 | 1.70 |
| 2 | 32/6/3/4 | 39/9/7/10 | 36 | 2 | 1.94 |
| 3 | 34/7/4/4 | 41/10/8/10 | 70 | 3 | 2.41 |
| 4 | 36/7/4/4 | 43/10/8/10 | 110 | 3 | 2.76 |
| 5 | 38/8/4/5 | 45/11/8/11 | 158 | 4 | 3.15 |
| 6 | 40/8/5/5 | 47/11/9/11 | 211 | 4 | 3.49 |
| 7 | 42/9/5/5 | 49/12/9/11 | 270 | 4 | 3.82 |
| 8 | 44/9/5/5 | 51/12/9/11 | 334 | 5 | 4.02 |
| 9 | 46/10/5/5 | 53/13/9/11 | 404 | 5 | 4.32 |
| 10 | 48/10/6/5 | 55/13/10/11 | 478 | 5 | 4.62 |
| 11 | 50/11/6/6 | 57/14/10/12 | 556 | 6 | 4.85 |
| 12 | 52/11/6/6 | 59/14/10/12 | 640 | 6 | 5.08 |
| 13 | 54/11/7/6 | 61/14/11/12 | 727 | 6 | 5.32 |
| 14 | 56/12/7/6 | 63/15/11/12 | 818 | 6 | 5.60 |
| 15 | 58/12/7/6 | 65/15/11/12 | 914 | 7 | 5.82 |
| 16 | 60/13/8/6 | 67/16/12/12 | 1013 | 7 | 6.06 |
| 17 | 62/13/8/6 | 69/16/12/12 | 1117 | 7 | 6.32 |
| 18 | 64/14/8/7 | 71/17/12/13 | 1224 | 7 | 6.56 |
| 19 | 66/14/8/7 | 73/17/12/13 | 1334 | 8 | 6.68 |
| 20 | 68/15/9/7 | 75/18/13/13 | 1448 | 8 | 6.91 |

Media simulada de nivel 1 hasta llegar a 20: **84.5 encuentros** en el escenario de enemigos equivalentes. Desde XP cero: LV1 ~1.69 encuentros, LV10 ~5.20, LV20 ~7.36. Con XP arrastrada: ~1.70, ~4.62 y ~6.91 respectivamente.

## Pruebas MCP y simulaciones

- 34 comprobaciones en Play Mode: bases nivel1; equipo inicial; crecimiento HP; subida; ausencia de curación completa; sobrante que sube hasta20; equipar/desequipar en20; todas las bandas de diferencia; categorías; grupos1/2/3; registro duplicado; victoria idempotente; fuga; nueva vida/respawn; máximo50; XP al máximo; guardado y restauración de nivel/XP/HP/equipo; migración sin campos; JUMP conserva fórmula y exclusión de suerte; ataque normal consume fuerza escalada; una vida no puede pagar otra vez en otro encuentro; recuperación parcial mediante configuración temporal.
- Prueba adicional de integración: tres bajas por la coroutine real `ResolveCombatDamage`, con enemigos normales LV1 y HP reducido a1 en memoria: 15XP → GamerBoy LV2/XP3 y `battle=false` al terminar. Se ejercitó el punto real de muerte y de victoria. No fue una sesión humana completa de combate o de JUMP.
- 1000 progresiones encadenadas y 1000 muestras desde barra vacía por nivel (1–20), semilla145. Fórmulas y pesos se ejecutaron en Unity MCP. Los grupos son uniformes1–3 y la selección de especies usa el asset existente. No incluye derrota en el cálculo de encuentros; supone victoria.
- 1000 duelos por nivel con enemigo equivalente, equipo inicial y HP completo en cada duelo, ataques normales, fallo y crítico mediante CombatMath. Victoria estimada100% enLV1 y97.1% enLV20; rondas medias3.06 y5.84. Excluye especiales, JUMP, pociones, fuga y acumulación de heridas entre combates. Es una comparación matemática limitada, no una garantía del balance real de grupos.
- Revisión visual del panel de personaje LV20: barra HP, barra XP, nivel, XP restante y equipo. Captura `Captures/PlayerLevel20Card.png`.
- Compilación y consola sin errores. Unity queda fuera de Play Mode después de las pruebas.

Reproducibles: `Tests/PlayerLevelsMcpTest.txt`, `Tests/PlayerLevelsSimulationMcp.txt`; resultados: `Tests/PlayerLevelResults.txt`, `Tests/PlayerLevelSimulation.csv`. Fixture aislado `Tests/PlayerLevelsFixture.json`, sin tocar slots reales. El test debe ejecutarse en Play Mode con el dungeon cargado; deshabilita su Update y utiliza reflexión para aislar el estado. La simulación se puede ejecutar en Edit Mode. Los scripts de prueba no son assets ejecutables del juego.

## Problemas detectados y recomendaciones, sin cambios de dificultad

- Zona1 conserva niveles1–4: a GamerBoy LV12 todos dan0XP. Zona2 conserva1–5: desdeLV13 todos dan0XP. Además, la penalización empieza antes de esos límites. Estas zonas no permiten una progresión continua hasta20 o50; necesitarán contenido posterior con niveles mayores cuando se diseñe, no se ha cambiado ahora.
- El crecimiento es escalonado por redondeo: ATK no aumenta en todas las subidas; por ejemplo LV12 y13 comparten ATK14 equipado. LUK sube lentamente. Es el resultado de los coeficientes solicitados, no un fallo; probar la percepción de progreso antes de ajustarlos.
- XP5 de enemigoLV1 y umbral12 permiten alcanzarLV2 con tres enemigos normales en un combate. Ese inicio es rápido; más adelante la curva requiere más encuentros.
- Categorías fuertes aceleran progresión: Big Tapaku y Gran Makuba usan multiplicadorÉlite del catálogo. El promedio ponderado refleja eso. No se cambiaron pesos.
- El escalado enemigo porcentual puede crecer más rápido que el ATK aditivo del jugador. Los duelos ya duran más en20. Conviene medir grupos y especiales en pruebas de recorrido antes de alterar daño, equipo o coeficientes.
- Sin curación al subir, las heridas se acumulan. Las tasas de victoria de duelos con HP restaurado no representan resistencia de un recorrido sin pociones. Mantener recuperación0 inicialmente y medir el consumo real; usar recuperación parcial si las pruebas lo justifican.
- La XP se difiere hasta victoria para cumplir que huir no otorgueXP. En una retirada tras derrotar parte del grupo se pierden esas recompensas. Confirmar esa experiencia durante playtesting antes de cambiarla.

No se añadieron objetos ni enemigos, y no se alteraron sprites, música, animaciones o minijuegos. Se conservaron los cambios previos de puertas, antorchas y diálogos. Respaldos en `Backups/ProgresionGamerBoy`.
