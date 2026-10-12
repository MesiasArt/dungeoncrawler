# Makubas, Mimic y tamaños de JUMP

Se integraron los PSD sin modificar los originales. Cada enemigo conserva tres idles, ataque, reacción al daño y muerte. Makuba Arco y Gran Makuba reproducen los dos frames de ataque. Lanza tiene tres frames especiales; Gran Makuba tiene cinco y un especial provisional de tres golpes cada tercer turno. Los frames comparten un lienzo con ancla de pies para conservar la alineación del PSD; se incluyen píxeles que sobresalen del lienzo original.

## Estadísticas provisionales editables

| Enemigo | HP | Fuerza | Defensa | Suerte | Categoría | Aparición aleatoria |
|---|---:|---:|---:|---:|---|---|
| Makuba Lanza | 18 | 4 | 4 | 3 | Enemigo | Sí |
| Makuba Arco | 14 | 3 | 2 | 6 | Enemigo | Sí |
| Gran Makuba | 28 | 6 | 7 | 4 | Fuerte | Sí |
| Mimic | 20 | 4 | 6 | 2 | Enemigo de cofre | No |

Cambiar en **Editor → Personajes - estadísticas y equipo**. Las cuatro fichas anteriores conservan sus valores. Los nuevos usan provisionalmente los bancos de frases de Tapaku, con reglas independientes editables. No se crearon cofres ni eventos de cofres; Mimic queda preparado visualmente y en el catálogo/galería para esa integración futura.

**Editor → Galería de enemigos y guía en pies** muestra ahora los ocho enemigos. Los controles de tamaño se guardan en EnemySizeSettings; se conservaron Tapaku 0.8, Hongo 0.7, Soldado 1.5 y Big Tapaku 1.5. Defaults nuevos: Lanza 1.1, Arco 1.1, Gran Makuba 1.8, Mimic 1. Las líneas en pies son una referencia visual.

## JUMP

Las caminatas vienen de las capas nuevas de `enemigos mini jump.psd`. Cada sesión utiliza únicamente los dos frames de la especie seleccionada, incluyendo Big Tapaku; ya no toma los idles de combate de Big Tapaku como caminata.

En **Editor → Panel de Virtual Dungeon**, sección JUMP (o el Inspector de EnemyJumpSettings), `matchEnemyReferenceSize` vincula el tamaño a los controles de la galería. Regla: `referenceEnemyPixels × escala del enemigo / escala de Tapaku`, limitada por `minimumEnemyPixels` y `maximumEnemyPixels`. Defaults: referencia 80, mínimo 48, máximo 180. El límite superior evita alturas que vuelvan imposible el salto dentro de la pantalla del brazo. Si crecen enemigos gigantes, este límite es ajustable hasta 180 en el prototipo actual.

Con los valores actuales: Tapaku 80, Hongo 70, Soldado 150, Big Tapaku 150, Lanza 110, Arco 110 y Gran Makuba 180 píxeles de alto. La proporción horizontal conserva el arte. La altura de colisión, el aterrizaje y el rebote usan la misma altura de la sesión; la trayectoria conserva los beats, el salto manual y las ventanas de PERFECT/GOOD/MISS. Mimic no tiene caminata dedicada en este PSD; su visual de JUMP usa idles como respaldo si en el futuro se habilita desde cofres.

En apariciones hay dos controles nuevos: `spawnMakubas` y `spawnGranMakuba`, además del existente `spawnBigTapaku`. Mimic está excluido explícitamente del sorteo y de las apariciones iniciales.

## Validación

- Unity MCP: ocho fichas con tres idles y ataque/daño/muerte; siete especies con dos frames de caminata; especiales Lanza/Gran completos.
- 21 cadenas completas del modelo rítmico: siete tamaños × 30/60/240 FPS.
- Play Mode: los tres Makubas se crean en celdas transitables con estadísticas y visuales reales. 3.000 selecciones aleatorias no eligieron Mimic.
- Play Mode: Gran Makuba realiza su especial después de dos turnos normales; la rutina aplica tres impactos (3 HP totales contra DEF 7 en la prueba sin azar), vuelve a idle y JUMP carga sus frames correctos, colisión de 180 píxeles y audio programado.
- Galería revisada visualmente mediante captura MCP. Consola sin errores al finalizar.

Pruebas: `Tests/MakubasMcpTests.cs`, `Tests/MakubasPlayModeMcpTests.cs`. Las estadísticas son provisionales; no representan un balance definitivo de grupos.

## Corrección de apoyo y espacio (11/10/2026)

Se recortaron los márgenes transparentes/casi invisibles de las caminatas de Gran Makuba, Lanza, Arco y Big Tapaku. El recorte usa alfa >= 8/255 para medir contenido visible, conserva los píxeles interiores y contornos negros, y alinea ambos frames por el borde inferior. Los PSD originales no se modificaron. Gran Makuba tenía un margen inferior de unos 164 píxeles casi invisibles que causaba el flotado incluso después de recortar alfa cero.

Se corrigió el signo del ancla horizontal de los sprites de combate/exploración para compensar la rotación de 180 grados del quad. La galería conserva su orientación y ancla propias.

**Editor → Escenario - espacio y techo** abre los controles persistentes: expansión automática, ancho/alto mínimo del pasillo, techo mínimo del combate y margen alrededor de enemigos. Se aplican al iniciar una partida. Con los tamaños actuales, los pasillos pasan de 3 a aproximadamente 5.18 metros para contener el lienzo completo de Gran Makuba sin reducir su escala 1.8. Puerta, marcos, interacción, antorchas y sombras de salida se colocan según ese ancho; las antorchas conservan su tamaño.

El combate tiene un techo mínimo de 4.5 m y puede elevarse más para futuros enemigos grandes. El ancho y la separación de la formación dependen de los lienzos de los participantes; la cámara se aleja cuando hace falta para encuadrarlos. El plano del reflejo del techo cambia al entrar en combate y se restaura al volver al dungeon.

Verificado con Unity MCP en Play Mode: lienzo completo de Gran Makuba dentro de paredes; error horizontal del centro visible de unos 0.007 m; extremo superior del lienzo a 3.449 m y techo a 4.5 m; puerta ampliada accesible y bloqueo de salida cerrado correcto. Captura visual confirma ambas caminatas apoyadas en el piso de JUMP. `Tests/EnemySpaceMcpTests.cs` comprueba también la expansión para un tamaño gigante futuro.

Se corrigió además la recreación innecesaria del singleton de audio al cerrar una escena con JUMP activo.
