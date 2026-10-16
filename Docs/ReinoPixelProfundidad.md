# Reino del Pixel: profundidad 2.5D y ajustes del prototipo

## Tecnica y reversibilidad

Las paredes existentes tienen un plano de cielo y recortes con alpha-test para nubes, montanas y arbustos. Se conservaron sus texturas y posiciones logicas. La prueba amplia el desplazamiento UV segun el vector entre camara y superficie en el espacio tangente de cada pared; cada capa comparte un material por textura y factor de profundidad.

Factores base: nubes 0.006, montanas 0.025, arbustos 0.075; multiplicador general 1.5. El cielo queda fijo. Las montanas reciben tinte azul y contraste reducido; los arbustos mantienen su color definido. No se usa movimiento automatico por tiempo.

La direccion de vista se calcula en coordenadas de mundo y se limita en angulos rasantes; girar la camara no desplaza por si solo una textura inmovil. Un margen horizontal transparente acomoda el movimiento aparente, manteniendo las bases de montanas y arbustos apoyadas en el suelo. Cada composicion queda dentro de su tramo de pared; no cubre entradas ni cambia colisiones.

Referencia primaria consultada: [implementacion de parallax de Unity Graphics](https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl). La implementacion de Codebound es una aproximacion multicapa limitada, no raymarching ni POM completo.

Editar `Assets/Resources/RetroDungeonExperiment/Settings.asset`:

- Layered Depth: desactivar la prueba de profundidad y atmosfera.
- Depth Strength: intensidad general; 0 anula el desplazamiento.
- Cloud / Mountain / Bush Parallax: factores independientes. Mantener nubes < montanas < arbustos.
- Mountain Atmosphere y Depth Atmosphere: tinte atmosferico de las montanas.
- Normal Strength: intensidad del relieve solicitado por separado.

Los controles de profundidad, intensidad de normales y niebla se actualizan durante Play sin reconstruir. El menu contextual **Reconstruir composicion experimental** queda para cambios de distribucion y texturas. Reconstruir es una herramienta de prueba visual; hacerlo fuera de una animacion de salida de enemigo.

El efecto simula profundidad, pero las siluetas siguen siendo planos y no crea espacios visitables detras de la pared. Desde angulos muy laterales se limita el desplazamiento para evitar deformaciones; una esquina sigue siendo una esquina de la geometria original. No se generaron texturas para el parallax. Su prueba no cambia la geometria, techo, suelo, iluminacion ni CRT configurados para este nivel.

## Normales solicitadas

Se prepararon 11 copias de las imagenes originales con TextureImporter.NormalMap y convertToNormalmap de Unity, altura 0.035 y filtro Point. Unity convierte su luminancia al importar; el shader usa esas normales. Son una aproximacion al relieve de pixel art, no mapas de volumen pintados a mano. Settings.normalMaps enlaza cada textura con su normal; se pueden sustituir por normales propios. Esto corresponde al encargo anterior de normales, independiente del parallax.

## Suelo, combate, tuberias y musica

El reflejo conserva DungeonFloorReflection. Usa el campo existente de distancia a paredes: cerca de ellas permanece definido; hacia el centro usa niveles mip mas suaves y reduce progresivamente su intensidad. No se agrego otra camara de reflejo.

El minimapa y el nombre de zona se ocultan durante combate y reaparecen al terminar. No se borran explored ni visited.

Tuberia vertical interactiva (17,14): el enemigo aparece desde el marcador Top mouth, situado en la abertura superior, y desciende al suelo. Tuberias frontales en (5,12) orientada al sur y (11,15) orientada al este: utilizan la imagen entregada FrontPipe.png, un anillo hueco extruido y un marcador Front mouth. Acercarse a dos casillas permite ver la salida. Son decorativas para navegacion; no se modifican colisiones ni la cuadricula. El sistema usa los mismos enemigos, IDs, factory y persistencia.

Las tuberias permiten aparicion visible de forma explicita para mostrar la animacion. Las apariciones normales conservan la restriccion de ocurrir fuera de vista. Siguen comprobando espacio, distancia por navegacion y el limite compartido de tres enemigos. Hay dos enemigos normales iniciales y el director vuelve a evaluar cada dos pasos; las tuberias no pueden rebasar ese limite.

Musica propia: `Assets/Resources/RetroDungeonExperiment/Audio.asset`. Exploracion: Pixel Platformer y Pixel Platformer 2. Combate: Bit-Battle y Bit-Battle 2. Se usa la fuente unica y las listas actuales, sin superponer canciones. CRT propio: `Assets/Resources/RetroDungeonExperiment/CRT.asset`.

Titulo: sin panel detras de las opciones; botones de 300x42 en lugar de 400x56. Musica Virtual Worlds Main Title enlazada en CodeboundTitle.unity y reproducida con DungeonAudio.

## Verificacion

Tests/RetroDepthFollowupResults.txt registra movimiento adelante/atras, giros, parallax en GPU, normales importadas y enlazadas, niebla, campo de distancia del reflejo, spawns normales y salida exacta desde bocas frontales/superior. Se revisaron capturas de pasillo y sala amplia. Tests/KingdomFollowupResults.txt verifica viaje real y snapshots. Los resultados finales y limitaciones se complementan en DigitalKingdomGuiaYAvisos.md.

Se comprobo ademas el caso de acercarse al enemigo antes de terminar su salida: la presentacion de combate toma el control del sprite y conserva una posicion de regreso a nivel del suelo. Los textos y sprites de combate mantienen su funcionamiento actual. La muestra de rendimiento en Editor esta en Tests/RetroDungeonPerformance.txt; no garantiza 60/120 FPS de una build.

Actualizacion vigente: Docs/AtmosferaViajesYMenu.md. Incluye las tres variantes de niebla y los viajes agrupados.
