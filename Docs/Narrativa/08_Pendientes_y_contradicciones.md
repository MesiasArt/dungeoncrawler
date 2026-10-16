# Pendientes, posibilidades y contradicciones

# 17. Elementos todavía pendientes de definir

Estos elementos no deben considerarse cerrados hasta desarrollarlos expresamente:

- La identidad del personaje digital que pidió ayuda a Miyuki.
- La causa exacta por la que el grupo quedó atrapado.
- El funcionamiento completo de la pérdida de recuerdos.
- El origen preciso del conflicto de Omnia y Protocolo Zero.
- Si Omnia actúa conscientemente, está equivocada o también sufre alguna alteración.
- Las historias personales de los cuatro Guardianes.
- El orden definitivo de sus encuentros.
- La relación exacta entre las distintas realidades de videojuegos.
- Las condiciones necesarias para escapar.
- El papel definitivo de Miyuki, Jixel y Nicole en la resolución.
- Los posibles finales y decisiones narrativas.

Estas preguntas deben conservarse abiertas para evitar contradicciones durante el desarrollo.

---


## Posibilidades de V1.0 que no deben cerrarse por inferencia

- Fuyo y Yoyo: posible vínculo exterior; su ayuda no debe resolver fácilmente el conflicto (§2).
- Guardianes: conflictos filosóficos orientativos; Nexus «puede explorar» aislamiento y ruptura (§6).
- GamerBoy: no establecer todavía poderes especiales contra Omnia ni condición de elegido (§7).
- Devoto: puede enfrentarse al jugador; el texto no define obligatoriedad ni resultado (§8).
- Makong: que Omnia también esté perdida es una posibilidad sugerida por él (§9).
- The Last Life: posibles servicios futuros; Bug Lab: futuras mejoras de HACKS; RNG Sewers: futura exploración procedural (§10).
- Dungeon de plataformas con parallax y tuberías: ejemplo visual; no una zona confirmada (§14).

## Ambigüedades documentales para consultar

No se han confirmado contradicciones con Unity: la implementación no se auditó y no se modificó.

| ID | Evidencia / asunto | Pregunta concreta | Estado |
|---|---|---|---|
| P01 | No se identificó una raíz de Codebound en los proyectos registrados. | ¿Cuál es la ruta exacta del proyecto Codebound donde debemos instalar `Docs/Narrativa`? | Pendiente. Entrega preparada aparte. |
| P02 | §2: Miyuki recibe ayuda de un personaje; ficha de Miyuki: «Su petición de ayuda inicial». | ¿Confirmas que la petición la envía el personaje digital a Miyuki y que la ficha se refiere a esa misma petición? | Ambigüedad de redacción, sin corrección. |
| P03 | §8: cinco habitantes; luego «Los cuatro refugiados aparecen muertos» y Devoto corrompido. | ¿Confirmas que los cuatro fallecidos son Centinela, Archivista, Cobarde y Optimista? | Lectura probable, no identidad cerrada adicional. |
| P04 | §8 no nombra al emisor de la transmisión. | ¿Quién transmite al comienzo y cuándo puede saberlo el jugador? | Pendiente. |
| P05 | §8: Devoto «puede enfrentarse» al jugador. | ¿El combate es opcional, inevitable al volver o depende de una elección? ¿Qué ocurre tras él? | Pendiente. |
| P06 | §8 fija el disparador pero no la persistencia. | ¿La tragedia ocurre al primer uso o al activar el teletransportador de Zona 2? ¿Qué estados y diálogos cambian al regresar? | Activación establecida; detalles operativos pendientes. |
| P07 | Solo Makong tiene parlamentos literales en la fuente. | ¿Hay diálogos aprobados adicionales de la transmisión, GamerBoy o refugiados que debamos incorporar sin cambios? | Pendiente de material. |
| P08 | Comparación con Unity aún no realizada. | ¿Qué escenas y archivos representan la versión actual para registrar diferencias? Ante cada diferencia, ¿cuál versión quieres conservar? | Requiere evidencia y decisión por diferencia. |
| P09 | §17 deja abierto el inicio. | ¿Quién pidió ayuda y qué causó que el grupo quedara atrapado? | Pendiente de canon. |
| P10 | §§4–5 y 17 dejan abierto el origen del conflicto. | ¿Cómo funciona y qué límites tiene la pérdida de memoria? ¿Omnia actúa conscientemente, se equivoca o está alterada? | Pendiente de canon. |
| P11 | §§6 y 17 no cierran los Guardianes. | ¿Cuáles son sus historias, ubicaciones, orden de encuentro y papel final? | Pendiente de canon. |
| P12 | §17 no cierra la resolución. | ¿Cómo se conectan las realidades, qué permite escapar y qué papel tienen Miyuki, Jixel y Nicole? ¿Habrá decisiones y varios finales? | Pendiente de canon. |

## Registro de diferencias con implementación

Sin entradas verificadas. No interpretar la ausencia de entradas como conformidad entre V1.0 y el juego.

Para cada hallazgo registrar: ID, fecha, documento y sección, ruta/escena de Unity, evidencia literal del comportamiento, diferencia, pregunta al usuario, decisión expresa y archivos autorizados. Mantener estado **PENDIENTE DE DECISIÓN** hasta recibir respuesta. No cambiar Unity ni resolver la diferencia automáticamente.

## Decisiones del usuario — 2026-10-15

- **Confirmado:** raíz del proyecto `E:\UNITY PRUEBA\DCG\dungeoncrawler`.
- **Canon / intención confirmada:** tras la tragedia, el combate con Devoto es inevitable al regresar si se quiere guardar. El regreso sigue siendo opcional dentro de la aventura. No se deduce un bloqueo del guardado en otras zonas ni un combate repetible después de derrotarlo.
- **Nombres:** explorar parodias de videojuegos; propuestas en `11_Propuestas_nombres_refugiados.md`, sin elección definitiva.
- El usuario indicó inicialmente que no tenía más diálogos, y después pidió revisar los existentes en el juego. Se encontraron textos adicionales y se documentan como implementación observada, sin declararlos aprobados automáticamente.

La fuente V1.0 se mantiene literal. Donde el extracto diga que Devoto «puede enfrentarse», esta decisión posterior precisa la intención para el regreso a guardar.

## Actualización de pendientes y revisión de diferencias

Esta tabla actualiza los estados iniciales de arriba; las entradas iniciales quedan como historial.

| ID | Estado actualizado / evidencia | Pregunta o siguiente decisión |
|---|---|---|
| P01 | Resuelto: ruta del proyecto proporcionada por el usuario. | Instalar esta carpeta en la ruta indicada. |
| P03 | La implementación revisada trata como víctimas a los cuatro NPC distintos de Devoto. | La identidad de las víctimas sigue pendiente de confirmación de canon por el usuario. |
| P05 | Resuelto parcialmente: combate inevitable al regresar si se quiere guardar. | Consecuencias narrativas tras derrotarlo aún pendientes de aprobación; el juego ya contiene una frase y recompensa. |
| P06 | El código `ActivateZoneTwoNode` dispara la tragedia al activar Zona 2. Hay variantes según si GamerBoy activó REFUGIO; si no lo hizo, Devoto lo activa fuera de pantalla. | ¿Estas dos variantes de activación se incorporan al canon? |
| P07 | Hay diálogos en el juego, inventariados en 10_Dialogos_en_implementacion.md. | ¿Se aprueban los textos encontrados por grupo, o cuáles se revisan primero? |
| P08 | Revisión estática de fuentes realizada; no se ejecutó Unity ni se resolvieron diferencias. | Revisar D01–D05 antes de modificar nada. |

| ID diferencia | Evidencia concreta | Interpretación / pregunta | Estado |
|---|---|---|---|
| D01 | El prólogo anterior celebraba una transferencia. | La reescritura aprobada elimina esa afirmación y mantiene la identidad del emisor desconocida. | Resuelto mediante el nuevo prólogo, 2026-10-15; no se inventa identidad ni explicación. |
| D02 | GamerBoy llamaba al lugar Internet. | El nuevo texto utiliza exclusivamente «la Red», según el encargo aprobado. | Resuelto e implementado, 2026-10-15. |
| D03 | El aviso del prólogo decía CONNECTION LOST. | Se solicitó explícitamente CONEXIÓN PERDIDA. SYSTEM BOOT y recovery.exe se conservan tal como fueron proporcionados. | Resuelto e implementado, 2026-10-15. |
| D04 | Makong tiene diálogo expandido y epílogo en asset/JSON; la Biblia conserva una selección de frases. `Docs/MakongNarrativaYFuente.md` afirma que el texto fue suministrado por el usuario. | Esa afirmación documental es evidencia histórica, no nueva aprobación de esta conversación. ¿Se adopta la versión completa del asset, manteniendo las frases aprobadas de V1.0? | Pendiente; no fusionado. |
| D05 | `Refugio.asset`: combate automático habilitado, distancia 2, paso fila 5. `CheckDevotoApproach` requiere posición y mirar al Devoto; `RefugeTerminalAvailable` comprueba proximidad y orientación de la terminal, sin exigir `devotoDefeated`. | Tu intención es combatir para guardar al regresar. La revisión estática no demuestra que sea imposible esquivarlo por la geometría. ¿Quieres verificar primero esa ruta en Unity y, si se puede evitar, bloquear la terminal hasta vencerlo? | Posible diferencia; sin cambio de Unity y sin prueba jugable. |

No se han aprobado nuevos parlamentos ni nombres por el hecho de encontrarlos en archivos. Los diálogos y archivos de implementación se conservan literalmente en `Fuentes/Implementacion_observada.md` para revisión.

## Nombres aprobados — 2026-10-15

El usuario aprobó las cinco propuestas mediante «me gustan todos»: **Solid Guarda** (Centinela), **Zeldoro** (Archivista), **Pacmiedo** (Cobarde), **Toadavía** (Optimista) y **Prayman** (Devoto).

Estos son los nombres propios confirmados. Los roles permanecen como descripciones; los extractos de V1.0 y de implementación retienen literalmente sus etiquetas anteriores. En las notas anteriores, «Devoto» corresponde a Prayman, incluido el combate al regresar para guardar. Véase [tabla de nombres aprobados](11_Propuestas_nombres_refugiados.md).

Los cinco nombres ya están implementados en Unity: Solid Guarda, Zeldoro, Pacmiedo, Toadavía y Prayman. Se actualizaron los nombres visibles y las etiquetas de hablante; los IDs internos y los parlamentos existentes permanecen intactos. Los demás pendientes siguen abiertos.


## Aplicación del tono — 2026-10-15

Confirmada la prioridad de voces distintas, tragedias variadas y descanso entre escenas oscuras. Pendientes de aprobación concreta: cinco muestras nuevas de voz para el refugio y alternativa cálida de The Last Life, en [revisión de tono](12_Voces_tragedia_y_descanso.md). Ninguno de esos textos se ha instalado ni sustituye material aprobado.


## Aprobación de diálogos y dirección — 2026-10-15

El usuario aprobó la revisión de voces, tragedia y descanso, las cinco nuevas líneas de los refugiados y la alternativa cálida de The Last Life. La línea de Pacmiedo se conserva exactamente como la indicó:

**PACMIEDO:** «Si vas a salir, ¿puedes cerrar despacio? No quiero saber quien oye esa puerta.»

La versión vigente de las cinco líneas y de la conversación completa está en [Voces, tragedia y descanso](12_Voces_tragedia_y_descanso.md). La aprobación editorial está resuelta. La incorporación en Unity sigue pendiente; en el refugio también queda pendiente la ubicación de las nuevas líneas. No se aprueban por extensión otros textos de implementación ni se reescriben la fuente V1.0 o sus diálogos originales. Las menciones anteriores a estas propuestas como pendientes quedan como historial de su revisión.

## Textos incorporados en Unity — 2026-10-15

Estado vigente: implementadas las cinco líneas aprobadas del refugio en `Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset`, antes de la última respuesta habitual de cada NPC, conservando las líneas anteriores y su orden relativo. Prayman pasa de tres a cuatro respuestas habituales; los demás pasan de dos a tres. La conversación especial inicial de Solid Guarda sobre los nodos se conserva.

La conversación de seis líneas de The Last Life está implementada en `Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset`. Conserva los servicios y la condición de conversación única.

Los cinco nombres nuevos ya estaban presentes al iniciar esta intervención; no se rehicieron sus cambios ni se modificaron IDs. No se editaron scripts, mecánicas, condiciones de combate, recompensas o partidas guardadas. Los textos de Makong, la tragedia y las fuentes históricas permanecen intactos.

Verificación: siete comprobaciones en el editor sobre contenido, orden y reproducción mediante WorldDialogue; ambos assets guardados y consola sin errores. No se realizó un recorrido visual en Play Mode, por lo que tiempos, música y composición en pantalla siguen pendientes de evaluación.

Partidas existentes: se conservan los contadores de conversaciones del refugio y la marca de conversación vista de The Last Life. No se fuerza a repetir contenido: una posada ya vista no reproducirá automáticamente el texto nuevo; un contador avanzado del refugio puede omitir una línea insertada. Las partidas nuevas recorren las nuevas secuencias desde el inicio. No se reinician estados para mostrar estos cambios.

Este estado sustituye las menciones históricas a «implementación pendiente» para estos seis cambios de conversación. Los demás pendientes narrativos y de mecánicas no se cierran por esta intervención.

## Revisión vigente del refugio — 2026-10-15

Por petición del usuario se revisaron las conversaciones habituales y las cuatro despedidas del refugio, manteniendo las cinco frases aprobadas. La versión completa implementada y sus verificaciones están en [Refugio: revisión vigente](13_Refugio_revision_vigente.md). Este documento sustituye las transcripciones anteriores de esos campos, que se conservan como historial.

Cerrojio es el guía independiente de las puertas; Solid Guarda sigue siendo el vigilante del refugio. Cerrojio no fue modificado. No se cambiaron mecánicas ni la confrontación de Prayman. Se completaron 20 comprobaciones de contenido y revelado en el editor; la evaluación visual en Play Mode sigue pendiente.
