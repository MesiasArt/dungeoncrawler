# CODEBOUND — Zonas y encuentros especiales

## Componentes reutilizados

EnemyCharacterCatalog sigue siendo la fuente de especies e IDs. EnemyZoneSettings conserva pesos, niveles, rareza, máximo y respawn. EnemyLevelSettings y CombatMath mantienen el escalado y las fórmulas existentes. Se conservan equipo, inventario, animaciones, audio, grupos aleatorios de 1–3 enemigos y JUMP.

## Extensiones

DungeonRegionSettings añade regiones con casillas, ID, color solo de edición, configuración local y áreas seguras. DungeonPrototype registra el origen de cada enemigo y respawnea en esa región; perseguir al jugador no cambia su origen ni su límite. Los compañeros de encuentros aleatorios pertenecen a esa misma zona. Los encuentros manuales permanecen independientes.

PlacedEnemyDefinition registra especie, nivel fijo/aleatorio, categoría, identificador, respawn, aparición única y participación en la población. Los jefes nunca reaparecen. Todas las instancias usan el escalado existente. TriggerSpecialEncounter(placementId) prepara eventos interactivos y Mimics sin implementar cofres.

CaptureEnemyProgression y CaptureTriggeredEncounterIds exponen identificadores y estado para un guardado futuro; todavía no existe restauración entre sesiones. Sin regiones configuradas se conserva la configuración única anterior. La migración conserva la Zona 1 y las colocaciones originales mediante Legacy Placements.

## Edición

Assets/Resources/Exploration/Zones reúne todos los editables. Editor → Carpeta de zonas abre la carpeta. Editor → Zonas y encuentros permite crear, renombrar y eliminar regiones; pintar casillas; ver sus conexiones; clonar configuraciones; colocar enemigos y validar solapamientos, huecos, especies, niveles e IDs. Eliminar una región no elimina su asset de enemigos.

El desplegable de especies utiliza el catálogo y guarda el ID estable. Muestra nombres visibles, peso y porcentaje calculado; impide seleccionar otra fila duplicada y avisa de duplicados existentes. Añadir todas incorpora únicamente especies aleatorias compatibles que falten, excluyendo Mimic y reservadas para eventos, conservando todos los pesos existentes. El botón añade nuevas filas con peso 1, así que usarlo sí incorpora nuevas especies al reparto por decisión del editor.

La cuadrícula distribuye regiones sobre el mapa actual; no crea nuevos pasillos ni modifica paredes. Para cambiar el plano físico se necesita editar el mapa de DungeonPrototype. No se implementaron escenas separadas ni cambios en la apariencia del minimapa.

Zona 2 estaba usando la configuración de Zona 1: ahora tiene una copia independiente con idénticos valores. La configuración suelta Nueva zona se conservó en 99-SinAsignar. La carpeta 00-Zona0 está reservada, vacía, sin introducir aún contenido del prólogo.

## Verificación

- Prueba reproducible vía Unity MCP: 30.000 selecciones, semillas 1102026–1102028 y 60.032 comprobaciones. Rangos, niveles individuales 1/3/5, distribuciones de niveles/especies, exclusión de Mimic, áreas seguras, desactivación, jefes, aparición única, validación de solapamientos y añadido sin duplicados.
- Guardado de ID y peso mediante SerializedObject, SaveAssetIfDirty y reimportación del asset. No se cerró ni reinició Unity: la persistencia se comprobó desde disco, sin afirmar una prueba de reinicio.
- Play Mode con fixtures temporales: 20 comprobaciones de poblaciones independientes, niveles/especies por región, respawn de origen, élite fija, jefe derrotado sin respawn, IDs persistentes, bloqueo de entrada al refugio, disparador de Mimic, daño mediante CombatMath y arranque real de JUMP con las estadísticas escaladas. Se restauró el estado al salir de Play Mode. Esta prueba comprueba el arranque de JUMP, no una partida completa con entradas humanas.
- Evidencia: Docs/PruebasZonas.txt y Docs/PruebasZonasPlayMode.txt. Scripts repetibles en Tests/DungeonRegionsMcpTests.cs y Tests/DungeonRegionsPlayModeMcpTests.cs, cuerpos de método para execute_code.

## Configuración actual que requiere distribuir casillas

Zona 1 y Zona 2 contienen las mismas 43 casillas. Se conservaron los cambios del usuario. La validación avisa del solapamiento y el runtime evita elegir un origen ambiguo; por eso esas casillas no generan población hasta repartirlas. Pintar una casilla para una zona elimina su asignación en las demás.

## Próxima fase

Para NPC hacen falta definiciones, colocación e interacción. Para narrativa, condiciones, disparadores y estado de eventos. Para el prólogo, diseñar Zona 0, su recorrido, diálogos y transiciones. Para guardar partidas, implementar persistencia/restauración usando los IDs expuestos. Los cofres deberán llamar al disparador de encuentro especial y aportar su propia interfaz. No se implementó El refugio de los olvidados, recompensas ni nuevos jefes.
