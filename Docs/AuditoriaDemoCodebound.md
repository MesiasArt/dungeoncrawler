# Codebound — Auditoría de demo pública

Fecha: 16 de octubre de 2026. Alcance: análisis, sin implementar optimizaciones.

## 1. Resumen ejecutivo

El prototipo dispone de una base funcional considerable: exploración por casillas, combate, JUMP y OVERCLOCK, SPEEDRUN, progresión, seis slots, snapshots por mapa, condiciones/eventos, Digital Kingdom y Reino del Píxel. Las fases DRPG 1–7 están documentadas como completadas. La documentación general antigua no describe todo el estado actual: no debe usarse para afirmar que faltan guardado o progresión.

**No recomiendo publicar todavía.** Se reprodujo la pérdida de la presentación de Reino del Píxel al reiniciar; el panel de slots genera lecturas y asignaciones evitables; falta una validación fiable del ejecutable de distribución. Dos arranques instrumentados terminaron con un cierre nativo, precedido por saturación del Profiler. La causa del cierre no está resuelta y no puede atribuirse al gameplay ni extrapolarse a Release.

No se modificaron scripts, escenas, mecánicas ni partidas. La verificación final compara 2.048 archivos de Assets/Packages/ProjectSettings y ocho archivos personales: cero diferencias. Unity serializó cuatro configuraciones al compilar; se restauraron exclusivamente bytes cuyo SHA256 coincide con la línea base. Las versiones serializadas se conservaron como evidencia en AuditDemoData/UnityAutomaticSerialization. Editor fuera de Play Mode, escena original restaurada y sin cambios pendientes de escena. No se instalaron paquetes.

## 2. Estado técnico actual

- Unity 6000.5.4f1, URP 17.5, Windows x64, backend Mono en la build diagnóstica.
- Escenas habilitadas: CodeboundTitle, SampleScene, DigitalKingdom y RetroDungeonExperiment. Laboratorios y validadores existen, pero no son escenas habilitadas de distribución.
- Combate y exploración siguen centralizados en DungeonPrototype y sus archivos parciales, con catálogos y adaptadores reutilizables. No se propone reescribir el controlador.
- Guardado: DungeonSaveStore, versiones 1/2, seis slots, cinco respaldos, snapshots por mapId; explored y visited separados. La ciudad utiliza el mismo estado, sin un guardado paralelo.
- Condiciones/acciones, perfiles de jefe y recompensas opcionales, PlayerProfileSettings y proveedor manual presentes. No hay generación procedural implementada y no se necesita para la demo.
- Presentación: sprites en mundo 3D, reflejos planos de suelo/techo, CRT con composición separada, iluminación/niebla por mapa y escenario de combate propio del Píxel.
- Calidad PC: renderScale 1, MSAA 1, HDR, depth/opaque textures, SSAO a resolución completa, sombras y SRP Batcher. Son posibilidades de costo, no prueba de que deban eliminarse.
- Recursos actuales contienen arte provisional, atlases y fixtures bajo Resources. Es necesario revisar qué se empaqueta y su procedencia antes de distribuir.
- Identidad del ejecutable aún DefaultCompany/dungeoncrawler, versión 0.1.0. Cambiarla exige preservar/migrar la ruta de partidas.

## 3. Mediciones de rendimiento

### Equipo y metodología

Ryzen 7 5800X (16 hilos lógicos), 32.694 MiB RAM, Radeon RX 7600 XT, capacidad declarada de VRAM 16.325 MiB, Direct3D 12, Windows 11 10.0.26300. La capacidad de VRAM **no es consumo medido**. No representa un equipo mínimo.

Editor: Game View fijo 1920×1080, VSync 0, targetFrameRate -1, Profiler CPU/GPU, sin Deep Profile. Ventanas de aproximadamente 12 segundos; ProfilerRecorder, CSV y metadatos conservados. Se excluye la primera muestra de cada contador por posible costo de invocación/compilación MCP. Los contadores son del proceso Editor, con su instrumentación y ventanas: no equivalen a rendimiento final. No se suman CPU y GPU, que trabajan solapados. La columna CPU es tiempo de frame reportado del hilo principal, no FPS de presentación.

| Escenario | CPU p50 / p95 ms | GPU p95 ms | GC asignado p50 KiB/frame |
|---|---:|---:|---:|
| Título | 1.04 / 1.40 | 1.39 | 35.2 |
| Refugio quieto | 21.46 / 28.09 | 5.71 | 159.8 |
| Panel de seis slots temporales | 31.34 / 56.21 | 5.94 | 2703.4 |
| Tres enemigos, fixture | 8.17 / 11.59 | 3.20 | 78.3 |
| Ventana JUMP y regreso | 8.01 / 11.37 | 3.46 | 78.3 |
| Ventana OVERCLOCK y regreso | 7.95 / 11.19 | 3.64 | 78.3 |
| Makong quieto, fixture | 7.94 / 11.13 | 3.18 | 78.2 |
| Digital Kingdom | 2.22 / 2.74 | 2.47 | 81.9 |
| Reino del Píxel, inicio | 11.25 / 14.90 | 3.95 | 96.8 |

### Interpretación y limitaciones

- Refugio: CPU p95 28,09 ms supera el presupuesto de 16,67 ms, pero es evidencia del Editor, no una condena del ejecutable. GPU p95 5,71 ms no sostiene por sí sola la hipótesis de cuello de GPU en este equipo.
- Slots: seis partidas temporales de aproximadamente 44.627 bytes cada una; 100 repeticiones de lectura conjunta promediaron 5,145 ms. DrawSaveSlots lee cada slot en OnGUI, que puede ejecutarse varias veces por frame. La mediana de GC sube de 159,8 a 2.703,4 KiB/frame. La comparación apoya priorizar esta ruta, aunque incluye interfaz y trabajo del Editor.
- Fixtures de combate: se añadió CombatLaboratory únicamente en memoria y se invocó StartLaboratoryCombat. Su panel quedó visible; las cifras incluyen ese panel. No son una medida limpia del combate normal. JUMP/OVERCLOCK comenzaron Active=true; las ventanas mezclan ejecución, fallo por falta de entrada y regreso. No certifican latencia rítmica ni jugadas perfectas. Makong mide reposo, no todo su repertorio.
- Píxel: durante esa ventana se ejecutó Capture/Restore del snapshot en memoria. No usar su máximo para atribuir un tirón a exploración normal.
- Las capturas Prologue y Refuge sin sufijo Solo compitieron con el primer Player y se excluyen de conclusiones. El prólogo necesita repetición limpia. No hubo recorrido representativo por pasillos ni captura limpia de enemigos visibles fuera de combate.
- El contador inicial Draw Calls Count no existía; no se reporta como cero. SRP Batcher Draw Calls Count mide ese subsistema, no todos los draws. UnityStats y contadores Editor mezclan cámaras; falta Frame Debugger de un frame de Player antes de fijar presupuesto de batches/triángulos.
- Un inventario runtime observó 3.801 renderers y 86 RenderTextures creadas, **incluyendo recursos del Editor, Profiler e historial de HACK**. No son todos recursos del frame ni prueba de fuga. RuntimeRenderTextures.txt conserva nombres, dimensiones y memoria reportada por objeto.
- Sin Memory Profiler instalado: no se certificaron fugas nativas ni retención después de una sesión larga. Memoria usada de Editor no equivale a RAM del Player; Gfx Used Memory no equivale a VRAM residente del controlador.

### Windows Development Build

Build exitosa: 0 errores, 2 avisos, 536.741.038 bytes (511,88 MiB), 161,67 s. Development, AllowDebugging, ConnectWithProfiler y DetailedBuildReport; salida temporal, sin cambiar las escenas habilitadas. El tamaño no representa un paquete comprimido Release.

Primer arranque llegó al título y se conectó. Su captura exportada no contiene frames válidos: no hay benchmark utilizable. Dos reintentos con Editor fuera de Play Mode terminaron en UnityPlayer con crash handler. Logs registran desbordamiento del límite de transporte Profiler (128 MiB) y avisos de RenderTexture.active; el segundo intento registró a disco y tampoco resuelve el problema. Sin símbolos nativos suficientes para asignar causa. Conservar PlayerSolo.log y PlayerRetry.log. **No se certifican FPS, memoria ni estabilidad de Release.** Repetir primero sin autoconexión/instrumentación; comparar luego API gráfica y driver en pruebas aisladas, sin asumir culpable.

Los dos avisos de compilación son CS0618 en DungeonWhiteTransition.cs:32: FindFirstObjectByType obsoleto. No son errores ni prueba de lag.

## 4. Problemas confirmados

Los identificadores siguientes se reutilizan en el backlog. Esfuerzo: horas orientativas de implementación más validación, no compromiso de calendario.

### D01 — P1 — Reiniciar pierde la presentación del Píxel

Sistema/archivo: DungeonPrototype.cs:110, RestartNewGame; RetroDungeonWallComposer. Evidencia: antes del reinicio composer=true; después composer=false, mismo mapId experiment.retro.luminous.v1 y misma escena. PixelRestart.json. El método destruye el objeto y copia únicamente inventario, puerta, mapa y regiones al nuevo controlador. Impacto real: el componente visual no sobrevive; otros perfiles no copiados son riesgo adicional. Solución: reconstruir desde la configuración de escena/perfil correcto, preservando el comportamiento original. Riesgo medio; 4–8 h. Validación: muerte/reintento y nueva partida en ambos mapas, audio/CRT/retorno correctos sin duplicados.

### D02 — P1 — Lectura y asignaciones del panel de slots

Sistema/archivo: DungeonPrototype.SaveSlots.cs, DrawSaveSlots. Evidencia cuantificada en sección 3 y SaveChecks.json. Impacto real: trabajo repetido y mayor GC; impacto en Player pendiente. Solución: cachear resúmenes al abrir o cambiar un slot y refrescar después de guardar, sin otro sistema de persistencia. Riesgo bajo/medio; 3–6 h. Validación: seis slots, sobrescritura, errores, actualizaciones y perfil comparativo con archivos pequeños/grandes.

### D03 — P1 — Respaldo válido no recupera principal dañado

Sistema/archivo: DungeonSaveStore.cs, Read/Write. Evidencia: principal temporal corrupto produjo ArgumentException pese a .bak1 válido. Las seis rutas fueron verificadas dentro de Temp antes de escribir. Impacto real: lectura bloqueada; no demuestra pérdida irrecuperable porque el respaldo existe. Solución: recuperación validada y comunicación al usuario; estudiar reemplazo atómico y conservar versión/compatibilidad. Riesgo alto; 8–16 h. Validación: JSON truncado, corte de escritura, versiones 1/2, seis slots, respaldos y snapshots. No ensayar fallos sobre archivos personales.

### D04 — P2 — Solapamiento de descripción y Siguiente en ciudad

Sistema/archivo: DungeonPrototype.DigitalKingdom.cs, DrawCityView. Evidencia: City720.png y City1080.png, diálogo inicial y descripción The Last Life. City1440.png muestra la ciudad sin diálogo. Impacto visible: texto difícil de leer cuando aparece Siguiente. Solución: reservar áreas distintas o no dibujar descripción durante el diálogo, conservando textos aprobados. Riesgo bajo; 2–4 h. Validación: todas las ubicaciones, diálogos largos y mochila en tres resoluciones.

### D05 — P2 — Asignaciones repetidas en selección de música

Sistema/archivo: DungeonAudio.EncounterMusic.cs, DungeonAudio.Playlists.cs y DungeonPrototype.Update. Evidencia estática: SetCombat/RequestMusic y TickPlaylist llaman Tracks, que construye listas. Impacto confirmado: creación repetida en la ruta; costo exclusivo no medido. Solución: cachear playlist validada y reaccionar a cambios reales de estado/configuración. Riesgo medio (transiciones/música); 3–6 h. Validación: exploración, HACK, jefe, ciudad, pause y cambios de canción sin superposición.

### D06 — P2 — Documentación técnica antigua contradice sistemas actuales

Sistema/archivo: Docs/AuditoriaGeneralYEstadisticas.md frente a AuditoriaDRPG.md y scripts vigentes. Evidencia: sistemas descritos como ausentes ya existen. Impacto real: planificación y diagnósticos erróneos. Solución: etiquetar documento histórico y mantener índice vigente sin alterar Biblia ni fuentes narrativas históricas. Riesgo bajo; 1–2 h. Validación: vínculos y checklist del contenido real.

### D07 — P2 — Identidad y ciclo de salida incompletos para publicación

Sistema/archivo: ProjectSettings/ProjectSettings.asset, CodeboundTitleScreen.cs y menús. Evidencia: DefaultCompany/dungeoncrawler, versión 0.1.0; no se encontró Application.Quit en producción. El usuario puede cerrar por Windows; no se ha demostrado fallo al cerrar. Impacto real de presentación; riesgo de progreso al cambiar ruta. Solución: definir identidad y migración, salida/retorno a título seguros. Riesgo medio; 4–8 h. Validación: instalación limpia y actualización con partidas antiguas, cierre en cada estado.

### D08 — P3 — API obsoleta en transición

Sistema/archivo: DungeonWhiteTransition.cs:32. Evidencia: dos warnings de build. Impacto actual: mantenimiento, sin costo probado. Solución: resolver referencia o FindAny según semántica. Riesgo bajo; 0,5–1 h. Validación: transición y readiness, objetos ausentes y cargas lentas.

## 5. Riesgos potenciales

### R01 — P1 — Cierre nativo de la build instrumentada

Archivo/evidencia: PlayerSolo.log, PlayerRetry.log y configuración Development/Profiler. Dos cierres observados; causa desconocida. Impacto real en la prueba, impacto en Release no establecido. Recomendación: aislar build sin instrumentación, conexión, DX12/DX11 y controlador, conservar dumps/símbolos. No confundir saturación del Profiler con una fuga del juego. Riesgo de corrección desconocido; diagnóstico 4–12 h. Validar diez arranques y recorrido completo en dos equipos. Es un bloqueo de certificación de publicación, no un P0 confirmado de gameplay Release.

### R02 — P1 — Escritura no atómica y validación limitada

Archivo: DungeonSaveStore.cs y restauración de PlayerProgressionRecord. File.Copy sobre destino, sin recuperación automática; Read valida versión y player, no todas las colecciones/referencias. Impacto estimado: guardado interrumpido o malformado puede fallar. No se produjo corte real de energía. Solución junto a D03; riesgo alto; 4–8 h adicionales según plataforma. Validar inyección de fallos exclusivamente en Temp y migraciones.

### R03 — P2 — Espera de transición sin salida de error

Archivo: DungeonWhiteTransition.cs. Espera readiness del jugador/ciudad sin límite ni ruta de recuperación. La transición normal probada sí terminó. Impacto potencial: blanco permanente con escena/configuración no preparada. Solución: timeout diagnóstico y recuperación segura, manteniendo blanco mientras carga normal. Riesgo medio; 3–6 h. Validar carga lenta, escena inválida y componente ausente en fixture independiente.

### R04 — P2 — Ciclo de vida de materiales y costo de presentación

Archivos: DungeonPrototype.cs, HelmetCRTWholeFrame.cs, HelmetCRTLayerMask.cs y reflejos. Materiales creados para superficies/neón no tienen destrucción explícita equivalente en OnDestroy; otras clases sí liberan buffers. No se ha demostrado fuga progresiva. La máscara busca renderers cada 0,25 s y recorre materiales; reflejos renderizan cámaras adicionales. Solución: primero contar recursos tras 20 ciclos y medir CPU/Frame Debugger; después corregir ownership o frecuencia donde se pruebe beneficio. Riesgo medio/alto; diagnóstico 4–8 h, implementación 4–12 h. Validar imagen/oclusiones/CRT/combate y memoria estabilizada.

### R05 — P2 — Contenido de prueba y procedencia de recursos

Archivos: Resources/DrpgPhase*Tests, assets de placeholder y audio. Resources puede incluir fixtures aunque sus escenas no estén habilitadas. BuildPackedTop.txt muestra audio de 7–11 MB por recurso y atlas provisional de 8 MB; no significa que todo esté simultáneamente en RAM. Derechos de redistribución/procedencia no verificados; no se concluye infracción. Solución: inventario de paquete y autorizaciones, separar recursos solo de Editor cuando se confirme que no tienen referencias runtime. Riesgo medio; 4–8 h. Validar build limpia y contenido completo.

## 6. Cinco optimizaciones de mayor beneficio probable

1. Cachear los seis resúmenes de guardado: mejor evidencia directa; no tocar formato.
2. Cachear playlists y evitar reasignarlas cada frame: bajo alcance y asignaciones visibles en código.
3. Medir/reducir trabajo repetido de máscara CRT y reflejos: beneficio probable en CPU/rendering; preservar composición y estética, no apagar efectos a ciegas.
4. Revisar presets URP/reflejos por calidad: SSAO, buffers HDR, sombras y resolución de reflejo; A/B con imagen y GPU de Player antes de decidir.
5. Revisar importación/carga de audio y texturas grandes, duplicados y fixtures empaquetados: beneficio probable en memoria/tamaño/carga, sin reemplazar arte ni bajar calidad sin evidencia.

AssetInventory.json registra 297 imágenes/PSD y 24 grupos duplicados de al menos 4 KiB. Dimensiones y estimaciones RGBA8 no son VRAM real; el importador y la compresión final deben medirse. No se cambiaron imports. Evitar optimizar miles de líneas o usar pooling masivo sin identificar marcadores dominantes.

## 7. Evaluación de estabilidad

| Prueba | Resultado y alcance |
|---|---|
| Compilación Windows | Correcta, 0 errores y 2 avisos |
| Nuevo juego desde título | Entró en SampleScene/prólogo |
| Prólogo | Arranque observado; recorrido completo limpio pendiente |
| Seis slots | Lectura/escritura solo Temp, lectura corrupta controlada |
| Snapshot Píxel | Capture/Restore conserva mapId y celda (5,1), visited=1, explored=4, entities=2 |
| Ciudad y regreso | Entró con blanco y terminó transition=false; regreso a SampleScene, misma celda (17,2) y mapId original |
| JUMP/OVERCLOCK | Iniciaron Active=true en fixture; sin certificar perfección, daño ni recompensas en esta auditoría |
| Tres enemigos/Makong | Encuentros creados en runtime; falta ruta real y repertorio completo |
| Reinicio Píxel | Regresión D01 confirmada |
| Antiguas partidas | No sobrescritas; batería de migración no reejecutada aquí |
| Sesión larga / 20 viajes / spam | Pendientes; no se certifica ausencia de fugas ni bloqueos |
| Release Windows | Pendiente; Development instrumentada no sustituye este gate |

Las fases anteriores tienen resultados históricos en Tests/DrpgPhase6Results, DrpgPhase7Results y SpeedrunCityResults. No contarlos como pruebas nuevas de esta auditoría. Errores de compilación/reflexión del propio harness MCP se distinguieron de errores de producción; no afirmar consola limpia global mientras existen esos mensajes diagnósticos. No se observó excepción gestionada del gameplay en las rutas controladas exitosas, pero no equivale a cobertura completa.

## 8. Evaluación de experiencia de usuario

**Técnico confirmado:** descripción/Siguiente se pisan en ciudad. Se inspeccionaron capturas a 1280×720 y 1920×1080; la de 2560×1440 se inspeccionó sin diálogo activo y no reproduce ese solapamiento; no certifica la variante con diálogo. El screenshot de combate contiene el panel del laboratorio añadido en memoria: su solapamiento no es un defecto del combate publicado.

**Propuestas de diseño, no bugs:** CRT y RGB/glitch fuertes dificultan lectura pequeña; ofrecer intensidad y apagado accesibles, preservando presets por escena. Verde/rojo de seguridad debería acompañarse siempre de palabras/símbolos. Tutorial breve de Bits, diferencia de nivel de SPEEDRUN y timing de cada HACK puede reducir ensayo/error. Evaluar tamaño mínimo de texto y volumen separado, reconfiguración de controles y navegación completa con mando; no se verificó todo con hardware de mando.

La ruta empieza con prólogo y refugio y dispone de minimapa/pausa, nodos y orientación por zonas. La claridad del objetivo, ritmo de encuentros, fatiga visual y duración requieren jugadores nuevos: no inferirlos de tests automatizados ni reescribir diálogos aprobados. Hacer pruebas con 3–5 personas y registrar dónde se atascan.

## 9. Estado de preparación de demo

Base disponible: inicio, prólogo, refugio, dungeon manual, encuentros, HACKS, Makong y ciudad. Reino del Píxel se presenta como contenido en desarrollo; su inclusión o exclusión requiere aprobación, no se decide aquí. Faltan cierre de recorrido/retorno a título verificable, higiene del paquete, migración/recuperación robusta y matriz real de pruebas Windows.

No se detectó necesidad de nuevas habilidades o sistemas para publicar. Hay música de título, exploración, combate, diálogo/jefe y ciudad; no afirmar que faltan por documentación antigua. Falta revisar mezcla/transiciones y cobertura de SFX en una vuelta jugada, especialmente supersalto. Texturas provisionales y referencias deben inventariarse antes de fijar contenido final.

**Propuesta pendiente de aprobación:** demo de 20–35 minutos: título → despertar/refugio → zona inicial → uno o dos encuentros que enseñen HACK → tramo hasta Makong → Digital Kingdom y cierre claro. Duración no cronometrada con jugadores. No implica cortar canon ni mapas.

**Objetivos propuestos, no alcanzados/certificados:** 60 FPS a 1080p (16,67 ms/frame) y mínimo provisional 30 FPS (33,33 ms) en equipo modesto. Exigir percentiles p95/p99 además del promedio. Cargas de escena ≤3 s en SSD como hipótesis inicial, ninguna transición vacía; definir umbral final después de medir. Sin crecimiento sostenido de memoria en 45–60 min.

**Hardware provisional para organizar pruebas, no requisitos comerciales:** Windows 10/11 x64, CPU de 4 núcleos, 8 GB RAM y GPU DX11 dedicada de 2–4 GB como candidato mínimo a 720p/30; 6 núcleos, 16 GB RAM y GPU de 6–8 GB como candidato recomendado a 1080p/60. No se probó ninguno: el único equipo medido tiene 32 GB y RX7600XT. No hay evidencia para prometer esas prestaciones ni elegir modelos concretos. Reservar al menos 2 GB de disco para instalar/actualizar la demo como margen provisional frente a la build Development de 512 MiB; registros/dumps de diagnóstico no se distribuyen. Confirmar mínimos únicamente con hardware real.

## 10. Backlog priorizado P0–P3

| Prioridad | Identificadores | Orden sugerido |
|---|---|---|
| P0 confirmado | Ninguno demostrado en Release | No confundir falta de certificación con bug comprobado |
| Gate previo a publicar | R01, ruta Windows completa | Build sin Profiler, reproducir/descartar crash y conservar evidencia |
| P1 | D01, D02, D03/R02 | Reinicio correcto, slots fluidos y progreso recuperable |
| P2 | D04, D05, D06, D07, R03, R04, R05 | UX, audio, documentación, identidad, transiciones, rendering y paquete |
| P3 | D08 y pulido validado con usuarios | Warnings y pequeños ajustes después de estabilidad |

Diez asuntos principales: R01, D01, D03/R02, D02, D04, R03, R04, D05, D07 y R05. Cada uno tiene evidencia/alcance, esfuerzo, riesgo y validación en las secciones anteriores. No sumar R02/D03 como dos pérdidas de datos distintas.

## 11. Plan de implementación en fases pequeñas

**Todo pendiente de autorización.** No equivale a reabrir las siete fases DRPG ya completadas.

1. Diagnóstico de build y seguridad de progreso (1–3 días): Release sin Profiler, crash, migraciones/backup, matriz Windows y checkpoints; no tocar arte.
2. Correcciones funcionales acotadas (2–3 días): D01 y recuperación/atomicidad, con fixtures aislados y pruebas de regresión por mapa.
3. Asignaciones demostradas (1–2 días): slots y playlists, A/B reproducible, mismo comportamiento.
4. Presentación medida (2–4 días): marcadores CRT/reflejos, ownership de materiales, presets e imports solo donde las mediciones lo justifiquen.
5. UX y empaquetado (1–2 días): D04, identidad/migración, salida, licencias/procedencia e inventario del paquete.
6. Candidato de demo (2–4 días): recorrido cronometrado, sesión larga, tres resoluciones, mando y dos equipos; regresión completa.

Estimación orientativa total: **9–18 jornadas**, según causa del cierre nativo, QA disponible y hardware. No es una promesa; se puede publicar antes si el alcance aprobado es menor y todos los gates pasan, nunca por omitir pruebas. Revisar estimación tras la primera fase.

## 12. Criterios verificables para publicar

- Release Windows arranca diez veces seguidas, sin crashes ni errores de producción; logs sin crecimiento descontrolado.
- Ruta aprobada completa de inicio a cierre, sin espacios vacíos, pérdida de input ni UI de combate encima de diálogos.
- Reinicio correcto en ambos mapas; retorno ciudad/dungeon conserva punto, progreso y presentación.
- Seis slots: guardar/cargar, respaldo recuperable, archivo truncado y versiones antiguas verificadas en fixtures; nunca usar partidas personales para pruebas destructivas.
- Cofres, puertas, nodos, eventos una vez y recompensas sin duplicación después de 20 cambios de mapa/guardados.
- JUMP, OVERCLOCK y SPEEDRUN: éxito/fallo/interrupción, costos exactos, turnos y victoria, sin duplicados; LEVEL UP CHEAT al máximo no consume.
- 1080p/60 y candidato modesto/30 comprobados en Player con percentiles, CPU/GPU y ruta representativa; no presentar datos Editor como cumplimiento.
- 45–60 min con ciclos de combate/mapas; memoria estabiliza tras calentamiento; investigar cualquier crecimiento por ciclo antes de fijar umbral.
- Textos/interacciones legibles en 720p/1080p/1440p y teclado/mando; CRT desactivable y avisos no dependientes únicamente del color.
- Paquete sin fixtures innecesarios, identidad/versionado definidos, procedencia de recursos revisada, contenido y duración aprobados.

### Evidencias y siguiente medición

AuditDemoData incluye Baseline/Integrity, CSV y metadatos, Summary.json, SaveChecks.json, PixelRestart.json, BuildResult/BuildPackedTop, inventario de assets y buffers, logs de Player y capturas. Los CSV preservan unidades originales: tiempos en ns, memoria/asignaciones en bytes. Los archivos raw voluminosos quedan fuera del repositorio; no son necesarios para reproducir percentiles a partir de CSV, pero falta una nueva captura de marcadores CPU para atribución fina.

Repetir sin herramientas durante las ventanas de muestreo; conectar Profiler antes del Player, grabar tramos delimitados y detener inmediatamente. En Release usar métricas de presentación externas compatibles, con la misma ruta y equipo. Para memoria usar snapshot inicial/caliente/final cuando se autorice la herramienta adecuada; para VRAM residente usar herramienta de GPU/controlador y diferenciar presupuesto, reservado y residente. No instalar paquetes ni cambiar producción hasta autorización.

**Auditoría cerrada. No se implementaron optimizaciones.**


# Actualización — Fase 1: estabilidad de Windows Release

Fecha: 16 de octubre de 2026. Checkpoint de código probado: `d770a89`, publicado en `origin/main` antes del diagnóstico. Incluye la UI de batalla del PSD y selección HACK en los botones del combate. Ese cambio fue autorizado antes de esta fase; no se atribuyen sus diferencias a optimización.

## Configuración y aislamiento

Build Windows x64 Mono, mismas cuatro escenas, arte, CRT, reflejos, luces, shaders y mecánicas. `BuildPlayerOptions.options = None`: sin Development, AllowDebugging, ConnectWithProfiler ni Deep Profile. Editor fuera de Play Mode, Profiler desactivado. El `boot.config` guardado como evidencia no contiene conexión ni espera a depurador. No se añadieron scripts, hooks o paquetes de diagnóstico al Player.

La única diferencia de configuración de producto en la copia de prueba es una identidad de almacenamiento aislada: compañía `CodeboundAuditIsolated20261016`, producto `CodeboundReleasePhase1`. Impide compartir ruta de guardados y preferencias con DefaultCompany/dungeoncrawler. No es una migración y no se copió ninguna partida personal. Se restauraron los bytes originales de ProjectSettings y tres assets URP después de compilar. Unity creó automáticamente un perfil Windows; se copió a Temp y se retiraron únicamente esos archivos nuevos para restaurar el árbol inicial. Resultado final: **2.079 archivos de producción y ocho archivos personales comprobados, sin diferencias**. No se cambió configuración permanente ni se desactivaron efectos.

Build temporal: `C:/Users/soyot/AppData/Local/Temp/CodeboundWindowsPhase1/Release/CodeboundRelease.exe`. Carpeta de datos temporal: `C:/Users/soyot/AppData/LocalLow/CodeboundAuditIsolated20261016/CodeboundReleasePhase1`. Estos archivos quedan locales; no son un paquete de distribución.

## Resultado de compilación

- Resultado: Succeeded; 0 errores, 2 avisos.
- Tamaño reportado: 469,607,492 bytes (447.85 MiB); duración 100.63 segundos.

Los dos avisos son CS0618 en DungeonWhiteTransition.cs:32. Se conservaron y no se corrigieron en esta fase. El menor tamaño frente a Development no se presenta como optimización del juego.

## Diez arranques consecutivos

Parámetros: 1920×1080, ventana, API por defecto Direct3D 12. No se pasó `-batchmode`, `-nographics`, flags de Profiler ni sustituciones de calidad. Se inició cada proceso con WindowStyle Hidden; **no hay confirmación visual del contenido de la ventana**. Cada intento se observó con 15 muestras espaciadas aproximadamente un segundo. Después se detuvo únicamente ese proceso temporal mediante Stop-Process. No interpretar esa detención programada como crash ni como cierre normal validado.

| Intento | Periodo completo sin salida espontánea | Crash handler en log | Profiler saturado | Máximo working set muestreado MiB |
|---|---|---|---|---:|
| 1 | Sí | No | No | 416.16 |
| 2 | Sí | No | No | 406.10 |
| 3 | Sí | No | No | 406.24 |
| 4 | Sí | No | No | 405.37 |
| 5 | Sí | No | No | 406.47 |
| 6 | Sí | No | No | 406.14 |
| 7 | Sí | No | No | 406.16 |
| 8 | Sí | No | No | 405.54 |
| 9 | Sí | No | No | 406.16 |
| 10 | Sí | No | No | 405.43 |

**Resultado acotado: 10/10 procesos completaron la observación sin salida espontánea y sin crash handler en sus logs.** Todos llegaron al marcador UnloadTime después de inicializar motor, gráficos e input. Esto no demuestra que el título se dibujara correctamente: no se inspeccionaron sus píxeles. MainWindowHandle no expuso una ventana principal visible y Responding no pudo evaluarse. No declarar diez partidas jugadas ni diez cierres normales.

Los diez logs incluyen `d3d12: failed to query info queue interface (0x80004002)`. No hubo cierre asociado en esta serie; por sí solo no identifica un fallo fatal. La línea D3D11 de Media Foundation corresponde a decodificación de vídeo y no implica que el render principal cambió de DX12 a DX11.

## Cobertura y mediciones

| Objetivo | Estado verificable |
|---|---|
| Release sin Profiler | BuildOptions.None y boot.config comprobados |
| Diez arranques | Completados como pruebas de vida del proceso y revisión de logs |
| Título | Arranque desde escena inicial; aspecto y controles no certificados |
| Nueva partida/carga/exploración/combate | No automatizados fiablemente en el ejecutable en esta fase |
| Cierre normal del usuario | Pendiente; Stop-Process no lo sustituye |
| Partidas personales | Ocho hashes idénticos; no copiadas, migradas ni sobrescritas |
| FPS/frame time/GPU/VRAM | No medidos en Release; no extrapolar desde Editor |
| Memoria del proceso | Working set muestreado, rango de máximos 405,37–416,16 MiB |
| CPU del proceso | 34,47–39,83 segundos acumulados de CPU por intento; varios núcleos, sin límite de FPS; no son ms/frame |
| Fugas/sesión larga | No evaluadas; procesos independientes de corta duración no demuestran ausencia de fuga |

La automatización de Windows mediante computer-use falló al inicializar el kernel: `CreateProcessSecurityEnvironment failed (HRESULT = 0x80070020)`. Se reinició y se repitió una vez, con el mismo resultado. No se usaron entradas a ciegas ni mecanismos alternativos de control para simular cobertura. Por eso la batería termina con las rutas interactivas pendientes; es una limitación explícita de herramientas, no un resultado positivo ficticio.

## Comparación con la auditoría y causa pendiente

La auditoría observó dos cierres nativos en Development con instrumentación y saturación del transporte Profiler. Esta serie Release no reproduce ese cierre durante arranque. **No identifica al Profiler como causa**: también difieren instrumentación, identidad de almacenamiento, duración, condiciones de ejecución y checkpoint de UI. El gameplay posterior al título no fue ejercitado. No se resuelve el stack nativo sin símbolos/dump y una reproducción controlada.

No se comparó DX11: los objetivos autorizan esa comparación únicamente si el diagnóstico la justifica, y no apareció un fallo Release que aislara la API como sospechosa. Los efectos originales permanecieron activos por configuración; falta verificación visual con ventana operativa.

## Recomendaciones concretas y cierre de fase

1. Recuperar el control de ventanas o efectuar una sesión manual observada sobre esta build aislada: título → nueva partida → prólogo → exploración → combate → cargar fixture → cerrar por Windows. Registrar el instante y resultado de cada acción. No usar guardados personales.
2. Repetir diez arranques **visibles con cierre normal**, registrar exit code y salida completa del motor. Mantener identidad temporal mientras se prueba.
3. Jugar 45–60 minutos con transiciones y cargas. Si aparece crash, conservar Player.log y el directorio de crash/dump antes de volver a iniciar; registrar escena/acción y API.
4. Solo entonces comparar DX11/DX12 con la misma ruta si hay indicios de fallo gráfico. Para atribuir costo, medir FPS de presentación del Player con una herramienta externa ya disponible o autorizada, separando medición de estabilidad de instrumentación pesada.
5. No aplicar correcciones de reinicio, persistencia ni optimizaciones hasta nueva autorización. Los hallazgos D01–D08/R01–R05 de la auditoría siguen abiertos salvo la evidencia acotada nueva de arranque Release.

Evidencias locales: `Docs/WindowsPhase1Data/ReleaseBuild.json`, `ReleaseBootConfig.txt`, `ExecutableSHA256.txt`, `TenStarts.json`, `Integrity.json` y diez `Release-Run-XX.log`. Incluyen resultados y condiciones originales; no se incluyó el ejecutable en Git. La build permanece en Temp para la prueba manual.

**Fase 1 cerrada con cobertura interactiva pendiente. No se certifica todavía la demo como estable ni publicable. No se implementaron correcciones u optimizaciones.**
