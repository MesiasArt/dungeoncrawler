# Zona 0 — Refugio

Refugio.asset: posiciones, NPC, conversaciones, retratos, nodos y nivel del Devoto.
Enemigos.asset: aparición aleatoria desactivada; la región también tiene Safe Area activado.
NucleoPlaceholder.asset: icono temporal del accesorio de recompensa.

Editor → Zona 0 — Refugio abre su configuración.
PlanoDungeon.asset y DungeonRegions.asset, en la carpeta superior, contienen el plano físico y las casillas de esta región.

Inicio en el rincón inferior; paso central hacia los NPC; salida arriba conectada físicamente con Zona 1. El evento de regreso no tiene indicadores de misión.
Más detalle en Docs/Zona0Refugio.md.

El nodo REFUGIO está ahora junto a los NPC, en (19,9), con llegada mirando al sur. La terminal permanece en (14,2). El encuentro con el Devoto comienza automáticamente al cruzar hacia la sala inferior y mirarlo tras la tragedia. Consulta Docs/Zona0Refugio.md.

La conversación de salida se inicia únicamente al pulsar E para abrir la puerta; la cámara gira hacia quien habla y vuelve a la puerta al terminar, iniciando su apertura. El teleport solo se usa con E sobre su propia casilla. Hablar con refugiados requiere tenerlos en la casilla inmediatamente delante.
