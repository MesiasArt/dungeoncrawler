# Música del menú, brillo CRT y conversaciones

La música ignora la pausa del AudioListener, mientras los efectos del mundo conservan la pausa. Al abrir el menú baja suavemente al 70% de su volumen configurado y al cerrarlo recupera el volumen. Se conserva el clip y su posición; no se reinicia. El parámetro Menu Music Multiplier está en Editor → Audio: música y SFX.

El CRT ahora compensa la diferencia de gamma de pantalla del asset comprado y corrige el nivel negro de su pasada URP. Esto conserva las zonas oscuras sin oscurecer excesivamente los tonos medios. Brightness en Editor → Efectos del casco CRT permite ajustar la luminosidad final. Scanlines, artefactos, separación cromática y curva conservan sus valores. La compensación funciona con CRT de mundo y CRT de fotograma completo.

Al conversar con un NPC se reduce suavemente el campo de visión a 44 grados por defecto, orientando la cámara hacia él. No se mueve al jugador y no se atraviesan paredes. Al terminar se restaura el campo de visión y la orientación. Se conectaron NPC del refugio y entidades del mapa; las intervenciones de GamerBoy mantienen el foco del interlocutor. Una transición, teleport o entrada a combate libera inmediatamente la cámara. Los controles están en Editor → Interfaz y barras, sección Conversation camera.

Pruebas Play Mode mediante MCP: música sigue reproduciéndose con AudioListener pausado, volumen del menú 0,196 frente a 0,278 restaurado con volumen base 0,28; zoom 44,007 frente a 68 inicial; posición del jugador idéntica; restauración correcta. La prueba usa un interlocutor temporal y no modifica NPC ni partidas. Tests/ComfortAdjustmentsMcpResults.txt.

Prueba del shader plano: señal gris pasa de 0,102 a 0,129 con la compensación configurada durante la prueba; shader sin errores. Tests/CRTReadabilityMcpResults.txt.
