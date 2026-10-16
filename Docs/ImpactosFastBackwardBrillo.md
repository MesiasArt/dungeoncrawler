# Impactos, Fast Backward, brillo y mapa — 2026-10-15

Respaldos previos: `Backups/ImpactFastBackward/20261015-090143`.

## Cambios

- OVERCLOCK: el daño se entrega una sola vez al llegar el rayo (fase 1/2.3 de su animación), en lugar de al terminar la carga. Durante el impacto usa el frame `hurt`, parpadeo, retroceso breve y sacudida del contenido del brazalete. Los comandos quedan bloqueados durante la presentación. Se conserva CombatMath y los multiplicadores 2.5/0.5.
- JUMP: texto grande +1, +2, +3 según combo, pequeña sacudida y un sprite temporal del enemigo derrotado que se aplasta y desvanece. Solo se añaden datos visuales de contacto al modelo; no cambia la física, el ritmo ni el daño.
- Cofre confirmado en X1/Y36, ID `dd55f8a1b779451bb2501875413a5400`: estaba vacío. Ahora entrega tres `item.fast-backward`.
- Fast Backward: consumible de exploración. Guarda en el registro global existente del jugador el mapId, ID del Save Point, posición segura desde la que se utilizó y orientación. Retorna sin curar ni restaurar BP. Usa snapshots y transiciones existentes para otro mapa. No se consume si falta destino, está ocupado o el jugador está en combate/evento. Se excluye del menú de objetos de combate.
- Los Save Points de entidades y la terminal antigua registran el retorno al utilizarse; no es necesario sobrescribir una partida para registrar el destino durante la sesión. El destino se conserva en los siguientes guardados. Una partida anterior sin este dato necesita utilizar un Save Point nuevamente. Un cofre ya abierto en una partida previa conserva ese estado.
- Drop de poción pequeña: 35% → 32%; restantes sin cambios.
- CRT: `Brillo exploración` 1.30 y `Brillo combate` 1.85, en Editor > Efectos del casco CRT. Gamma de tonos medios del mundo .78. El shader de composición aplica gamma/exposición al mundo antes de incorporar la UI. El oscurecimiento lejano del escenario pasa de .24 a .55 y se eleva la iluminación mínima de sprites enemigos. Se mantiene la niebla y sus distancias. La gamma nueva se aplica en la composición separada que está activa en el proyecto.
- Mapa de pausa: al abrir se ajusta completo (24x69 en el mapa actual), aunque antes estuviera ampliado. Mantiene rueda/arrastre y añade VER TODO. CENTRAR EN TI conserva el zoom elegido.

## Pruebas Unity MCP

- Compilación y consola sin errores observados.
- OVERCLOCK en combate real contra Makong: al completar carga HP intacto hasta el impacto; al llegar el rayo quita 16 HP, sin duplicados. Callback independiente también entregado una sola vez. Se inspeccionó captura de impacto.
- JUMP: cadenas de diez golpes completas a 30, 60 y 240 FPS. Captura confirma +1 y sprite de contacto; aplastamiento/desvanecimiento temporizados .42 s.
- Cofre: interacción real entrega tres Fast Backward; repetir interacción no duplica recompensa.
- Fast Backward: sin Save Point no consume; mismo mapa retorna y resta una unidad; combate impide uso sin consumo; ida a fixture FloorB y retorno al mapa principal conserva snapshots y resta una unidad; JSON de guardado completo y restauración conservan el destino. Pruebas realizadas en memoria, sin escribir slots personales. Una invocación adicional a la terminal fue rechazada por revisión automática por posible acceso al guardado; se sustituyó por validación del registro/restauración en memoria.
- Brillo: contexto de exploración 1.30 / combate 1.85 confirmado. GPU: tono .1 llega a .261 con exposición 1.55/gamma .78 durante prueba; UI opaca mantiene (.302,.200,.102). Captura de combate revisada tras finalizar transición.
- Mapa: zoom forzado previamente a 48; al reabrir se restablece 4.266525 y muestra 24x69 casillas. Captura revisada.

Capturas locales en `Captures/ImpactFastBackward`. No se han sobrescrito partidas personales. El retorno desde la ciudad está conectado al tránsito existente, pero esta sesión no incluyó prueba de viaje desde Digital Kingdom.
