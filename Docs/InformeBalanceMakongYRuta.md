# Balance de Makong y ruta de preparación

Makong LV10: HP210 / ATK13 / DEF9 / LUK6 / terremoto16. El objetivo preliminar HP180/ATK11/DEF8 se reforzó tras pedir que el primer subjefe no fuera fácil. Bases105/8/6/5/especial10, crecimiento local HP1/9, ATK4/63, DEF0.06, LUK0.03; se aplica una sola vez EnemyLevelSettings. CombatMath permanece intacto.

## IA
Terremoto: cooldown3 turnos completos, prioridad90, probabilidad1. Devorar: HP estrictamente inferior45%, cura18, máximo2 usos, cooldown4, ayudante de al menos2 turnos; no XP ni drops del consumido. Llamada: cooldown4, probabilidad75%, doble65%, máximo2 ayudantes vivos; niveles7–9, Tapaku/Hongo existentes, no respawn ni población guardada. Un turno normal separa especiales. Solo invoca con HP≥45%. Prioridades/probabilidades se editan en Combat/MakongSpecialSettings.

## Equipamiento
No se equipa al recogerlo. Tres ranuras originales conservadas.
- Power Glove MK-II: ATK+7 LUK+3; cofre X17/Y26.
- T-Shirt de Resistencia: HP+12 DEF+7; cofre X2/Y44.
- Medias del Glitch: HP+6 DEF+2 LUK+6; cofre X21/Y51.
GamerBoy LV10 con las tres piezas: HP66/ATK17/DEF15/LUK14. El equipo inicial y las bases originales se conservan.

## Coordenadas verificadas
| Elemento | Coordenada real | Configuración |
|---|---|---|
| Primera puerta | X16/Y42 | Abierta, orientación este; entrada habitación derecha X18–21/Y40–45 |
| Puerta roja | X3/Y46 | Llave Roja |
| Puerta verde | X20/Y47 | Llave Verde; misma entidad desplazada desde X20/Y48 para impedir rodearla |
| Puerta de Makong | X11/Y46 | Llave Maestra |
| Makong | X12/Y53 | Nivel10, único, sin respawn |
| Llave Roja | X20/Y41 | Cofre existente reutilizado; Big Tapaku LV7 en X19/Y44 |
| Llave Verde | X1/Y51 | Gran Makuba LV8 en X3/Y50 |
| Llave Maestra | X19/Y51 | Gran Makuba LV9 en X20/Y50 |

Guardianes únicos e inmóviles: recompensas garantizadas una poción pequeña, una pequeña y una mediana respectivamente, además de drops normales existentes. No alteran las poblaciones aleatorias. Cofres vinculados a su guardián: deben derrotarlo antes de recibir la llave. El Hongo opcional LV4 X4/Y45 se conserva.

BFS por aristas de puerta, considerando cofres/NPC/savepoints como obstáculos: sin llaves354 casillas, roja374, roja+verde392, tres llaves416. Makong solo accesible en el último estado; todas las llaves disponibles antes de su puerta; los tres equipos accesibles antes del subjefe. Los regresos son posibles por las mismas rutas bidireccionales. Se conserva cada ID de puerta. X17/Y24 no contenía una puerta.

## Editor
Herramienta Vacío: ~, sin suelo ni pared, conserva techo, bloquea navegación sin caída; no admite entidades. Se representa oscuro en el mapa y se retira de las regiones al pintarlo. Undo/Redo disponible. Propiedades del objeto seleccionado muestran únicamente campos pertinentes; las propiedades ocultas se conservan. Cofres incluyen guardián; puertas incluyen inicio abierto y llave únicamente cuando están bloqueadas. Guardianes y pociones editables en Regiones y encuentros.

## Simulación reproducible
38,400 combates: 200 semillas por escenario, LV8–11, cuatro equipos, tres desempeños de JUMP/OVERCLOCK, dos stocks, dos estrategias. HP inicial60–100%. CombatMath y MakongTactics reales; desempeño humano aproximado, no una predicción garantizada.

| LV10, pocas pociones, elimina ayudantes | Desempeño | Victoria | Turnos medios |
|---|---|---|---|
| Inicial | Bajo | 0.00% | 14.91 |
| Inicial | Medio | 0.00% | 15.96 |
| Inicial | Alto | 1.00% | 16.65 |
| Tres piezas nuevas | Bajo | 0.00% | 24.28 |
| Tres piezas nuevas | Medio | 38.50% | 24.22 |
| Tres piezas nuevas | Alto | 93.00% | 19.38 |

Resultados completos: ../Tests/MakongBalanceSimulation.csv y MakongBalanceStats.csv. El total invocado durante el combate puede superar dos, aunque nunca hay más de dos vivos.

## Riesgo de progresión
- LV8, zone-1: XP media/enemigo 1.957; 171 enemigos para el siguiente nivel.
- LV9, zone-1: XP media/enemigo 0.910; 444 enemigos para el siguiente nivel.
- LV10, zone-1: XP media/enemigo 0.606; 790 enemigos para el siguiente nivel.
- LV8, zone-2: XP media/enemigo 2.483; 135 enemigos para el siguiente nivel.
- LV9, zone-2: XP media/enemigo 1.510; 268 enemigos para el siguiente nivel.
- LV10, zone-2: XP media/enemigo 0.782; 612 enemigos para el siguiente nivel.

Recomendación pendiente de decisión: un tramo localizado con enemigos7–9 antes del subjefe o ajustar su nivel de entrada. No se cambiaron zonas, pesos, Director ni curva de XP. La recuperación completa al subir se activó por instrucción posterior del usuario; la simulación del combate no otorga XP hasta terminar y por ello no añade curaciones dentro de la pelea.

## Verificación
Unity MCP en Play Mode: Makong89PASS/0FAIL (invocación, daño con DEF, devorar sin XP, victoria/guardado, JUMP/OVERCLOCK, introducción). Editor y ruta: pruebas adicionales en Tests/MapInfrastructureMcpResults.txt. Consola sin errores de compilación. Laboratorio listo con Makong10/GamerBoy10, selector de cuatro conjuntos de equipo. No hubo apagado nuevo.
