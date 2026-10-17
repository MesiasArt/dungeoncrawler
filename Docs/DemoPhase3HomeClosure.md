# Fase 3 — Reanudación y validación local, 17/10/2026

Estado: compilación y regresión completadas; aceptación de estabilidad Release pendiente por fallo nativo al cerrar. No iniciar Fase 4.

## Alcance y aislamiento

Checkpoint e0a8029, ya descargado. No se volvió a descargar, instalar ni sincronizar nada. No se modificó código, escenas, arte, mecánicas ni configuración permanente. Laboratorio físico separado: `E:/UNITY PRUEBA/DCG/Codebound-Phase3-Home-Lab-20261016`. Unity 6000.5.4f1; Ryzen 7 5800X, RX 7600 XT, 32 GB. Las métricas de oficina pertenecen a otro equipo y no son una comparación de velocidad con este.

Guardados sintéticos en identidad temporal `CodeboundPhase3HomeIsolated/CodeboundPhase3Home`; no se usaron partidas personales. El fixture de combate adelanta al jugador a (4,44), frente a un Hongo en (4,45), y fija nivel 10 exclusivamente en ese JSON. No equivale a recorrer la campaña. FinalIntegrity.json confirma 2093 hashes de producción y 8 archivos personales sin cambios.

## Pruebas terminadas

- 95 PASS, 0 FAIL en regresión aislada: reinicios del Píxel y dungeon, seis slots, versiones y respaldos, corrupción, rollback de transiciones, persistencia y pruebas de combate ATTACK/Jump/OVERCLOCK/ITEM/huida. Parte del combate usa invocaciones del fixture y no certifica una cadena de minijuegos ejecutada manualmente.
- 12 comprobaciones Edit Mode de caché aprobadas.
- Movimiento real en Play del proyecto original: avanzar, retroceder y dos giros, 4 PASS. Sin errores de consola durante esa prueba.
- Tres comparaciones CRT de 2073600 píxeles cada una: 0 distintos; pesos desiguales, incluso próximos, conservan la máscara.
- Release BuildOptions.None correcta: 0 errores, 0 advertencias, 469685646 bytes, 95,36 segundos. Sin Profiler ni autoconexión. Incluye las cuatro escenas originales. La identidad temporal evita compartir partidas y no constituye el paquete público definitivo.

Ejecutable: `E:/UNITY PRUEBA/DCG/Codebound-Phase3-Home-Lab-20261016/Builds/Codebound-Phase3-Home/Codebound.exe`.

## Mediciones del Editor, no del ejecutable

Fuente completa: DemoPhase3HomeData/phase3-home-after.json; 1920×1080, DX12. No hay baseline anterior equivalente en este equipo.

| Caso | Media CPU PlayerLoop | p95 CPU | Media GPU | p95 GPU |
|---|---:|---:|---:|---:|
| Mundo estático | 11,59 ms | 48,75 ms | 3,21 ms | 3,72 ms |
| Cámara en movimiento y ambos reflejos | 11,24 ms | 13,13 ms | 5,39 ms | 6,23 ms |

Panel de slots caliente: 0 lecturas de disco en 50 dibujos, 0,178 ms/dibujo y 46 eventos de asignación por dibujo. Lookup de playlists: 0 asignaciones en 10000 operaciones. Materiales propios retirados todavía vivos tras 20 reinicios: 0. La prueba fuerza explícitamente la máscara CRT: sus 12,70 ms no representan el camino normal omitido cuando los pesos coinciden. `allocatedBytes=-1` y `workingSet=-1` significan no disponible; los contadores GC.Alloc de micropruebas son eventos, no bytes.

No se certifican 60 FPS sostenidos: hay picos en Editor, faltan tiempos de presentación del Player y una sesión larga. No se instalaron medidores.

## Prueba visible Release y nuevo fallo

Se utilizó computer-use para observar y accionar la ventana. Se observaron título, nueva partida y comienzo del prólogo, menú CONTINUAR y carga del fixture de dungeon con CRT y escenario visibles. Tras cargar, las pulsaciones Up/W/Right/Escape no produjeron movimiento o menú verificable; no se da por cubierto el combate visible ni se atribuye la falta de respuesta a una causa sin reproducirla manualmente. Los tests del Editor no sustituyen esta cobertura.

El cierre solicitado por Windows terminó con **-1073741819 / 0xC0000005**. El registro llega a Cleanup de PhysX, apagado de Input System y `CodeReloadManager destroyed`. Windows registra APPCRASH en UnityPlayer.dll. Hay dos offsets observados, 0x10a181 y 0x100915; no bastan para identificar una función ni una causa.

Se detectó además una ejecución auxiliar concurrente tras un lanzamiento cuyo shell no pudo escribir los archivos de seguimiento. Esa pareja no se utiliza como reproducción aislada. Se confirmó que no quedaban procesos Codebound antes de los controles siguientes:

| Control con un único proceso | Ruta | Resultado |
|---|---|---|
| DX12 | Título, sin cargar partida, cierre con X | 0xC0000005; HomeTitleOnlyExit.json |
| DX11 temporal | Título, sin cargar partida, cierre con X | 0xC0000005; HomeDX11TitleExit.json |

La API de cada ejecución está confirmada en su Player log. Cambiar a DX11 no resuelve este cierre. No demuestra que ambos crashes tengan idéntica causa interna ni que sea el mismo fallo de la auditoría original. El control del título muestra que cargar una partida o iniciar combate no es requisito para reproducirlo. No se atribuye a CRT, Profiler, gameplay ni al driver. Release no lleva Profiler. No se desactivaron efectos.

Evidencias: logs Player completos de estos intentos, códigos de salida y WindowsCrashEvents.json. WER archivó informes; su lectura directa fue denegada y no se cambiaron permisos. No se obtuvo dump ni stack nativo simbolizado. La advertencia de reducción automática del atlas de sombras está conservada; no se ha probado relación causal.

## Incidencias de la infraestructura Editor

Los primeros intentos ocultos no avanzaron hasta finalizar por timeout. Se conserva una excepción UnityEditor.Search.SearchDatabase y avisos JobTempAlloc. Al ejecutar el laboratorio visible se completaron regresión y benchmark. No está demostrado que la excepción Search causara la falta de avance; no se mezcla con el fallo nativo del Player.

## Pendientes concretos

1. Reproducir con teclado manual la falta de respuesta tras cargar y completar combate visible, transiciones y minijuegos. No corregir basándose solo en entradas automatizadas.
2. Investigar el cierre mínimo desde título con símbolos/dump de UnityPlayer; conservar la build y estos logs antes de nuevos ensayos. Revisar los callbacks de cierre y recursos nativos como hipótesis, sin refactorización ni cambios de paquetes.
3. Tras resolverlo, repetir cierres normales y sesión de 45–60 minutos; medir Player de forma separada. Los diez arranques históricos no prueban un cierre limpio de esta build.
4. Mantener Fase 4 sin iniciar. Esta entrega completa la compilación pendiente y registra la validación, pero la estabilidad Release sigue abierta.

## Diagnóstico posterior del cierre

El [diagnóstico dedicado del 17/10/2026](DemoReleaseShutdownDiagnosis.md) conserva nuevos dumps, símbolos verificados y 14 ejecuciones fallidas. No hay corrección de producción ni build pública nueva. Fase 4 continúa bloqueada.
