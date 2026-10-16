# SPEEDRUN y LEVEL UP CHEAT — Implementados

Autorizado e implementado el 16 de octubre de 2026. Informe, controles y pruebas: [SPEEDRUN, consumible y Digital Kingdom](SpeedrunCheatDigitalKingdom.md). Las notas siguientes conservan el alcance solicitado.

Auditar combate, turnos, UI, progresión, Bits, inventario, recompensas, persistencia y efectos existentes.

SPEEDRUN: acción contextual, no HACK ni minijuego. Se desbloquea a nivel 10; cuesta 5 Bits; requiere nivel del jugador menos nivel enemigo >=10. Excluye jefes, subjefes, élites y enemigos protegidos por eventos usando las clasificaciones existentes. Botón contextual en la derecha, sin mover ATTACK/HACK/ITEM. Elegir solo objetivos válidos, revalidar al confirmar, gastar una vez y terminar el turno normalmente. Un fallo de validación no consume turno ni Bits. Efecto reutilizable de escaneo, glitch y borrado, duración 0,4–0,6 s; restaurar materiales; no cambiar CRT global. Usar SYSTEM MESSAGE inferior y el circuito normal de derrota, XP por zona, monedas, drops, eventos y victoria.

Mensajes: SPEEDRUN.EXE / WEAK TARGET CONFIRMED_; INSTANT CLEAR! / TARGET DELETED_; INSUFFICIENT BITS / EXECUTION CANCELLED_; ACCESS DENIED / INVALID TARGET_.

LEVEL UP CHEAT: consumible que sube exactamente un nivel mediante progresión existente, con XP coherente, estadísticas y desbloqueos. No consumir al nivel máximo. Icono de cartucho dorado con flecha ascendente. Descripción: «Un código de trucos prohibido que altera los parámetros del jugador. Sube un nivel al instante.» Usar SYSTEM MESSAGE: CHEAT CODE ACCEPTED / LEVEL UP! / SYSTEM PARAMETERS UPDATED_, con una breve animación digital.

Entregas únicas: uno de The Last Life, uno de Pay 2 Win y uno de Temple of Continue al conocerlos por primera vez. Evitar duplicaciones al cargar o repetir conversaciones. Añadir mochila abajo a la izquierda de Digital Kingdom para consultar inventario y personaje.

## NPC de Digital Kingdom implementados

- The Last Life: **Tifanía**, posadera amable, fuerte y protectora. Recibe a aventureros agotados y resuelve problemas a puñetazos si hace falta.
- Pay 2 Win: **Buho-Nero**, comerciante misterioso que intenta venderlo todo, incluso lo que debería ser gratis.
- Temple of Continue: **San Solair**, sacerdote de la luz de los puntos de guardado. Cada Game Over es una oportunidad de renacer.
- Bug Lab: **Dr. Billy Byte**, científico brillante, egocéntrico y poco confiable. Los bugs son oportunidades científicas.
- Fast Travel Station: **Capitán Turbo**, piloto presumido, energético, dramático y obsesionado con la velocidad. Presenta cada viaje como una carrera de campeonato.
- RNG Sewers: **Don Peligro**, guardián anciano con consejos poco útiles: «Es peligroso entrar solo. Pero si vas acompañado, también.»

Mantener los nombres propios de los establecimientos. El usuario autorizó retomar e implementar este bloque junto con SPEEDRUN, LEVEL UP CHEAT y la mochila. No modificar la Biblia V1.0 ni Docs/Narrativa/Fuentes.

Frase para retomar: «Continúa con SPEEDRUN, LEVEL UP CHEAT, la mochila y los NPC de Digital Kingdom que dejamos pendientes».
