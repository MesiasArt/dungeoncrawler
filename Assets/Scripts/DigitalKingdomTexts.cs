using System;
using UnityEngine;
// City copy stays configurable alongside the existing city asset. There is no localization package in this project.
[Serializable] public sealed class DigitalKingdomTexts {
 public string back="Volver a la ciudad",next="SIGUIENTE",save="Guardar partida",restore="Curar y registrar retorno",viewItems="Ver objetos";
 public string hackUpgrades="Mejoras de HACKS - En desarrollo",specialAttacks="Ataques especiales - En desarrollo",minigameUpgrades="Mejoras de minijuegos - En desarrollo",anomalies="Anomalías digitales - En desarrollo",service="Servicio - En desarrollo";
 public string restored="Integridad restaurada. Punto de retorno registrado.",respawn="El sistema te recuerda. Continúa.",unavailable="Este servicio está en desarrollo.",emptyStock="Estamos preparando las existencias.";
 public string stock="{0} - En tu inventario: {1}. Las compras están en desarrollo.";
 public string controls="Flechas / Cruceta: elegir\nEnter / A: aceptar - Esc / B: volver";
}
