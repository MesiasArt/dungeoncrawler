# Diálogos aprobados — conservación literal

Estado: texto aprobado de referencia en V1.0, §9. Se conserva íntegra la sección para mantener orden, hablante, acotaciones y contexto. No normalizar los glitches ni reescribir frases. La fuente no proporciona diálogos literales de GamerBoy, los refugiados, la transmisión o los Guardianes; quedan pendientes y no se inventan aquí.

# 9. Makong — Primer jefe del dungeon

Makong es el primer jefe principal, situado antes de la salida del dungeon de demostración.

Es una parodia de los reyes y villanos clásicos de videojuegos.

### Personalidad original

Makong ya era malvado en su propio juego.

Es arrogante, orgulloso, autoritario y disfruta interpretando su papel de gran villano.

No necesita convertirse en una buena persona ni arrepentirse de su pasado.

### Su situación actual

Makong está confundido.

No reconoce el lugar donde se encuentra.

No recuerda completamente su reino.

Siente que está desempeñando una historia que conoce, pero ha olvidado cómo empezó.

Escucha una voz femenina que le promete que pronto dejará de sentirse confundido.

No sabe quién es esa voz.

No conoce necesariamente a Omnia ni comprende el Protocolo Zero.

En ciertos momentos pierde el control de sus palabras y de su cuerpo.

### Frases fundamentales

**MAKONG:**

"Este no es mi castillo."

"¿Dónde están mis súbditos?"

"Tú no perteneces aquí."

"Ella puede verte."

"No recuerdo haberla conocido."

"Pero escucho su voz..."

"Incluso cuando estoy solo."

"Que todo estará bien."

"Que pronto dejaré de sentirme confundido."

"Que ya no tendré que recordar..."

Cuando comienza a perder el control:

**MAKONG:**

"N0 D3BES... PASAR..."

"LA SALIDA... DEBE..."

"D3BE PERMANECER..."

Después recupera momentáneamente la lucidez:

**MAKONG:**

"Mis manos..."

"Se están moviendo solas..."

"Pequeño extraño..."

"Aléjate de mí."

Finalmente:

**MAKONG:**

"Parece que..."

"Parece que debo matarte."

### Después de la derrota

Makong recuerda que, como villano de su videojuego, debería ser derrotado por un héroe.

Pero no comprende por qué esta vez su historia es diferente.

**MAKONG:**

"Un héroe llega..."

"Derrota al rey malvado..."

"Y todos celebran."

"Lo he vivido tantas veces..."

"Pero esta vez..."

"Esta vez no recuerdo cómo empezó."

Su última revelación:

**MAKONG:**

"Ella sigue mirando..."

"Incluso ahora."

"No sé..."

"Pero creo..."

"Que también está perdida."

Esta última frase introduce una posibilidad inquietante: la entidad responsable de la corrupción quizá tampoco comprende completamente lo que está haciendo.

No constituye una explicación definitiva sobre Omnia.

### Presentación audiovisual

El encuentro utiliza únicamente texto, sin voces.

Los errores aparecen progresivamente en las palabras de Makong.

La música de exploración se atenúa y entra una ambientación oscura.

Los momentos de corrupción activan sonidos digitales y breves interferencias visuales.

Cuando Makong recupera lucidez, la presentación vuelve a estabilizarse.

**La corrupción debe parecer una voluntad ajena imponiendo palabras sobre los pensamientos de Makong.**

---



## Decisiones del usuario — 2026-10-15

- **Confirmado:** raíz del proyecto `E:\UNITY PRUEBA\DCG\dungeoncrawler`.
- **Canon / intención confirmada:** tras la tragedia, el combate con Devoto es inevitable al regresar si se quiere guardar. El regreso sigue siendo opcional dentro de la aventura. No se deduce un bloqueo del guardado en otras zonas ni un combate repetible después de derrotarlo.
- **Nombres:** explorar parodias de videojuegos; propuestas en `11_Propuestas_nombres_refugiados.md`, sin elección definitiva.
- El usuario indicó inicialmente que no tenía más diálogos, y después pidió revisar los existentes en el juego. Se encontraron textos adicionales y se documentan como implementación observada, sin declararlos aprobados automáticamente.

La fuente V1.0 se mantiene literal. Donde el extracto diga que Devoto «puede enfrentarse», esta decisión posterior precisa la intención para el regreso a guardar.

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