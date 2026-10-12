# Enemigos y encuentros

Edita Assets/Resources/Exploration/EnemyEncounterSettings.asset en el Inspector fuera de Play Mode. Respawn Seconds: 30 segundos; Detection Steps: 2 casillas; Minimum/Maximum Enemies: 1 y 3; Move Seconds: duración del paso; Maximum Map Enemies limita la población.

Los enemigos avanzan una casilla al moverte o girar, y esperan si estás quieto. Detectan al jugador a dos casillas en un pasillo sin paredes ni puertas cerradas. Una vez alertados, buscan una ruta por las casillas libres. No atraviesan paredes, puertas ni otros enemigos.

Los encuentros reúnen enemigos del mapa y pueden incorporar compañeros aleatorios de Tapaku, Hongo y Soldado. Cada enemigo conserva su vida al cambiar de objetivo o huir. Al pulsar ATTACK en un encuentro múltiple, aparece la selección de objetivo. Mueve la flecha blanca con las flechas del teclado o Q/E; confirma con Enter, Espacio o pulsando ATTACK otra vez. También puedes pulsar el nombre del enemigo. Escape cancela la selección. ATTACK y HACK afectan al objetivo seleccionado; cada superviviente contraataca una vez. El combate termina al derrotar a todos o huir. Los cadáveres permanecen hasta acabar la frase de muerte.

Vida reducida: Tapaku 10, Hongo 16, Soldado 13. Daño: Tapaku 3, Hongo 2, Soldado 2; granada 4. El daño impar se divide y redondea hacia arriba porque el HP usa enteros.

La reaparición cuenta desde la desaparición del cadáver. No aparece ningún enemigo durante una introducción, combate o movimiento; las reapariciones pendientes se procesan al volver a explorar, en casillas libres a tres o más casillas del jugador. Se reutilizan los espacios de enemigos derrotados.

Tapaku reemplaza a Ghost e hereda sus frases, tiempos de animación, sonidos y reflejo. Los textos conservan sus archivos de Ghost para mantener los bancos y sus referencias; el perfil del Inspector ahora se llama TAPAKU. El PSD de Tapaku contiene tres idles, ataque, daño y muerte. JUMP utiliza los dos dibujos de Tapaku del PSD enemigos mini jump.psd.

JUMP: enemigos de 80 unidades de alto (antes 40), con colisión ampliada y dibujo de 112 unidades de ancho conservando proporción. Los tiempos musicales y controles se mantienen.

El combate se presenta en un corredor independiente de 9 metros de ancho, con enemigos a escala 1 y profundidad normal. Los compañeros salen desde detrás del principal hacia los lados durante una transición de cámara con glitch. El mapa de exploración conserva sus paredes, antorchas y densidad de texturas originales. Al terminar o huir, vuelve la cámara al mapa y el corredor de combate queda oculto para reutilizarlo.

Los diálogos de grupos se colocan sobre la cabeza. Los sprites reciben luz, sombras y el efecto de horizonte. HACK también solicita un objetivo al elegir el minijuego: navega con flechas o Q/E y confirma con Enter/Espacio o pulsando HACK otra vez. Escape cancela sin iniciar el minijuego.
