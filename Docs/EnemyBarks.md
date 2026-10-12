# Frases contextuales de enemigos

Edita los TXT de Assets/Resources/Combat/Barks/GHOST, HONGO, SOLDADO y BOSS. Una frase por línea; no hace falta modificar código. Los archivos incluyen una selección temporal de las frases proporcionadas. Hit.txt conserva el banco adicional de golpes confirmados para una futura categoría independiente; todavía no se dispara en combate.

Categorías: CombatStart (inicio), Hurt (recibir daño), Attack (ataque normal), Special (habilidad especial), Defeat (derrota) y ExtremeCorruption (reservada para jefes futuros). BOSS no se activa actualmente.

Selecciona EnemyBarkSettings.asset en esa misma carpeta para configurar probabilidades (0–1), duración, intervalo entre frases normales, historial y posición del cuadro. Cada enemigo tiene su propio banco y probabilidades.

Se evitan las últimas cinco frases de cada enemigo y categoría durante la partida. Usa al menos seis frases únicas por categoría; si no quedan opciones válidas, el enemigo guarda silencio. Las líneas duplicadas se eliminan al cargar. Reinicia Play Mode tras editar los TXT para recargar los bancos.

Valores iniciales: inicio 100%, ataque 25%, recibir daño 18%, especial 90%, derrota 70%. El 100% del inicio presupone que existe una frase disponible.

Las frases caducan automáticamente, no capturan controles ni cambian los tiempos de combate. Derrota y habilidades especiales tienen prioridad frente a comentarios normales. Durante HACK se oculta el cuadro y sigue corriendo su duración; no se muestra encima del minijuego ni se encolan frases atrasadas.
