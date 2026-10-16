# Diálogos encontrados en el juego

**Estado: IMPLEMENTACIÓN OBSERVADA — pendiente de aprobación narrativa.** Revisión de archivos del 15 de octubre de 2026; no es una prueba en Play Mode. No sustituye el texto aprobado de V1.0. No se corrigieron ni tradujeron diálogos existentes.

[Fuentes literales con rutas y huellas SHA-256](Fuentes/Implementacion_observada.md).

## Inventario y lectura

| Grupo | Fuente principal | Alcance / condición |
|---|---|---|
| Prólogo | `Assets/Scripts/IntroSequence.cs` | 26 parlamentos aprobados, identificadores wake.dialogue.01–26; arranque y aviso de desconexión separados. Emisor «???» sin identidad revelada. |
| Cinco refugiados | `Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset` | Conversaciones, nombres actuales y últimas palabras; Centinela tiene una conversación inicial adicional sobre teletransportadores. |
| Salida del refugio | Mismo asset, `exitDialogue` | Diálogo de despedida y entrega de pociones. |
| Terminal y nodos | Mismo asset y `DungeonPrototype.RefugeNarrative.cs` | Primera terminal, usos posteriores, activar/rechazar nodo, variantes de Zona 2 según quién activó REFUGIO. |
| Regreso y Devoto | Mismo asset; `DungeonPrototype.Refuge.cs` | Llegada, primer cuerpo y dos variantes de confrontación (12 y 13 líneas); última frase tras derrota en script. El array antiguo `devotoDialogue` también existe, pero el flujo revisado utiliza `devotoPlayerDialogue` / `devotoDevoteeDialogue`. |
| Letrero de preservación | `PreservationCorridorSettings` y `DungeonPrototype.Preservation.cs` | Texto del letrero y comentarios de GamerBoy; respuestas solo en primera lectura según el flujo revisado. |
| Makong | `Assets/Resources/Combat/MakongSpecialSettings.asset` | Asset cargado por el juego; texto y metadatos de presentación. |
| Fuente de instalación de Makong | `Tools/MakongNarrative.json` | 51 líneas de introducción, 21 de derrota y 3 de epílogo. El instalador es explícito; el JSON no garantiza por sí solo igualdad con el asset. |
| Digital Kingdom | `Assets/Resources/DigitalKingdom/Locations/*.asset`, `City.asset` | Conversaciones de establecimientos, descripciones y mensajes de servicios. |
| Frases de combate | `Assets/Resources/Combat/Barks/**/*.txt` | Devoto, BOSS, GHOST, HONGO y SOLDADO; conservar separadas de escenas y de diálogos aprobados. |

## Prólogo — Despertar en la Red (aprobado e implementado, 2026-10-15)

Sustituye exclusivamente el prólogo anterior por encargo expreso del usuario. Conserva literalmente sus 26 parlamentos. La copia histórica de `Fuentes/Implementacion_observada.md` y la Biblia V1.0 no se modifican.

Arranque: silencio y negro; **SYSTEM BOOT**, **Ejecutando recovery.exe...**, **Restaurando sesión...** (la última línea se corta visualmente antes de completarse).

- **wake.dialogue.01 — ???:** ¿Me escuchas?
- **wake.dialogue.02 — ???:** Despierta.

Vistazo con glitch y fundido de 0,34 s al refugio desde la cámara real; regreso a negro.

- **wake.dialogue.03 — GAMERBOY:** Ah...
- **wake.dialogue.04 — ???:** Eso es. ¿Puedes oírme?
- **wake.dialogue.05 — GAMERBOY:** Hay alguien...

Interferencia breve y fundido de 2 s para revelar el refugio.

- **wake.dialogue.06 — GAMERBOY:** ¿Qué...?
- **wake.dialogue.07 — GAMERBOY:** No conozco este lugar.

Pausa de contemplación de 1,1 s.

- **wake.dialogue.08 — GAMERBOY:** Yo estaba...
- **wake.dialogue.09 — GAMERBOY:** Estaba con alguien.
- **wake.dialogue.10 — GAMERBOY:** ¿Por qué no puedo...?
- **wake.dialogue.11 — ???:** ¿Sigues ahí?
- **wake.dialogue.12 — GAMERBOY:** Sí. Estoy aquí.
- **wake.dialogue.13 — GAMERBOY:** No sé dónde es aquí.
- **wake.dialogue.14 — ???:** Estás dentro de la Red.
- **wake.dialogue.15 — GAMERBOY:** ¿La Red?
- **wake.dialogue.16 — GAMERBOY:** Espera. ¿Tú sabes cómo llegué?

Interferencia breve.

- **wake.dialogue.17 — ???:** La conexión no va a durar.
- **wake.dialogue.18 — GAMERBOY:** No, espera. Había alguien conmigo. No consigo recordar quién.
- **wake.dialogue.19 — ???:** Escúchame...
- **wake.dialogue.20 — ???:** No sé cuánto tiempo podré mantener esta conexión.
- **wake.dialogue.21 — GAMERBOY:** Entonces dime cómo salir de aquí.
- **wake.dialogue.22 — ???:** Hay algo que debes saber...
- **wake.dialogue.23 — ???:** Y, sobre todo... no confíes en—

Aviso automático **CONEXIÓN PERDIDA** y sonido digital de desconexión.

- **wake.dialogue.24 — GAMERBOY:** ¿Hola?
- **wake.dialogue.25 — GAMERBOY:** No. Espera.
- **wake.dialogue.26 — GAMERBOY:** Tiene que haber alguien aquí.

Al finalizar se libera el control y vuelve gradualmente la música de exploración. Las partidas cargadas no repiten esta secuencia.

## Makong — texto de la fuente de instalación

Esta transcripción procede del JSON de instalación, no sustituye al asset actual ni a V1.0. Ver la copia literal del asset en las fuentes para revisar diferencias.

### introduction

**MAKONG:**

```text
Je, je, je...
```

**MAKONG:**

```text
Otro pequeño héroe ha venido a desafiarme.
```

**MAKONG:**

```text
Acércate. Arrodíllate ante el gran Makong.
```

**GAMERBOY:**

```text
¿En serio? ¿Una rana gigante con corona?
```

**MAKONG:**

```text
¡SILENCIO, INSECTO!
```

**MAKONG:**

```text
¡Estás ante el soberano de este...!
```

**MAKONG:**

```text
...
```

**MAKONG:**

```text
¿De este...?
```

**MAKONG:**

```text
Qué extraño...
```

**MAKONG:**

```text
Este no es mi castillo.
```

**MAKONG:**

```text
¿Dónde están mis súbditos?
```

**GAMERBOY:**

```text
¿Me lo preguntas a mí? Acabo de llegar.
```

**MAKONG:**

```text
Tú...
```

**MAKONG:**

```text
Tú no perteneces aquí.
```

**MAKONG:**

```text
No eres uno de mis enemigos.
```

**MAKONG:**

```text
Ni uno de mis héroes.
```

**GAMERBOY:**

```text
Bueno, gracias por darte cuenta.
```

**MAKONG:**

```text
N0...
```

**MAKONG:**

```text
No deberías estar aquí.
```

**MAKONG:**

```text
Ella puede verte.
```

**GAMERBOY:**

```text
¿Ella? ¿De quién hablas?
```

**MAKONG:**

```text
No lo sé.
```

**MAKONG:**

```text
No recuerdo haberla conocido.
```

**MAKONG:**

```text
Pero escucho su voz...
```

**MAKONG:**

```text
Incluso cuando estoy solo.
```

**GAMERBOY:**

```text
¿Y qué te dice?
```

**MAKONG:**

```text
Que todo estará bien.
```

**MAKONG:**

```text
Que pronto dejaré de sentirme confundido.
```

**MAKONG:**

```text
Que ya no tendré que recordar...
```

**MAKONG:**

```text
...
```

**MAKONG:**

```text
¿Por qué estoy aquí?
```

**MAKONG:**

```text
¿Cuánto tiempo llevo sentado en este trono?
```

**GAMERBOY:**

```text
Oye, ¿estás bien?
```

**MAKONG:**

```text
¡NO ME COMPADEZCAS!
```

**MAKONG:**

```text
¡SOY MAKONG! ¡EL REY DE...!
```

**MAKONG:**

```text
...
```

**MAKONG:**

```text
No... otra vez no...
```

**MAKONG:**

```text
N0 D3BES... PASAR...
```

**MAKONG:**

```text
LA SALIDA... DEBE...
```

**MAKONG:**

```text
D3BE PERMANECER...
```

**GAMERBOY:**

```text
¡Oye! ¡Tranquilo!
```

**MAKONG:**

```text
Mis manos...
```

**MAKONG:**

```text
Se están moviendo solas...
```

**MAKONG:**

```text
Pequeño extraño...
```

**MAKONG:**

```text
Aléjate de mí.
```

**GAMERBOY:**

```text
¿Qué?
```

**MAKONG:**

```text
¡HE DICHO QUE TE ALEJES!
```

**MAKONG:**

```text
Antes de que...
```

**MAKONG:**

```text
Je... je... je...
```

**MAKONG:**

```text
Parece que...
```

**MAKONG:**

```text
Parece que debo matarte.
```

### defeat

**MAKONG:**

```text
Imposible...
```

**MAKONG:**

```text
El gran Makong... derrotado...
```

**MAKONG:**

```text
Je, je...
```

**MAKONG:**

```text
Así es como debía terminar, ¿verdad?
```

**GAMERBOY:**

```text
¿De qué estás hablando?
```

**MAKONG:**

```text
Un héroe llega...
```

**MAKONG:**

```text
Derrota al rey malvado...
```

**MAKONG:**

```text
Y todos celebran.
```

**MAKONG:**

```text
Lo he vivido tantas veces...
```

**MAKONG:**

```text
Pero esta vez...
```

**MAKONG:**

```text
Esta vez no recuerdo cómo empezó.
```

**MAKONG:**

```text
Ella sigue mirando...
```

**MAKONG:**

```text
Incluso ahora.
```

**GAMERBOY:**

```text
¡Espera! ¿Quién es ella?
```

**MAKONG:**

```text
No sé...
```

**MAKONG:**

```text
Pero creo...
```

**MAKONG:**

```text
Que también está perdida.
```

**MAKONG:**

```text
Ah...
```

**MAKONG:**

```text
Por fin...
```

**MAKONG:**

```text
Ya no tengo que...
```

**MAKONG:**

```text
...
```

### epilogue

**GAMERBOY:**

```text
...
```

**GAMERBOY:**

```text
Un jefe que no recuerda su propio juego.
```

**GAMERBOY:**

```text
Definitivamente hay algo muy mal aquí.
```


## Actualización de implementación - 2026-10-15

Los cinco nombres ya están implementados en Unity: Solid Guarda, Zeldoro, Pacmiedo, Toadavía y Prayman. Se actualizaron los nombres visibles y las etiquetas de hablante; los IDs internos y los parlamentos existentes permanecen intactos. Los demás pendientes siguen abiertos. Las fuentes observadas conservan sus etiquetas históricas. La equivalencia vigente está en 11_Propuestas_nombres_refugiados.md.
