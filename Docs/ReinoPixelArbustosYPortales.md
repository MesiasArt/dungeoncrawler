# Reino del Píxel: PSD, arbustos y portales

## Resultado

Se extrajeron 46 imágenes del PSD actualizado. Se conserva el archivo fuente sin modificaciones. Ocho grupos de pared se dibujan en el orden del PSD: nubes lejanas, nubes, montañas lejanas, montañas medias, hojas grandes lejanas, montañas cercanas, hojas y arbustos. El suelo usa la capa verde con flores.

Los restos semitransparentes de fondo se eliminan al renderizar mediante un umbral alfa de 0,7. Los rectángulos UV usan los límites de la silueta visible, por lo que no quedan márgenes transparentes debajo de los arbustos. Es un recorte de presentación reversible, no una reescritura del PSD. El borde de tierra y césped usa un recorte interior para evitar las tapas redondeadas de la ilustración: las piezas se solapan ligeramente, también en esquinas. Los huecos conservan su profundidad y reciben una franja del mismo arte en el canto superior.

Se conservan la niebla, el CRT, la iluminación y los factores de profundidad que el usuario ajustó. Las capas lejanas tienen multiplicadores menores que las cercanas. Los cambios también se aplican al escenario de combate propio del reino; allí tampoco aparecen tuberías.

## Enemigos y decoración

Las tres tuberías interactivas se sustituyen por arbustos apoyados en el suelo. Se conservan sus IDs anteriores, las colocaciones y completedEvents, incluidos los identificadores que todavía contienen «pipe»: renombrarlos duplicaría encuentros en partidas existentes. El arbusto avisa con una sacudida; después de la advertencia, un desplazamiento del jugador puede disparar el salto. Si el destino está ocupado, fuera del mapa o la zona llegó a su límite, no se fuerza la aparición. Las apariciones visibles usan comprobaciones de ocupación y navegación, sin la distancia mínima del generador aleatorio que impedía salir cerca del jugador. El generador normal sigue funcionando.

El enemigo usa su sprite actual, salta desde detrás del follaje y aterriza en la casilla existente. No se cambian daño, recompensas, estadísticas ni turnos. La carga cancela cualquier sacudida pendiente y vuelve a preparar la advertencia cuando corresponda. Los eventos consumidos siguen consumidos.

Se añaden arbustos decorativos con distribución reproducible por semilla. Una parte tiene oscilación suave y otra permanece quieta. No tienen colisiones, enemigos ni registros de guardado. No toda vegetación en movimiento anuncia un enemigo: la oscilación ambiental es mucho más suave que la sacudida de advertencia.

## Salidas y aviso

Las salidas a Digital Kingdom se presentan como luz blanca, sin panel, candado ni interacción E. Cruzarlas hacia fuera inicia la transición blanca antes de intentar entrar en la pared límite. Se conserva la condición de derrota de Makong para la salida original, el punto de retorno y los snapshots por mapa.

El guía avisa una sola vez de que el reino está en desarrollo, que aún no hay objetivos ni recompensas nuevas y que se puede explorar. También indica que la luz de detrás permite regresar. El texto es editable en Story.asset → Destinations → entrada del Reino del Píxel → Entry Dialogue. Se registra con el sistema existente de eventos del mapa al terminar el diálogo. Las partidas anteriores sin ese aviso lo reciben una vez.

## Inspector

Seleccionar Assets/Resources/RetroDungeonExperiment/Settings.asset, o usar Codebound → Reino del Pixel → Editar atmosfera y profundidad.

- Ordered Layers: orden, texturas, densidades, alturas y multiplicador de parallax de cada grupo. Distribución y alturas se aplican al entrar en Play o reconstruir la composición.
- Layer Alpha Cutoff: limpieza de bordes, aplicada en vivo. Un valor demasiado alto puede borrar detalles finos.
- Texture Regions: recortes UV, sin modificar las imágenes originales.
- Floor Edge Height: altura de la franja de césped y tierra.
- Decorative Bush Frequency / Swaying Bush Fraction: cantidad de arbustos decorativos y proporción animada; requieren reconstrucción.
- Bush Tell Seconds / Bush Shake / Jump Seconds / Jump Height: aviso y salto de encuentros.
- Se conservan los controles existentes de profundidad, niebla A/B/C y CRT.

El menú Aplicar PSD actualizado y arbustos instala esta versión extraída; no vuelve a analizar automáticamente un PSD modificado después. El manifiesto de la extracción está en Tests/PixelLayerManifest.json.

## Archivos principales

RetroDungeonSettings.cs; RetroDungeonWallComposer.cs; RetroDungeonWallComposer.Layers.cs; RetroDungeonWallComposer.Battle.cs; RetroDungeonBush.cs; RetroFoliageSway.cs; DungeonCityPortal.cs; DungeonPrototype.CityPortal.cs; DungeonPrototype.MapEntities.cs; DungeonPrototype.RetroExperiment.cs; DungeonPrototype.EntryNotice.cs; DigitalKingdomStorySettings.cs; DungeonPrototype.Save.cs; RetroDungeonLayer.shader; PixelBushPortalsInstaller.cs; Settings.asset; Story.asset y UpdatedLayers.

DungeonPrototype.cs conserva su movimiento original: únicamente se adelanta la comprobación del cruce del portal respecto al rechazo de la pared límite. No se cambia el tiempo, repetición ni buffer de movimiento.

## Respaldo e integridad

Backups/PixelBushPortals/20261015-222453 contiene scripts, assets y el PSD anteriores al bloque; DungeonPrototype.Save.cs se respaldó antes de añadir el reinicio visual al cargar. Las partidas personales y CombatMath permanecen iguales a los hashes registrados. También permanecen iguales DungeonPrototype.Movement.cs y las escenas originales.

Durante la sesión el usuario editó el mapa retro añadiendo huecos y ajustó sus regiones. Esas modificaciones están conservadas: no se restauró la geometría del respaldo. Objects.asset y el PSD se mantienen idénticos al respaldo de este bloque. Tests/PixelBushPortalsIntegrity.txt detalla la comparación.

## Verificación

Ver Tests/PixelBushPortalsResults.txt para el resultado final. Las pruebas usan archivos dentro de Tests; no sobrescriben los seis slots personales. Comprueban capas y recortes, ausencia de tuberías, decoración móvil y quieta, aviso único, una casilla por acción, giros de 90 grados, advertencia y salto, ausencia de duplicados, guardado/carga, combate normal con el arte actualizado, portal transitable y snapshot conservado al volver a la ciudad.

Capturas: Captures/PixelBushPortals (pasillo, esquina, sala, aviso, salto, combate y portal). La unión elimina huecos geométricos del borde; el patrón de una textura no diseñada para repetición perfecta puede conservar una costura de color. El follaje es 2.5D: a poca distancia sigue siendo plano.

SPEEDRUN, LEVEL UP CHEAT y la mochila de Digital Kingdom quedan pendientes de confirmación; no se implementaron en este bloque.

Resultado final de Play Mode: 30 comprobaciones correctas, 0 fallos y 0 entradas de error durante la pasada. El encuentro del arbusto inicia el combate normal, usa el suelo actualizado y no genera tuberías en el fondo. Unity queda fuera de Play Mode.
