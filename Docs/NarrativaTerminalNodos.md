# Terminal y nodos del refugio

Los diálogos del documento se encuentran en Editor → Zona 0 — Refugio como arrays editables. Se reutiliza WorldDialogue con efecto de escritura y avance, sin cinemáticas nuevas. El Save Point presenta sus siete líneas una vez; al completarlas cura y guarda incluyendo savePointIntroSeen. En usos siguientes muestra la confirmación breve.

El Centinela dispone de la conversación sobre nodos antes de sus conversaciones habituales. El nodo REFUGIO permanece desactivado al examinarlo. Tras sus seis líneas se ofrecen ACTIVAR NODO y DEJARLO APAGADO, con flechas/Enter/E y botones. Elegir dejarlo apagado no registra ni activa el nodo; se puede intentar más tarde.

La activación de Zona 2 siempre dispara silenciosamente la tragedia. Si GamerBoy activó REFUGIO, recibe cuatro líneas; si no lo hizo, el Devoto activa REFUGIO fuera de pantalla y se muestran ocho líneas sin revelar su autor. Se puede cerrar el menú de destinos con Escape y continuar. No se fuerza el regreso ni se añaden misiones.

Al regresar se muestran dos líneas de GamerBoy una vez. Al examinar el primer cuerpo se añaden otras dos antes de sus últimas palabras. El encuentro del Devoto usa doce o trece líneas según quién activó el nodo y empieza el combate al terminar. Sus barks de combate permanecen intactos.

RefugeProgress conserva: savePointIntroSeen, refugeTeleportActivatedByPlayer, refugeTeleportActivatedByDevotee, zone2TeleportActivated, refugeTragedyTriggered, refugeTragedyDiscovered, firstBodyDiscovered, sentinelTeleportSeen, devotoConfrontationSeen y devotoDefeated. La actividad real del nodo utiliza unlockedNodes. Las partidas antiguas con REFUGIO registrado se migran como activación del jugador, porque era la única activación implementada entonces.

Pruebas mediante Unity MCP en Play Mode: ambas elecciones, autor correcto de la activación, destino REFUGIO disponible, ambas variantes del Devoto comienzan combate, ninguna activación transporta automáticamente, reacciones de llegada no se repiten, JSON conserva estados, terminal no cura antes de terminar su introducción, la introducción se guarda y el siguiente uso es breve. Guardado/carga desde archivo aislado conserva la atribución. No se modificó el archivo de partida del usuario.

Las pociones de salida se entregan al terminar cada línea correspondiente: Archivista (pequeñas), Optimista (medianas), Devoto (grande). Cada entrega tiene un aviso individual y queda registrada para no duplicarse. El letrero se presenta en cuadro de texto pequeño, paginado, con escritura progresiva. Los comentarios de GamerBoy solo aparecen en la primera lectura; preservationSignRead se guarda en RefugeProgress.
