# Editables de zonas

Abre **Editor → Carpeta de zonas** para encontrar esta carpeta en Unity.
Abre **Editor → Zonas y encuentros** para dibujar regiones, ver conexiones y colocar enemigos.

- **DungeonRegions.asset**: casillas de cada región, nombre, ID estable, color del Editor, activación, área segura y colocaciones manuales/eventos.
- **00-Zona0/**: refugio seguro, NPC, terminal, nodos y evento opcional.
- **01-Zona1/Enemigos.asset**: configuración original: niveles 1–4 y sus cuatro especies/pesos conservados.
- **02-Zona2/Enemigos.asset**: copia independiente de los valores anteriores para editar Zona 2 sin cambiar Zona 1.
- **99-SinAsignar/**: configuraciones existentes que aún no están asignadas a una región; se conservan aquí.

Las carpetas nuevas se numeran según el nombre de la zona (Zona 3 → 03-Zona3). Los IDs y referencias de Unity se mantienen al mover assets desde Project.

## Dibujar y conectar

Selecciona una zona y pinta sus casillas con clic o arrastrando. Pintar una casilla la quita de las demás zonas, evitando solapamientos. Borrar asignación deja la casilla sin zona. La cuadrícula muestra paredes, coordenadas y colores de edición; no cambia el minimapa del jugador.

**Conecta por casillas con** enumera las regiones vecinas. La conexión depende de casillas transitables contiguas; pintar regiones no abre paredes ni cambia la geometría del dungeon. El diseño físico continúa en el mapa existente de DungeonPrototype.

El plano físico ahora se edita en **Editor → Plano físico del dungeon**, asset PlanoDungeon.asset. Allí # es pared y . es suelo. La cuadrícula del Inspector permite dibujar esas casillas. Después distribuye cada suelo en el editor de zonas.

Conexión actual: Zona 1 → primera puerta (9, 7.5) → pasillo seguro Zona 1.5, casillas (9,8) a (9,14) → segunda puerta (9,14.5) → Zona 2. El letrero está en (9,9). La copia anterior de regiones se conserva en Assets/ZoneBackups/AntesDelPasillo.asset.

El texto y respuestas se editan en 01.5-PasilloSeguro/Letrero.asset. Pulsar E cerca permite leer; E, SPACE o ENTER completan las letras y luego avanzan. La Zona 0 ya está configurada; abre Editor → Zona 0 — Refugio.

## Apariciones

En Enemigos.asset configura rango o niveles individuales, especies desde el desplegable, pesos, máximo activo y segundos de respawn. La probabilidad se calcula con los pesos elegibles. Para crear nuevas regiones, crea o clona su configuración propia.

Safe Area bloquea apariciones y entrada de enemigos: útil para Zona 0. Spawns Enabled desactiva apariciones sin convertir la región en refugio. Guarda fuera de Play Mode con Guardar zonas y encuentros.

## Enemigos manuales

Añadir enemigo manual crea un ID persistente. En Colocar enemigo selecciona la colocación y pulsa su casilla. Configura especie, nivel fijo o aleatorio, categoría, respawn, aparición única y si cuenta en el límite local. Los jefes no reaparecen aunque Respawn esté activado. Un nivel fijo puede superar el rango de la zona.

Interactive Event reserva el encuentro para una futura interacción. Mimic solo usa ese tipo; aún no se han creado cofres ni su interfaz.

Ambas puertas se cierran automáticamente tras cruzarlas en cualquier dirección. Se pueden volver a abrir con E; la zona segura también bloquea el paso de enemigos aunque estén abiertas.
