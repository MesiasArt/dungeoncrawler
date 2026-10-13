# Panel del personaje activo preparado

El HUD conserva su posición superior y los botones mantienen su tamaño y posición actuales. Solo hay un miembro real registrado: GamerBoy. No se añadieron personajes, estadísticas ni turnos.

L/R en teclado y los botones superiores izquierdo/derecho del mando solicitan el cambio. Las teclas y duración total de la animación están en **Editor → Interfaz y barras**, sección Future active-character panel. El panel completo (avatar y barras) sale por la izquierda; al quedar fuera, cambia de miembro y el siguiente entra por la izquierda. La transición usa tiempo independiente de frames y pausa al abrir el menú.

Con un único personaje no se ejecuta una animación vacía. Se bloquea el cambio durante acciones, minijuegos, selección de objetivo, menús y derrota. Los candidatos cuya función canAct devuelve falso se omiten. Al finalizar ATTACK o HACK se solicita el siguiente disponible. Los botones permanecen en su sitio.

Para los personajes futuros, el controlador de grupo podrá usar `DungeonPrototype.RegisterPartyPanel` con un Member que incluya ID estable, read (vista del HUD derivada de sus datos reales), canAct (acción disponible), selected (notificación de selección) y command (enrutamiento de órdenes al combate existente). No se guardan estadísticas paralelas en el presentador. El controlador de turnos de varios aliados y sus HACKS se integrará al añadir esos personajes; no se implementó una nueva lógica de turnos ahora.

Pruebas MCP: un solo miembro no cambia, IDs únicos, límite de cuatro, entrada repetida bloqueada, salida completa antes de seleccionar, entrada a posición original, callback una vez, candidatos sin acción omitidos en ambos sentidos y reinicio al miembro principal. Resultados en `Tests/PartyPanelSwitcherMcpResults.txt`.
