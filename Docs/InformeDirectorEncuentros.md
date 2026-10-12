# Informe: distribución y ritmo de encuentros

## Causas identificadas antes de modificar
- La siembra inicial usaba initialRandomPerZone = 2 para toda Zona 2, que tiene 284 casillas de suelo. No había una densidad cercana que acompañara la exploración.
- FindSpawn elegía uniformemente entre todas las casillas, con distancias euclidianas; podía elegir habitaciones lejanas o posiciones físicamente separadas por paredes.
- La separación de cuatro casillas también se medía en línea recta, bloqueando movimientos legítimos alrededor de paredes.
- No había descanso por pasos después de combate ni chequeo de visión para nuevos agentes.
- Los respawns vencidos podían procesarse juntos. Los guardados antiguos ya tenían una corrección de población ausente; se conserva.

## Implementación
Se reutilizan DungeonRegionSettings, EnemyZoneSettings, EnemyCharacterCatalog, EnemyLevelSettings, EnemySpawnSlot, los temporizadores y la IA existentes. No cambian CombatMath, pesos, estadísticas, minijuegos, arte o narrativa.

El director usa BFS de casillas transitables. La distancia desde el jugador se cachea hasta cambiar posición, puertas u obstáculos. La separación entre enemigos se comprueba con BFS limitado al radio mínimo. También considera casillas reservadas, áreas seguras y aproximaciones a puertas.

Los sectores son agrupaciones internas de casillas de la misma región conectadas por navegación. No crean zonas narrativas ni modifican el mapa visible. Se prefieren sectores poco poblados y candidatos cercanos. Las nuevas apariciones no mueven ni teletransportan a enemigos vivos.

La visibilidad utiliza WorldToViewportPoint (incluye el formato de pantalla y un margen) y una comprobación física de paredes/puertas. Se prefieren nuevas apariciones a 6–12 pasos navegables; si la siembra o un respawn no tienen candidato dentro de esa preferencia, pueden usar candidatos válidos más lejanos. La reposición dinámica durante exploración solo busca dentro de la preferencia.

La densidad dinámica se revisa cada dos pasos realmente recorridos, no al girar. Tras combate o huida hay seis pasos sin nuevas apariciones; los enemigos que ya existen pueden iniciar combate. El descanso restante se guarda y carga. El máximo de enemigos vivos de cada zona sigue siendo la autoridad; un respawn futuro no reserva una plaza como si estuviera vivo. Si hay respawns vencidos, se les da prioridad antes de crear agentes nuevos.

El respawn conserva su plazo y zona; si no hay candidato, se pospone sin reiniciar su temporizador. Se procesa como máximo una reaparición por intervalo, y las búsquedas se limitan a una comprobación cada medio segundo. Se conserva el ID de la instancia al reaparecer. Las colocaciones manuales conservan su posición y los jefes/únicos no adquieren respawn.

## Controles
Editor → Director de encuentros abre EnemyEncounterSettings:
- Encounter Director Enabled: activar o volver al comportamiento anterior.
- Minimum Spawn Navigation Steps: 6.
- Preferred Maximum Spawn Steps: 12.
- Minimum Map Spacing: 4 (se reutiliza el parámetro existente).
- Post Combat Rest Steps: 6.
- Desired Enemies Per Sector: 2.
- Sector Navigation Radius: 6.
- Director Check Every Steps: 2.
- Long Quiet Walk Steps: 16.
- Respawn Stagger Seconds: 1.2.
- Doorway Buffer Steps / Safe Area Buffer Steps: 2 / 2.
- Spawn Outside View: activado.

Se conservan los límites y tiempos de EnemyZoneSettings, incluido el máximo de siete enemigos y el respawn de Zona 2. Configuraciones y densidad siguen siendo ajustables según la prueba del usuario.

## Comparación reproducible sobre el mapa actual
Mapa: 24×56. Ruta BFS de 125 casillas, cuatro recorridos de ida/vuelta: 500 pasos, 402 dentro de regiones que permiten enemigos. Semilla 145. Se aplican la navegación, IA, selección de candidatos y director reales. El combate se abstrae para la simulación (se elimina al agente encontrado y se añade un tiempo virtual de cuatro segundos), y cada paso equivale a 0.25 segundos virtuales. No representa una partida humana completa ni sus decisiones de combate. Los grupos se verificaron por separado en Play Mode.

| Métrica | Antes | Después |
|---|---:|---:|
| Encuentros | 14 | 35 |
| Pasos medios entre encuentros | 34.4 | 13.6 |
| Mayor intervalo total | 114 | 55 |
| Mayor tramo continuo sin encuentro en área peligrosa | 99 | 55 |
| Pico de enemigos vivos de Zona 2 | 2 | 7 |
| Nuevas apariciones a menos de 6 pasos | — | 0 |
| Nuevas apariciones visibles en la simulación | — | 0 |

Registro completo:
```text
ANTES: route=125 steps, simulated=500, encounters=14, mean=34,4, max gap=114, trailing steps=19, dangerous steps=402, max continuous dangerous quiet=99, zone2 peak=2, close new=0, visible new=0, intervals=29,3,13,5,20,11,114,38,42,10,76,98,9,13
DESPUÉS: route=125 steps, simulated=500, encounters=35, mean=13,6, max gap=55, trailing steps=23, dangerous steps=402, max continuous dangerous quiet=55, zone2 peak=7, close new=0, visible new=0, intervals=29,6,4,9,28,3,6,36,11,4,6,7,6,5,16,19,13,6,21,26,11,8,8,10,27,8,13,55,11,5,3,29,8,10,10
```

La mejora de la media no garantiza un encuentro cada número fijo de pasos. Pueden quedar recorridos de hasta 55 pasos en esta prueba; no se fuerzan encuentros invisibles para eliminarlos. También se observaron intervalos cortos con enemigos ya presentes, que el descanso expresamente no bloquea.

## Pruebas Unity MCP / Play Mode
- BFS de pasillo con paredes: dos casillas a distancia euclidiana 4 requieren 16 pasos reales.
- Habitación abierta: distancia navegable correcta de 10 pasos.
- Callejón sin salida alcanzable; habitación desconectada excluida.
- Cero casillas de refugio aceptadas por el generador.
- Descanso activo rechaza nuevas apariciones.
- Encuentros solicitados de 1, 2 y 3 enemigos producen exactamente 1, 2 y 3.
- Vida, ataque y defensa coinciden con EnemyLevelSettings.Evaluate al nivel asignado.
- Descanso guardado = 6; restaurado = 6.
- Los candidatos nuevos de la simulación cumplen distancia mínima y ocultan apariciones fuera de visión.
- Temporizadores pospuestos no se reinician; verificado también en las ramas de código.
- Consola sin errores al compilar y ejecutar estas pruebas.
- No se sobrescribieron los slots personales.

Guion: Tests/EncounterDirectorMcpTest.txt. Fixture: Tests/DirectorRouteFixture.json. Para reproducir, iniciar Play, abrir las puertas de navegación de la escena, ejecutar el guion como cuerpo de método mediante Unity MCP. El fixture conserva el estado inicial de prueba.

## Cuándo no genera
Durante combate, introducción, pausa, diálogo, movimiento pendiente o descanso; en zonas seguras/desactivadas; al llegar al límite de población; si ya hay presencia cercana suficiente; si hay un respawn vencido que debe procesarse primero; o cuando no hay casilla alcanzable, separada, fuera de visión y lejos de puertas/objetos. Nunca traslada enemigos vivos para llenar un sector.

## Textos que atravesaban paredes
Los TextMesh del dungeon reciben WorldTextOcclusion y un material con ZTest LEqual. Los rótulos se ocultan cuando una pared o puerta bloquea la vista; también siguen la visibilidad del sprite durante el parpadeo de muerte. En la prueba, siete rótulos tenían oclusión y cinco ocultos por obstáculos dejaron de verse. No afecta a los cuadros de diálogo del HUD.

