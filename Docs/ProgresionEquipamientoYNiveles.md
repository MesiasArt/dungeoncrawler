# Progresión por equipamiento y niveles de enemigos

Validación reproducible en Unity MCP, 11 de octubre de 2026.

## Estadísticas reales por especie

HP / ATK / DEF / LUK; las bases se leen del catálogo existente.

| Especie | LV.1 | LV.5 | LV.10 | LV.20 | Especial LV.20 |
|---|---|---|---|---|---|
| TAPAKU | 10 / 3 / 2 / 5 | 15 / 4 / 2 / 6 | 21 / 5 / 3 / 6 | 33 / 7 / 4 / 8 | 7 × 1 |
| HONGO | 15 / 2 / 3 / 3 | 22 / 3 / 4 / 3 | 31 / 3 / 5 / 4 | 49 / 5 / 6 / 5 | 5 × 1 |
| SOLDADO | 20 / 4 / 5 / 3 | 30 / 5 / 6 / 3 | 42 / 7 / 8 / 4 | 66 / 9 / 11 / 5 | 9 × 1 |
| BIG TAPAKU | 24 / 6 / 9 / 4 | 36 / 8 / 11 / 4 | 50 / 10 / 14 / 5 | 79 / 14 / 19 / 6 | 7 × 3 |
| MAKUBA LANZA | 18 / 4 / 4 / 3 | 27 / 5 / 5 / 3 | 37 / 7 / 6 / 4 | 59 / 9 / 9 / 5 | 7 × 1 |
| MAKUBA ARCO | 14 / 3 / 2 / 6 | 21 / 4 / 2 / 7 | 29 / 5 / 3 / 8 | 46 / 7 / 4 / 9 | 7 × 1 |
| GRAN MAKUBA | 28 / 6 / 7 / 4 | 41 / 8 / 9 / 4 | 58 / 10 / 11 / 5 | 92 / 14 / 15 / 6 | 5 × 3 |
| MIMIC | 20 / 4 / 6 / 2 | 30 / 5 / 7 / 2 | 42 / 7 / 9 / 3 | 66 / 9 / 13 / 3 | 7 × 1 |

## Distribución de apariciones

10.000 selecciones de especie y nivel por configuración; semillas 20261011–20261016. Tolerancia: seis desviaciones estándar más dos muestras. Especie y nivel usan tiradas independientes.

### Configuración inicial verificada — niveles 1–5, factor 0,65, semilla 20261011
| Nivel | Probabilidad | Muestras / 10.000 |
|---|---:|---:|
| 1 | 39,59406 % | 3988 |
| 2 | 25,73614 % | 2578 |
| 3 | 16,72849 % | 1628 |
| 4 | 10,87352 % | 1100 |
| 5 | 7,06779 % | 706 |

| Especie | Peso | Probabilidad | Muestras |
|---|---:|---:|---:|
| Tapaku | 1 | 14,286 % | 1446 |
| Hongo | 1 | 14,286 % | 1471 |
| Soldier | 1 | 14,286 % | 1389 |
| BigTapaku | 1 | 14,286 % | 1422 |
| MakubaLanza | 1 | 14,286 % | 1446 |
| MakubaArco | 1 | 14,286 % | 1389 |
| GranMakuba | 1 | 14,286 % | 1437 |

### Configuración actual conservada — niveles 1–4, factor 0,65, semilla 20261016
| Nivel | Probabilidad | Muestras / 10.000 |
|---|---:|---:|
| 1 | 42,60532 % | 4334 |
| 2 | 27,69345 % | 2730 |
| 3 | 18,00074 % | 1776 |
| 4 | 11,70048 % | 1160 |

| Especie | Peso | Probabilidad | Muestras |
|---|---:|---:|---:|
| Tapaku | 1 | 25,000 % | 2565 |
| Hongo | 1 | 25,000 % | 2541 |
| Soldier | 1 | 25,000 % | 2438 |
| BigTapaku | 1 | 25,000 % | 2456 |


## Dependencias revisadas y comportamiento

Se revisaron la auditoría anterior, DungeonPrototype y sus parciales de encuentros, precisión, equipo, pausa e inventario; CombatMath; los catálogos y los tres Makubas; EnemyJumpHack y sus modelos; el panel del Editor y los registros de respawn. Las bases actuales de las ocho especies se preservaron. No se alteraron PSD, sprites, poses, música, física ni sincronización de JUMP.

Cada aparición obtiene una especie y un nivel con tiradas independientes. El catálogo contiene únicamente estadísticas base; EnemyLevelSettings calcula una instantánea al nacer. Se fija HP/ATK/DEF/LUK, daño especial, número de golpes, intervalo y probabilidades base del enemigo. Normal y especial usan esa instantánea; editar el catálogo después no altera a un rival que ya existe. El daño especial crece con el coeficiente ATK, desde su propio valor base, con un solo redondeo. Los niveles no cambian la escala visual ni la frecuencia de los especiales.

Configuración inicial de Zona 1, usada en las primeras pruebas: niveles 1–5; factor 0,65; máximo nueve enemigos vivos; respawn de 30 segundos; siete especies con peso 1. Durante esta revisión se cambiaron desde el Editor sus valores a niveles 1–4 y cuatro especies (Tapaku, Hongo, Soldado y Big Tapaku). Se conservaron y guardaron esos ajustes. Makuba Lanza, Makuba Arco y Gran Makuba siguen integrados; basta incluir sus IDs en la lista de la zona para que aparezcan allí. Mimic está excluido del selector aleatorio, incluso si se introduce su ID en una lista. Las posiciones iniciales conservan sus especies cuando la zona las permite; reciben niveles ponderados. Si una especie inicial está prohibida, se usa una especie válida de la zona; si no hay ninguna válida, no se crea un sustituto arbitrario. Las opciones antiguas de Big Tapaku/Makubas controlan las colocaciones iniciales; la lista de la zona controla las nuevas apariciones aleatorias.

Los encuentros siguen reclutando de uno a tres rivales y usan la selección independiente de objetivo para ATTACK y HACK. El máximo cuenta objetos vivos, sin bloquearse por índices muertos. Respawn reutiliza el índice y asigna un GUID de instancia nuevo. El registro derrotado se conserva hasta la reaparición. Pausa mantiene congelado el tiempo restante de respawn.

## Cómo editarlo en Unity

- **Editor → Balance de combate**: fórmula y límites existentes, crecimiento global, especie, nivel de vista previa, base LV.1, estadísticas finales y probabilidades de fallo/crítico finales. En la ficha de especie, activa `overrideGrowth` para usar sus cuatro coeficientes en vez de los globales. No es necesario duplicar fichas por nivel.
- En esa misma ventana, **Zone1**: cambia `minimumLevel`, `maximumLevel`, `rarityFactor`, pesos de especies, `maximumEnemies` y `respawnSeconds`. Debajo aparece la probabilidad normalizada de cada nivel. Peso cero deshabilita una especie. Factor cero produce solo el nivel mínimo; uno produce niveles equiprobables. Los rangos del Inspector se limitan a 1–10.000.
- Los IDs de especies existentes son `Tapaku`, `Hongo`, `Soldier`, `BigTapaku`, `MakubaLanza`, `MakubaArco`, `GranMakuba`. `Mimic` se reserva para cofres. Estos IDs no dependen del nombre mostrado.
- **Editor → Panel de Virtual Dungeon → Enemigos: apariciones**: conserva movimiento, detección, grupos y cadencia; el botón lleva al balance por zona. Respawn y máximo simultáneo antiguos se ocultan para evitar dos controles aparentes de lo mismo.
- Se puede crear otro asset de configuración de zona, sin crear un dungeon, y asignarlo al campo `enemyZoneSettings` de DungeonPrototype. Si está vacío se usa Zone1. No se han implementado volúmenes ni transiciones automáticas entre zonas.
- **Editor → Personajes - estadísticas y equipo**: tres ranuras iniciales por ID; las etiquetas siguen siendo tus nombres. **Editor → Objetos equipables** conserva tus tres objetos, sus iconos, bonos y ranuras; muestra el ID y permite preparar `specialEffectIds`. Estos IDs de efecto son referencias de datos para una implementación futura; no tienen comportamiento todavía.

## Equipamiento, inventario y preparación para adquisición

GamerBoy inicia HP37/ATK9/DEF7/LUK10 con Power Glove de Jugete, Tshirt de Pacas y Medias de la suerte. No hay EXP ni crecimiento de nivel del protagonista. Los identificadores de los objetos y pociones se generan y guardan una vez. La migración de las asignaciones antiguas por nombre se hace una sola vez, evitando que una ranura que vacías voluntariamente se vuelva a llenar.

El equipo se identifica por ID, incluso al renombrar o reordenar el catálogo. Hay tres ranuras equipadas y dieciocho casillas de bolsa. Cada equipo no equipado ocupa una casilla; las pociones se apilan por ID. La bolsa compacta sus entradas para no dejar casillas inaccesibles después de gastar una poción. Un objeto equipado sigue siendo propiedad del jugador, pero ocupa su ranura de equipo, no una casilla de bolsa.

`AcquireEquipment(id)` y `AcquireItem(id, cantidad)` preparan la adquisición durante exploración; rechazan IDs inexistentes, duplicados de equipo y falta de espacio. No se añadieron objetos nuevos, drops ni cofres. `EquipItem(id)` sustituye el objeto de la misma ranura y devuelve el anterior a la bolsa; `UnequipSlot(0..2)` requiere espacio. ENTER o clic en un equipo de la bolsa lo equipa. Las pociones mantienen su uso dentro y fuera de combate.

Cambiar equipo calcula bonos de nuevo y solo limita el HP actual si baja el máximo; subir el máximo no cura. Ejemplo verificado: HP37 → quitar Tshirt → HP32/máximo32 → reequipar → HP32/máximo37.

## Datos preparados para futuro guardado

`CapturePlayerProgression()` devuelve HP actual, inventario con IDs y cantidades, y tres IDs equipados. `CaptureEnemyProgression()` devuelve GUID de instancia, ID de especie y zona, nivel asignado, HP actual, casilla, alerta, derrota, respawn restante y la instantánea de estadísticas. Los registros son serializables; se probó conversión JSON del jugador. No se añadió ningún botón, archivo de partida, carga, EXP ni lógica completa de restauración. Guardar la instantánea permite mantener el balance de una instancia aunque luego se edite el catálogo.

## Pruebas y evidencia

- Suite `Tests/EnemyProgressionMcpTests.cs`: **123.521 comprobaciones PASS**, ocho especies, LV.1 exacto, crecimiento monotónico hasta LV.100, especiales, mínimo de daño, límites de suerte, override por especie y equipo inicial. También comprueba el caso de redondeo de 19,5 a 20.
- **60.000 selecciones**: seis configuraciones ×10.000, semillas 20261011–20261016; Zona1 geométrica, uniforme, especies con pesos 1/3/6 y niveles7–12, nivel fijo, caída rápida de rareza y la configuración actual conservada del usuario. Pesos de especie, niveles e independencia pasaron una tolerancia de seis desviaciones estándar más dos muestras. Resultados completos en [SimulacionesProgresion.md](SimulacionesProgresion.md). Son pruebas de configuración, no nuevos dungeons.
- `Tests/EnemyProgressionPlayModeMcpTests.cs`: **PASS** en SampleScene mediante Unity MCP. Se comprobó spawn de las siete especies, 37/9/7/10, reducción y aumento de HP máximo sin curación, adquisición, renombrado y reordenado, bolsa llena de 18 casillas, sustitución sin espacio adicional, tres rivales en combate, snapshots, respawn válido y preparación de registros. Los objetos adicionales usados para probar bolsa llena son fixtures temporales en memoria; el catálogo real sigue teniendo exactamente tres objetos.
- Gran Makuba LV.20: ATK14, especial5×3; frente a DEF7 produjo **8 HP** en el golpe normal y **3+3+3 HP** en el especial, con críticos/fallos desactivados en el fixture. Cambiar después el catálogo a daño especial999 y suerte999 no modificó esos golpes. Los assets se restauraron al salir de Play Mode.
- JUMP real con entradas SPACE, audio DSP y cadena completa contra Hongo LV.5: usó DEF4 y aplicó **16 HP** al ganar. JUMP conserva su exclusión de suerte y críticos aleatorios. El minijuego no se modificó.
- Compilación Unity sin errores; bases y tres objetos verificados de nuevo después de salir de Play Mode. Evidencia permanente en [ProgresionPlayMode.txt](ProgresionPlayMode.txt).

## Archivos nuevos y modificados en esta fase

Nuevos: `Assets/Scripts/EnemyLevelSettings.cs`, `EnemyZoneSettings.cs`, `DungeonPrototype.Progression.cs`; `Assets/Editor/EnemyProgressionEditor.cs`; assets `Assets/Resources/Combat/EnemyLevelSettings.asset` y `Assets/Resources/Exploration/Zone1.asset`; dos suites `Tests/EnemyProgression*`; este informe, simulaciones y evidencia. Unity generó sus archivos `.meta`.

Modificados: `EnemyCharacterCatalog.cs`, `EnemyCombatStats.cs`, `PlayerCharacterSettings.cs`, `CombatInventorySettings.cs`, `EnemyEncounterSettings.cs`, `DungeonPrototype.cs`, parciales `Enemies`, `Accuracy`, `Equipment`, `Pause`, `InventoryGrid`; `NeonDungeonUI.cs` para mostrar LV sin romper el retrato; editores `CombatBalanceEditor`, `CharacterStatisticsEditor`, `EquipmentItemsEditor`, `VirtualDungeonEditorWindow`; assets de catálogo de enemigos, personaje y consumibles solo para guardar IDs, coeficientes por defecto y metadatos de migración. Las bases, bonos, nombres, cantidades e iconos se conservaron. CombatMath, minijuegos y recursos artísticos no fueron modificados por esta fase.

## Problemas y siguientes prioridades

1. Corregida la consulta del catálogo en caliente para especiales y suerte; ahora la instancia es coherente durante su vida. Corregida la identificación de equipo por nombre y preparada adquisición con propiedad/capacidad.
2. Unity advierte que reduce resolución de sombras para alojar seis luces puntuales en el atlas de 2048. No es un error de compilación ni de niveles; conviene perfilar sombras en un build antes de modificar iluminación. No se atribuye un FPS ni una mejora de rendimiento a estas pruebas.
3. Hay una sola zona de dungeon activa. Siguiente desarrollo: regiones/transiciones de zona y encuentros diseñados; luego cofres y adquisición conectada (incluido Mimic), efectos de equipamiento y finalmente guardado versionado con restauración completa. No forman parte de esta fase.
4. La rareza de especies fuertes sigue siendo editable, con peso inicial igual al de las demás para conservar la selección previa. Actualmente las cuatro especies permitidas tienen probabilidad 25% cada una; el nivel se selecciona por separado. Conviene ajustar pesos y rango tras sesiones de juego reales; no se cambiaron arbitrariamente bases ni coeficientes de daño.
5. Ataques especiales solo se activan donde las animaciones y reglas existentes lo permiten. Las fichas de Arco/Mimic conservan los campos de especial preparados, sin inventar movimientos nuevos.
