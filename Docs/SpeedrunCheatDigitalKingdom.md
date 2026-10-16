# SPEEDRUN, LEVEL UP CHEAT y Digital Kingdom

Implementado el 16 de octubre de 2026, por autorización de continuar el bloque pendiente. Respaldo previo: `Backups/SpeedrunCheatCity/20261016-011217/` (scripts, assets de combate/ciudad y partidas personales).

## Arquitectura e implementación

Se reutilizan `ResolveCombatDamage`, `RecordDefeated`, el registro de condiciones DRPG, los colectores de XP/drops, `PartyActionFinished`, la victoria normal y los snapshots existentes. No se introduce otro sistema de guardado ni otra escena.

SPEEDRUN es una acción contextual independiente de los HACKS. Requiere nivel de jugador >=10 y diferencia jugador-enemigo >=10. Cuesta exactamente 5 Bits. Nivel 10 contra nivel 1 no cumple; nivel 11 contra nivel 1 sí. No depende de HP, fuerza ni defensa.

Se excluyen rangos fuertes, subjefes y jefes del catálogo, categorías Elite/Boss, especies reservadas para eventos, invocaciones de combate y colocaciones únicas, interactivas, inmóviles, de exploración Elite o protegidas explícitamente. Esta exclusión conservadora evita eliminar actores narrativos. `protectedFromInstantDefeat` permite proteger futuras especies y colocaciones desde el Inspector.

El botón aparece a la derecha durante el turno disponible, cuando hay algún objetivo válido. Se deshabilita si faltan Bits. Clic, tecla R o botón norte del mando (Y en Xbox) preparan la acción. Con varios objetivos: flechas o Q/E, clic sobre un objetivo válido y ENTER o el botón SPEEDRUN para confirmar; ESC cancela. Se revalida al confirmar y se bloquean ejecuciones simultáneas.

El escaneo, marco cian, franjas de corrupción y fragmentos digitales duran 0,5 s. Shader temporal compatible con URP, textura del sprite existente, sin nuevas animaciones por enemigo. Los materiales y la animación original se restauran incluso al cancelar. El efecto no modifica el CRT global. El enemigo pasa después por el circuito normal de derrota; si quedan oponentes resuelven su turno normal.

Los cuatro mensajes de SPEEDRUN utilizan el panel SYSTEM MESSAGE inferior y sus prioridades existentes.

## Consumible

`level-up-cheat` se añadió al catálogo existente con nombre LEVEL UP CHEAT, cantidad inicial 0, descripción solicitada e icono de cartucho dorado dibujado con el sistema de iconos pixelados existente.

Su uso añade exactamente la XP restante del nivel actual mediante `GainPlayerXP` y `PlayerLevelSettings.AddXP`. Sube un nivel, conserva los cálculos de estadísticas, curación configurada y desbloqueos. La nueva barra de XP empieza en 0; al nivel máximo no se consume. Se puede utilizar desde el inventario y como ITEM en combate, donde ocupa la acción normal sin gastar Bits. Muestra CHEAT CODE ACCEPTED / LEVEL UP! / SYSTEM PARAMETERS UPDATED_ y un breve pulso digital.

Tifanía, Buho-Nero y San Solair entregan uno cada uno al terminar su bienvenida. Las marcas `city.welcome.gift.v1:<locationId>` se guardan en los `storyFlags` ya existentes. Una mochila llena conserva el regalo pendiente; se entrega al regresar con espacio. Las conversaciones ya vistas de partidas antiguas no se reinician: se muestra solamente la entrega pendiente cuando corresponde.

## Mochila y NPC

Botón MOCHILA abajo a la izquierda de la ciudad; también tecla I. Permite abrir INVENTARIO y PERSONAJE reutilizando las pantallas actuales. Funciona con ratón, teclado y mando. Conserva el estado de ciudad y el punto de retorno al cerrar.

| Establecimiento | Nombre visible |
|---|---|
| The Last Life | Tifanía |
| Pay 2 Win | Buho-Nero |
| Temple of Continue | San Solair |
| Bug Lab | Dr. Billy Byte |
| Fast Travel Station | Capitán Turbo |
| RNG Sewers | Don Peligro |

Cada asset de establecimiento contiene su personalidad, nombre y diálogo. Se añadieron presentaciones breves y las frases de regalo; se conservan los parlamentos anteriores. Los avisos de historia y la bienvenida al Reino del Píxel usan los nuevos nombres. Don Peligro incluye «Es peligroso entrar solo. Pero si vas acompañado, también.».

## Pruebas y resultados

`Tests/SpeedrunCityResults.txt`: **102 comprobaciones correctas / 0 fallidas** en Unity Play Mode mediante MCP.

- Seis ejemplos de diferencia de niveles; HP/DEF no alteran elegibilidad; rangos y eventos excluidos; selección filtrada y revalidación.
- Bits insuficientes sin costo ni turno, doble ejecución rechazada, un cargo de 5 Bits, una sola derrota, enemigos restantes continúan y último enemigo produce victoria normal.
- XP 0 por gran diferencia de niveles según la configuración existente, registro de recompensas y ausencia de duplicados; regresión de ATTACK.
- Consumible con barra de XP casi llena, cantidad exacta, nivel máximo, uso en combate, crecimiento de estadísticas y desbloqueo real de OVERCLOCK al pasar de 7 a 8.
- Restauración de materiales tras interrumpir el efecto.
- Partida antigua sin el nuevo objeto, guardado/carga, ida a Digital Kingdom y regreso, persistencia de nivel, cantidades y punto de retorno.
- Tres regalos, sin duplicaciones tras recargar/revisitar; mochila llena y reintento posterior.
- Los seis slots se escribieron y leyeron como fixtures bajo Tests.
- Capturas de combate a 1280×720, 1920×1080 y 1024×768; revisión visual de la posición del botón, escaneo e inventario. Capturas en `Captures/SpeedrunCity/`.
- Con límites de 60 y 120 FPS: una eliminación y un cargo, tiempos observados 0,508 s y 0,505 s. Son comprobaciones funcionales con límite de FPS, no un perfil completo de rendimiento GPU.
- Consola sin errores en las pasadas verificadas. Tras las 102 comprobaciones se permitió además mostrar SYSTEM MESSAGE al consumir el objeto dentro del inventario pausado de exploración; compilación y uso verificados, sin repetir toda la batería por este ajuste de presentación.

Incidencia del primer fixture: al descargar el mapa experimental, su componente restauró la ruta de guardado anterior. La prueba creó «City fixture» en el slot 2, que estaba vacío; no sobrescribió una partida. Se comprobó que no existía en el respaldo, se verificó su etiqueta y se trasladó a `Tests/SpeedrunCityAccidentalFixture.json`. El fixture se corrigió para establecer y comprobar la ruta dentro de Tests antes de cada escritura. La pasada final ya usa este aislamiento. Todos los archivos personales originales coinciden por SHA-256 con el respaldo; slot 2 volvió a quedar vacío. Evidencia en `Tests/SpeedrunCityIntegrity.txt`.

## Edición y límites

NPC y regalos: `Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset` a `06-city.sewers.asset`. Nombre, Personality, Dialogue, Welcome Gift ID/Quantity/Dialogue están en el Inspector. Historia y aviso del reino: `Assets/Resources/DigitalKingdom/Story.asset`.

Consumible: `Assets/Resources/Combat/CombatInventorySettings.asset`. Reglas de SPEEDRUN: constantes al inicio de `DungeonPrototype.Speedrun.cs`; duración al iniciar el efecto en el mismo archivo. El menú Codebound/SPEEDRUN y ciudad/Configurar consumible y NPC es un instalador idempotente, no hace falta ejecutarlo otra vez para jugar y puede reponer los textos predeterminados si se ejecuta tras editarlos.

El efecto visual se ha comprobado con los sprites actuales de Tapaku; usa las texturas y tamaños de los renderers para otros enemigos elegibles, pero no se capturó cada especie individual. No se hizo un recorrido manual exhaustivo con todos los modelos de mando. La biblioteca de iconos sigue el estilo de contornos pixelados existente.

Movimiento, fórmula de daño, curva de niveles, drops, catálogo de enemigos y formato de los seis slots permanecen iguales por hash. No se tocaron mapas, geometría, IA, minijuegos, Biblia V1.0 ni fuentes históricas.

## Archivos del bloque

- `Assets/Editor/SpeedrunCityInstaller.cs`
- `Assets/Resources/Combat/CombatInventorySettings.asset`
- `Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset`
- `Assets/Resources/DigitalKingdom/Locations/02-city.shop.asset`
- `Assets/Resources/DigitalKingdom/Locations/03-city.temple.asset`
- `Assets/Resources/DigitalKingdom/Locations/04-city.lab.asset`
- `Assets/Resources/DigitalKingdom/Locations/05-city.travel.asset`
- `Assets/Resources/DigitalKingdom/Locations/06-city.sewers.asset`
- `Assets/Resources/DigitalKingdom/Story.asset`
- `Assets/Resources/Shaders/SpeedrunDelete.shader`
- `Assets/Scripts/CodeboundSystemMessages.cs`
- `Assets/Scripts/CombatInventorySettings.cs`
- `Assets/Scripts/DigitalIcons.cs`
- `Assets/Scripts/DigitalKingdomLocation.cs`
- `Assets/Scripts/DungeonPrototype.CheatAndBackpack.cs`
- `Assets/Scripts/DungeonPrototype.CityModel.cs`
- `Assets/Scripts/DungeonPrototype.DigitalKingdom.cs`
- `Assets/Scripts/DungeonPrototype.Enemies.cs`
- `Assets/Scripts/DungeonPrototype.InventoryGrid.cs`
- `Assets/Scripts/DungeonPrototype.Pause.cs`
- `Assets/Scripts/DungeonPrototype.PlayerLevels.cs`
- `Assets/Scripts/DungeonPrototype.Save.cs`
- `Assets/Scripts/DungeonPrototype.Speedrun.cs`
- `Assets/Scripts/DungeonPrototype.cs`
- `Assets/Scripts/DungeonRegionSettings.cs`
- `Assets/Scripts/EnemyCharacterCatalog.cs`
- `Assets/Scripts/SpeedrunCityValidation.cs`
- `Assets/Scripts/SpeedrunDeleteEffect.cs`
- `Assets/Scripts/SystemMessageSettings.cs`

Incluye los .meta generados por Unity. Documentación: este informe, Docs/PendienteSpeedrun.md, Docs/Narrativa/14_DigitalKingdom_NPC.md, índice narrativo y registro de cambios. Pruebas y capturas indicadas arriba.
