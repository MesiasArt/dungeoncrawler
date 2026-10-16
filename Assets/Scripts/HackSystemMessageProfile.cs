using UnityEngine;
[CreateAssetMenu(menuName="Codebound/HACK System Message overrides")]
public sealed class HackSystemMessageProfile:ScriptableObject {
 [Tooltip("Vacío utiliza el mensaje genérico. {name} es el nombre del catálogo.")]
 [TextArea(2,4)]public string initialized,completed,successful,perfect,failed,interrupted;
 public string Result(HackExecutionOutcome outcome){switch(outcome){case HackExecutionOutcome.Success:return successful;case HackExecutionOutcome.Perfect:return perfect;case HackExecutionOutcome.Failed:return failed;case HackExecutionOutcome.Interrupted:return interrupted;default:return completed;}}
}
