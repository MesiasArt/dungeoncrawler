# Pasillo de preservación — Zona 1.5

La puerta de salida original ahora conduce a siete casillas de pasillo seguro (9,8)..(9,14). Un letrero en la pared derecha de (9,9) permite leer el protocolo con E. Primero aparece el texto solicitado completo con escritura progresiva; después las tres respuestas de GamerBoy, en el orden solicitado, con avatar y diálogo a la izquierda. E, SPACE o ENTER completan las letras; la siguiente pulsación continúa. Se puede volver a leer el letrero.

La segunda puerta, entre (9,14) y (9,15), abre hacia Zona 2. Se reutilizó DungeonDoor con su animación y sonido. El pasillo no genera enemigos, bloquea su entrada y evita encuentros incluso con enemigos adyacentes del exterior. La Zona 2 tiene una sala inicial para conectar la puerta; su diseño completo queda pendiente.

## Cómo se construye el mapa

Editor → Plano físico del dungeon abre PlanoDungeon.asset: # pared y . suelo, o clic sobre la cuadrícula para alternarlas. Las filas están ordenadas de sur a norte. Reinicia Play Mode tras editar para reconstruir el escenario.

Editor → Zonas y encuentros decide qué zona ocupa cada casilla del plano. No abre paredes. Reparte el suelo con colores, define Safe Area y enlaza su configuración de enemigos. Las conexiones son vecinos transitables; las puertas son objetos que bloquean el paso hasta abrirse.

Letrero.asset, dentro de 01.5-PasilloSeguro, permite editar el protocolo, respuestas y coordenadas de letrero/segunda puerta. La primera puerta mantiene su posición original (9,7.5). Si cambias el plano, revisa también las coordenadas de puertas, letrero, posiciones iniciales y regiones.

## Conservación y pruebas

Respaldo original de regiones: Assets/ZoneBackups/AntesDelPasillo.asset. Se conservaron los assets de enemigos, sus valores, IDs y las zonas existentes; se retiraron únicamente las asignaciones que solapaban la conexión solicitada. Zona 0 sigue sin construirse.

Unity MCP: compilación, 12 comprobaciones de lectura/regiones seguras, animación real de ambas puertas y sus eventos de cruce. La primera ya no termina el nivel; la segunda anuncia Zona 2. Capturas visuales revisadas y corregidas para tamaño/orientación del texto del letrero. Evidencia en Docs/PruebasPasilloPreservacion.txt y Captures/PasilloLetreroValidado.png. Pruebas mediante llamadas al flujo real; no se simuló un recorrido completo de teclado desde el inicio del dungeon.

Ambas puertas se cierran automáticamente tras cruzarlas en cualquier dirección. Se pueden volver a abrir con E; la zona segura también bloquea el paso de enemigos aunque estén abiertas.
