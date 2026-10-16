# Teleports, Makong y ajustes de HACKS — 2026-10-15

- Respaldos previos: `Backups/TeleportMakongHacks/20261015-084547`.
- Teleports antiguos y entidades del plano consultan una sola lista de nodos activados resolubles en el mapa actual. Los tres nodos del dungeon comparten destinos; no se revelan ni activan automáticamente nodos desconocidos.
- Makong queda fijo en X11/Y53 (`holdPosition`). Al cargar una partida donde había caminado, vuelve a su colocación configurada. La aproximación dispara dos líneas editables en MakongSpecialSettings y después el combate. El laboratorio conserva inicio directo.
- Balance: HP base 105 → 95 (nivel 10: 210 → 190); ATK base 8 → 7; daño especial base 10 → 9; curación fija 18 → 14. Se conservan nivel, escalado, defensa, IA y especiales.
- `Editor > HACKS > JUMP y OVERCLOCK`: pestañas de JUMP, OVERCLOCK y catálogo de desbloqueos/BP. También accesible desde la sección HACKS del panel Virtual Dungeon. Edita assets existentes; previsualiza daño con ATK/DEF introducidos. JUMP usa jumpMaximumQuality/jumpGoodWeight; OVERCLOCK successQuality/failureQuality.
- OVERCLOCK ya calculaba daño correctamente. Su resultado visual ahora muestra el daño confirmado por el combate. No se ha modificado CombatMath ni los multiplicadores actuales (éxito 2.5, fallo 0.5).

## Pruebas Unity MCP, Play Mode

- Tres nodos activados: cada uno enumera los otros dos; seis viajes dirigidos entre pares completados.
- Makong conserva X11/Y53 tras turno de navegación enemiga.
- Aproximación a X11/Y52: diálogo activo, combate todavía inactivo. Terminar diálogo activa combate automáticamente.
- OVERCLOCK contra Makong: carga completa retira 16 HP (190 → 174), coincide con CombatMath usando ATK/DEF reales del test; fallo retira 3 HP. Tick/pulsación después del resultado no duplica daño.
- Consola: cero errores de compilación o ejecución observados.
- Las pruebas alteraron únicamente estado temporal de Play Mode; no se escribieron slots personales.
- No se han probado viajes entre teleports de mapas diferentes; la red compartida aquí corresponde a los nodos del dungeon actual.
