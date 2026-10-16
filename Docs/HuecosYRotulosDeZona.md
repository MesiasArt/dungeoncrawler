# Huecos profundos y rotulos de zona

Los huecos (~) conservan su restriccion de paso. Se generan paredes bajo todos los bordes exteriores, sin paredes internas entre dos huecos, y un fondo oscuro a tres casillas de profundidad. Un material propio reutiliza la textura de las paredes y la degrada hasta casi negro, incluyendo la emision y la niebla. Los nuevos meshes carecen de colliders y no alteran la navegacion.

Configuracion: DungeonVisualSettings, Void Depth In Cells (3) y Void Fade Fraction (0.8). Se aplica al construir el dungeon. El material normal mantiene el degradado desactivado.

Los nombres de zona se muestran junto al minimapa y en el mapa de pausa. Verde: zona segura o sin encuentros configurados. Rojo: zona con poblacion o enemigos colocados, y zona actual durante un combate. Los prompts y destinos de los teleports usan el mismo criterio; no dependen de si momentaneamente queda algun enemigo a la vista.

Renombrados sin cambiar IDs ni conexiones: node.zone2 en (7,16) a Entrada Zona 2; ed55c002d9bc4be296015a7c609b32b1 en (10,49) a Salida del Dungeon. Refugio conserva su nombre.

Respaldo: Backups/VoidDepthAndZoneLabels/20261015-144349. Validacion Unity MCP: 38 huecos, 58 bordes exteriores generados, 38 fondos, cero colliders decorativos activos y todos los huecos siguen rechazados por Open. Colores observados: zone-0, zone-1.5 y zone-city-approach verdes; zone-1 y zone-2 rojos. FindTransferNode resuelve los tres IDs con los mismos destinos. Captura revisada: Captures/VoidDepth/DepthPreview.png. Compilacion sin errores. No se escribieron slots personales.
