# SYSTEM MESSAGE y puertas digitales

## Implementación
- Panel único en la posición y tamaño de DungeonUILayout.message: sustituye al aviso inferior anterior. Los avisos de HACK, acceso, objetos, recuperación, guardado, teleport, subida de nivel y victoria usan ese panel. El estado e instrucciones de combate aparecen en el mismo panel cuando no hay una notificación prioritaria. Los diálogos narrativos conservan su presentación.
- Nombre de HACK obtenido del catálogo; perfil opcional por entrada. Resultados reales de JUMP y OVERCLOCK, con finalización genérica para futuras implementaciones que no distingan resultados. Sin cambios de daño.
- Configuración general: Resources/UI/SystemMessageSettings. Perfil opcional: HackSystemMessageProfile. Una sola notificación visible y cola limitada.
- Puertas digitales con identidad cromática, holograma y desbloqueo cosmético; las normales sin llave usan azul brillante. Se conservan IDs, colisión y persistencia existentes. Prefabs en Assets/Prefabs/DigitalDoors.
- Escena independiente: Assets/Scenes/DigitalSystemsDemo.unity. Los guardados de demostración se aíslan en caché temporal del Editor.
- Iconos compartidos y nombres digitales de objetos conservando sus IDs y cantidades. Antivirus, Reboot y Emergency Backup quedan marcados no disponibles, sin distribución.

## Pruebas Unity MCP
- JUMP: resultados éxito, perfecto y fallo; notificaciones de inicio y resultado sin daño duplicado.
- OVERCLOCK: éxito con 20 pulsaciones y fallo por tiempo; mantener no genera pulsaciones; daño único, retorno y victoria con XP/recompensas. Interrupción libera el flujo. Nombre dinámico probado con catálogo temporal.
- Puertas: token incorrecto deniega, correcto abre; puerta normal sin token; puerta ROOT y estado abierto restaurado mediante JSON y escritura/lectura en ruta temporal aislada.
- Integración inferior revisada en Play Mode mediante captura Captures/DigitalSystems/bottom-panel.png: tres líneas y cabecera dentro del panel, sin el cuadro antiguo duplicado. Consola sin errores tras compilar.
- No se escribieron slots personales en estas pruebas. No se alteraron geometría ni fórmulas de combate.

## Límites
FULL RESTORE conserva los 100 HP del consumible anterior para no alterar balance; su nombre no implica una nueva curación porcentual. No se ha realizado una matriz exhaustiva de resoluciones/dispositivos. Los avisos técnicos heredados mantienen su texto donde no existe una plantilla específica; todos comparten el panel SYSTEM MESSAGE.

## Archivos principales
Assets/Scripts/CodeboundSystemMessages.cs, SystemMessageSettings.cs, DungeonPrototype.SystemMessages.cs, DigitalDoorVisual.cs, DigitalIcons.cs, HackSystemMessageProfile.cs; integración en DungeonPrototype, DungeonDoor, catálogo/editor de HACKS, CombatHack, EnemyJumpHack, OverclockHack e inventario. Respaldos en Backups/DigitalSystemMessages y Backups/SystemMessageBottomPanel.
