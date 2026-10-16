# Bienvenida del refugio y cierre del prototipo

Implementado el 15 de octubre de 2026.

- Evento automático en X17/Y6, suelo libre confirmado de Zona 0. Solo partidas nuevas armadas desde Nueva partida; no se activa en partidas antiguas avanzadas.
- 33 parlamentos exactos de la revisión: Zeldoro abre la conversación y se incluye el intercambio de Toadavía y Pacmiedo.
- Giro suave para mirar detrás tras la advertencia de Pacmiedo, pausa y vuelta. No desplaza la casilla ni altera la orientación lógica. El silencio colectivo es una pausa antes de Zeldoro.
- El evento usa DungeonInteractionSettings y los completedEvents/executions existentes en los snapshots. No consume ni modifica las conversaciones individuales.
- Configuración: Assets/Resources/Exploration/RefugeWelcome.asset y RefugeWelcomeDialogue.asset. Guion esperado en Tests/RefugeWelcomeExpected.json; instalador explícito Editor/Narrativa/Instalar bienvenida del refugio.
- Escena inicial independiente Assets/Scenes/CodeboundTitle.unity, primera en Build Settings. Usa Assets/UI/Codebound Title Screen.png y Codebound Title Screen Logo.png por referencia; Nueva partida, Continuar (seis slots existentes) y Ajustes (preferencia CRT y volumen). No instancia dungeon ni geometría. El resumen de slots reutiliza DescribeSaveSlot.
- Prólogo sin omisión por ENTER ni texto para omitir; SPACE/A continúa el texto. Retrato y vistazo con glitch conservados.
- Digital Kingdom: Exit City oculto de la lista compartida por ratón/teclado/mando; sin instrucciones inferiores. Bug Lab conserva Mejoras de HACK y Volver. Servicios de mejora siguen siendo provisionales: no se inventaron mecánicas. El retorno mediante nodos/API existente se conserva.
- La salida tras Makong es una superficie ovalada de luz, sin panel deslizante visible, rótulo en la luz, colisión ni aviso E. Cruzar hacia adelante utiliza la transición blanca existente; se bloquea el cruce antes de derrotar al jefe. El ID persistente de la salida se conserva.

## Evidencia

- Tests/RefugeWelcomeResults.txt: comprobaciones de disparador por movimiento real, orden de 33 líneas, giro, pausa, control, persistencia y compatibilidad aprobadas (el conteo incluye muestras por frame durante el giro).
- Tests/TitleResults.txt: siete comprobaciones aprobadas de escena ligera, arte, orden de arranque, Nueva partida sin menú duplicado, prólogo sin skip y Continue con posición/nivel conservados.
- Tests/PrototypePortalResults.txt: tres aprobadas; Tests/PrototypeCityMenuResults.txt: dos aprobadas.
- Tests/DigitalKingdomSceneMcpResults.txt: 38 regresiones aprobadas, incluida transición real, audio, ciudad, guardado/carga, nodos y retorno.
- Tests/PrologueResults.txt: regresión completa, incluida negativa de omisión. Consola sin errores en la ejecución final. Capturas revisadas en Captures/PrototypeFinish/Title.png y Captures/RefugeWelcome.
- Pruebas de guardado exclusivamente en Tests. No se sobrescribieron slots personales ni se alteró geometría de niveles. Respaldo Backups/PrototypeFinish/20261015-182541 y Backups/RefugeWelcome/20261015-172457.

## Guion vigente

**ZELDORO:** Un momento. No recuerdo haberte visto llegar.

**SOLID GUARDA:** ¡Eh! ¡Tú! ¿De dónde saliste?

**GAMERBOY:** ¡Ah! ¡Por fin alguien! Oigan, ¿dónde estoy?

**SOLID GUARDA:** ¡Las preguntas las hago yo! ¿Cómo entraste aquí?

**GAMERBOY:** No tengo ni idea. Acabo de despertar.

**ZELDORO:** Qué extraño... Estoy seguro de que antes no estabas aquí.

**TOADAVÍA:** ¡Pues ahora sí está! ¡Y parece bastante confundido!

**PACMIEDO:** ¡Esperen! ¿Y si no vino solo?

**GAMERBOY:** ¿Qué?

**PACMIEDO:** ¡Mira detrás de ti! ¿Te siguió alguien?

**GAMERBOY:** ¿¡Quién se supone que me iba a seguir!?

**PACMIEDO:** ¡No lo sé! ¡Por eso pregunto!

**SOLID GUARDA:** ¡Ya basta! Estás poniendo nervioso al recién llegado.

**GAMERBOY:** Un poco tarde para eso.

**TOADAVÍA:** ¡Bueno, bueno! Lo importante es que estás aquí. Este lugar es nuestro refugio.

**GAMERBOY:** ¿Refugio? ¿De qué se están escondiendo?

**ZELDORO:** De muchas cosas. Aunque últimamente cuesta recordar cuáles.

**GAMERBOY:** ¿También tienen problemas para recordar?

**ZELDORO:** ¿También?

**GAMERBOY:** Yo estaba con alguien... Pero no recuerdo quién.

**PACMIEDO:** ¡Eso es terrible!

**TOADAVÍA:** ¡Pacmiedo!

**PACMIEDO:** ¿¡Qué!? ¡Lo es!

**PRAYMAN:** Déjenlo tranquilo. Acaba de despertar.

**PRAYMAN:** No necesitas encontrar todas las respuestas ahora.

**GAMERBOY:** Pues me gustaría encontrar al menos una.

**PRAYMAN:** No te preocupes. Todo va a estar bien.

**GAMERBOY:** ¿Cómo puedes estar tan seguro?

**PRAYMAN:** ...

**PRAYMAN:** Digamos que alguien me lo prometió.

**SOLID GUARDA:** Bueno, ya tendremos tiempo de hablar. Por ahora, quédate dentro de las paredes.

**TOADAVÍA:** ¡Y bienvenido al refugio!

**GAMERBOY:** ...Gracias, supongo.

