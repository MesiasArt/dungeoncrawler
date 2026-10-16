# Codebound — Archivo narrativo

Base documental: **Biblia narrativa y dirección artística V1.0**, actualizada el **15 de octubre de 2026**. Universo: **Freakier Than Normal**.

## Índice

- [Reglas de continuidad, estilo y dirección artística](01_Reglas.md)
- [Biblia del mundo](02_Biblia_del_mundo.md)
- [Personajes](03_Personajes.md)
- [Cronología](04_Cronologia.md)
- [Diálogos aprobados](05_Dialogos.md)
- [Eventos](06_Eventos.md)
- [Misterios](07_Misterios.md)
- [Pendientes y contradicciones](08_Pendientes_y_contradicciones.md)
- [Registro de cambios](09_Registro_de_cambios.md)
- [Fuente íntegra sin modificaciones](Fuentes/Texto%20pegado(6).txt)

## Estados editoriales

**CANON CONFIRMADO EN V1.0:** afirmación establecida por la fuente. No certifica que esté implementada en Unity ni que cada personaje conozca esa verdad.

**DIRECTRIZ CONFIRMADA:** instrucción de tono, escritura, presentación o continuidad; no es un acontecimiento del mundo.

**PROPUESTA / POSIBILIDAD:** formulación opcional, ejemplo o función potencial de la fuente. No convertirla en hecho cerrado.

**PENDIENTE:** información que la fuente deja sin definir o que exige confirmación del usuario.

**DIFERENCIA CON IMPLEMENTACIÓN:** registrar evidencia y preguntar antes de cambiar canon, documentación de intención o archivos del juego. No se ha auditado Unity en esta entrega.

Las síntesis editoriales están identificadas. Los extractos literales y la fuente íntegra prevalecen sobre sus resúmenes. Las incertidumbres de un personaje no equivalen a revelaciones objetivas.

## Procedimiento de actualización

1. Leer reglas, fuente y pendientes antes de escribir nuevas escenas.
2. Identificar qué es canon y qué se propone; no rellenar vacíos por inferencia.
3. Preservar literalmente los diálogos aprobados, incluidos signos, mayúsculas, puntos suspensivos y glitches. Solicitar aprobación para cambiarlos.
4. Registrar diferencias con la implementación con ruta, escena y evidencia cuando estén disponibles. Preguntar qué versión conservar antes de resolverlas.
5. Incorporar decisiones expresas del usuario con fecha y referencia en el registro de cambios; actualizar documentos afectados de forma coherente.

Carpeta instalada en `E:\UNITY PRUEBA\DCG\dungeoncrawler\Docs\Narrativa`, por indicación del usuario. La fuente V1.0 sigue íntegra; las decisiones posteriores están registradas y prevalecen como precisiones sin reescribirla.

## Revisión del proyecto y propuestas

- [Diálogos encontrados en la implementación](10_Dialogos_en_implementacion.md)
- [Nombres aprobados de los refugiados](11_Propuestas_nombres_refugiados.md)
- [Copias literales de las fuentes revisadas](Fuentes/Implementacion_observada.md)

**Precisión confirmada del usuario:** después de la tragedia, combatir con Devoto es inevitable al regresar si se quiere guardar. La correspondencia exacta con la ruta jugable requiere comprobación. Ver D05 en pendientes antes de cambiar Unity.

Los cinco nombres de los refugiados están aprobados por el usuario; los diálogos encontrados en el proyecto siguen pendientes de aprobación narrativa. Las preguntas originales se conservan como historial; sus estados actualizados aparecen al final de pendientes.

## Nombres aprobados — 2026-10-15

El usuario aprobó las cinco propuestas mediante «me gustan todos»: **Solid Guarda** (Centinela), **Zeldoro** (Archivista), **Pacmiedo** (Cobarde), **Toadavía** (Optimista) y **Prayman** (Devoto).

Estos son los nombres propios confirmados. Los roles permanecen como descripciones; los extractos de V1.0 y de implementación retienen literalmente sus etiquetas anteriores. En las notas anteriores, «Devoto» corresponde a Prayman, incluido el combate al regresar para guardar. Véase [tabla de nombres aprobados](11_Propuestas_nombres_refugiados.md).

Los cinco nombres ya están implementados en Unity: Solid Guarda, Zeldoro, Pacmiedo, Toadavía y Prayman. Se actualizaron los nombres visibles y las etiquetas de hablante; los IDs internos y los parlamentos existentes permanecen intactos. Los demás pendientes siguen abiertos.


- [Voces, tragedia y descanso: criterio aprobado y propuestas por escena](12_Voces_tragedia_y_descanso.md)


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

## Digital Kingdom — 2026-10-16

Implementados los seis nombres y personalidades aprobados, con regalos únicos de LEVEL UP CHEAT en The Last Life, Pay 2 Win y Temple of Continue. Véase [NPC de Digital Kingdom](14_DigitalKingdom_NPC.md). Los establecimientos mantienen sus nombres propios.
