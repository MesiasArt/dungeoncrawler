# Mapas revelados por teleports

Al activar/interactuar por primera vez con un nodo, después de su diálogo se muestra el mapa completo y se descargan las casillas de sus regiones en 1,8 segundos, con entrada y pausa final (2,8 segundos en total). La música continúa. El mundo se detiene durante la descarga.

## Asociaciones actuales
- Refugio, X19/Y9 (`node.refuge`): `zone-0`.
- Pasillo, X7/Y16 (`node.zone2`, ID histórico conservado): `zone-1` y `zone-1.5`.
- Cerca de Makong, X10/Y49 (`ed55c002d9bc4be296015a7c609b32b1`): `zone-2`.

No se alteraron las posiciones ni los eventos narrativos existentes. Se revelan las casillas de la región y las paredes/vacíos contiguos, sin descubrir suelo de otras regiones.

## Editor
En el plano, selecciona un Teleport y marca **Zonas que revela el teleport**. Puedes asociar varias zonas al mismo nodo. Los nodos heredados del refugio se configuran también desde Editor → Zona 0 — Refugio, desplegando Nodes.

Los nombres proceden de DungeonRegionSettings y se almacenan sus IDs existentes. Una lista vacía no revela mapas. Los teleports nuevos deben recibir una asociación.

## Guardado y compatibilidad
Las casillas descubiertas utilizan el guardado existente de exploración. RefugeProgress.mapRevealedNodes recuerda los nodos cuya descarga terminó. Las partidas antiguas pueden descargar el mapa al volver a interactuar con un nodo, aunque ya estuviera activo. La activación narrativa automática del refugio no revela su mapa hasta que el jugador lo usa.

## Pruebas Unity MCP
Tests/TeleportMapRevealMcpResults.txt: cobertura y exclusión de las tres asociaciones, progresión parcial en Play Mode, restauración del tiempo, callback único, bloqueo de descargas repetidas y serialización de casillas e IDs. Sin errores de consola al terminar. Captura: Captures/TeleportMapReveal/Reveal.png.
