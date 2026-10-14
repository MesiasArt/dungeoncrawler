# Movimiento de exploración

## Estado actual: controles originales restaurados
Restaurar comportamiento original activa la rama anterior del controlador (Experimental=false), con paso y giro de 0,18 s. Los parámetros visibles también quedan en 0,18 s, interpolación lineal, movimiento continuo/repetición/solapamiento desactivados y buffer cero. No se elimina la infraestructura experimental.

Unity MCP/Play Mode: Tests/MovementOriginalRestored.txt verifica a relojes de 60/120 pasos por segundo una pulsación = una casilla/90°, mantener tecla sin repetición, entradas durante animación ignoradas, alternancia rápida sin acciones adicionales, soltar sin orden pendiente y alineación exacta. Todos pasaron. Se simularon eventos del Input System sobre un teclado temporal; no se modificaron assets de mapas ni slots personales.

Restaurar solo los tiempos dejando Experimental=true no equivale al comportamiento antiguo: sigue usando el lector de entradas y el bucle experimental. RestoreOriginal selecciona explícitamente la rama anterior. Los 0,15 s no se han activado ni probado en esta restauración. Para una futura comparación se pueden configurar con Experimental=true y todas las opciones de repetición/buffer/solapamiento desactivadas.

El resto del informe describe la fase experimental anterior.

## Auditoría
El movimiento original usa interpolación lineal con duración fija de 0,18 s para avanzar y girar. El bucle empieza en t=0 y cede un frame adicional al final. Solo recibe wasPressedThisFrame cuando moving es false: mantener la tecla no repite pasos y una pulsación durante la animación se pierde. Tras cada acción, espera EnemyTurn (0,18 s, incluso sin enemigos en movimiento). Por ello la cadencia original supera aproximadamente 0,36 s por acción.

No existe FixedUpdate de navegación: Move es una coroutine que mueve la cámara, luego confirma posición/heading y revela la casilla. LateUpdate mantiene otros sistemas, sin nueva oscilación.

## Configuración aplicada
Editor → Movimiento de exploración. Asset: Assets/Resources/Exploration/MovementSettings.asset. DungeonPrototype también acepta un asset específico en Movement Settings.

- Paso: 0,14 s, Linear para evitar frenar en cada casilla.
- Giro: 0,12 s, SmoothStep.
- Continuous Movement y Repeat Turns habilitados; repetición mínima 0 s.
- Buffer de una sola orden: 0,22 s. Una orden nueva sustituye la pendiente, no se acumulan múltiples movimientos.
- Overlap Enemy Animation habilitado: el siguiente desplazamiento puede empezar durante la animación del turno anterior. Las decisiones de EnemyTurn permanecen intactas y serializadas, una por acción, con los mismos filtros. Si el jugador alcanza el siguiente límite antes de que termine la animación enemiga, espera allí antes de ejecutar otro turno. No se modificó EnemyEncounterSettings.moveSeconds ni la IA.

Los bloqueos de mapas/puertas/NPC/cofres/enemigos permanecen antes del desplazamiento. El movimiento confirma cámara y posición exactas. Diálogos, pausa, guardado, descarga de mapa y combate vacían la orden pendiente. Se añade D-pad de mando para las mismas cuatro acciones.

## Restauración
Pulsa Restaurar comportamiento original en el Inspector del asset (o desmarca Experimental). Recupera 0,18 s, controles originales por pulsación e interpolación/espera anterior. Aplicar configuración experimental restaura los valores indicados. También puedes desactivar solo Overlap Enemy Animation para comparar la espera original conservando el buffer.

## Pruebas y límites de medición
Unity MCP/Play Mode: avance sostenido de varias casillas, giros opuestos encadenados, buffer una sola vez, alineación de posición y rotación, paredes, puertas abiertas/cerradas, cofres, encuentros y bloqueo de entrada durante diálogo. Fixtures temporales restauran la partida en memoria, sin escribir slots personales.

Tests/MovementControls.txt verifica además pasos de reloj deterministas de 1/60 y 1/120 segundos. La cadencia puede estar limitada por los 0,18 s de animación enemiga; la duración visual del paso y del giro son independientes.

Tests/MovementPerformance.txt recoge muestras reales del Editor con límites de 60/120 FPS, 120 frames por muestra. Original: 43,7 FPS /22,90 ms y 87,2 FPS /11,47 ms. Experimental: 44,1 FPS /22,70 ms y 80,5 FPS /12,42 ms. Picos de 93–104 ms y variación del Editor/MCP impiden atribuir la diferencia al controlador. No se alcanzaron 60/120 FPS constantes de renderizado; no es una prueba en build ni una garantía de rendimiento. No se cambiaron efectos CRT ni renderizado para esta fase.

Scripts: DungeonMovementSettings.cs, DungeonPrototype.Movement.cs, DungeonPrototype.cs (integración), DungeonMovementSettingsEditor.cs. MovementValidation.cs es una prueba solo del Editor.

Resultados finales de reloj fijo: paso 0,150 s y giro 0,133 s a 60; paso 0,142 s y giro 0,125 s a 120. La diferencia respecto a los parámetros corresponde al redondeo al siguiente frame. Todos los controles, colisiones y gating probados pasaron; consola sin errores al terminar. Medición de reloj real conservada en Tests/MovementControlsRealClock.txt.
