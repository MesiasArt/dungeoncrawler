# Avisos de puertas y nombres de refugiados

## Avisos

Las nueve puertas existentes muestran ZONA SEGURA en verde o ZONA PELIGROSA en rojo, segun la zona del otro lado. Las dos caras se resuelven independientemente, incluyendo el umbral corto que desemboca desde el refugio en Zona 1. Digital Kingdom se anuncia como seguro. Las puertas de habitaciones de Zona 2 mantienen el aviso rojo.

A peticion del usuario, los avisos estan sobre el simbolo central y montados sobre el panel de la propia puerta. Se deslizan con el panel al abrir y cerrar. El color de seguridad no sustituye el color de la llave o cerradura. Los textos tienen prueba de profundidad, ocultacion por paredes, una sola cara visible segun el lado del jugador y desvanecimiento a distancia. No se agregan colliders activos ni cambios de acceso.

El NPC existente en X17/Y34 explica los avisos mediante cuatro lineas, con el sistema de dialogo existente. No otorga objetos ni cambia eventos. Su nombre y hablante son Cerrojio, independiente de Solid Guarda y los cinco refugiados.

## Nombres implementados

- centinela: Solid Guarda
- archivista: Zeldoro
- cobarde: Pacmiedo
- optimista: Toadavía
- devoto: Prayman

Se actualizaron nombres visibles, etiquetas de hablante y el nombre de combate Prayman corrompido. Los IDs de NPC, la especie DevotoCorrompido, la colocacion refuge.devoto y las claves de progreso siguen intactos. EnemyRoster acepta los nombres de combate nuevo y anterior. Los parlamentos anteriores no se reescribieron.

La documentacion vigente se actualizo en Docs/Narrativa/README.md, 03_Personajes.md, 08_Pendientes_y_contradicciones.md, 09_Registro_de_cambios.md, 10_Dialogos_en_implementacion.md, 11_Propuestas_nombres_refugiados.md y Docs/Zona0Refugio.md. La Biblia V1.0 y Docs/Narrativa/Fuentes se verificaron por SHA-256 y no se modificaron.

## Validacion

Unity MCP en Play Mode: nueve puertas con dos caras, aviso rojo al salir del refugio y verde al volver, Digital Kingdom seguro; avisos hijos del panel y desplazamiento exacto al abrir/cerrar; misma resolucion de sprite con los dos nombres de combate; cinco nombres y 37 hablantes actualizados; interaccion del NPC X17/Y34 abre sus cuatro lineas con Solid Guarda. Compilacion y consola sin errores.

Tests/DoorZoneWarningsResults.txt, Tests/RefugeNamesDoorPanelResults.txt y Tests/RefugeNamesIntegrity.txt conservan los resultados. Captura del aviso revisada en Captures/DoorZoneWarnings/PanelWarning.png. El dialogo del NPC se verifico mediante el estado real de WorldDialogue y su finalizacion. No se escribieron partidas personales.

Respaldos: Backups/DoorZoneWarnings/20261015-150335 y Backups/RefugeNamesAndDoorText/20261015-152036.


## Revision: fondo del aviso y reglas de llaves

Se elimina totalmente la placa negra: quedan solamente los glifos del aviso, sin recortar el circulo. El NPC X17/Y34 se llama Cerrojio en su etiqueta y en las cuatro lineas de dialogo; Solid Guarda conserva su identidad en el refugio.

Todas las puertas normales se cierran al atravesarlas en cualquier sentido. Las puertas desbloqueadas mediante una llave consumen una unidad cuando termina correctamente su animacion de apertura; no consumen al iniciar una apertura que se cancela. Quedan abiertas permanentemente y rechazan las ordenes normales de cierre. La marca keyConsumed se guarda en DungeonEntityState, dentro de los snapshots existentes por mapId. No hay un segundo guardado.

Al cargar partidas anteriores, una puerta con llave ya abierta y sin desbloqueo por evento se migra como permanente; la llave retenida se consume una sola vez si aun esta en el inventario. Cargar una partida anterior donde la puerta seguia cerrada restablece el estado correctamente. Las conexiones abiertas mediante eventos siguen usando su sistema existente. La puerta luminosa se registra cerrada al atravesarla, antes de capturar el estado para Digital Kingdom.

Pruebas aisladas: Tests/DoorKeyRulesResults.txt, 18 comprobaciones aprobadas, incluyendo inventario, reapertura, migracion, cancelacion, guardado/carga, autocierre en ambos sentidos y nombres independientes. Vista sin placa negra revisada en Captures/DoorZoneWarnings/NoBlackBackplate.png. Respaldo previo: Backups/DoorAutoCloseKeys/20261015-153947.
