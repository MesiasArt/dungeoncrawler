# CODEBOUND — Desbloqueo de HACKS por nivel

Implementación del 11 de octubre de 2026. **Editor → Catálogo de HACKS**. Asset: `Assets/Resources/Combat/PlayerHackCatalog.asset`.

## Auditoría

El menú anterior enumeraba todos los componentes `CombatHack` del dungeon. La única implementación concreta existente es `EnemyJumpHack` (JUMP). Su ejecución, selección de objetivo, modelo, ritmo, calidad de ataque, música y sprites se conservan. `PlayerLevelSettings` ya calcula niveles; el nuevo catálogo consulta `PlayerLevel`, sin una tabla adicional de niveles ni cambios en estadísticas, curva XP o CombatMath. La transición existente transmite `DungeonSaveData` y llama a `ApplyMapTravel`/`RestoreDungeonSave`.

## Cambios

- `PlayerHackCatalog` contiene definiciones con ID estable, nombre, descripción, nivel mínimo, disponibilidad, modo de desbloqueo y referencia a un script concreto que derive de `CombatHack`.
- La referencia de script se selecciona con un ObjectField restringido a MonoScript; el Editor deriva el nombre completo del tipo para resolverlo en runtime. No hay que escribir el nombre del tipo. El mismo componente existente se reutiliza; si no existe, se agrega al dungeon únicamente cuando una entrada es válida y está desbloqueada.
- El catálogo real registra solo **hack.jump / JUMP / nivel1**. No hay SCAN, GLITCH, OVERCLOCK, VIRUS, REWIND o SYSTEM BREAK, ni minijuegos nuevos.
- `DungeonPrototype.HackUnlocks.cs` filtra el menú por IDs válidos y únicos, disponibilidad, nivel, tipo concreto CombatHack y componente activo/habilitado. Entradas duplicadas se excluyen, no se elige arbitrariamente una de ellas. El menú usa el nombre visible del catálogo y muestra la descripción de la selección.
- La ejecución vuelve a comprobar el ID seleccionado y su implementación en `BeginTargetHack`, después de elegir objetivo. Un menú viejo o una llamada directa no permite saltarse el requisito.
- Un hook de `GainPlayerXP` compara el nivel anterior con el final y encola todos los desbloqueos funcionales cruzados. Los avisos NEW HACK UNLOCKED se muestran uno a uno durante4 segundos, debajo del aviso LEVEL UP. Los IDs ya anunciados no se repiten en la misma partida cargada.
- Al abrir o dibujar el menú se actualiza la lista; si una entrada se desactiva, se bloquea por nivel o pierde su implementación, deja de ser seleccionable. La selección se ajusta al tamaño de la lista.

## Configuración en Unity

1. Abrir **Editor → Catálogo de HACKS**, o seleccionar el asset del catálogo en el Inspector.
2. Pulsar **Registrar HACK (ID automático)**. Mantener ese ID estable después de utilizarlo; nombre y descripción sí se pueden cambiar.
3. Asignar nombre, descripción y nivel mínimo. Marcar Available.
4. Elegir **PlayerLevel** y arrastrar a Implementación el script existente de un minijuego/HACK derivado de CombatHack. Scripts abstractos, de otras clases o vacíos se marcan como inválidos y no aparecen.
5. El componente recibe su configuración mediante el mecanismo que ya utiliza su implementación. El catálogo registra qué script ejecutar; no crea parámetros duplicados de cada minijuego. JUMP sigue usando sus settings actuales.
6. Consultar las advertencias y usar **Previsualizar nivel**, cuyo máximo viene del PlayerLevelSettings existente. La lista indica Disponible, Bloqueado, Desactivado, Sin implementación o Reservado para evento.
7. Pulsar **Guardar catálogo**. Propiedades editadas mediante SerializedObject y altas mediante Undo.RecordObject soportan Undo/Redo.

Se pueden crear catálogos alternativos y asignarlos al campo Player Hack Catalog del DungeonPrototype. Si no hay uno asignado, se usa el catálogo Resources existente. No se necesita una lista manual paralela de niveles ni agregar objetos a mapas.

## Guardado y transiciones

Los desbloqueos de nivel no se guardan como una segunda lista: se reconstruyen con el nivel guardado y la configuración vigente. Cargar una partida prepara la lista sin emitir avisos retroactivos. Una partida anterior sin campos nuevos conserva JUMP mediante su nivel1.

Se añade `eventHackIds` al registro existente del jugador, inicialmente vacío. El modo FutureEvent y estos IDs preparan persistencia futura sin implementar eventos, objetos secretos o una API que los entregue. IDs desconocidos se conservan por compatibilidad, pero no habilitan nada sin una definición correspondiente, válida y funcional. No hay entradas secretas en el catálogo real.

CapturePlayerProgression transmite los IDs preparados; RestoreDungeonSave restaura el nivel y reconstruye HACKS. ApplyMapTravel ya usa ese mismo registro y conserva la configuración serializada del catálogo.

## Pruebas Unity MCP

25 comprobaciones en Play Mode, con catálogo temporal que referencia únicamente el JUMP existente para simular entradas de prueba nivel3 y5. No se añadieron scripts ni minijuegos para probar los desbloqueos:

- JUMP visible y disponible desde nivel1; inicio sin avisos antiguos.
- Ejecución directa de entrada nivel3 bloqueada en1.
- Sin implementación, desactivada o reservada a evento no ejecuta.
- IDs duplicados detectados y excluidos del menú.
- Recompensa suficiente para subir de1 a7: ambas entradas de prueba se desbloquean, menú actualizado y dos avisos en cola.
- Repetir comprobación no duplica avisos y los desbloqueos antiguos permanecen silenciosos.
- Nombres visibles, VOLVER y lista filtrada correctos; disponibilidad editada actualiza lista.
- Guardado/carga del nivel, reconstrucción de lista y ausencia de notificaciones retroactivas.
- JSON de partida anterior devuelve LV1/JUMP.
- IDs de eventos futuros desconocidos se conservan sin otorgar HACKS.
- Ejecución real de JUMP: Model y Rhythm iniciados, exclusión de suerte conservada. Se cerró mediante su OnDisable existente. No fue una partida humana completa del minijuego.
- Catálogo real restaurado: únicamente JUMP.
- Componente de JUMP deshabilitado queda excluido; se rehabilitó después.
- Índice inexistente en menú no ejecuta ningún HACK.
- Ruta real ApplyMapTravel restauró nivel7, reconstruyó disponibilidad y conservó IDs futuros. Se probó la recepción de datos sobre el mapa ya cargado, sin cruzar físicamente una puerta hacia otra escena.

También se verificaron guardar/reimportar el catálogo y conservar referencia, ID y requisito; validación final sin advertencias. Consola sin errores. Fixture aislado `Tests/HackUnlockFixture.json`, sin sobrescribir slots personales. Script reproducible principal `Tests/HackUnlockMcpTest.txt` y resultados `Tests/HackUnlockResults.txt`. Unity queda fuera de Play Mode.

## Límites y conservación

El sistema ya permite registrar implementaciones futuras, pero la partida actual no desbloquea otros HACKS porque no existen otras implementaciones. FutureEvent es infraestructura; no se añadieron eventos secretos. Las pruebas cubren el inicio de JUMP y la integración, no una sesión completa ni un build standalone. Para futuros scripts resueltos solo por reflexión, habrá que revisar el stripping de tipos en builds con IL2CPP.

Se conservaron XP, estadísticas, CombatMath, escalado y apariciones de enemigos, zonas, director, mapas, sprites, audio y funcionamiento de los minijuegos. Respaldos en `Backups/HacksPorNivel`.
