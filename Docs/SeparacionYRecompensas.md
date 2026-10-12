# Separación y recompensas de combate

Editor → Separación de enemigos selecciona EnemyEncounterSettings. Minimum Map Spacing está en 4 casillas. Las apariciones aleatorias y los movimientos respetan esa distancia. Los enemigos de guardados antiguos que ya estén demasiado juntos pueden moverse si aumenta su separación. Las colocaciones manuales conservan las casillas elegidas por el diseñador. Los encuentros de 1–3 enemigos mantienen su comportamiento y formación de combate.

Editor → Recompensas de combate permite editar monedas y pociones usando un dropdown. Valores iniciales: 2–6 monedas por enemigo derrotado; al ganar el combate, poción pequeña 25%, mediana 10%, grande 3%, una unidad por resultado. Las tiradas son independientes. Las monedas no tienen uso todavía; se muestran en pausa y se guardan en cada slot. Las recompensas usan los avisos y sonido de recepción ya existentes. No se entregan al huir o perder. Si no cabe una poción, queda pendiente en la partida y se recoge cuando hay espacio.

La carga de partidas repone poblaciones regionales que faltan sin duplicar enemigos vivos o respawns pendientes. Los Mimics y eventos especiales no sustituyen esa población; las colocaciones eliminadas del mapa no resucitan al cargar. Se probó cargar el Slot 1 antiguo: Zona 2 recupera dos enemigos normales, mantiene sus niveles y estadísticas, no repite Mimics y una segunda carga no duplica población.

Pruebas Unity MCP en Play Mode: distancia mínima inicial 4; victoria simulada reproducible (semilla 145) entrega 11 monedas por tres derrotas; llamada repetida no vuelve a premiar; guardado/carga conserva las monedas; consola sin errores. Las pruebas no sobrescribieron los slots personales.

Actualización: las monedas por enemigo son 2–6 de base más un bonus según estadísticas reales: +2 por cada nivel por encima del primero, +1 por cada 10 HP máximos, +1 por cada 3 de ataque y +1 por cada 2 de defensa. Estos divisores y el bonus de nivel se editan en Editor → Recompensas de combate. Las pociones son ahora un poco más raras: pequeña 20%, mediana 7%, grande 2%. Prueba reproducible en Play con la misma semilla confirmó que un enemigo más fuerte entrega más monedas.
