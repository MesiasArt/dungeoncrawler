# Registro de cambios

## 2026-10-15 — Organización inicial desde V1.0

- Fuente: `Texto pegado(6).txt`, Biblia narrativa y dirección artística V1.0, fechada 15 de octubre de 2026.
- Conservada una copia íntegra del adjunto en `Fuentes/`, sin modificaciones.
- Creados índice, reglas, biblia del mundo, personajes, cronología, diálogos, eventos, misterios, pendientes/contradicciones y este registro.
- Extractos originales conservados literalmente; cronología, tablas y estados son organización editorial identificada, sin introducir respuestas de canon.
- Diálogos de Makong conservados literalmente y con contexto en `05_Dialogos.md`; las copias en personajes/eventos proceden de la misma sección original.
- Posibilidades y cuestiones abiertas separadas del canon establecido. No se inventaron diálogos de otros personajes.
- Ningún archivo existente de Unity fue editado. No se comparó la implementación ni se resolvieron diferencias.
- Carpeta entregada aparte, pendiente de confirmar la ruta del proyecto Codebound. Esta organización no constituye una nueva versión de canon.

## Decisiones posteriores

Sin decisiones nuevas del usuario. Registrar aquí fecha, pregunta/ID, respuesta expresa, documentos afectados y si se autorizó alguna modificación de implementación.

## 2026-10-15 — Ruta confirmada y revisión de textos existentes

- Usuario confirmó la raíz de Codebound y precisó que el combate con Devoto es inevitable al regresar si se quiere guardar.
- Se registró esa decisión posterior sin modificar los extractos de V1.0 ni los diálogos aprobados.
- Se revisaron fuentes del prólogo, refugio, terminal/nodos, letrero, Makong, ciudad y frases de combate. Copias literales con SHA-256 en `Fuentes/Implementacion_observada.md`.
- Añadidos inventario de diálogos de implementación y propuestas de nombres paródicos, ambos con estados explícitos.
- Registradas preguntas D01–D05; no se resolvieron divergencias por inferencia.
- Se instaló únicamente `Docs/Narrativa`. No se editaron archivos de Unity ni otros documentos existentes. Revisión estática; sin pruebas en Play Mode.

## 2026-10-15 — Aprobación de los nombres de los refugiados

El usuario aprobó las cinco propuestas: Solid Guarda, Zeldoro, Pacmiedo, Toadavía y Prayman. Actualizados nombres aprobados, personajes, eventos, pendientes e índice. Se conserva el texto literal de V1.0 y de las fuentes observadas. No se cambiaron archivos Unity, etiquetas de hablante ni IDs. Implementación de nombres pendiente.


## 2026-10-15 — Aplicación de la prioridad de tono

El usuario aceptó variar voces, diferenciar tragedias y permitir descanso entre escenas oscuras. Añadidas reglas editoriales y revisión por escenas con fichas de voz, cinco muestras de diálogo y una alternativa para The Last Life. Esas líneas son propuestas pendientes; no se consideran aprobadas por aceptar la prioridad general. Se preservaron V1.0, sus parlamentos y fuentes históricas. Solo se editaron documentos narrativos; Unity permanece sin cambios.


## 2026-10-15 — Revisión de tono aprobada y The Last Life adoptada

El usuario aprobó las propuestas del documento 12 y pidió conservar su corrección exacta de Pacmiedo. Se actualizó esa frase y se adoptó la conversación cálida de The Last Life como versión narrativa vigente. Actualizados estados en reglas, diálogos, eventos, pendientes e índice. Las aprobaciones quedan resueltas; implementación en Unity y ubicación de las líneas del refugio pendientes. No se modificaron archivos de Unity, fuentes históricas ni parlamentos originales de V1.0.


## 2026-10-15 - Nombres de los cinco refugiados implementados en Unity

Los cinco nombres ya están implementados en Unity: Solid Guarda, Zeldoro, Pacmiedo, Toadavía y Prayman. Se actualizaron los nombres visibles y las etiquetas de hablante; los IDs internos y los parlamentos existentes permanecen intactos. Los demás pendientes siguen abiertos.

Refugio.asset y sus etiquetas de hablante reflejan la equivalencia aprobada; el encuentro mantiene el ID DevotoCorrompido y muestra Prayman corrompido. La línea final usa Prayman. Se conservan las claves de NPC (centinela, archivista, cobarde, optimista, devoto), eventos, inventario, progreso y compatibilidad de guardado. El NPC ya colocado en X17/Y34 usa Solid Guarda como hablante y explica los avisos verdes y rojos de las puertas. Esta es una nueva interacción solicitada, sin sustituir parlamentos previos.

La Biblia V1.0 y todos los archivos de Docs/Narrativa/Fuentes permanecen byte por byte intactos; se registraron hashes SHA-256 antes de editar. No se aprobaron ni instalaron las muestras narrativas pendientes.


## Textos incorporados en Unity — 2026-10-15

Estado vigente: implementadas las cinco líneas aprobadas del refugio en `Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset`, antes de la última respuesta habitual de cada NPC, conservando las líneas anteriores y su orden relativo. Prayman pasa de tres a cuatro respuestas habituales; los demás pasan de dos a tres. La conversación especial inicial de Solid Guarda sobre los nodos se conserva.

La conversación de seis líneas de The Last Life está implementada en `Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset`. Conserva los servicios y la condición de conversación única.

Los cinco nombres nuevos ya estaban presentes al iniciar esta intervención; no se rehicieron sus cambios ni se modificaron IDs. No se editaron scripts, mecánicas, condiciones de combate, recompensas o partidas guardadas. Los textos de Makong, la tragedia y las fuentes históricas permanecen intactos.

Verificación: siete comprobaciones en el editor sobre contenido, orden y reproducción mediante WorldDialogue; ambos assets guardados y consola sin errores. No se realizó un recorrido visual en Play Mode, por lo que tiempos, música y composición en pantalla siguen pendientes de evaluación.

Partidas existentes: se conservan los contadores de conversaciones del refugio y la marca de conversación vista de The Last Life. No se fuerza a repetir contenido: una posada ya vista no reproducirá automáticamente el texto nuevo; un contador avanzado del refugio puede omitir una línea insertada. Las partidas nuevas recorren las nuevas secuencias desde el inicio. No se reinician estados para mostrar estos cambios.

Este estado sustituye las menciones históricas a «implementación pendiente» para estos seis cambios de conversación. Los demás pendientes narrativos y de mecánicas no se cierran por esta intervención.

## 2026-10-15 - Cerrojio y reglas de puertas

El NPC independiente ya colocado en X17/Y34 se llama Cerrojio y conserva su dialogo sobre senales de seguridad. Solid Guarda sigue siendo el Centinela del refugio. Se retira el fondo negro del aviso de puertas; las puertas normales se cierran al cruzar, mientras que las abiertas mediante llave consumen una unidad y conservan la apertura en sus estados persistentes. No se modifican V1.0 ni las fuentes historicas.


## Revisión vigente del refugio — 2026-10-15

Por petición del usuario se revisaron las conversaciones habituales y las cuatro despedidas del refugio, manteniendo las cinco frases aprobadas. La versión completa implementada y sus verificaciones están en [Refugio: revisión vigente](13_Refugio_revision_vigente.md). Este documento sustituye las transcripciones anteriores de esos campos, que se conservan como historial.

Cerrojio es el guía independiente de las puertas; Solid Guarda sigue siendo el vigilante del refugio. Cerrojio no fue modificado. No se cambiaron mecánicas ni la confrontación de Prayman. Se completaron 20 comprobaciones de contenido y revelado en el editor; la evaluación visual en Play Mode sigue pendiente.

## 2026-10-15 — Prólogo aprobado: Despertar en la Red

- Sustituida la introducción inicial por los 26 parlamentos exactos suministrados por el usuario. Sin revelaciones de Guardianes, Omnia, Protocolo Zero ni identidad del emisor.
- Arranque negro en silencio, terminal con cursor y sesión interrumpida, vistazo de la cámara real, fundido al refugio, transmisión suave y CONEXIÓN PERDIDA.
- No se muestran manos ni cuerpo; no se cambia la posición o rotación inicial. Los desplazamientos permanecen bloqueados hasta la última línea.
- Cargar una partida omite el prólogo incluso con la marca antigua de introducción ausente o falsa. No se reescribieron archivos de partidas personales.
- Actualizada la transcripción vigente y resueltos D01–D03 por sustitución aprobada. Biblia V1.0, `Fuentes` y diálogos de los cinco NPC intactos.
- Informe técnico y pruebas: `Docs/PrologoDespertar.md`.


## 2026-10-15 - Primera bienvenida en el refugio

Implementado evento único tras el prólogo en X17/Y6 con la revisión de 33 parlamentos. Incluye mirada atrás, silencio y voces de los cinco refugiados; las conversaciones individuales y fuentes históricas se mantienen intactas. Guion vigente y pruebas: Docs/BienvenidaYCierrePrototipo.md.


## 2026-10-16 — NPC de Digital Kingdom y regalos

Implementados Tifanía, Buho-Nero, San Solair, Dr. Billy Byte, Capitán Turbo y Don Peligro, con sus personalidades aprobadas y presentaciones breves. Actualizados hablantes de servicios, avisos y guía del Reino del Píxel. Los tres primeros entregan un LEVEL UP CHEAT cada uno con recibos persistentes; se conserva el diálogo anterior y la compatibilidad con conversaciones ya vistas. Se añade mochila para inventario y personaje. Biblia V1.0 y Fuentes permanecen intactas. [Versión vigente](14_DigitalKingdom_NPC.md) e [informe técnico](../SpeedrunCheatDigitalKingdom.md).
