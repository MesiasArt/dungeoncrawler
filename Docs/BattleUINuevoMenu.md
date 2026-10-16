# UI de batalla — Nuevo Menu

16 de octubre de 2026.

Arte: `Assets/UI/Nuevo Menu.psd`, conservado sin modificaciones. Capas exportadas a `Assets/Resources/UI/BattleUI`. El margen vacío de Attack/Item se compensa con coordenadas UV; no se redibuja el arte fuente.

Edición: seleccionar `Assets/Resources/UI/BattleUI/Settings.asset`. Player/Enemy ajustan paneles, Command Offset/Size/Gap los botones, Message el panel inferior. Referencia 1280×720; escala uniforme con resolución. Use New Artwork permite volver a la presentación previa. Los botones iniciales son 220×78: dejan espacio al escenario. No se agregaron botones de depuración ni se activó el minimapa durante combate.

HACK reemplaza las opciones de la izquierda por el catálogo desbloqueado (JUMP/OVERCLOCK), SPEEDRUN y VOLVER. La selección ya no usa la pantalla de muñeca; los minijuegos existentes conservan su presentación. SPEEDRUN sigue siendo acción contextual, fuera del catálogo y sin minijuego; no cambian nivel, costo ni exclusiones. Se muestra deshabilitado con motivo cuando no procede. Su acceso separado de la derecha se retiró. Catálogos/listas de objetos mayores de cinco filas tienen navegación anterior/siguiente con ratón y mantienen teclado.

Validación Play Mode mediante fixture temporal: menú contiene JUMP/OVERCLOCK/SPEEDRUN/VOLVER; VOLVER retorna a Main; JUMP y OVERCLOCK iniciaron Active=true; grupo real de tres enemigos requirió selección y JUMP solo comenzó tras confirmar; SPEEDRUN no ejecutó sobre objetivo no elegible. No se afirma repetir aquí toda la batería histórica de daño/recompensas SPEEDRUN. Consola sin errores en ejecución limpia. Hubo NullReference en DungeonDoorZoneSigns durante recompilación en Play; no apareció al reiniciar Play y no se modificó ese sistema. No se guardaron partidas personales. Captura revisada a 2032×1018; pruebas adicionales de resolución quedan para QA de demo.

Archivos: BattleUISettings.cs, BattleUIArt.cs, DungeonPrototype.cs, DungeonPrototype.PartySwitch.cs, DungeonPrototype.Enemies.cs, DungeonPrototype.HackUnlocks.cs, CodeboundSystemMessages.cs y texturas/configuración de BattleUI. Respaldo: Backups/BattleUI/20261016-085535.

La Fase 1 de diagnóstico Windows se ejecuta después de este cambio; no se implementaron optimizaciones de rendimiento.


## Botones rectangulares y transición HACK

Se exportó la capa `Botton de Hacks` del PSD a `BattleUI/HackButton.png`, conservando el PSD. JUMP, OVERCLOCK, SPEEDRUN y VOLVER utilizan ese marco y texto centrado sin subtítulos ni subrayados.

Al pulsar HACK, los comandos salen por la izquierda; a mitad de transición se sustituye la lista fuera de pantalla y la nueva entra desde la izquierda. VOLVER hace lo mismo hacia el menú principal. La entrada se bloquea durante el deslizamiento; las animaciones usan tiempo sin escala y se cancelan si desaparece el combate o comienza un minijuego/diálogo.

Inspector de `Assets/Resources/UI/BattleUI/Settings.asset`: **Hack Button Size**, **Hack Button Font Size**, **Menu Slide Seconds** (inicial 0,28 segundos para el cambio completo) y **Menu Slide Padding**. No se alteraron costos, daño, desbloqueos ni minijuegos. Las posiciones previamente configuradas se conservan.

Pruebas Play Mode: siete comprobaciones de transición, bloqueo y retorno correctas (`Tests/BattleHackSlideResults.txt`), captura revisada de botones rectangulares e inicio de JUMP confirmado después de la transición. Sin errores de consola. Fixtures temporales sin escritura de partidas personales.
