# CODEBOUND — Diagnóstico del cierre nativo de Release

Fecha: 17/10/2026, America/La_Paz. Checkpoint recuperable previo: `a57947b5d26cbfab79fb71664a9dc751745d8f80`.

**Resultado: fallo reproducido y mejor localizado; no corregido. No se aplicaron cambios de producción. Fase 4 bloqueada.**

## Conclusión y grado de certeza

Una captura de la build original registra una excepción nativa de lectura `0xC0000005`, con dirección objetivo `0xE0`, durante el cierre solicitado con la X. La dirección de instrucción capturada corresponde a `RuntimeStatic<PlatformAccessibilityManager, 0>::StaticDestroy(void *)` en los símbolos locales de Unity 6000.5.4f1. Se verificaron GUID y edad del PDB contra el registro CodeView de **ese** UnityPlayer.dll: coinciden. No se obtuvo este nombre por analogía con un offset de otra versión.

Eso identifica el punto de fallo de esa ejecución, **no la causa raíz ni una corrección segura**. El gestor nativo de accesibilidad y el orden de destrucción de la ventana son hipótesis prioritarias. Otros controles producen una excepción en `PlayerMainWndProc` y salida `0xC000041D`; no se consideran automáticamente el mismo defecto interno. No hay evidencia suficiente para atribuirlo al CRT, GPU, driver, PhysX, Input System o gameplay.

Se realizaron 14 ejecuciones nuevas: una captura de la build original y 13 controles con cierre por `Application.Quit(0)`. Ninguna terminó correctamente. No se declaró solucionado el problema ni se preparó una build pública corregida.

## Protección y recuperación

- El checkpoint local conserva el informe y las evidencias anteriores; no se hizo push ni se volvió a descargar el repositorio.
- Proyecto original: `E:/UNITY PRUEBA/DCG/dungeoncrawler`.
- Laboratorio existente reutilizado para compilar: `E:/UNITY PRUEBA/DCG/Codebound-Phase3-Home-Lab-20261016`.
- Builds y pruebas nuevas: `E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017`.
- Identidad de todas las builds: `CodeboundPhase3HomeIsolated/CodeboundPhase3Home`. Solo se cargó un guardado sintético ya existente en esa identidad; no se cargaron ni escribieron partidas personales.
- `DemoReleaseShutdownData/Integrity.json`: **2093 archivos de producción y ocho archivos personales sin cambios de hash**.
- Se conservaron las builds previas, los logs y los minidumps. No se actualizaron Unity, paquetes, drivers ni permisos del sistema. No se cambió la configuración gráfica permanente.
- Los archivos nuevos de prueba se retiraron de Assets del laboratorio y quedaron archivados; `DemoPhase2Boot.cs` y su meta fueron restaurados desde las copias exactas previas. Los resultados y builds experimentales permanecen fuera del proyecto original.

## Revisión de código

| Área | Revisión y alcance de la evidencia |
|---|---|
| Callbacks de título/audio | `CodeboundTitleScreen.OnDestroy` llama a `DungeonAudio.SetTitleTrack(null)`. Este evita recrear el singleton cuando no existe, pero puede seleccionar música si todavía vive. Se revisó esa ruta y no apareció una excepción administrada que demostrara su responsabilidad. La escena neutra, sin DungeonAudio activo, también falla. |
| CRT | `HelmetCRTWholeFrame.OnDisable` detiene corutinas y libera buffers; OnDestroy repite liberación y destruye materiales/cámara. `Free` pone referencias a null, y las rutas separan RenderTexture.active/targetTexture. `HelmetCRTLayerMask` libera máscara y material. No se aplicó un cambio por sospecha. El título observado no contiene estos componentes y aun así falla. |
| Reflejos | `DungeonFloorReflection` crea cámara/RT y al destruir libera la RT y cámara. Mantener enlaces de material/cámara al liberar es un punto a revisar si aparece una reproducción específica, pero no explica por sí solo este cierre: el título no tiene reflejos y desactivarlos en la copia cargada tampoco evita el fallo. |
| Materiales del dungeon | Se revisaron destrucción de materiales propios, meshes, puertas y recursos del escenario. El fallo ocurre sin crear el dungeon. No se alteraron estas rutas. |
| Input System | Se revisaron acciones de introducción y OVERCLOCK; este último deshabilita, hace Dispose y pone acciones a null. En el título no se ejecutan esas acciones. El log informa apagado del módulo, pero su posición en el log no demuestra causalidad. |
| Eventos y persistentes | Se revisaron los registros de sceneLoaded, singletons de audio/transición y sus resets. Los callbacks del observador de laboratorio llegan a OnApplicationQuit, OnDisable y OnDestroy antes del fallo nativo. No se obtuvo evidencia de acceso administrado a un objeto destruido en esa secuencia. |
| Plugins | No se encontraron DLL/SO/bundles nativos propios dentro de Assets. La ejecución carga los componentes nativos de Unity, Mono, DirectStorage y el plugin Burst generado. También existen módulos de terceros del entorno; su presencia no demuestra culpa y no se desactivaron programas ni protecciones del sistema. |

La búsqueda de OnApplicationQuit de producción encontró además scripts de ejemplo del paquete CRT; no forman parte de los objetos activos de los controles del título. No existe una ruta de salida mediante Application.Quit en el menú actual: esa comparación se implementó **solo en la build de diagnóstico**, sin agregar un botón al producto.

## Captura nativa y símbolos

Herramientas ya instaladas: Python/ctypes, API de depuración de Windows, `dbghelp.dll`, MiniDumpWriteDump y llvm-symbolizer incluido con Unity WebGL. No se instaló software ni se accedió a un servidor de símbolos.

La captura se lanzó con un solo proceso Codebound y conservó excepciones de primera y segunda oportunidad, código de salida, módulos y dos minidumps locales. GUID del PDB y del binario, en bytes: `f21483d626d018419e5f44f4440a8075`; edad 1 en ambos. Ver `PdbMatch.json` y `MatchedFaultSymbols.txt`.

**Límite del capturador:** el StackWalk64 experimental devuelve una cadena con direcciones no plausibles y nombres de módulo aproximados. Esa cadena NO se utilizó para atribuir causalidad. Solo se usaron la dirección de excepción, sus parámetros y el símbolo resuelto posteriormente con el PDB verificado. Los datos originales se conservan para auditar esta limitación.

Los dumps generados por el propio Unity en dos controles contienen `0xC0000005` en `PlayerMainWndProc`; el proceso termina después con `0xC000041D`. Ver `UnityDumpExceptions.json`. El informe anterior contenía además el offset 0x100915, que el PDB asocia con `PlatformAccessibilityNodeRoot::get_BoundingRectangle`; ese dato histórico es contexto y no una pila nueva capturada.

## Experimentos controlados

Se comprobó la ausencia de otro proceso Codebound antes de cada lanzamiento automatizado. Las variantes son procesos independientes; ninguna modifica preferencias persistentes para desactivar efectos. Los controles automáticos no consultan las ventanas mediante computer-use. Esto no garantiza que ningún software del entorno consulte accesibilidad, pero descarta que una consulta explícita nueva de nuestra herramienta sea necesaria para esos fallos.

| Experimento | Repeticiones | Resultado |
|---|---:|---|
| Build original, título, X, DX12, depurador nativo | 1 | 0xC0000005, lectura de 0xE0; dump conservado |
| Título, Application.Quit, DX12, sin consultar ventana | 3 | 3 × 0xC0000005 |
| Título, Application.Quit, DX11, sin consultar ventana | 3 | 3 × 0xC0000005 |
| Escena neutra, Application.Quit, DX12 | 2 | 2 × 0xC000041D; dump nativo disponible en uno |
| ForceDisabled de estado de lector al iniciar, título | 1 | 0xC0000005 |
| ForceDisabled de estado de lector en OnApplicationQuit | 1 | 0xC000041D; dump conservado |
| Cargar fixture de dungeon y salir, DX12 | 1 | 0xC0000005 |
| Misma carga, deshabilitar HelmetCRTWholeFrame, salir | 1 | 0xC0000005 |
| Misma carga, deshabilitar DungeonFloorReflection, salir | 1 | 0xC0000005 |

Códigos decimales observados: `-1073741819` y `-1073740771`. No se observaron cierres con código 0 en esta batería.

La escena neutra conserva el proyecto y el pipeline, pero solo una cámara y un CodeboundTitleScreen deshabilitado que evita el bootstrap automático; no tiene música, arte del título, dungeon, componentes de CRT ni reflejos activos. No es todavía un proyecto mínimo independiente de Codebound y no elimina todos sus inicializadores estáticos.

Las variantes de accesibilidad usan `AssistiveSupport.screenReaderStatusOverride`, API documentada en el XML local de Unity 6000.5.4f1. Fuerzan el estado que ve la aplicación; **no equivalen a desinstalar ni desactivar todo el subsistema nativo de accesibilidad**. Ambas fallaron, por lo que no se promovieron como workaround.

Las desactivaciones de CRT/reflejos se hacen después de cargar. Prueban si detenerlos antes del cierre evita el problema; no demuestran que nunca se crearan recursos. El control del título, donde esos componentes están ausentes, aporta la evidencia complementaria.

## Builds y archivos modificados

Las tres builds diagnósticas son Release con BuildOptions.None, sin Profiler/autoconexión. Probe y Probe2 informan 0 errores y 6 advertencias; Neutral informa 0 errores y 0 advertencias. Hay avisos de APIs obsoletas en compilación de validadores; no se corrigieron durante este diagnóstico.

- Build original conservada: `E:/UNITY PRUEBA/DCG/Codebound-Phase3-Home-Lab-20261016/Builds/Codebound-Phase3-Home/Codebound.exe`.
- Observador inicial: `E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Builds/Probe/Codebound.exe`.
- Variantes: `E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Builds/Probe2/Codebound.exe`.
- Control neutro: `E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Builds/Neutral/Codebound.exe`.

**No hay build final corregida. Estas builds de diagnóstico no se deben distribuir como demo.**

En producción solo se añaden este informe, `Docs/DemoReleaseShutdownData` y referencias en los informes previos. El código de observación y el builder están conservados bajo `DemoReleaseShutdownData/ProbeSources`, fuera de Assets. Los minidumps y binarios se conservan localmente, no se añaden al repositorio; rutas, tamaños y hashes constan en `LocalArtifacts.json`.

## Estado de verificación y siguientes pasos

No existe corrección antes/después que validar: no se alteró código del juego. Las 95 regresiones anteriores y 12 comprobaciones Edit Mode siguen siendo evidencia histórica, no se presentaron como una nueva ejecución ni como prueba de estabilidad nativa. Título, carga y callbacks de salida se ejercitaron ahora; movimiento, combate y guardado interactivo no quedaron nuevamente verificados. No se certifica apariencia de variantes con efectos desactivados, estabilidad de sesión larga ni rendimiento Release.

La evidencia justifica centrar el siguiente diagnóstico en la vida del proveedor de accesibilidad/ventana de Unity y su interacción con el entorno. No justifica parchear el binario, forzar terminación del proceso, desactivar accesibilidad para usuarios, actualizar Unity o reescribir CRT. Un siguiente paso útil sería un proyecto mínimo independiente con la misma versión y símbolos, para un informe reproducible al proveedor; cualquier actualización o instalación requiere autorización según el alcance del usuario.

**Se detiene aquí el diagnóstico sin aplicar correcciones especulativas. Fase 4 y rediseño CRT no iniciados.**
