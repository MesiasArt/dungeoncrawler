# Refugio — revisión vigente de conversaciones y últimas palabras

Implementado el 15 de octubre de 2026 por petición expresa del usuario: «cambia los dialogos del refugio como tu los sugeriste, tambien ajusta los dialogos de cuando ellos mueren en el refugio». Esta autorización permite revisar esos parlamentos existentes. Las cinco frases aprobadas previamente se conservan literalmente, incluida la corrección de Pacmiedo.

Fuente vigente de implementación: `Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset`. Las versiones anteriores y la Biblia V1.0 son históricas y no se reescriben. Esta revisión concreta no aprueba automáticamente todos los demás textos del juego.

## Solid Guarda — vigilancia y protección fallida

1. Cierra al pasar. Mientras nadie sepa que estamos aquí, podemos proteger este lugar.
2. Si oyes golpes, avísame antes de abrir.
3. Si sales, fíjate en lo que hay al otro lado antes de volver. No queremos que algo te siga.

**Últimas palabras:** «No entró nadie... yo estaba vigilando.»

## Zeldoro — detalles de memoria e identidad

1. Antes éramos más. Recuerdo las conversaciones, pero cada vez me cuesta más ponerles una cara.
2. Recuerdo que aquí faltaba una lámpara. Es extraño lo que uno consigue recordar.
3. A veces recuerdo una voz. Intento escribir lo que decía antes de volver a olvidarlo.

**Últimas palabras:** «Recuerdo una voz... me llamaba por mi nombre.»

La voz recordada no se identifica con Omnia ni con otra entidad. Los recuerdos son testimonios del personaje, no una revelación objetiva nueva.

## Pacmiedo — precauciones concretas y seguridad perdida

1. ¿Vas a salir? Espera. ¿Tienes con qué defenderte?
2. Si vas a salir, ¿puedes cerrar despacio? No quiero saber quien oye esa puerta.
3. Yo me quedo aquí. Desde este rincón puedo ver la entrada.

**Últimas palabras:** «Me quedé aquí... para estar a salvo.»

La frase aprobada por el usuario se preserva con su escritura exacta, incluido «quien».

## Toadavía — ayuda práctica y confianza

1. ¡Alguien nuevo! Ven, cuéntanos qué has visto. Quizá podamos ayudarte.
2. Si encuentras algo útil, tráelo. Entre todos veremos qué hacer.
3. Si encuentras ayuda, ¿podrías volver por nosotros? Yo les diré que estás buscándola.

**Últimas palabras:** «Volviste... sabía que no nos dejarías.»

La despedida expresa confianza, sin acusar al jugador de haber causado la tragedia.

## Prayman — amabilidad y consuelo inquietante

1. Puedes quedarte un rato. Aquí todos necesitamos recuperar el aliento.
2. A veces sueño con una voz. Cuando la escucho, todo parece más sencillo.
3. Siéntate un momento. No tienes que resolverlo todo ahora.
4. Ojalá los demás pudieran sentirse así. Se pasan el día esperando que ocurra algo malo.

Se conserva su confrontación existente y su frase tras la derrota: «Yo también... tenía miedo...». Prayman no comparte el evento de últimas palabras de las cuatro víctimas; no se cambia esa lógica ni se crea otro fallecimiento.

## Cerrojio

Nombre confirmado por el usuario para el guía independiente de las puertas, en X17/Y34. Es un NPC distinto de Solid Guarda. Sus datos, nombre y diálogo no se modificaron en esta revisión; la huella SHA-256 del asset que lo contiene fue idéntica antes y después.

## Implementación y comprobaciones

- Revisadas 16 respuestas habituales y cuatro despedidas. Se conservan los conteos, IDs, posiciones, visuales, retratos, estados de conversación y todos los demás campos del asset. Comparación semántica de los demás campos verificada antes de guardar.
- Permanecen intactos: introducción especial del Centinela sobre nodos, despedida de salida y pociones, variantes de Prayman, reacciones de llegada y primer cuerpo, condiciones de tragedia, combate y recompensas, Makong y The Last Life.
- 20 comprobaciones correctas en el editor: texto exacto, hablante y revelado mediante WorldDialogue. Asset guardado y consola sin errores al finalizar.
- Una primera comprobación de avance en Edit Mode no pudo completarse porque el sonido de avance necesita su inicialización en ejecución. No se modificó el audio del juego; se retiró el objeto temporal creado por esa comprobación. La revisión final valida contenido y revelado, no avance con audio ni recorrido en Play Mode.
- Se conserva el estado de partidas existentes: últimas palabras ya escuchadas no vuelven a mostrarse; conversaciones habituales siguen el contador existente. No se reiniciaron partidas.
- Pendiente de evaluación visual: ritmo, audio y presentación jugando. Se salió de Play Mode para editar los datos y se dejó el editor fuera de ejecución.