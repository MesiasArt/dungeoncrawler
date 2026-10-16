# Fase 3 — Rendimiento de Codebound

16/10/2026. Checkpoints recuperables: `26c8f93` antes de producción de Fase 2 y `7d4df3d` antes de las optimizaciones. Se revisó el cierre funcional previo; el informe de Fase 2 se actualizó con la build persistente y sus límites. No se interpreta la auditoría histórica de arranque como prueba de gameplay Release.

## Método y protección

Se ejecutó Unity 6000.5.4f1 en una copia física aislada `E:\PROJECTO DUNGEON\dungeoncrawler\Temp\DemoPhase2`, sin enlaces a Assets del proyecto. Las rutas de guardado personales se sustituyeron en esa copia por excepciones bajo UNITY_EDITOR; los fixtures usan exclusivamente su Temp. El laboratorio también recibió una ruta temporal solo en la copia. Ninguna partida personal se usó como fixture ni se deserializó. La comprobación de integridad usa tamaño, fecha y SHA256.

Hardware medido: Intel i5-7400, Radeon RX 580, GameView 1920×1080, VSync desactivado y sin límite de frames solo durante el experimento. Mismo mapa, semilla, 3.865 renderers y nueve enemigos antes/después. Este equipo es distinto del de la auditoría original: sus cifras no son comparables directamente. Unity MCP no estaba expuesto en esta sesión; las pruebas se ejecutaron mediante el Editor real aislado, no se afirma haber usado MCP.

Los microbenchmarks de interfaz usan 50 dibujos IMGUI reales tras calentamiento; la consulta de playlists usa 10.000 llamadas. `GC.Alloc` cuenta eventos de asignación del hilo actual, **no bytes**. Un control positivo verifica ese contador. Los bytes por frame proceden de `GC Allocated In Frame`. `GC.GetAllocatedBytesForCurrentThread` devolvía cero en este Mono y se descartó: `BeforeUnsupportedAllocationCounter.json` es evidencia de ese experimento inválido, no una métrica aceptada. WorkingSet devolvió cero y figura como no disponible (-1). Los frames del Editor son muestras cortas, no una certificación de FPS Release.

## Cambios aceptados

- Slots: caché de seis resúmenes y textos. Se refresca al abrir, inmediatamente después de escrituras/recuperaciones propias y mediante metadatos cada segundo para cambios externos. Se observan principal, cinco respaldos y ruta heredada aplicable. Cambio de ruta de pruebas invalida la caché. La carga explícita siempre vuelve a validar disco. La lectura de resúmenes no repara partidas.
- Audio: listas reutilizables independientes para exploración/combate. Se detectan sustitución de perfil/array, edición de elementos, clips destruidos y fallback. Se mantiene orden, eliminación de duplicados, secuencia y selección aleatoria. El algoritmo sin lista auxiliar reprodujo exactamente 100 selecciones del anterior con la misma semilla. No se cambian clips, volumen, fades ni rutas de reproducción.
- CRT: `GetSharedMaterials(List)` reutiliza un contenedor en lugar de generar un array por renderer. Misma máscara, orden de comandos, shader y resolución. Comparación de una máscara no vacía de 2.073.600 píxeles: cero píxeles diferentes. Se conserva por la reducción comprobada de asignaciones; su tiempo nativo aislado no demuestra mejora.
- Materiales: al destruir el controlador se liberan únicamente sus cinco materiales creados por Surface/Glow. Veinte reinicios dejan cero de esas instancias vivas, frente a cien antes. No se destruyen assets ni materiales compartidos de catálogo.

## Métricas antes y después

Fuentes: `DemoPhase3Data/Before.json`, `AfterSlots.json`, `AfterAudio.json`, `AfterRendering.json`. Datos de la ejecución completa final de rendimiento, sin interpolar grupos intermedios.

| Medida | ANTES | DESPUÉS | Interpretación |
|---|---:|---:|---|
| Dibujo caliente del panel, ms/dibujo | 11,025 | 0,389 | Mejora local medida |
| Lecturas JSON en 50 dibujos | 300 | 0 | No cuenta consultas de metadatos |
| Eventos GC por dibujo | 2.967 | 46 | No equivale a bytes |
| Consulta de playlist, ms/llamada | 0,000460 | 0,0000473 | Coste pequeño, medido en lote |
| Eventos GC por consulta de playlist | 2 | 0 | Lista reutilizada |
| Eventos GC por construcción de máscara CRT | 3.287 | 0 | Misma imagen de máscara |
| Máscara CRT, ms/llamada | 21,879 | 24,992 | Tiempo inconcluso; no se afirma aceleración |
| Panel abierto: CPU PlayerLoop medio, ms | 53,10 | 32,89 | Editor, muestra corta |
| Panel abierto: GC, bytes/frame medios | 2.728.717 | 33.418 | Mejora conjunta de asignaciones |
| Exploración estática: CPU medio, ms | 36,09 | 33,17 | No aisla coste GPU/Editor |
| Exploración estática: GPU medio, ms | 33,19 | 30,96 | No certifica 60 FPS |
| Exploración estática: GC, bytes/frame | 166.571 | 35.477 | Mismo escenario de referencia |
| Materiales propios retirados vivos tras 20 reinicios | 100 | 0 | Recuento directo de referencias nativas |
| Bytes nativos de esos materiales retirados | 339.520 | 0 | No mide todas las fugas del motor |

Las mediciones de máscara incluyen envío nativo de comandos y posible espera de GPU. La diferencia de tiempo entre procesos no permite atribuir una regresión o mejora de CPU al cambio del contenedor; el beneficio principal aceptado es eliminación de 3.287 asignaciones con igualdad de píxeles. Una comprobación adicional alternó el orden de ambas implementaciones en el mismo proceso, 24 pares tras calentamiento: 25,75 ms original y 23,07 ms nueva (`MaskPaired.json`). No reprodujo la aparente regresión de la primera microcomparación, pero tampoco aísla CPU/GPU. No se extrapolan estas medias a una sesión larga ni a otros equipos. Los p95, tamaño de muestra y tiempos GPU completos están en los JSON.

## URP, reflejos y recursos

Se comparó la referencia con el SRP Batcher invertido y después restaurado, manteniendo escena, resolución y efectos. En la ejecución optimizada, CPU medio de referencia/alternativa/restaurada: 33,17 / 33,13 / 33,26 ms; GPU: 30,96 / 30,33 / 30,64 ms. Las diferencias no justifican un cambio permanente. No se modificó ningún asset URP ni nivel de calidad. La restauración está comprobada por la prueba y por hashes de configuración.

No se redujeron resolución, frecuencia ni número de reflejos, ni se desactivaron CRT, scanlines, glow, glitches, iluminación o niebla. El coste exclusivo de las reflexiones no quedó separado mediante captura GPU detallada; no se propone una optimización especulativa. Tampoco se omitió el pase de máscara cuando coinciden intensidades: es una oportunidad que requeriría validación adicional de composición completa.

Inventario de contenido: 332 archivos de imagen/audio, 319.010.251 bytes, 32 grupos de bytes idénticos (13.275.384 bytes redundantes en origen). Algunos se importan como color y normal y no son intercambiables. La inspección de importadores recoge 310 texturas y 40 audios; solo dos texturas tenían Read/Write, ambas bajo MapTests/RetroStyle. `ResourceImporters.json` registra dimensiones, compresión, mipmaps, memoria reportada y modo de carga de audio. Esa inspección carga assets para medirlos; sus totales no representan la memoria del gameplay. Sin evidencia suficiente de beneficio sin cambios visibles, **no se cambió ningún importador ni se fusionaron recursos**.

## Verificación y entrega

La regresión final de reinicio, seis slots, versiones 1/2, recuperación, fallos de transición y destinos normales está en `FinalRegressionResults.txt`. Edit Mode nativo comprueba 12 condiciones de caché (`FinalEditModeResults.txt`). Cada grupo tuvo su ejecución Play Mode y sus métricas: `SlotPlayModeResults.txt`, `AudioPlayModeResults.txt`, `RenderingPlayModeResults.txt`. Son validadores ejecutados por Unity; no se presentan como una suite NUnit/Test Runner.

`GameplaySmokeResults.txt`: 13 comprobaciones correctas en Play Mode: entrada real en combate del laboratorio, ATTACK y su coroutine de turno, submenú HACK, sesión UI/bloqueo de Jump y resolución de fallo, OVERCLOCK y su resultado, submenú ITEM y consumo de una poción fixture, callback de huida con restauración de cámara/tiempo y rebote preciso del modelo Jump. El snapshot de ida/vuelta entre mapas conserva exploración, evento y recibo de recompensa; es una prueba del estado persistente, no una caminata manual entre todas las puertas. Se adelantó el reloj del modelo para finalizar HACKS, sin cambiar sus ajustes. El rebote usa el modelo puro; no se automatizó una cadena manual perfecta de diez saltos. HUIR ejercita el callback de éxito, no certifica todas las tiradas aleatorias. No se hizo una batería exhaustiva de SPEEDRUN, todos los jefes o puzles ni una navegación manual completa.

Durante la preparación del validador se corrigieron referencias de fixture al mapa por defecto, una espera insuficiente de la animación OVERCLOCK y el acceso a una propiedad privada. Este último produjo un error de compilación y pudo mostrar Safe Mode en la instancia temporal; se cerró únicamente esa instancia propia, se corrigió el validador y se repitió hasta pasar. ITEM se aisló en un combate nuevo para evitar que el daño acumulado de otros subcasos invalidara la prueba. No se corrigió gameplay para satisfacer esos fixtures. La copia final volvió a compilar y generó Release sin errores.

Build Windows x64 Release: **E:\PROJECTO DUNGEON\Builds\Codebound-Phase3\Codebound.exe**, con su carpeta completa de datos. `ReleaseBuild.json`: compilación correcta, cero errores, cero avisos, cuatro escenas originales, 469.684.860 bytes, 42,48 segundos. El ejecutable no se lanzó contra partidas personales. No se completó una sesión manual de 45–60 minutos ni una medición de FPS de distribución.

`PersonalBefore.json` / `PersonalAfter.json`: el archivo personal presente conserva exactamente tamaño, fecha y SHA256. `ConfigurationBefore.json` / `ConfigurationAfter.json`: 133 archivos de Resources/ProjectSettings/Packages vigilados, cero cambios. No se modificó la configuración de Codex ni preferencias personales. `TestSourceIntegrity.json` comprueba las fuentes optimizadas frente a la copia; solo el store difiere por las protecciones Editor y el contador de lecturas descritos.

Persisten avisos nativos JobTempAlloc al cerrar el Editor aislado y una excepción ocasional del índice UnityEditor.Search en el arranque. No se atribuyen al gameplay ni se marcan resueltos. No se certifica ausencia global de fugas, estabilidad completa o causa de los crashes históricos de Development.

## Archivos de esta fase

Producción: `Assets/Scripts/DungeonSaveSlotCache.cs` y `.meta` (nuevo); modificados `DungeonSaveStore.cs`, `DungeonPrototype.SaveSlots.cs`, `DungeonAudio.Playlists.cs`, `HelmetCRTLayerMask.cs`, `DungeonPrototype.cs`.

Validación: `Tests/DemoPhase3Benchmark.cs`, `DemoPhase3CacheTests.cs`, `DemoPhase3AudioTests.cs`, `DemoPhase3EditValidation.cs`, `DemoPhase3GameplaySmoke.cs`, `DemoPhase3MaskComparison.cs`, `DemoPhase3ResourceInventory.cs`; ampliaciones condicionales de `DemoPhase2Boot.cs`, `DemoPhase2Validation.cs` y `RunDemoPhase2Tests.ps1`. Este último admite `-Phase3`, `-BenchmarkLabel after` o `-GameplaySmoke` para reproducir cada ruta en otra copia aislada. La generación de build admite `-BuildRelease`; nunca instala herramientas.

Documentación: este informe, `AuditoriaDemoCodebound.md`, cierre de `DemoPhase2FunctionalFixes.md`, evidencias bajo `DemoPhase3Data` y evidencia adicional de cierre bajo `DemoPhase2Data`. No hay cambios de escenas, mapas, narrativa, diálogos, balance, mecánicas, UI, materiales de catálogo, paquetes ni configuración personal. Fase 4 fuera del alcance.

Fase 3 cerrada con las mejoras comprobadas y las limitaciones anteriores. Se mantiene pendiente la validación larga e interactiva del Player y el diagnóstico nativo histórico. No se certifican 60 FPS estables ni una demo lista para publicar. No se comienza Fase 4.
