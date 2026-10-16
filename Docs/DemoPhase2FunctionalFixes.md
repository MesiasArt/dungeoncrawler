# Fase 2 — Correcciones funcionales

Fecha: 16 de octubre de 2026. Checkpoint previo a producción: `26c8f93`.

## Estado antes de Fase 3

Implementadas y verificadas D01, D03/R02 y R03, con los límites de cobertura indicados abajo. La regresión conjunta registró 82 comprobaciones correctas y la prueba adicional del aviso confirmó Enter mediante Input System. La primera build temporal se eliminó al cerrar el Editor y no se ofrece como entregable. Se repitió la generación en una carpeta persistente: **E:\\PROJECTO DUNGEON\\Builds\\Codebound-Phase2\\Codebound.exe**, Windows x64 Release, 0 errores, 0 avisos, 469.681.100 bytes, 52,25 segundos. `DemoPhase2Data/PersistentReleaseBuild.json` y `LatestPersistenceChecks.txt` registran la compilación y la última regresión de persistencia. El checkpoint de trabajo previo a las optimizaciones es `7d4df3d`. Cierre funcional de Fase 2; no equivale a certificar estabilidad larga de Release.

## D01 — Reinicio

El controlador nuevo recibe la configuración serializada del anterior y los perfiles de JUMP/OVERCLOCK. Se descartan referencias a la sesión de combate. El componente de composición del Píxel se reconstruye desde su perfil después de destruir el escenario anterior; sus valores de CRT, audio, atmósfera y superficies siguen siendo los existentes.

Tres reinicios consecutivos conservaron el hash de composición, un controlador, un compositor, un host de audio y las dos reflexiones. El reinicio del dungeon normal no incorpora el compositor del Píxel. Evidencia: `DemoPhase2Data/D01Results.txt` y `PixelRestart.png`. Se invocó el mismo método que usan muerte/reintento; no se automatizó un clic físico en esos botones.

## D03/R02 — Guardado

La lectura de resúmenes es de solo lectura. Busca el principal y después `.bak1` a `.bak5` en ese orden; no repara archivos al dibujar la interfaz. Un principal de una versión futura no se degrada silenciosamente. La ruta heredada conserva su uso cuando no existe una partida moderna.

Una carga explícita desde respaldo valida el contenido, intenta reparar el principal y conserva sus bytes dañados en `.corrupt-<fecha>-<id>`. No altera el respaldo usado. Si no puede reparar el principal por permisos/bloqueo, permite usar el respaldo e informa esa limitación. El aviso exige confirmación con clic, Enter, espacio o botón sur del mando; la entrada principal no continúa mientras el aviso está activo.

La escritura valida el registro, escribe un temporal hermano con nombre único, fuerza `Flush(true)`, relee/valida ese temporal y usa `File.Replace` para un principal existente o `File.Move` para uno nuevo. No sustituye esta operación por una copia destructiva si el reemplazo falla. La rotación anterior de cinco respaldos se conserva; un principal inválido no reemplaza un respaldo válido. No hay cambio de formato: versiones 1/2 y seis slots.

Pruebas: principal truncado, ausente e inválido; respaldo reciente inválido; rotación de cinco respaldos; versión futura; destino bloqueado; reparación denegada; temporal abandonado; inventario inválido; ausencia de candidatos válidos; campos opcionales de versión 1; carga real de versiones 1/2 en seis slots. Archivos exclusivamente generados en la copia temporal. Evidencias: `D03Results.txt`, `FinalResults.txt`, `RecoveryNoticeResults.txt`, `RecoveryNotice.png`.

## R03 — Transiciones

El usuario autorizó conservar temporalmente el origen mediante carga aditiva y volver a ese origen si falla la preparación. El fundido existente y sus tiempos normales se conservan. El origen se descarga solo después de comprobar la inicialización completa del controlador y, para Píxel, de su compositor. La ciudad exige su controlador preparado y una cámara activa.

Plazos actuales: 120 segundos para carga y 30 segundos para preparación, medidos con tiempo real. Una falla restaura los objetos originales, cámara, controles, pausa, audio y atmósfera; no depende de escribir o leer una partida. La carga tardía no cancelable se descarga cuando termina y se evitan nuevos viajes concurrentes. No se reproducen diálogos pendientes del destino fallido. El progreso narrativo completado se conserva.

Pruebas con escenas fixture: inexistente, componente ausente, componente retrasado, compositor sin preparar con cámara existente y plazo de carga agotado con limpieza tardía. Se verificaron entrada real fallida a ciudad, entrada normal, ciudad/Píxel, Píxel/Píxel y recuperación desde ciudad/Píxel. Se comprobó identidad del controlador de origen, progreso, cámara, pausa, audio, CRT, reflejos, atmósfera y ausencia de duplicados. Evidencia: `FinalResults.txt`, `TransitionRecovered.png`.

## Archivos

Producción modificada: `DungeonPrototype.cs`, `DungeonPrototype.Save.cs`, `DungeonPrototype.SaveSlots.cs`, `DungeonPrototype.DigitalKingdom.cs`, `DungeonSaveStore.cs`, `CodeboundTitleScreen.cs`, `DigitalKingdomController.cs`, `DigitalKingdomTransit.cs`, `DungeonWhiteTransition.cs`, `RetroDungeonWallComposer.cs`.

Producción añadida: `DungeonPrototype.TransitionRecovery.cs`, `DungeonAudio.TransitionRecovery.cs`, `DungeonTransitionOrigin.cs`, `DungeonSaveRecoveryNotice.cs`, con sus `.meta`.

Pruebas añadidas bajo `Tests`: `DemoPhase2Boot.cs`, `DemoPhase2Validation.cs`, `DemoPhase2SaveValidation.cs`, `DemoPhase2TransitionValidation.cs`, `DemoPhase2DelayedDungeon.cs`, `DemoPhase2ReleaseBuild.cs`, `RunDemoPhase2Tests.ps1`.

## Límites y riesgos

- El Editor aislado tuvo una excepción de su índice de búsqueda en algunos arranques y avisos nativos de JobTempAlloc al cerrar. No se atribuyen a gameplay ni se consideran resueltos. Se conservaron resultados textuales sin incluir tokens de licencia de los logs generales.
- Retener dos escenas durante la transición aumenta temporalmente el uso de memoria. Solo una permanece activa; el incremento requiere perfilado posterior en hardware objetivo.
- `Flush(true)` y el reemplazo atómico reducen ventanas de corrupción, pero no certifican resistencia a un corte físico de energía o fallos del dispositivo. La rotación completa de respaldos no es una transacción única.
- Una carga nativa bloqueada que impida ejecutar cualquier frame tampoco permite ejecutar código de recuperación. El timeout cubre las esperas cuando Unity continúa procesando frames.
- Se probaron fixtures y Editor Play Mode. La nueva build no se ejecutó ni se hizo una sesión manual larga de Release. La validación Windows de Fase 1 sigue teniendo sus límites originales.
- No se tocaron archivos personales de partida, escenas, mapas, diálogos, balance, materiales ni configuración gráfica del proyecto. El checkpoint previo incorpora únicamente los ajustes CRT que ya eran locales.

La Fase 3 está autorizada por el usuario. No se autoriza Fase 4.
