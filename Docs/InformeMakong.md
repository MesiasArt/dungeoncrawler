# Makong y laboratorio de combate

## Uso

Abre `Assets/Scenes/CombatLaboratory.unity` y pulsa Play. También está disponible en **Editor → Pruebas de combate**.

Selecciona un enemigo del catálogo, su nivel y el nivel de GamerBoy. Pon **Compañeros adicionales = 0** para una prueba individual; **1 o 2** añade Tapaku y Hongo. El botón Reiniciar repite la prueba y Elegir enemigo vuelve al selector. Makong tiene dos botones para probar directamente sus especiales.

Los parámetros guardados de la escena arrancan con Makong, nivel enemigo 1, GamerBoy nivel 8 y dos compañeros. No sustituye el mapa principal ni se añadió a la lista de escenas del build. En Editor usa una ruta de guardado de laboratorio separada.

## Animaciones y combate

Se exportaron 20 poses de `Assets/sprites/sub jefes/makong.psd`. El PSD original no se modificó. Los frames comparten canvas de 600 × 350, referencia de pies y escala de cuerpo, incluyendo los efectos extendidos de lengua, salto y muerte.

- Idle: 3 frames; ataque normal de lengua: 2; daño: 1; muerte: 2, compatibles con el parpadeo existente.
- Super salto: impulso, ascenso visible, salida por arriba, anillo creciente en el suelo, caída y dos poses de pisotón. El daño se aplica una sola vez al aterrizar mediante el cálculo de daño existente, con defensa, suerte y crítico.
- Devorar: preparación, tres poses de lengua, extensión hacia un compañero, secuencia inversa mientras lo arrastra y dos poses de ingestión/curación. El compañero sale inmediatamente del registro de turnos al ser consumido. La curación nunca supera el HP máximo escalado.
- Sin compañeros vivos elegibles, el especial utiliza el salto. No devora otros subjefes o jefes.
- Las bajas de compañeros consumidos usan el registro existente de XP y drops: se cobran al vencer el encuentro, una sola vez; huir no cobra XP.

Se conserva el controlador DungeonPrototype, EnemyCombatStats, EnemyLevelSettings, CombatMath, progresión, equipo y recompensas existentes. El recorrido de turnos usa una copia de la lista para soportar bajas causadas por un especial durante el turno enemigo. Si desaparece el objetivo seleccionado, se selecciona un superviviente.

Makong tiene ID estable `Makong`, categoría Subjefe y está reservado para encuentros manuales/eventos. No se cambió ningún peso de apariciones ni se colocó en mapas existentes. Los IDs anteriores conservan su posición; Mimic continúa excluido del spawn aleatorio.

## Editables

**Editor → Makong → Parámetros de especiales** abre `Assets/Resources/Combat/MakongSpecialSettings.asset`:

- Frecuencia: un especial cada 3 ataques.
- Salto: impulso 0.45 s, ascenso 0.25 s, espera arriba 0.8 s, caída 0.35 s, dos poses de impacto de 0.2 s; altura 8 unidades.
- Anillo: radio inicial 0.3 y final 2.
- Devorar: se prefiere cuando el HP es 65% o menos; recupera 30% del HP máximo más un valor fijo configurable.
- Frames de lengua 0.18 s; arrastre 0.65 s.

Daño, HP, defensa, suerte y demás estadísticas siguen en **Editor → Personajes - estadisticas y equipo → Subjefes → Makong**, usando el catálogo existente. Los valores provisionales iniciales no sustituyen ajustes posteriores del usuario. Tamaño: `EnemySizeSettings.makong`, el mismo recurso que usa la galería. El laboratorio tiene su propio Plano, Regiones y Enemigos en `Assets/CombatLaboratory`.

Para colocarlo en un mapa, usa el editor existente de entidades/encuentros, selecciona Makong en el catálogo y configura nivel y aparición única sin respawn. Esta fase prepara el enemigo y el laboratorio; no añade todavía una ubicación narrativa.

## Archivos principales

- `Assets/Scripts/MakongEnemyVisual.cs`: secuencias del PSD, orientación y tamaño.
- `Assets/Scripts/DungeonPrototype.Makong.cs`: salto, lengua, ingestión y curación.
- `Assets/Scripts/MakongSpecialSettings.cs`: parámetros editables.
- `Assets/Scripts/MakongEffect.cs` y `Assets/Resources/Shaders/MakongEffect.shader`: anillo y extensión temporal de lengua.
- `Assets/Scripts/CombatLaboratory.cs` y `DungeonPrototype.CombatLaboratory.cs`: selector y reinicio reutilizando combate.
- `Assets/Editor/MakongEditorIntegration.cs`: menú, catálogo, importación y creación del laboratorio.

## Verificación

Pruebas ejecutadas mediante Unity MCP en Play Mode, con registro en `Tests/MakongResults.txt`: sprites sin distorsión, categorías/IDs, exclusión aleatoria en 1000 tiradas, escalado por nivel, salto con marcador creciente, daño único y defensa, aterrizaje, lengua y arrastre, curación, baja/XP/drop único, limpieza de efectos, selección y turnos después de devorar, alternativa sin compañeros, sprites disponibles para JUMP/OVERCLOCK, dos frames de muerte y reinicio a otra especie.

`Tests/MakongLaboratorySpecies.txt` registra la creación correcta de las 9 especies visuales desde el selector. Las partidas personales no se usaron en estas pruebas. No se realizó un build independiente.

Resultado final: **47 comprobaciones aprobadas, 0 fallidas**, más creación correcta de las 9 especies desde el laboratorio. Consola final sin errores de compilación.

## Ajuste de idle, formación e impacto

Idle reexportado desde el PSD corregido: los tres frames conservan altura visible de 190 px y pies alineados en la fila 200. Makong ocupa el centro de la formación independientemente de su orden de creación; los compañeros alternan izquierda/derecha con espacio suficiente y la cámara encuadra la formación. El aterrizaje activa una sacudida fuerte de amplitud 0.38 durante 0.85 s; estos parámetros se editan en los especiales de Makong. El efecto restaura exactamente la cámara y no lo interrumpe un golpe menor.

## Lengua separada del PSD

La capa `Lengua separada` se exporta a `Assets/Resources/Enemies/Makong/tongue_sprite.png`. Su eje se preparó horizontalmente para utilizar **SpriteRenderer → Draw Mode: Sliced**, malla Full Rect y bordes de 40 px a izquierda y derecha. El cuerpo central se alarga; los extremos mantienen su escala y el grosor no varía con la longitud. El PSD original permanece intacto.

Se actualizó `swallow_tongue_2.png` desde la capa corregida `Special 2 tirando lengua 3`, sin lengua incorporada. Las primeras poses siguen usándose antes de extender; durante extensión y arrastre se mantiene la tercera pose y se anima la pieza separada, evitando una segunda lengua dibujada dentro del personaje.

`MakongTongueVisual` ajusta posición, orientación y longitud entre boca y compañero. Durante el regreso, el compañero se mueve junto al extremo. El sprite se oculta al entrar en la boca y se destruye al finalizar o reiniciar el encuentro. La curación y los registros de bajas, XP y drops se conservan.

En **Editor → Makong → Parámetros de especiales** se puede editar tiempo de extensión, grosor y desplazamiento de la boca. Los bordes Sliced y la escala por píxel están en el Inspector del sprite `tongue_sprite.png`.

Prueba específica: `Tests/MakongSlicedTongueResults.txt`, con extensión de 2 a 8 unidades, grosor constante y extremos protegidos. La suite de Makong también comprueba arrastre, ingestión, curación y eliminación del sprite al terminar.
