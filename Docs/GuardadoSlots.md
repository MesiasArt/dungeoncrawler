# Guardado por slots

Al iniciar Play aparece Nueva partida / Cargar partida. Selecciona Cargar partida, elige uno de los seis slots y pulsa Cargar. Una partida guardada después de la introducción la omite al cargarse.

Durante el dungeon pulsa Enter para abrir pausa: Guardar / Slots y Cargar / Slots. F5 abre el selector para guardar; F9 abre el selector para cargar, también desde la introducción. Cada slot tiene un nombre editable, vida, posición y fecha. Guardar en un slot ocupado requiere pulsar otra vez para confirmar el reemplazo.

Los Save Points guardan en el slot activo, seleccionado al guardar o cargar. Los archivos están en Application.persistentDataPath, Codebound-slot-1-v1.json hasta Codebound-slot-6-v1.json. El antiguo Codebound-save-v1.json sigue disponible como Slot 1 si aún no existe el archivo nuevo; no se elimina ni modifica durante la migración.

Guarda vida, equipo, inventario, posición/dirección, exploración, enemigos, respawn y estados narrativos/cofres/nodos. No se permite guardar durante combate, movimiento, diálogos ni apertura pendiente de un cofre mediante el selector. Si cambias el mapa y eliminas el suelo de la posición guardada, la carga avisa del destino inválido.

Probado con Unity MCP en Play Mode: menú inicial, dos slots con nombres/vida/estados narrativos diferentes, archivos independientes, slot vacío intacto, carga desde el selector restaura vida, cierra el inicio, omite introducción y reanuda el juego. Los tests utilizaron archivos de Docs, sin modificar las partidas del usuario.
