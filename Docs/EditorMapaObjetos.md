# Editor de mapas y objetos

Abre Editor → Plano físico del dungeon. La cuadrícula conserva las herramientas de paredes y suelo y añade Seleccionar, NPC, Enemigo, Cofre, Puerta, Savepoint, Teleport y BorrarObjeto.

Selecciona una herramienta y pulsa suelo vacío para colocar. Pulsa un objeto para ver sus propiedades debajo del mapa. Mover a otra casilla conserva el ID; después pulsa una casilla libre. Borrar objeto conserva el suelo. En casillas compartidas aparece + y un menú permite elegir el objeto. Solo se permite superponer el cofre Mimic y su encuentro asociado. Undo/Redo funciona mediante el Undo de Unity.

La geometría sigue en DungeonMapSettings.rows. Los objetos nuevos están en un asset DungeonEntitySettings asignado a Entities. Los enemigos se guardan directamente en DungeonRegionSettings.placedEnemies: no se crea una segunda lista de estadísticas. Las configuraciones heredadas de NPC, nodos y terminal se editan en Refugio.asset; las puertas conservan sus configuraciones de origen. El borrado de una puerta o terminal heredada la incluye en Disabled Legacy Objects del plano, sin destruir su configuración. Puedes quitar su ID de esa lista para recuperarla.

## Propiedades

NPC: ObjectField del catálogo del refugio, dropdown del NPC, orientación inicial, ID propio y diálogos opcionales. Si no se asignan diálogos propios, reutiliza las conversaciones del NPC elegido. Los NPC colocados aquí son instancias nuevas; los existentes del refugio se muestran sin copiarlos.

Enemigo: dropdown de EnemyCharacterCatalog, nivel fijo/aleatorio por zona, Normal/Elite/Boss, aparición única, respawn y límites existentes. Para un encuentro único, marca unique y desactiva respawn. No se modifican CombatMath ni EnemyLevelSettings.

Cofre: Normal, Mimic o KeyItem; contenido mediante dropdown de pociones/equipo, cantidad. Para Mimic pulsa Crear encuentro Mimic asociado; utiliza TriggerSpecialEncounter y la colocación persistente existente. Al mover el cofre se mueve su colocación asociada. Apertura y contenido recogido se guardan por ID; un cofre lleno no pierde contenido si no cabe en el inventario.

Puerta: Normal, Locked o Transition; selección de llave, configuración DungeonDoorSettings, zona de destino mediante dropdown, mapa de destino mediante ObjectField y casilla de llegada. Heading orienta la puerta. Las transiciones conservan la progresión del jugador y utilizan el mapa seleccionado.

Savepoint: ID, curar y/o guardar. Teleport: ID, orientación de llegada, estado inicialmente activo, requiere activación y destinos mediante lista de toggles. El destino solo puede usarse si está desbloqueado; sin destinos se informa al jugador y se puede seguir caminando. Los nodos del refugio mantienen su narrativa específica.

Validar y guardar mapa + objetos detecta IDs duplicados, coordenadas fuera del suelo, objetos incompatibles, referencias NPC inexistentes, contenidos inválidos, encuentros Mimic ausentes y destinos inválidos. Los cambios afectan el asset inmediatamente con Undo; guardar conserva los datos en disco. Salir y volver a Play reconstruye el mapa.

## Mapa de prueba

Assets/MapTests/PlanoPrueba.asset, Objetos.asset, Regiones.asset y PruebaEditorMapa.unity. El dungeon de esta escena tiene mapSettings asignado explícitamente y desactiva los objetos heredados. Contiene un NPC, un enemigo fijo de nivel 2, tres cofres (normal, Mimic y objeto clave), una puerta, un savepoint y un teleport. No sustituye SampleScene ni los mapas del juego.

## Validación realizada

Unity MCP / Play Mode: mapa 10×8, siete objetos y un enemigo fijo en sus posiciones; conversación NPC; cofre normal entrega contenido una sola vez; cofre de objeto clave entrega contenido; savepoint cura y guarda en archivo de prueba; cofres abiertos y nodo activo se conservan al cargar; siete objetos tras cargar, sin duplicaciones; puerta inicia apertura; Mimic inicia combate y usa su visual existente. Undo/Redo del movimiento comprobados. Assets reimportados desde disco conservan referencias e ID; validación sin errores. SampleScene conserva 24×19, cinco NPC, tres dispositivos y no añade objetos duplicados.

La prueba descubrió IDs de pociones que no estaban serializados. Se guardaron en el asset original para que los cofres y el guardado usen IDs estables. No se cambiaron cantidades ni curación.

No se cerró/reabrió la aplicación Unity: se comprobó reimportación de assets, recompilación y salida/entrada de Play. No se completó una prueba manual de todos los botones del Inspector ni de transición entre dos mapas distintos. Los objetos tienen visuales provisionales.

## Área de dibujo y recompensas

Tamaño de casillas permite reducir o ampliar la cuadrícula. Ajustar cuadrícula al ancho calcula el tamaño adecuado para el Inspector. Altura del área de dibujo permite reservar más espacio. Configuración del plano y Propiedades del objeto seleccionado se pueden plegar.

Los regalos y cofres muestran avisos temporales con cantidad y nombre, con sonido de recepción; no requieren pulsar para cerrarlos. Los cofres bloquean el movimiento y las interacciones/pistas requieren mirar al objeto. El teleport se usa sobre su base, mirando su consola. Los iconos temporales del equipo inicial están en Assets/Resources/UI/EquipmentTemporary.

Verificado con Unity MCP en Play Mode: las tres líneas del regalo de pociones, puerta disponible delante y no detrás, cofre bloquea el paso, aviso +1 Poción pequeña, sin diálogo genérico y sin recompensa duplicada. Sin errores de consola.

Los cofres usan los seis frames de Assets/sprites/objetos/Cofres.psd, exportados con pies alineados y lienzo común. Siempre se orientan hacia la cámara y animan la apertura. El HUD usa un minimapa cercano de nueve casillas de ancho/alto (minimapVisibleCells ajustable); el menú de pausa → Mapa conserva el mapa completo descubierto.

En Editor → Zonas y encuentros, selecciona Marco rectangular y arrastra desde una esquina hasta la otra para asignar todas las casillas de suelo a la zona seleccionada. Borrar asignación aplica el borrado al mismo bloque. No incluye paredes; reasigna casillas sin solapamientos y soporta Undo/Redo.

En Pausa → Mapa hay una vista ampliada desplazable y un mapa general lateral. Flechas o arrastrar desplazan la vista, la rueda cambia el zoom, C centra en GamerBoy y un clic en el mapa general lleva a esa área. Un marco amarillo indica el área visible. Solo muestra las casillas exploradas y conserva celdas cuadradas.
