# Zona 0 — El refugio de los olvidados

## Implementación

La sala sigue la distribución corregida: parte inferior para GamerBoy (17,2), terminal a la izquierda (14,2) y nodo a la derecha (19,9); muro interior en y=5 con un paso central x=17; cinco NPC en el área superior; puerta de salida en (17,9.5). La salida conecta físicamente con Zona 1 mediante un pasillo exterior. No se crea otra escena, no se muestran manos/cuerpo del jugador y se conserva el movimiento y la introducción original.

Zona 0 no permite enemigos aleatorios ni persecución. La única excepción explícita es el encuentro manual, único y por interacción del Devoto tras la tragedia. Zona 1.5 mantiene sus siete casillas seguras y puertas con cierre automático. Los planos, regiones y assets anteriores tienen copias en Assets/ZoneBackups; los scripts previos se respaldaron en Backups/Zona0-antes-de-integrar.

Las conversaciones usan DialogueTypewriter, NeonDungeonUI y los sonidos ya existentes. E permite interactuar; E, SPACE y ENTER completan las letras o avanzan. Los NPC conservan por ID cuántas conversaciones terminadas tienen. La conversación de salida se activa una vez al acercarse a la puerta.

El terminal restaura toda la vida y guarda en Codebound-save-v1.json dentro de Application.persistentDataPath. No existía restauración completa de partidas; se añadió DungeonSaveStore y un módulo DungeonPrototype.Save utilizando los registros de jugador y enemigos existentes. El menú de pausa (ENTER fuera de combate) ofrece CARGAR ÚLTIMA PARTIDA si hay un archivo. Al iniciar una partida nueva se reproduce la introducción; cargar una partida con introducción completada la omite.

Los nodos tienen ID, nombre y casilla. Primero se registran; solo se ofrecen destinos registrados. Elegir destino usa flechas y ENTER o clic, ESC cancela. El refugio y Zona 2 tienen nodos iniciales configurados. Registrar el nodo de Zona 2 activa el cambio permanente en memoria, sin aviso, objetivos, marcador de misión ni instrucción para volver. Se conserva al guardar y cargar. El jugador puede continuar sin regresar ni descubrirlo.

Los cuatro cuerpos permanecen hasta terminar sus últimas palabras, una sola vez por ID. Se guardan individualmente. Su desaparición temporal reduce la figura durante 0,6 segundos; no existe un recurso propio de desintegración para esos NPC. El Devoto amistoso desaparece y se crea su encuentro reservado en el rincón inicial. Hablar inicia la conversación de nueve líneas y luego el combate existente. Su nivel es configurable; valor inicial 4. Usa EnemyCharacterCatalog, EnemyLevelSettings y CombatMath; ninguna fórmula paralela.

Al derrotarlo no reaparece; muestra sus últimas palabras y obtiene Núcleo del Refugio, accesorio Defensa +3 / Suerte +2. Usa el catálogo de equipo, inventario y tres ranuras existentes. Si el inventario está lleno, la entrega queda pendiente hasta liberar espacio; no se pierden ni duplican objetos. El terminal, tras completar el evento, agrega Usuarios activos en el refugio: 1. Los cuerpos no examinados siguen disponibles.

## Editables

- Editor → Zona 0 — Refugio: Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset. Coordenadas de inicio/terminal/puerta, NPC, diálogos, retratos, colores temporales, ID/nombre/casilla de cada nodo, nivel del Devoto y conversaciones de salida/previas al combate.
- Editor → Plano físico del dungeon: PlanoDungeon.asset, paredes y suelos. Las filas se ordenan de sur a norte.
- Editor → Zonas y encuentros: DungeonRegions.asset, casillas y áreas seguras; encuentro manual refuge.devoto. Su ID estable, especie reservada y excepción explícita en refugio se conservan al abrir el Inspector.
- Editor → Personajes: estadísticas del Devoto bajo Subjefes, en el catálogo existente; ataques normales/especiales, fallos, crítico y escalado.
- Editor → Objetos equipables: Núcleo del Refugio, ID refuge.core, icono, ranura, bonos y descripción. Se añadió descripción sin cambiar los objetos anteriores.
- Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO: frases contextuales editables. LowHealth se añadió al final del enum para conservar valores existentes.

Si cambias el plano físico, ajusta también las regiones, coordenadas de puerta, NPC y dispositivos. El dibujo de regiones no abre paredes.

## Recursos temporales que debes reemplazar

Cinco figuras de cápsula identificadas por nombre y color, con pose acostada para cuerpos. Retratos opcionales desde el Inspector. Terminal y nodos son figuras geométricas con letreros. El Devoto usa los sprites y animaciones de Big Tapaku, incluidos los de JUMP; sus estadísticas e ID son propios. Núcleo tiene un icono pixelado temporal. Faltan arte de NPC/víctimas, Devoto normal y corrompido, terminal, portal, núcleo y un efecto propio de desintegración.

## Pruebas Unity MCP

36 comprobaciones en Play Mode: inicio real e introducción; cinco NPC y primeras/segundas líneas; terminal cura y escribe; nodo inicial y bloqueo de destino no descubierto; salida única; activación sin notificación; regreso; cuerpos; Devoto escalado; últimas palabras; guardado y restauración desde disco; conversación previa y arranque real del combate.

JUMP se inició con el rival y defensa escalada del Devoto. Es una prueba de arranque, no una partida completa jugada a mano. La derrota se ejecutó mediante la corutina real de daño, reduciendo la vida del rival como fixture para completar la prueba rápidamente. Diez comprobaciones adicionales verificaron últimas palabras, recompensa única, equipamiento, terminal final, restauración desde archivo, desaparición definitiva del minijefe y conservación de cuerpos pendientes.

Se usó Docs/Refuge-test-save.json como archivo aislado; no se sobrescribieron partidas personales. Evidencia: Docs/PruebasRefugio.txt y Tests/RefugeMcpTests.cs (cuerpo de método para execute_code). Compilación y consola sin errores al finalizar.

## Pruebas manuales y límites

Revisar el recorrido a mano con tus futuros sprites, legibilidad de rótulos y balance del Devoto con equipo distinto. Probar rutas de regreso, cargar usando el botón del menú y viajes entre nuevos nodos que añadas. Todavía no existe un menú inicial de selección de múltiples ranuras de guardado; hay una partida local y la opción de cargar en pausa. El archivo guarda los estados solicitados y los registros existentes; no pretende mantener una partida compatible con cualquier edición posterior del plano.

Hay una casilla previamente editada en (5,0) que sigue sin zona. La conservamos sin reasignarla: el editor avisa y allí no genera enemigos aleatorios. No forma parte del refugio ni de su conexión nueva.


Actualización del refugio: el nodo REFUGIO está en (19,9), en la sala de NPC. La llegada usa dirección 2 (sur), mirando hacia los cuerpos; la terminal sigue en (14,2). Cada nodo permite ajustar Arrival Heading (0 norte, 1 este, 2 sur, 3 oeste).

Apariencias: Centinela = Tapaku; Archivista = Makuba Lanza; Cobarde = Makuba Arco; Optimista = Hongo; Devoto = Gran Makuba, también en combate. Se conservan las estadísticas propias del Devoto y sus diálogos. Visual Species Id en cada NPC controla su sprite.

Tras la tragedia, cruzar el paso a la sala inferior y mirar al Devoto a distancia de hasta dos casillas inicia automáticamente sus nueve líneas. Al terminar la conversación comienza el combate. No se requiere E para iniciarlo. Automatic Devoto Encounter, Refuge Passage Row y Devoto Approach Range se ajustan en Editor → Zona 0 — Refugio. El disparador permanece consumido mientras se sigue en la misma aproximación, para evitar reiniciar inmediatamente al huir.

Verificado mediante Unity MCP en Play Mode: cinco apariencias, llegada y orientación, ausencia de activación antes de cruzar o al mirar en otra dirección, diálogo automático, combate automático, Gran Makuba y terminal intacta; sin errores de consola. Pruebas con estado temporal sin guardar la partida del usuario.

La conversación de salida se inicia únicamente al pulsar E para abrir la puerta; la cámara gira hacia quien habla y vuelve a la puerta al terminar, iniciando su apertura. El teleport solo se usa con E sobre su propia casilla. Hablar con refugiados requiere tenerlos en la casilla inmediatamente delante.
