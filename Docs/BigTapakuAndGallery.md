# Big Tapaku y galeria de tamanos

Abre **Editor > Galeria de enemigos y guia en pies**. La escena se guarda en Assets/Scenes/EnemySizeGallery.unity.

Selecciona **Galeria de enemigos - controles de tamano**. Al final del Inspector estan los cuatro tamanos, guardados en EnemySizeSettings. Tambien se editan en **Editor > Tamanos de enemigos**. 1 es el tamano original; 2 lo duplica. Los controles afectan a los sprites del dungeon, combate y la galeria. El mapa conserva su factor de perspectiva de 0.75 y el combate usa 1.

La regla parte del suelo: un pie equivale a 0.3048 metros. Hay una marca por pie, incluidas 3, 4 y 5 pies. Maximum Guide Feet muestra hasta 100 pies. Camera Zoom aleja la camara y Spacing separa los personajes para comparar gigantes. Preview Pose permite revisar idle, ataque, golpe, muerte y especiales. La galeria evita que se genere el dungeon al entrar en Play Mode.

Big Tapaku usa los 12 cuadros del PSD con transparencia: idle de tres cuadros, ataque, golpe, muerte y seis cuadros de especial. El especial se reproduce cuadro a cuadro cada tres ataques por defecto. Valores provisionales: 24 HP, 4 de dano normal, especial de 3 golpes de 1 HP cada uno; configurables en Editor > Enemigos. Puede aparecer y reaparecer junto a los otros tipos. Usa temporalmente las frases de Tapaku cuando no tenga un perfil propio. En JUMP usa sus cuadros idle porque el PSD aun no tiene cuadros de caminar especificos.

El PSD original no se modifica; los cuadros exportados se guardan en Resources/Enemies/BigTapaku. Las escalas se aplican en vivo, sin cambiar los dibujos ni los archivos del PSD.
