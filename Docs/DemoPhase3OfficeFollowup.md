# Fase 3 — Continuación en el equipo de oficina, pausada

16/10/2026. Pausa explícita del usuario para continuar desde casa. Checkpoint previo: `fc71a04`. No iniciar Fase 4.

Equipo informado y medido: i5-7400, Radeon RX 580 8 GB, RAM 32 GB. El usuario confirmó que el lag ocurrió en el dungeon de neón azul/rosa y planteó que el disco estaba casi lleno. No se comprobó el espacio disponible en el momento del lag ni se atribuye causalidad al disco. No se borraron archivos para liberar espacio.

## Cambio comprobado

`HelmetCRTWholeFrame.cs` evita construir la máscara de enemigos cuando sus intensidades CRT y las del fondo son exactamente iguales. El shader usa esa máscara solamente para interpolar esos dos pesos: si coinciden, no cambia la imagen. No se desactiva CRT ni se modifica ningún ajuste visual. Los perfiles de ciudad y Píxel tienen pesos distintos y conservan el camino anterior; no extrapolarles esta mejora.

Tres comparaciones de composición completa a 1920×1080, intensidades iguales 0 / 0,37 / 1: **cero píxeles distintos**. Se verificó que pesos distintos, incluso muy próximos, mantienen la máscara.

| Muestra del Editor a 1080p | Antes, ms | Después CRT, ms |
|---|---:|---:|
| CPU, vista estática | 38,00 | 12,96 |
| CPU, cámara en movimiento con ambos reflejos | 47,99 | 24,89 |
| GPU, vista estática | 9,14 | 9,92 |
| GPU, cámara en movimiento | 37,25 | 22,94 |
| Intervalo de frame del Editor, estático | 42,27 | 15,37 |
| Intervalo de frame del Editor, movimiento | 50,93 | 27,36 |

Son muestras cortas del Editor, no FPS de Release ni certificación de 60 FPS. Fuentes: `DemoPhase3FollowupData/Before.json`, `AfterCRT.json`, `Comparison.json`, `CRTPixelEquality.json`. La mejora medida principal es CPU; la GPU estática no mejoró de forma demostrada. No se redujeron resolución, sombras, reflejos ni efectos.

## Ensayo descartado

Agrupación estática de 2.104 superficies, probada únicamente en la copia aislada: cero píxeles distintos en esa vista, pero sin mejora consistente. CPU estática 12,80 ms frente a 12,96; movimiento 27,36 ms frente a 24,89 y peor p95. **No se implementó en producción.** `DemoPhase3BatchTrial.cs` conserva el experimento reproducible, condicionado a un símbolo y marcador de pruebas. `BatchTrial.json` y las métricas experimentales de `AfterCRT.json` son evidencia del descarte.

## Pruebas y punto de reanudación

Pasaron caché, persistencia, playlists y las comprobaciones CRT. La regresión final repitió reinicio del Píxel, seis slots, respaldos, transiciones y combate (ATTACK, HACK, Jump, OVERCLOCK, ITEM, huida). Un validador antiguo asumía que siempre existía el componente de máscara; se corrigió su preparación para crearlo explícitamente en la prueba, sin forzar trabajo innecesario en producción.

**La compilación Release nueva se interrumpió al solicitar la pausa. No hay informe de compilación correcta y no se ofrece como build terminada.** Cualquier contenido de `E:\PROJECTO DUNGEON\Builds\Codebound-Phase3-Office` es parcial y no debe distribuirse. La build anterior de Fase 3 no contiene esta optimización.

Para continuar: revisar este commit y los resultados, volver a generar Release y probarla de forma visible en el PC objetivo, incluyendo movimiento y combate. Continúan pendientes las mediciones del Player y una sesión larga. No instalar herramientas ni tocar partidas personales.

La copia de pruebas se trasladó fuera de Temp, a `E:\PROJECTO DUNGEON\Codebound-Performance-Lab`, porque Unity elimina el Temp del proyecto al reiniciar. Es una copia física local no versionada; debe recrearse en casa con `Tests/RunDemoPhase2Tests.ps1 -Phase3 -RenderElision -BenchmarkLabel after -TestRootPath <ruta-separada>`. El marcador `phase3-office-followup` activa el caso de cámara en movimiento. Para la regresión conjunta se usó `phase3-gameplay-after-regression`; la generación usa `phase2-build-after-tests` y `demo-build-output`. Todos los fixtures de guardado están aislados.

Producción modificada en esta continuación: únicamente `Assets/Scripts/HelmetCRTWholeFrame.cs`. Pruebas: `DemoPhase2Validation.cs`, `DemoPhase3Benchmark.cs`, `DemoPhase3MaskComparison.cs`, `RunDemoPhase2Tests.ps1`; añadidos `DemoPhase3RenderElisionTests.cs` y `DemoPhase3BatchTrial.cs`. Documentación y evidencias: este informe, la referencia en la auditoría y `DemoPhase3FollowupData`.
