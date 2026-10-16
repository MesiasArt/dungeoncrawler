# CODEBOUND — Auditoría DRPG y plan por fases

Estado: Fases 1–7 autorizadas e implementadas. Fases 6–7 verificadas el 16 de octubre de 2026; resultados y límites al final. Digital Kingdom pertenece al encargo previo, autorizado e implementado por separado.

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

## FASE B — Plan de implementación (Fases 1–7 autorizadas)

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
Las Fases 1–5 fueron autorizadas por el usuario y completadas. El 16 de octubre de 2026 el usuario autorizó completar la Fase 6 y continuar inmediatamente con la Fase 7, sin otra aprobación.

El encargo establece: «Después de presentar la auditoría y el plan, espera mi autorización para comenzar». La autorización posterior explícita cubre las Fases 6 y 7 en ese orden. No se extiende a escribir un generador ni a añadir contenido a la mazmorra principal.


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


## Fase 4 — Condiciones y eventos reutilizables: completada

La base del checkpoint anterior se terminó y verificó el 15 de octubre de 2026. El trabajo se limita a la Fase 4: no se implementó la Fase 5 ni generación procedural.

### Sistema implementado

- `DungeonInteractionSettings`: perfiles con ID estable, condición de una sola ejecución y adaptación opcional de la interacción original. Los grupos `All` (AND) y `Any` (OR) pueden combinarse entre sí sin escribir código.
- Condiciones: cantidad de objeto/llave o posesión de equipamiento, HACK desbloqueado y funcional, interruptor activo, evento completado y enemigo/jefe derrotado. Admiten negación y referencia a otro mapa para estados locales.
- Acciones: abrir/cerrar puertas, activar/desactivar interruptores, desbloquear puertas/atajos o nodos existentes, mensajes SYSTEM, diálogos existentes, recompensas, completar eventos y ejecutar perfiles narrativos anidados.
- `DungeonSwitch`: componente con ID y casilla, conectado al controlador de exploración existente. Solo se activa mediante una interacción válida y cercana. El editor del plano ofrece Interruptor y muestra sus propiedades específicas.
- Puertas, cofres, NPC, terminal y nodos heredados conservan su funcionamiento cuando no se les asigna un perfil. Los perfiles pueden reemplazarlo o continuar con él. La continuación espera a que terminen los diálogos configurados.
- El progreso por mapa conserva interruptores, conexiones desbloqueadas, eventos terminados, cursores de ejecución, recibos de recompensa y derrotas de colocaciones con ID estable. Todo se serializa dentro de los snapshots existentes, sin otro gestor de guardado ni otros slots.
- El registro de una derrota añade únicamente metadatos de evento a la colocación nombrada. Sus fórmulas, turno, XP, IA y respawn siguen intactos. Una condición de derrota ya registrada permanece cierta después del respawn.

### Correcciones del checkpoint

1. Los diálogos ya no dejan que las acciones posteriores se ejecuten antes de cerrarlos. Su callback reanuda la secuencia y los eventos anidados en el orden correcto.
2. Cada acción avanza su cursor después de completarse. Ante inventario lleno o referencia inválida, el evento queda pendiente y puede reintentarse sin repetir las recompensas anteriores.
3. Se impide la reentrada de un evento pendiente y se bloquean ciclos de eventos. Cargar una partida invalida callbacks del mundo anterior.
4. Se completaron las continuaciones de adaptadores de NPC, puertas y nodos. Las puertas heredadas de salida/pasillo también admiten el adaptador opcional.
5. Los interruptores de otros mapas consultan su snapshot; si todavía no existe un estado explícito, respetan el valor inicial configurado.
6. El diagnóstico admite grupos malformados sin lanzar excepciones y reconoce contradicciones entre cantidades mínimas de objetos.

### Editor y configuración

1. Crear un asset con **Create > Virtual Dungeon > Interacción y evento**.
2. Mantener su ID estable. Configurar `Groups`, `Join` y `Actions`. Las acciones nuevas necesitan IDs diferentes; el Inspector incluye un botón para añadirlas con ID nuevo.
3. Asignar el perfil al campo `Interaction` del objeto en el plano. Para objetos heredados, usar `Legacy Interactions` con su ID en la configuración del mapa.
4. `Continue Legacy` permite ejecutar después el comportamiento anterior del objeto. Desactivarlo si el perfil es dueño de toda la interacción. Una puerta con llave sigue respetando su llave; para desbloquearla mediante un mecanismo, usar la acción explícita `UnlockConnection`.
5. Declarar eventos narrativos auxiliares en `Event Ids`. Una referencia de condición a otro piso utiliza `Source Map`; los objetivos de acciones pertenecen al mapa activo.
6. **Editor > Diagnóstico DRPG** integra la validación de perfiles, destinos, IDs, ciclos, grupos imposibles, HACKS no funcionales y dependencia de un interruptor sobre sí mismo cuando es comprobable estáticamente.

Los assets de demostración están en **Assets/Resources/DrpgPhase4Tests**: dos pisos, interruptores, puerta, atajo, nodo, cofre condicionado, NPC y secuencias narrativas. Se generan con **Editor > DRPG > Fase 4 > Preparar mapas independientes**. No se colocaron interruptores ni perfiles nuevos en el mapa principal. Los fixtures de ejecución se lanzan en Play Mode sobre SampleScene mediante `DrpgPhase4Validation.Run()`; los de diagnóstico usan `DrpgPhase4DiagnosticTests.Run()`.

### Pruebas mediante Unity MCP

| Evidencia | Resultado |
|---|---|
| `Tests/DrpgPhase4Results.txt` | **58 aprobadas, 0 fallos**: las cinco clases de condición, AND/OR y negación, interruptor real adyacente, puerta y cofre existentes, atajo/nodo, recompensas únicas, diálogos secuenciales/anidados, reentrada, reintento tras error, checkpoint al cargar, seis slots temporales, ida/vuelta entre dos mapas, compatibilidad antigua y movimiento preciso. Incluye transición real a Digital Kingdom, guardado/carga en ciudad, retorno al piso y prevención de premios duplicados. |
| `Tests/DrpgPhase4Diagnostics.txt` | **21 aprobadas**: referencias inválidas de cada tipo, puertas/conexiones/acciones inexistentes, ciclos, OR vacío, AND contradictorio, dependencia imposible del propio interruptor, IDs duplicados, cantidades incompatibles, grupo nulo y diagnóstico principal sin incidencias. |
| `Tests/DrpgPhase2Results.txt` | Regresión de **26 comprobaciones funcionales aprobadas**, más marcador final: snapshots, seis slots temporales, visitas, migración y puente de ciudad. |
| `Tests/DrpgPhase3Results.txt` | Regresión de **24 comprobaciones aprobadas**: objetos descubiertos/ocultos, símbolos, aperturas reales, nodos, restauración y tamaños de casilla. |
| `Tests/DrpgPhase4Integrity.txt` | **29 assets protegidos, siete archivos personales de guardado y seis scripts de sistemas protegidos sin cambios** frente al respaldo previo. |

La primera extensión del fixture de ciudad detectó una referencia a su controlador anterior durante la descarga de escena. Se corrigió la espera del fixture para comprobar también la escena activa y se repitió la suite completa: 58/58 y consola sin errores en la ejecución final. No era un fallo del sistema de persistencia.

Las pruebas escriben únicamente en rutas temporales bajo `Tests/Phase4Save.json` y `Tests/Phase2Save.json` con sus slots derivados. No sobrescriben partidas personales. La Fase 1 sigue integrada en el diagnóstico: el mapa principal y ambos fixtures válidos dan cero incidencias. No se repite la matriz completa de resoluciones de la Fase 3 porque esta fase no cambia su dibujo.

### Respaldo y archivos

Respaldo original del checkpoint: `Backups/DrpgPhase4/20261013-134641`. Respaldo antes de completar esta fase: **Backups/DrpgPhase4Completion/20261015-112706**, con scripts, assets, partidas personales y manifiestos SHA-256.

- Base de la fase: `Assets/Scripts/DungeonInteractionSettings.cs`, `DungeonSwitch.cs`, `DungeonPrototype.Interactions.cs` y `Assets/Editor/DungeonInteractionValidation.cs`.
- Adaptadores/esquema existentes ampliados: `DungeonEntitySettings.cs`, `DungeonMapSettings.cs`, `DungeonMapPersistence.cs`, `DungeonSaveStore.cs`, `DungeonPrototype.MapEntities.cs`, `DungeonPrototype.Refuge.cs`, `DungeonPrototype.Save.cs`, `DungeonPrototype.CityModel.cs`, `DungeonPrototype.cs`, `DungeonPrototype.Preservation.cs` y registro de derrota en `DungeonPrototype.Progression.cs`.
- Integración del editor existente: `DungeonMapSettingsEditor.cs` y `DungeonMapDiagnostics.cs`.
- Fixtures nuevos: `Assets/Editor/DrpgPhase4FixtureBuilder.cs`, `DrpgPhase4DiagnosticTests.cs`, `Assets/Scripts/DrpgPhase4Validation.cs` y los assets bajo `Assets/Resources/DrpgPhase4Tests`.
- Informe y resultados citados. La geometría/distribución, los assets de estadísticas y balance, las fórmulas y los minijuegos se conservaron.

### Límites documentados

- No cambiar IDs ni reordenar acciones de un evento publicado que pueda tener una ejecución pendiente: el cursor conserva el índice de la siguiente acción.
- Al cargar durante un diálogo, la interacción pendiente se reanuda al activarla otra vez y vuelve a presentar ese bloque de diálogo. No guarda la letra o página exacta; sí conserva los premios ya recibidos.
- La validación estática detecta referencias y contradicciones locales comprobables; no demuestra la solución de cualquier puzle ni realiza una búsqueda global de todas las combinaciones de eventos.
- Una partida antigua no permite reconstruir derrotas históricas de enemigos que ya reaparecieron antes de existir este registro. Se conserva la evidencia de derrotas presente en sus registros y se registran las futuras, sin inventar eventos pasados.
- Los recibos de recompensa son por mapa, fuente de interacción, perfil y acción. Reutilizar un perfil en dos objetos distintos permite una recompensa por objeto; para una recompensa compartida, usar un evento anidado único o una condición de evento completado.

**Fase 4 completada. Detener aquí. No comenzar la Fase 5 sin autorización.**


## Fase 5 — Comportamientos de exploración (2026-10-15)

Autorización: el usuario indicó «vamos a la fase 5». Respaldo previo: `Backups/DrpgPhase5/20261015-160412`, con scripts, assets de exploración y manifiesto SHA-256. La Fase 6 no se ha comenzado.

### Resultado y configuración

Se amplió `PlacedEnemyDefinition` con `exploration`, sin sustituir `EnemyTurn`, las colisiones, el BFS ni el sistema de combate. Los enemigos aleatorios y todas las colocaciones publicadas conservan Legacy. No se reasignaron perfiles ni rutas en la mazmorra principal. Makong conserva `holdPosition` y su carácter único.

Para configurar: **Editor > Zonas y encuentros > Enemigos manuales / eventos > colocación > Exploration**, o seleccionar el asset de regiones en el Inspector. El drawer ahora también muestra `holdPosition` y las recompensas garantizadas que ya existían.

| Perfil | Comportamiento fuera del combate |
|---|---|
| Legacy | Paseo aleatorio, detección y persecución anteriores, con sus valores originales. |
| Static | Permanece inmóvil. El encuentro por proximidad sigue usando el flujo existente. |
| Patrol | Recorre las casillas absolutas de `patrol`, mediante pasos BFS. Puede invertir la ruta al final o recorrerla en bucle. No abandona la patrulla para perseguir. |
| Pursuit | Detecta y persigue; recuerda la última casilla vista durante unos turnos. Puede tener una ruta de reposo opcional. |
| Guardian | Persigue dentro del radio de su casilla de origen y regresa a ella al perder la alerta. |
| Elite | Persecución con ruta opcional y rombo magenta visible durante exploración. No cambia la categoría de XP ni sus estadísticas. |

`holdPosition` prevalece sobre cualquier perfil. La detección puede exigir línea recta o seguir una ruta local de navegación; ninguna modalidad atraviesa paredes, puertas cerradas o zonas seguras. Las rutas respetan la ocupación y separación existentes. Por defecto los perfiles nuevos permanecen en su región de origen.

En `EnemyEncounterSettings`, valores generales nuevos: detección 4 casillas, memoria 3 turnos y radio del guardián 6 casillas. Los campos correspondientes de cada colocación usan 0 para heredar esos valores. Estos campos no alteran `detectionSteps` ni `moveSeconds` de Legacy. El temporizador opcional de respawn por colocación usa 0 para heredar el de la zona.

### Persistencia y reaparición

- **Legacy:** conserva `respawn`, `unique` y la categoría originales.
- **TimedRespawn:** usa el bucle de reaparición existente, sus límites y las restricciones del Director. Permite su propia casilla reservada; no permite aparecer junto al jugador ni atravesar restricciones. Cuando `countsTowardLimit` es falso, esa colocación explícita no cuenta para el límite de población.
- **ResetOnReentry:** devuelve a la casilla de origen y restaura la vida al volver realmente a ese mapa, incluido el retorno desde la ciudad. Cargar una partida en el mismo mapa no lo reinicia.
- **PermanentDefeat:** la derrota persiste al guardar, cargar y regresar al piso.
- `unique` y Boss impiden reaparecer/reiniciarse incluso si se configura por error otra política. El diagnóstico avisa de esa contradicción.

Los campos de IA se añaden a `EnemyInstanceRecord`, dentro de los snapshots por `mapId` ya existentes: origen, punto y dirección de patrulla, memoria y última posición vista. `hasExplorationState` permite reconocer guardados anteriores: reconstruye el origen de la colocación, sin inventar historial de patrulla. No hay otro almacén de guardado ni cambios en los seis slots. La historia de derrotas de la Fase 4 se conserva aunque una colocación permita reaparecer.

### Pruebas y resultados

| Evidencia | Resultado |
|---|---|
| `Tests/DrpgPhase5Results.txt` | 40 comprobaciones aprobadas en Play Mode: perfiles, turnos reales, alineación, ruta inversa/bucle, puertas, paredes, zonas seguras, ocupación, memoria, compatibilidad del paseo Legacy con la misma semilla, guardado/carga, partidas antiguas, respawn con y sin Director, marcador de élite y recorrido entre dos mapas. |
| `Tests/DrpgPhase5Diagnostics.txt` | 15 comprobaciones aprobadas: rutas vacías, puntos sobre pared, destinos desconectados, valores inválidos, contradicciones de respawn, configuración del Inspector, fixtures y mapa principal. |
| `Tests/DrpgPhase4Results.txt` | Regresión: 58 aprobadas, 0 fallos, incluidos eventos/recompensas únicas, seis slots temporales, mapas, guardados antiguos, símbolos y transición real a Digital Kingdom con guardado/carga y regreso. |
| `Tests/DrpgPhase4Diagnostics.txt` | Regresión: 21 aprobadas, 0 fallos. |
| `Tests/DrpgPhase5Integrity.txt` | Movimiento original, CombatMath, geometría, colocaciones y objetos del mapa principal coinciden con el respaldo previo. |

La prueba antigua de ciudad necesitaba esperar el final de la transición blanca antes de intentar salir; se actualizó solamente esa espera del fixture y se repitió la regresión completa. La ejecución final no registra errores de consola. Se observó el aviso existente de ajuste del atlas de sombras de URP durante las pruebas; no se cambió el renderizado.

Las pruebas escriben exclusivamente en `Tests/Phase5Save.json` y los slots de prueba de la Fase 4. No se sobrescribieron partidas personales. Los cambios concurrentes de conversaciones en `Refugio.asset` se conservaron y no pertenecen a esta fase.

### Archivos

Nuevos: `EnemyExplorationSettings.cs`, `DungeonPrototype.EnemyExploration.cs`, `EnemyExplorationMarker.cs`, `DrpgPhase5Validation.cs`, `Assets/Editor/DrpgPhase5FixtureBuilder.cs`, `DrpgPhase5DiagnosticTests.cs`, y los mapas/regiones/objetos independientes de `Assets/Resources/DrpgPhase5Tests`.

Ampliados: `DungeonRegionSettings.cs`, `EnemyEncounterSettings.cs`, `EnemyLevelSettings.cs`, `DungeonPrototype.cs` (creación del estado/indicador), `DungeonPrototype.Enemies.cs`, `.Regions.cs`, `.Progression.cs`, `.Save.cs`, `DungeonMapPersistence.cs`, `DigitalKingdomTransit.cs` (política de reentrada), `Assets/Editor/DungeonRegionsEditor.cs` y la espera de ciudad de `DrpgPhase4Validation.cs`.

### Límites

- La validación de patrullas comprueba geometría/regiones; las puertas pueden abrirse después. No declara imposible una ruta solo porque una puerta esté cerrada inicialmente.
- Una patrulla espera si la ocupación o el espaciado impiden su siguiente paso; no empuja enemigos ni atraviesa objetos.
- Los perfiles se configuran para colocaciones manuales. La población aleatoria conserva Legacy; no se cambiaron pesos ni reglas de aparición.
- No se alteraron movimiento, ataques, IA de combate, balance, HACKS ni estadísticas. La comprobación de integridad de CombatMath y movimiento complementa las pruebas de navegación; no equivale a repetir todos los minijuegos.

**Fase 5 completada. Detener aquí. No comenzar la Fase 6 sin autorización.**


## Fases 6 y 7 — Implementación y verificación (2026-10-16)

Autorización explícita del usuario: «cuando termines esto haz la fase 6 y luego haz la fase 7 inmediatamente no esperes aprobacion». Se finalizó primero el bloque SPEEDRUN/LEVEL UP CHEAT/ciudad, se implementó y verificó la Fase 6, y después la Fase 7. Las frases históricas anteriores que pedían detenerse al final de la Fase 5 quedan sustituidas por esta autorización.

Respaldo previo: `Backups/DrpgPhase67/20261016-070108/`. Incluye scripts, Editor, assets de combate/exploración/ciudad, mapas del Reino del Píxel y archivos personales. `Phase7Baseline` conserva además los scripts y editores con la Fase 6 ya terminada, antes de iniciar la 7.

### Fase 6 — Cofres

`DungeonChestRewardTable` combina contenido fijo del cofre, recompensas garantizadas y tiradas secundarias ponderadas. El contenido fijo se entrega siempre primero. Los cofres KeyItem ignoran las tiradas aleatorias; el diagnóstico exige que las llaves de las tablas sean garantizadas, en vez de depender de un peso.

Se usa un generador local con hash estable de mapId/cofre/tabla y Seed Salt; no altera UnityEngine.Random ni los drops de combate. El resultado se registra una sola vez en el `DungeonEntityState` ya existente: `chestRewardsRolled`, `chestRewards`, `chestRewardCursor`. Cada entrega exitosa avanza el cursor; si falta espacio, el resto queda pendiente. Recargar o regresar al piso conserva el resultado y las entregas realizadas, sin volver a sortear ni duplicar premios.

Una tabla puede referenciar las condiciones AND/OR de la Fase 4; se evalúan como requisitos, sin ejecutar sus acciones. Los cofres sin tabla y los Mimics conservan la ruta anterior. No se asignaron tablas nuevas a los cofres publicados.

En el plano: seleccionar un cofre normal o de historia y asignar `Chest Rewards`. Los assets se crean con Create > Virtual Dungeon > Cofres > Tabla de recompensas. El contenido de historia sigue en Content ID/Quantity o en Guaranteed. CombatDropSettings permanece intacto.

### Fase 6 — Jefes

`DungeonBossProfile` contiene IDs estables, umbrales de HP, perfiles de evento al alcanzar cada fase y un perfil de victoria. Se asigna opcionalmente en `PlacedEnemyDefinition.bossProfile`. El adaptador Makong Legacy solo acepta la especie Makong y conserva su controlador especializado: no decide ni sustituye ataques.

Los cambios de salud del circuito normal de daño emiten señales únicas. Las señales y los eventos pendientes se guardan dentro de `DungeonInteractionProgress`, que ya pertenece al snapshot de cada mapa. Las acciones reutilizan el ejecutor de la Fase 4 y sus recibos de recompensa. Una carga pendiente puede reanudarse sin duplicar premios.

**Límite deliberado:** las acciones de fase se ejecutan cuando finaliza el combate o la huida, sin interrumpir los ataques, animaciones, minijuegos o narrativa actuales. Esta fase prepara fases/eventos configurables; no incorpora nuevos patrones de ataque ni fases mecánicas de Makong. No se asignó un perfil nuevo a Makong en el mapa publicado. Los perfiles referenciados deben seguir disponibles mediante el mapa/regiones o Resources para resolver eventos pendientes tras cargar.

El diagnóstico consolidado incluye tablas inválidas, objetos desconocidos, llaves aleatorias, IDs duplicados, umbrales inválidos y especie incompatible con Makong Legacy. La validación de la Fase 4 recorre también las condiciones de cofres y los eventos de los perfiles de jefe.

### Verificación de la Fase 6

`Tests/DrpgPhase6Results.txt`: **29 correctas / 0 fallidas**, repetidas después de integrar la Fase 7.

- Tirada estable, llave fija garantizada y ausencia de azar en cofres de historia.
- Condición por llave; animación y apertura real; inventario lleno tras una entrega parcial.
- Guardado/carga del resultado y cursor; liberar espacio y reintentar sin duplicar la llave.
- Cofre abierto sin recompensas repetidas; ida A→B→A con estados intactos.
- Señal de fase única y persistente, sin interrumpir combate; carga de evento pendiente y premio único.
- Derrota en el circuito normal, condición de enemigo derrotado y evento de victoria sin duplicaciones.
- Adaptador Legacy, diagnósticos negativos y estado de cofre antiguo sin campos nuevos.

Fixtures independientes: `Assets/Resources/DrpgPhase6Tests/` y `DrpgPhase6Validation`. Preparación: Editor > DRPG > Fase 6 > Preparar mapas independientes. El harness inicializa expresamente la colocación de prueba al entrar desde la escena experimental; la primera pasada detectó esta dependencia y se corrigió antes de la pasada final. No se modificó la inicialización publicada de enemigos para resolver el fixture.

### Fase 7 — Perfiles

`PlayerProfileSettings` agrupa referencias a PlayerCharacterSettings, PlayerLevelSettings, PlayerBitsSettings, CombatInventorySettings y PlayerHackCatalog. Se creó solamente `Assets/Resources/Combat/GamerBoyProfile.asset`, enlazando exactamente los assets existentes. No contiene nuevas estadísticas, curvas ni miembros jugables.

El controlador consume esos valores como predeterminados; una referencia explícita en el controlador o en el tránsito de ciudad conserva prioridad. El esquema de progreso del jugador y los seis slots no cambian. La ciudad utiliza el mismo perfil al cargar, consultar la mochila y regresar al dungeon.

Edición: Codebound > Perfiles > Editar GamerBoy. El Inspector advierte referencias ausentes. El perfil configura al jugador actual; no implementa selección de personajes ni un grupo nuevo.

### Fase 7 — Proveedor de mapa

`IMapProvider` produce una `DungeonMapDefinition` con filas, referencia a regiones/entidades, punto de entrada, mapId y metadatos de origen. `ManualMapProvider` envuelve el asset actual. El controlador utiliza esa definición al construir el mapa y la ciudad. Las filas se entregan como copias; regiones y entidades conservan sus referencias existentes, que el proveedor no escribe.

El proveedor manual mantiene exactamente el mapId y la clave de snapshot de la Fase 2. Un proveedor asignado para otro piso no sustituye el destino de una transición. El origen ofrece Provider ID, Algorithm Version y semilla opcional; el contrato permite distinguir claves de futuras instancias por semilla/versión sin generar geometría.

**No existe generador procedural ni híbrido.** Tampoco se crean pisos, enemigos ni objetos nuevos en los mapas de juego. Un proveedor futuro deberá asignar una identidad estable de instancia y adaptar su registro/resolución a los snapshots; el contrato de metadatos no implementa esa carga procedural por sí solo.

Edición del ejemplo manual: Codebound > Perfiles > Editar proveedor manual original. Asset `Assets/Resources/MapProviders/OriginalManual.asset`. El controlador sin proveedor explícito utiliza el mismo adaptador manual sobre su mapa activo.

### Verificación de la Fase 7 e integridad

`Tests/DrpgPhase7Results.txt`: **23 correctas / 0 fallidas** en el editor: referencias originales, perfil incompleto, geometría/pose/IDs, copias de filas y metadatos, separación de claves por semilla/versión, rechazo de mapas inválidos, valores predeterminados y overrides, destino de viaje respetado y ausencia de generadores/personajes nuevos.

La regresión final de juego en `Tests/SpeedrunCityResults.txt` terminó con **102 correctas / 0 fallidas** después de integrar ambas fases: consumibles, desbloqueos, Bits, turnos y victoria, ciudad, regalos, mochila, guardado/carga, seis slots temporales y retorno al dungeon. La comparación por hash está en `Tests/DrpgPhase67Integrity.txt`: partidas personales, mapas/regiones/objetos, IA de exploración, ataques/controlador/visual de Makong, fórmulas, minijuegos, estadísticas y drops coinciden con el respaldo previo a estas fases. No se reejecuta aquí cada fixture histórico de Fases 1–5 ni se presenta esta revisión como una nueva prueba completa de balance.

### Archivos

Listado concreto en `Tests/DrpgPhase67Files.txt`, más los .meta generados y assets aislados de `Resources/DrpgPhase6Tests`.

- Fase 6: nuevos DungeonChestRewardTable, DungeonPrototype.ChestRewards, DungeonBossProfile, DungeonPrototype.BossEvents, DrpgPhase6Diagnostics, builder y harness de pruebas. Extensiones de DungeonEntityState, DungeonInteractionProgress, PlacedEnemyDefinition, MapEntities, circuito de daño, Update y diagnósticos/Inspector.
- Fase 7: nuevos PlayerProfileSettings, DungeonMapProvider/IMapProvider/DungeonMapDefinition, ManualMapProvider, DungeonPrototype.Profiles, instalador y pruebas. Integración puntual en inicio del mapa, modelo de ciudad y getters de personaje/niveles/Bits/HACKS. Dos assets de referencias, sin cambios a los assets de estadísticas existentes.

Consola final sin errores; Unity fuera de Play Mode. Total de este cierre: 102 + 29 + 23 = **154 comprobaciones correctas**.

**Fases 6 y 7 completadas dentro del alcance acordado. No se inicia generación procedural ni otra fase.**
