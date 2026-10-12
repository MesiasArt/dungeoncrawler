# Cómo acomodar la interfaz

Selecciona Assets/Resources/UI/DungeonUILayout.asset en Project. En el Inspector, cada Rect tiene X, Y, Width (ancho) y Height (alto). Medidas sobre un lienzo de referencia de 1280 × 720: el juego lo escala según la ventana.

- Player Health: cuadro de vida, desde arriba a la izquierda.
- Player Avatar: retrato del HUD, desde arriba a la izquierda. Sus X/Y y Width/Height son independientes del cuadro de vida. El retrato conserva sus proporciones dentro de ese espacio.
- Map: desde arriba a la derecha. X negativo lo mueve hacia dentro.
- Message: desde abajo a la derecha. X/Y negativos; muestra solo la acción más reciente.
- Dialogue: desde abajo a la izquierda. X positivo, Y negativo.
- Dialogue Avatar: avatar de GamerBoy, desde abajo a la izquierda. Ajusta su posición junto a Dialogue para que la punta toque su retrato.
- Enemy Health: desde arriba al centro. X negativo lo desplaza hacia la izquierda.
- Combat Offset: posición del final de la lista desde abajo a la izquierda.
- Combat Button Size y Gap: tamaño y separación de botones.
- Preserve Panel Aspect: conserva proporciones de cuadros; el arte se ajusta dentro de tu rectángulo, alineado abajo a la derecha.
- Discovered Floor / Wall: colores del mapa.
- Letters Per Second: velocidad del texto; 0 lo muestra inmediatamente.
- Text Font Size: tamaño de fuente de la interfaz.

Puedes ajustar estos assets durante Play Mode y ver el resultado al momento. Los cambios en assets se conservan al salir: haz una copia del asset si quieres experimentar y luego volver atrás.

Selecciona Assets/Resources/Combat/Barks/EnemyBarkSettings.asset para las frases de enemigos: Display Rect es un desplazamiento desde su cabeza (X positivo sitúa la caja a la derecha para que la punta izquierda apunte hacia él), tamaño, Letters Per Second y Display Seconds (tiempo visible después de terminar de escribir).

SPACE o botón sur del mando completa una frase de introducción; la siguiente pulsación avanza. ENTER sigue saltando la introducción completa. En combate SPACE/ENTER también completa los textos, mientras realiza su acción habitual; las frases nunca bloquean ataques ni minijuegos. Durante HACK se ocultan las frases.

Arte: Assets/Resources/UI/MenuUI contiene las imágenes exportadas del PSD. Menús HACK usan el brazo existente y sus controles propios; estos ajustes son para el HUD y diálogos del dungeon.

## Sonido y fondo de JUMP

DungeonAudioSettings.asset, Dialogue Volume: volumen del bip de las letras (0 lo silencia), multiplicado por Effects Volume. Completar una frase no reproduce bips pendientes.

El fondo de JUMP usa las cuatro capas del PSD, todas fijas. El componente Jump Parallax Backdrop conserva Brightness para ajustar su intensidad.

## Globo del enemigo

EnemyBarkSettings.asset: Display Rect X/Y son desplazamientos respecto a la cabeza del enemigo, no respecto a la pantalla. X positivo mueve a la derecha; Y negativo sube. Width/Height cambian el espacio de la caja, conservando proporciones del dibujo. Text Font Size controla solo la letra de las frases de enemigos. Letters Per Second controla velocidad; Display Seconds el tiempo de lectura tras terminar de escribir. Los TXT de cada enemigo contienen las frases.

Continue Blink Seconds controla el intervalo del triángulo de continuar. Solo aparece en diálogos que esperan confirmación y cuyo texto terminó de escribirse. Las frases automáticas de enemigos y los resultados de combate no lo muestran. El marco rosa del PSD resalta el botón de combate seleccionado.

Dialogue Volume controla las letras; Dialogue Advance Volume el bip al avanzar de frase. JUMP usa Stomp.wav para pisar enemigos y JumpPoint.wav para sumar puntos, ambos temporales originales.
