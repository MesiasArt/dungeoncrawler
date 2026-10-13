# Casco CRT, HUD, Bits y guardado

## CRT
Se retiró `Assets/CRT-Free` y el shader, material y textura de ruido de la adaptación anterior. Se utiliza el asset comprado en `Assets/CRTPostprocess`, mediante `HelmetCRTRendererFeature`, que hereda su `NTSCFeature` y conserva sus pasadas RenderGraph. No se reemplazaron las cámaras. Se aplica solo a la cámara con `HelmetCRTController`, después del postprocesado del mundo y antes del HUD IMGUI; no se aplica a cámaras de reflejos ni al experimento independiente.

**Editor → Efectos del casco CRT** abre `Assets/Resources/HelmetCRT/Settings.asset`. Curvature activa la curva; Curvature Amount negativo la invierte hacia dentro, positivo hacia fuera y cero la deja plana. Se añadió esta posibilidad al shader comprado, manteniendo su curva original para las cámaras que no usan el controlador de Codebound. El perfil admite scanlines, artefactos y fringe cromático, resolución de procesamiento, glitches automáticos, intervalos, duración y límite de intensidad. El nuevo asset no utiliza la textura de polvo anterior. Los valores que edites en el Inspector se conservan.

**Pausa → Ajustes → Pantalla CRT del casco** permite desactivarlo y conserva la preferencia entre sesiones. Se omiten sus pasadas cuando está desactivado. El asset comprado utiliza sus propios buffers NTSC; no se añadió otra cadena CRT encima. No se realizó un benchmark de build.

Glitches automáticos separados por 18–40 segundos por defecto, duración 0,1–0,22; todas las peticiones se limitan a 0,1–0,3 segundos e intensidad máxima configurable. Se conectaron impactos, daño importante, entradas a regiones con tema corrupto, teleport y la interferencia existente. Otros eventos pueden llamar `DungeonPrototype.TriggerHelmetGlitch(intensity, seconds)` o `HelmetCRTController.TriggerContext`. La pausa suspende los glitches automáticos.

## HUD y Bits
Dentro del cuadro actual: vida roja sin texto numérico, Bits azul y XP fina sin texto. No se redujo todavía el cuadro para cuatro personajes. El panel de personaje incluye barras y HACKS desbloqueados. Se quitaron los nombres flotantes azules de los NPC; sus nombres siguen disponibles en conversaciones.

Se usa la fuente importada Thaleah; el recurso de runtime está en `Assets/Resources/UI/Fonts`. Se verificaron ñ y á.

Capacidad inicial: 100 Bits, editable en `Assets/Resources/Combat/PlayerBitsSettings.asset`. Costes en **Editor → catálogo de HACKS**, campo Bits Cost: JUMP 10, OVERCLOCK 25. Se cobra al comenzar realmente el HACK, después de validar objetivo y disponibilidad. Cancelar la selección no cobra. Un HACK fallido consume su coste; sin Bits suficientes no se inicia. Savepoints con curación recuperan vida y Bits. No se añadió regeneración automática ni objetos nuevos.

El guardado conserva Bits actuales; las partidas antiguas sin este campo reciben capacidad completa. JUMP continúa con reloj independiente sin pausar o reiniciar música. Drop de poción pequeña: 35%.

## Guardado
Las terminales solicitan un slot antes de guardar. F5 y el menú ya ofrecen la misma selección. Un slot ocupado requiere confirmar reemplazo. Cada escritura conserva cinco versiones anteriores (`.bak1` a `.bak5`); se mantienen fuera de Assets.

No existía una copia de la partida de nivel 6. Con autorización se reconstruyó el slot 1: nivel 6, guante MK-II y camiseta de resistencia, equipo de pies anterior, XP de nivel en cero y exploración parcial (305 de 1344 casillas). Posición segura próxima al cofre de camiseta, X2/Y43 en Zona 2. No se añadió el tercer equipo nuevo ni se reveló toda la zona. Es una reconstrucción aproximada, no recuperación exacta. El archivo anterior se conservó también en `Backups/RecoveredSave/Slot1-before-reconstruction.json`.

Cargar con **Cargar partida / F9 → slot 1, Reconstruida — LV6 Zona 2**.

## Verificación MCP
Nueve comprobaciones de Bits/música/intensidad/duración correctas: consumo 10 y 25, no cobro duplicado, bloqueo por falta de Bits, restauración de cero Bits, recuperación a 100, música continua y límites de glitch. `Tests/HelmetBitsMcpResults.txt`.

Avisos de puerta: visible cerrada y de frente, oculto de espaldas, abierta y distante. Se eliminó el mensaje permanente que se asignaba al intentar atravesar una puerta. Glitch vuelve a cero. Ambas direcciones de curva se renderizaron; capturas de habitación, pasillo oscuro, menús y teleport en `Captures/HelmetCRT/Verified`. `Tests/HelmetVisualMcpResults.txt`. El asset también se observó en combate, con HUD separado. Consola y shader sin errores en la verificación final.
