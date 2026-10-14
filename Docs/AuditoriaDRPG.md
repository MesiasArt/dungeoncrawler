# CODEBOUND — Auditoría DRPG y plan por fases

Estado: auditoría y plan entregados; Fases 1, 2 y 3 autorizadas e implementadas. Las fases 4–7 permanecen pendientes. Digital Kingdom pertenece al encargo previo, autorizado e implementado por separado.

## Evidencia y límites
Se revisaron scripts, assets y herramientas de Unity. Se ejecutaron pruebas MCP de la ciudad, cambios reales de escena, guardado, Continue y controles. Las pruebas de movimiento preciso se documentan en Tests/MovementOriginalRestored.txt. Las fórmulas, JUMP, OVERCLOCK, IA y balance se inspeccionaron por código; esta auditoría no repite todas sus pruebas ni los declara nuevamente verificados.

BFS de geometría del mapa actual: 448 casillas de suelo, todas conectadas desde X17/Y2; Makong X11/Y53 y puerta del reino X11/Y66 conectados. El BFS ignora llaves, puertas cerradas y ocupación por NPC/cofres: acredita conectividad geométrica, no ausencia de bloqueos de progresión. Resultado detallado en Tests/DrpgAuditTopology.txt.

## FASE A — Sistemas existentes, huecos y riesgos

| Sistema | Base que se reutiliza | Estado y huecos | Riesgo |
|---|---|---|---|
| Cuadrícula y movimiento | DungeonMapSettings, DungeonPrototype.Move, DungeonMovementSettings | Suelo/pared/vacío, giros de 90°, colisiones y controles originales restaurados. La espera del turno enemigo pertenece al ritmo anterior; no se vuelve a cambiar velocidad/cámara. | Alto si se altera la secuencia de turnos. |
| Zonas y población | DungeonRegionSettings, EnemyZoneSettings, EnemyLevelSettings, Director de Encuentros | IDs, regiones seguras, pesos, rangos/listas de nivel, separación de enemigos, límites y respawn existentes. Hay inconsistencias entre geometría y regiones. | Medio; puede cambiar dónde aparecen enemigos. |
| Puertas y pisos | DungeonMapEntity, DungeonDoor, TravelMapDoor | Puertas normales, llaves, transiciones, orientación y estado abierto persistente. Las transiciones entre assets no guardan snapshots independientes por piso. | Alto por posible pérdida de estado al regresar. |
| Mapa automático | explored, RevealVisitedCell, minimapa, mapa de pausa, revelación por teleport | No existe un registro independiente de casillas visitadas: exploración y descarga del mapa comparten un booleano. Faltan símbolos de objetos en el mapa de juego. | Bajo/medio si se amplía conservando explored. |
| Enemigos de exploración | EnemyTurn, SeesPlayer, PursuitStep, PlacedEnemyDefinition | Estáticos con holdPosition; paseo aleatorio/persecución por BFS, guardianes de cofres y élites por categoría. No hay rutas de patrulla ni detección configurable por instancia ni política de reinicio de piso explícita. | Alto si se sustituye IA; extender con valor Legacy por defecto. |
| Cofres y botín | DungeonChestKind, DungeonEntityState, OpenMapChest, CombatDropSettings | Normal, Mimic, objeto clave, contenido fijo y guardianes; animación y notificación existentes. No hay tabla específica por cofre, llave propia de cofre o condición/trampa genérica. | Medio por duplicación de recompensas. |
| Progresión | PlayerLevelSettings, PlayerCharacterSettings, PlayerHackCatalog, inventario/equipo | XP, crecimiento configurable, máximo de nivel, HACKS por nivel y IDs de eventos preparados. Curación actual por nivel: healMaximumFraction=1. No hay perfiles reales de varios personajes ni recompensas de exploración genéricas. | Alto si se cambian números; conservar coeficientes y equipo. |
| Jefes | MakongTactics, MakongSpecialSettings, MakongEnemyVisual, persistencia de colocaciones | Condiciones por HP, probabilidades, enfriamientos, invocación y consumo de aliados existentes. El adaptador de combate sigue especializado en Makong; no hay catálogo genérico de fases/eventos de jefe. | Alto; mantener Makong como adaptador Legacy. |
| Zonas seguras y ciudad | Refugio, savepoints, nodos, Digital Kingdom | Guardado, cura, red de nodos y Continue de ciudad funcionales. No se añadió un nodo de ciudad automáticamente ni se cambiaron conexiones existentes. | Medio por interacción de eventos narrativos. |
| Interacciones/eventos | InteractMapEntity y flujo del refugio | Condiciones específicas de llave/guardián, conversaciones únicas y eventos secretos. No hay interruptores ni interfaz común de condiciones/acciones. | Medio/alto; añadir adaptadores, no reemplazar interacciones. |
| Guardado | DungeonSaveData, DungeonSaveStore, seis slots y backups | Jugador, HP/Bits, equipo, XP, enemigos, objetos, nodos, exploración y ciudad. No registra por separado visited, interruptores o mapas de múltiples pisos. returnMap de ciudad identifica el asset por nombre; para pisos futuros se necesita un ID estable explícito. | Alto; migración compatible imprescindible. |
| Procedural | DungeonMapSettings y región/entidades separados | El mapa sigue siendo manual. No hay generador. Se puede consumir la misma definición de mapa desde un proveedor futuro. | Alto si el generador modifica el asset manual; usar instancias independientes. |
| Diagnóstico | ValidatePlan, validadores de regiones/entidades, pruebas de balance | Hay validadores parciales, pero el botón de plano no consolida todos los diagnósticos ni hace BFS condicionado por llaves. | Bajo para un informe de solo lectura. |

### Hallazgos prioritarios

1. **43 errores en regiones:** 36 coordenadas asignadas a regiones son paredes y 7 casillas de suelo están sin asignar: X2/Y1, X6/Y5 y X14–18/Y23. El detalle completo contiene las coordenadas de paredes en Tests/DrpgAuditTopology.txt. No se reasignaron por conjetura.
2. Al pintar Vacío se retira la casilla de las regiones, pero al pintar Pared el editor actual no hace la misma limpieza. Esto permite repetir la inconsistencia. El validador del plano y el de regiones también están separados.
3. El validador del plano incluye entidades y objetos heredados: su informe adicional está en el mismo archivo. Un objeto heredado sobre pared debe revisarse antes de moverlo; el diseño manual reciente puede haber cambiado sus accesos.
4. **Estado entre pisos:** TravelMapDoor limpia explored y ApplyMapTravel sustituye registros de enemigos por los del nuevo mapa. No hay diccionario de snapshots por mapa. La ciudad ya usa un snapshot completo y una escena Single, pero ese puente no equivale a un sistema general de varios pisos.
5. **Visitado vs descubierto:** los guardados antiguos no permiten reconstruir con exactitud qué suelo se recorrió y qué se reveló mediante teleport. La migración debe reconocer esa limitación, no inventar visitas.
6. La base del jugador y el equipamiento siguen separados. La curación completa al subir está activada por el asset actual. No se propone modificar balance automáticamente.
7. Una máquina de estados de exploración de enemigos no debe cambiar acciones, fórmulas o minijuegos del combate. El sistema especializado de Makong debe conservarse hasta contar con pruebas equivalentes de un adaptador.

## FASE B — Plan de implementación (Fases 1, 2 y 3 autorizadas y completadas)

### Fase 1 — Diagnóstico consolidado y sincronización de regiones (prioridad inicial)
- Integrar validadores actuales y BFS de solo lectura en una ventana de diagnóstico.
- Detectar IDs duplicados, objetos sobre paredes, destinos inválidos, eventos ausentes y accesibilidad de interacciones desde casillas adyacentes.
- Evitar nuevas asignaciones inválidas al pintar paredes/vacíos.
- Preparar una reparación revisable de las 36 asignaciones sobre paredes. Pedir definición de zona para las siete casillas sin asignar cuando sea ambigua.
- Archivos: DungeonMapSettingsEditor.cs, DungeonRegionsEditor.cs, DungeonEntitySettings.cs y nuevo coordinador Editor. Assets de regiones solo si se aprueba su reparación; no cambiar geometría.
- Pruebas: mapa fixture con errores conocidos, Undo/Redo, persistencia de reparaciones, comparación del mapa antes/después.

### Fase 2 — Persistencia por piso y mapa visitado
- Añadir mapId estable a la configuración y snapshots por mapa al guardado.
- Separar visited de explored; mantener explored para minimizar regresiones.
- Migrar partidas antiguas al mapa principal, con visited conocido solo cuando exista evidencia.
- Reutilizar el puente de cambio de escena de la ciudad como adaptador, no como segundo sistema de guardado.
- Archivos: DungeonMapSettings.cs, DungeonSaveStore.cs, DungeonPrototype.Save.cs, MapEntities.cs y DigitalKingdomTransit.cs.
- Pruebas: dos mapas, ida/vuelta, cofres/puertas/jefes intactos, guardado desde ciudad y carga de partida antigua.

### Fase 3 — Símbolos y conexiones en el mapa
- Puertas abiertas/cerradas, cofres abiertos/cerrados, savepoints, nodos y transiciones, solo en casillas descubiertas.
- Consumir DungeonEntityState y los IDs existentes; no mantener una copia del estado visual.
- Archivos: DungeonPrototype.Minimap.cs, PauseMap.cs y configuración de leyenda UI.
- Pruebas: actualización después de abrir objetos, geometría cuadrada y resolución variable.

### Fase 4 — Condiciones y eventos reutilizables
- Interfaz de condición (llave, enemigo derrotado, interruptor, evento o HACK) y acciones configurables.
- Adaptadores para puertas/cofres/NPC actuales; eventos únicos con ID persistente.
- Interruptores y atajos solo en mapas de prueba, sin colocar nuevos objetos de historia sin autorización.
- Archivos: adaptadores nuevos, DungeonEntitySettings.cs y extensiones puntuales de MapEntities.cs/guardado.
- Pruebas: evento único, condiciones combinadas, referencias inválidas y restauración.

### Fase 5 — Comportamientos de exploración
- Perfil Legacy por defecto; Static, Patrol, Pursuit, Guardian y Elite visibles configurables por instancia.
- Patrulla por casillas y detección local; usar colisiones/BFS/espaciado existentes.
- Política explícita de respawn/reinicio/derrota permanente por colocación.
- Archivos: PlacedEnemyDefinition, EnemyTurn/adaptador, EnemyEncounterSettings y registros de IA.
- Pruebas: rutas, puertas, zonas seguras, estados y ausencia de cambios en combate.

### Fase 6 — Cofres y arquitectura extensible de jefes
- Tablas de cofre y condiciones sin reemplazar CombatDropSettings.
- Contenido de historia fijo; tablas aleatorias para contenido secundario.
- Definiciones de fases/eventos y adaptador Makong Legacy; no cambiar sus ataques actuales.
- Archivos: configuración de cofre/condiciones, adaptadores de jefe y editor de validación.
- Pruebas: recompensa única, llaves garantizadas, eventos de victoria y ataques existentes.

### Fase 7 — Perfiles de personajes y preparación procedural
- Perfiles configurables que consuman crecimiento/inventario existentes; no añadir personajes ni estadísticas todavía.
- Propuesta: IMapProvider produce una definición de filas/regiones/entidades. ManualMapProvider envuelve assets actuales; ProceduralMapProvider e HybridMapProvider serían futuros proveedores con semilla/versionado y snapshots separados.
- No escribir un generador, no crear pisos procedurales ni modificar la mazmorra manual.

## Autorización y alcance
Las Fases 1, 2 y 3 fueron autorizadas por el usuario y completadas. Las fases 4–7 siguen pendientes de autorización.

El encargo establece: «Después de presentar la auditoría y el plan, espera mi autorización para comenzar». Las fases posteriores a la Fase 3 permanecen como propuesta; cada fase requiere autorización y verificación antes de la siguiente.


## Fase 1 — Resultado final

- Ventana `Editor > Diagnóstico DRPG`: consolida validadores de objetos, regiones y BFS geométrico desde el inicio real. El botón de validar y guardar del plano usa el mismo diagnóstico.
- Detecta IDs duplicados, objetos fuera del suelo, destinos inválidos, referencias de guardianes/NPC/Mimics ausentes, zonas de destino inexistentes, suelo desconectado y falta de casillas de aproximación para interacciones. Incluye objetos heredados activos.
- Pintar Pared o Vacío retira todas las asignaciones de zona de la casilla. Geometría y región se restauran juntas con Undo/Redo. Los marcos y pintura de zonas solo asignan suelo; no asignan paredes ni vacío.
- El editor de zonas permite seleccionar el plano físico que está validando y avisa antes de guardar regiones inválidas.
- Se eliminaron 36 asignaciones sobre paredes, sin alterar el terreno. Se asignaron X2/Y1 y X6/Y5 a `zone-1`; X14–18/Y23 a `zone-2`, por coincidencia inequívoca con sus vecinas. Respaldo: `Backups/DrpgPhase1`.
- Resultado: **43 incidencias antes, 0 después; 448/448 casillas de suelo conectadas geométricamente**. Undo/Redo, guardado e importación comprobados.
- Fixture: 13 comprobaciones aprobadas de detección y edición. Play Mode: cinco comprobaciones aprobadas de presencia del mundo, zonas reparadas, diagnóstico y conservación del movimiento original. Consola sin errores de compilación ni ejecución durante estas pruebas. Resultados: `Tests/DrpgPhase1Results.txt`.
- **Límite del BFS:** no simula llaves, puertas cerradas, guardianes ni ocupación de actores. La conectividad geométrica no demuestra que una ruta de historia respete todas sus condiciones; esa simulación no se añadió en esta fase.
- No se cambiaron estadísticas, enemigos, IA, combate, guardados ni la geometría en esta fase. Digital Kingdom conserva su escena independiente; sus resultados anteriores están en `Tests/DigitalKingdomSceneMcpResults.txt`.


## Fase 2 — Persistencia independiente por mapa y visitas

### Resultado y esquema

Implementación completada con `DungeonMapSnapshot`, integrado en el mismo `DungeonSaveData` y `DungeonSaveStore`. No existe un segundo sistema de guardado para pisos o ciudad. Los seis slots, sus nombres de archivo y backups se conservan. El lector admite versiones 1 y 2; los guardados nuevos escriben versión 2.

Cada `DungeonMapSettings` tiene `mapId` persistente. Se genera una vez a partir del GUID del asset y no depende de su nombre. El editor muestra el ID en la configuración del plano; el diagnóstico detecta IDs ausentes o duplicados. Para un mapa nuevo copiado de otro asset, seleccionar la copia y usar **Assets > Codebound > Dar ID nuevo a copia de mapa** antes de utilizarlo en partidas. No cambiar IDs de mapas publicados: invalidaría sus vínculos de guardado.

El guardado conserva el jugador, inventario, equipamiento, progreso narrativo global del refugio y ciudad una sola vez. La lista `maps` contiene por piso: posición/dirección local, enemigos y sus IDs/HP/derrota/temporizador restante, encuentros activados, estados de cofres/puertas, puertas heredadas, descanso restante del Director, `explored` y `visited`. Los campos antiguos de estado local siguen presentes como representación del piso activo para compatibilidad con el puente de ciudad existente; se sincronizan al capturar, sin crear otro controlador de persistencia.

- **explored:** casillas descubiertas, incluidas las reveladas por nodos y los bordes visibles.
- **visited:** solo suelo al que GamerBoy llegó físicamente. Girar o revelar un mapa no añade visitas. Caminar, aparecer en una entrada o teletransportarse a una casilla sí registra esa llegada.
- Al salir de un piso se captura su snapshot. Al regresar se restaura, sin sustituir sus enemigos por una población nueva. El primer acceso inicializa únicamente la configuración del destino.
- Cargar un slot desde otro piso reconstruye el mapa identificado por el guardado, sin validar sus coordenadas contra el piso equivocado.
- Digital Kingdom conserva y transporta la misma lista de snapshots. Su retorno tiene `returnMapId`; se mantiene `returnMap` antiguo para migración. La casilla exacta de retorno de la puerta se establece antes de capturar el snapshot.

### Respaldo y migración

Respaldo inicial: **Backups/DrpgPhase2/20261013-115110** (`Project`, `Saves` y `Manifest.json`). Incluye scripts afectados, datos de exploración/ciudad y todos los archivos personales de guardado encontrados. Las copias auxiliares fuera de Resources recibieron únicamente `mapId`; sus representaciones anteriores a ese campo se documentaron por separado en `PreIdCopies`.

Una partida versión 1 sin identidad se vincula al mapa principal, o al retorno de ciudad cuando proporciona esa referencia. Conserva `explored` y todos los datos del jugador. **No transforma la exploración antigua en visitas**: solo registra la posición guardada válida, cuya ocupación es conocida. La migración ocurre en memoria y no sobrescribe un slot hasta que el jugador guarda explícitamente. Las puertas antiguas que nunca almacenaron su estado físico conservan sus valores iniciales; no puede reconstruirse una apertura que el archivo no registró.

### Pruebas realizadas mediante Unity MCP

- `Tests/DrpgPhase2Results.txt`: **26 comprobaciones funcionales aprobadas**, más un marcador de finalización. Transiciones reales controlador A→B→A, primer acceso, aislamiento de cofres/visitas, enemigo herido con ID conservado, enemigo derrotado sin duplicación, estados de puerta/cofre/eventos, guardar y cargar versión 2 en los seis slots temporales, carga desde otro piso, revelación sin visitas, puente de ciudad, retorno y persistencia del otro piso. Diagnóstico de Fase 1 y configuración original de movimiento aprobados.
- `Tests/DigitalKingdomSceneMcpResults.txt`: **30 aprobadas, 0 fallos**. Puerta de Makong, escena Single, descarga del mundo, entrada exacta de retorno, controles, templo, guardar/cargar en ciudad, destinos desbloqueados y Continue.
- `Tests/DrpgPhase2LegacySlots.txt`: **9 aprobadas**. Lectura/migración del slot personal versión 1 y cinco slots vacíos; carga real de la partida antigua conserva nivel, XP, HP y equipo, con una única visita confirmada.
- `Tests/DrpgPhase2Integrity.txt`: geometría y regiones principales idénticas al respaldo; archivo legado, slot personal y sus cinco backups sin alteraciones. Las pruebas escribieron exclusivamente en `Tests/Phase2Save.json` y `Tests/CitySceneSave.json`.
- Consola final sin errores de compilación ni ejecución. Durante desarrollo las pruebas detectaron y permitieron corregir la serialización de listas y el orden de captura del retorno de ciudad; los informes citados contienen la ejecución final.

Mapa de prueba B: `Assets/Resources/DrpgPhase2Tests/FloorB.asset`, regiones y objetos asociados. Es independiente del dungeon principal, no tiene conexiones añadidas en la historia. Fixture Editor: `DrpgPhase2Validation`; ejecutarlo sobre SampleScene en Play Mode con `Run()` para repetir las comprobaciones. Los estados de combate se preparan en los registros del fixture: no se presentan como una repetición de los minijuegos o del balance de combate.

### Archivos modificados

- Datos: `DungeonMapSettings.cs`, `DungeonSaveStore.cs`, nuevo `DungeonMapPersistence.cs`.
- Integración: `DungeonPrototype.Save.cs`, `DungeonPrototype.MapEntities.cs`, `DungeonPrototype.cs`, `DungeonPrototype.SaveSlots.cs`, `DungeonPrototype.Regions.cs`.
- Ciudad: `DungeonPrototype.CityModel.cs`, `DungeonPrototype.DigitalKingdom.cs`, `DigitalKingdomTransit.cs`, `DigitalKingdomSettings.cs`.
- Editor: `DungeonMapSettingsEditor.cs`, `DungeonMapDiagnostics.cs`, nuevo `DungeonMapIdEditor.cs`.
- Fixtures: nuevo `DrpgPhase2Validation.cs` y assets de `Resources/DrpgPhase2Tests`.
- Identidad: campo `mapId` en assets existentes de mapa; geometría y configuraciones de aparición sin cambios en esta fase.

### Límites y configuración futura

1. Para restaurar en una carga fría, un mapa debe estar en Resources o ser alcanzable desde un mapa de Resources mediante referencias `destinationMap`. Un ID guardado sin asset disponible produce un error de carga explícito; no se sustituye silenciosamente por otro piso.
2. Los snapshots conservan el tiempo restante de respawn al salir; ese temporizador queda suspendido mientras el piso no está activo. Las reglas de respawn y la IA activa no se modificaron.
3. Los eventos narrativos del refugio y los nodos desbloqueados mantienen su persistencia global existente. Los encuentros activados y objetos se conservan localmente por piso. Mantener IDs estables de NPC y nodos al configurar eventos nuevos.
4. Las transiciones ordinarias entre pisos siguen reconstruyendo el controlador en la escena dungeon existente. Digital Kingdom mantiene su escena independiente. No se introdujo un sistema de escenas por cada piso, ni generación procedural.
5. `visited` se guarda, pero no se añadió una nueva apariencia/leyenda del mapa. Esa UI pertenece a fases posteriores.
6. No se modificaron movimiento, geometría principal, estadísticas, CombatMath, IA ni minijuegos. La duración original sigue en 0,18 s para paso y giro, sin continuo, repetición o buffer.

**Fase 2 finalizada. Detener aquí; no comenzar Fase 3 sin autorización.**


## Fase 3 — Símbolos y conexiones del mapa

### Implementado

Minimapa y mapa de pausa comparten el dibujo de símbolos de píxeles, sin texturas nuevas ni estado persistente adicional. `VisibleMapMarkers()` construye datos de vista a partir de los objetos del mapa activo, sus IDs, `DungeonEntityState`, puertas existentes y `refuge.unlockedNodes`. No crea entradas de estado durante el dibujo.

- Puertas cerradas: marco naranja cerrado; abiertas: marco verde abierto.
- Cofres disponibles: cofre amarillo; abiertos: cofre gris con tapa elevada. Un Mimic abierto cuyo objeto desapareció deja de mostrar icono.
- Savepoints: símbolo verde de guardado, incluidos los heredados del refugio.
- Nodos sin activar: rombo morado vacío; desbloqueados: rombo rosa lleno.
- Conexiones entre pisos: escalera blanca para puertas `Transition` con `destinationMap` distinto del mapa actual. La entrada de Digital Kingdom tiene un arco blanco propio. Una franja naranja indica una conexión que sigue cerrada.
- Solo aparecen objetos cuya casilla está en `explored`, dentro del mapa activo y del encuadre. Descubrir mediante un nodo permite verlos; no exige haber visitado físicamente la casilla.
- Suelo descubierto: azul oscuro; visitado: cian más claro. `visited` nulo en datos antiguos no se transforma en recorrido inventado; la migración existente solo conserva su visita confirmada.
- El jugador utiliza una flecha de píxeles escalada dentro de su casilla.
- Leyenda en dos filas en el mapa de pausa. El minimapa no incorpora una leyenda adicional. Pasar sobre un icono en el mapa de pausa muestra su nombre y el destino cuando es una conexión.

La cuadrícula lógica no cambia. La altura del área de mapa de pausa se ajustó de 405 a 355 píxeles de referencia para alojar la leyenda sin tapar los botones existentes; se conserva zoom, arrastre, mapa lateral y los controles anteriores.

### Corrección encontrada en pruebas

Activar un nodo ejecutaba `RefreshMapEntityStates` y podía cerrar una puerta recién abierta cuya apertura aún no se había capturado. La actualización ordinaria ahora respeta la puerta viva; `RestoreDungeonSave` solicita explícitamente aplicar los estados guardados al restaurar. No se cambian llaves, condiciones de acceso, costes, movimiento ni persistencia de snapshots.

### Configuración

En `Assets/Resources/UI/DungeonUILayout.asset`:

- `Map Symbols`: activar/desactivar símbolos.
- `Map Legend`: mostrar/ocultar leyenda de pausa.
- `Discovered Floor` y `Visited Floor`: colores de descubierto y visitado.

La figura de cada símbolo se define en `DungeonPrototype.MapSymbols.cs`. Las conexiones consumen las referencias de puertas actuales; no se colocaron nuevas puertas ni escaleras físicas.

### Pruebas finales Unity MCP

- `Tests/DrpgPhase3Results.txt`: **24 aprobadas**. Ocultación de objetos no descubiertos, consulta sin escrituras, animación real de cofre, llave/apertura real de puerta, activación de nodo, savepoint heredado, entrada del reino, conexión entre pisos, descubierto/visitado, restauración de símbolos, Mimic desaparecido, partida antigua y encaje del símbolo en casillas de 4, 18, 32 y 60 píxeles de referencia.
- `Tests/DrpgPhase3TravelResults.txt`: **3 aprobadas**. Piso B muestra su cofre abierto; regresar a A elimina los símbolos de B; regresar a B restaura símbolos desde su snapshot.
- `Tests/DrpgPhase3Resolutions.txt`: **3 aprobadas**, renders reales a **1280×720, 1920×1080 y 1024×768**. Dimensiones de PNG comprobadas y capturas inspeccionadas visualmente: leyenda, botones y cuadrículas sin solapamientos.
- `Tests/DrpgPhase2Results.txt`: regresión completa nuevamente aprobada (**26 comprobaciones funcionales y un marcador de finalización**), con seis slots temporales, enemigos/objetos/eventos, ida/vuelta, ciudad, Fase 1 y movimiento original.
- `Tests/DrpgPhase3Integrity.txt`: geometría y regiones sin cambios. La comparación inicial de los archivos personales fue idéntica al respaldo; en la comprobación final el slot 1 tenía un guardado versión 2 posterior (2026-10-13T17:36:47Z, LV7, X15/Y3) y su rotación normal de backups. Se conservó ese archivo reciente, sin restaurar una versión anterior. Las pruebas no incluyen escrituras deliberadas en slots personales.
- Consola final sin errores. Durante la verificación visual el fixture de selección de resoluciones necesitó corregir sus índices y limpiar las resoluciones temporales de manera acotada; se recuperó el editor y se repitieron correctamente las capturas finales. No se instaló un script de reconexión automática.

Capturas: `Captures/MapSymbols/Map-1280x720.png`, `Map-1920x1080.png`, `Map-1024x768.png`. Los fixtures preparan descubrimiento amplio solo en memoria para mostrar símbolos; no revelan la partida personal.

### Archivos de esta fase

- Nuevo `Assets/Scripts/DungeonPrototype.MapSymbols.cs`.
- `DungeonPrototype.Minimap.cs`, `DungeonPrototype.PauseMap.cs`, `DungeonUILayout.cs`.
- Ajuste puntual de actualización/restauración: `DungeonPrototype.MapEntities.cs`, `DungeonPrototype.Save.cs`.
- Fixtures Editor: `DrpgPhase3Validation.cs`, `DrpgPhase3TravelValidation.cs`, `DrpgPhase3VisualValidation.cs`.
- Informe, resultados y capturas citados. Respaldo de scripts antes de cambiarlos: `Backups/DrpgPhase3`.

### Límites

- El proyecto no tiene un tipo de entidad Escalera independiente: el icono de escalera representa la conexión entre mapas ya configurada en una puerta. No se inventa si el destino es un piso superior o inferior, pues no hay un índice de piso que lo determine.
- En el mapa lateral completo, donde una casilla es menor que 7 píxeles de referencia, se usa un punto del color del símbolo para evitar que una figura diminuta invada casillas vecinas. El mapa principal con zoom y el minimapa muestran las figuras completas cuando hay tamaño suficiente.
- Los iconos de enemigos del minimapa conservan su comportamiento anterior. No se añadió IA, información de combate ni nuevas reglas de descubrimiento.
- No se implementan condiciones/eventos de la Fase 4.

**Fase 3 finalizada. Detener aquí; Fase 4 requiere autorización nueva.**


## Checkpoint de Fase 4 — en curso

La implementación de condiciones AND/OR, acciones, recibos de recompensas, interruptores y estados en snapshots está guardada como trabajo en curso. Se crearon `DungeonInteractionSettings.cs`, `DungeonSwitch.cs`, `DungeonPrototype.Interactions.cs` y `DungeonInteractionValidation.cs`, con adaptadores opcionales en entidades y refugio. El respaldo previo está en `Backups/DrpgPhase4/20261013-134641`.

La compilación fue comprobada durante el desarrollo, pero todavía faltan los fixtures y las pruebas de ejecución, guardado/carga, cambio de piso y regresión de las fases anteriores. La creación del mapa de prueba quedó pendiente al interrumpirse la sesión. **No se declara la Fase 4 completada ni validada para producción.** No se colocaron interruptores ni perfiles en la mazmorra principal. No comenzar Fase 5.

Este checkpoint se registra a petición del usuario para hacer commit y push. Los guardados temporales de pruebas y las escenas `_Recovery` permanecen locales.
