# CRT por elemento

Abre **Editor → Efectos del casco CRT** y activa `Separate Layer Intensity`.

- `Background Intensity`: CRT del escenario.
- `Enemy Intensity`: CRT sobre las siluetas visibles de los enemigos 3D.
- `UI Intensity`: CRT de toda la interfaz IMGUI, incluidos textos y controles nativos.

Cero conserva la imagen limpia. Uno aplica el efecto completo configurado por
Scanlines, Artifacts y Chromatic Fringe. `Affect UI` desactivado equivale a UI=0.
El brillo y la curvatura son controles globales. Los cambios en el asset se
conservan al salir de Play.

La cámara del jugador renderiza el mundo en una textura temporal. IMGUI se dibuja
en otra textura transparente, incluyendo GUI.Label, GUI.Button, GUILayout, mapas,
retratos y menús. La UI se procesa sin el escenario y se compone al final con su
alfa real. No se utilizan rectángulos para adivinar dónde está la interfaz. Los
minijuegos dibujados por IMGUI pertenecen completos a esa superficie de UI.

Los enemigos 3D usan una máscara de siluetas con profundidad y alfa de sprite.
El fondo y los enemigos se mezclan con sus intensidades antes de añadir la UI.
Las reflexiones mantienen sus cámaras independientes. Al desactivar el efecto o
salir de Play se restaura la textura de destino original de la cámara y se
liberan los buffers temporales. No se cambian materiales ni capas de los objetos.

Pruebas: `Tests/RunHelmetCRTLayerTests.ps1`. Abre un proyecto temporal aislado
bajo `Temp/CRTSeparatedTest` y ejecuta Play/Stop reales. Comprueba UI=0 píxel a
píxel, UI=1, controles nativos, transparencias sin huecos, brillo uniforme,
máscara de enemigo fuera del centro y restauración de cámara al detener.
