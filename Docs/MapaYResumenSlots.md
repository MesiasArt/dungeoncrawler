# Corrección de mapa y slots — 2026-10-15

- El mapa vuelve a abrir con una vista amplia de 24x16 casillas en el panel principal y el plano general lateral, siguiendo la referencia. Tamaño inicial 18; no conserva ampliaciones anteriores. VER TODO mantiene el ajuste al plano completo.
- Poción pequeña: drop 32% → 28%.
- Los slots muestran únicamente zona, lugar y tiempo jugado HH:MM. No muestran nombre personalizado, nivel, HP, coordenadas ni fecha/hora del guardado. Los metadatos anteriores se conservan internamente para compatibilidad.
- `PlayerProgressionRecord` incorpora `hasPlayTime` y `playedSeconds`; la captura/restauración global y las transiciones existentes conservan el contador. El contador empieza al iniciar/cargar la partida e incluye el tiempo en menús durante esa sesión.
- Partidas antiguas: muestran Sin registro hasta volver a guardarse. Al cargarlas se empieza a contar desde cero; no es posible reconstruir las horas previas a partir de la fecha del archivo.
- Respaldos previos en Backups/MapSlots/20261015-094048.

Pruebas Unity MCP: cero errores de consola observados; serialización y restauración completa conserva 7384 segundos; resumen Zona 0 — El refugio de los olvidados — 02:03; partida antigua Sin registro; vista inicial 24x16 y drop 28 confirmados. Pruebas en memoria sin escribir slots personales. Captura intermedia en Captures/MapSlots/map.png (antes del ajuste final a 24x16).

Archivos: DungeonPrototype.PauseMap.cs, DungeonPrototype.SaveSlots.cs, DungeonPrototype.SlotSummary.cs, DungeonPrototype.Progression.cs, DungeonPrototype.Save.cs y CombatDropSettings.asset.
