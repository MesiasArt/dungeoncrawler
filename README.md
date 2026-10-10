# Dungeon Crawler

Prototipo en Unity 6000.5.4f1 con URP. Abre el proyecto en Unity y pulsa Play en SampleScene. El prototipo se genera al iniciar la escena.

- W/S o flechas arriba/abajo: avanzar y retroceder por casillas.
- A/D o flechas izquierda/derecha: girar 90 grados.
- Acercarte a un enemigo inicia combate por turnos. Usa los botones para atacar, habilidad, poción o huir.
- Tres enemigos, 30 HP y tres pociones. Al derrotar todos completas el nivel.

Las paredes usan `Assets/Resources/Textures/textura azul luffy2.jpg`; el piso usa `textura rosa luffy2.jpg`. Las referencias visuales están en `References` y el PSD original en `ArtSource`.

Esta primera versión usa sentinelas geométricos provisionales y una interfaz básica. Faltan sprites definitivos, audio, balance, guardado y acabado visual. La habilidad hace 10 de daño sin coste por ahora; el ataque hace 7.

Git excluye los directorios temporales de Unity. Conserva y versiona los archivos `.meta` junto a sus recursos.
