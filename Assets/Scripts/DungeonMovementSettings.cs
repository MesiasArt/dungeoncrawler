using UnityEngine;
public enum DungeonMovementInterpolation { Linear, SmoothStep, EaseOut }
[CreateAssetMenu(menuName="Virtual Dungeon/Movimiento de exploración")]
public class DungeonMovementSettings:ScriptableObject {
 public bool experimental=true;
 [Min(.03f)]public float stepSeconds=.14f,turnSeconds=.12f;
 public DungeonMovementInterpolation stepInterpolation=DungeonMovementInterpolation.Linear,turnInterpolation=DungeonMovementInterpolation.SmoothStep;
 public bool continuousMovement=true,repeatTurns=true;
 [Min(0)]public float repeatInterval;
 [Range(0,.5f)]public float inputBufferSeconds=.22f;
 public bool overlapEnemyAnimation=true;
 public static DungeonMovementSettings Current {get{return Resources.Load<DungeonMovementSettings>("Exploration/MovementSettings");}}
 public static float Interpolate(float t,DungeonMovementInterpolation mode){t=Mathf.Clamp01(t);return mode==DungeonMovementInterpolation.SmoothStep?t*t*(3-2*t):mode==DungeonMovementInterpolation.EaseOut?1-(1-t)*(1-t):t;}
 [ContextMenu("Restaurar comportamiento original")]public void RestoreOriginal(){experimental=false;stepSeconds=turnSeconds=.18f;stepInterpolation=turnInterpolation=DungeonMovementInterpolation.Linear;continuousMovement=repeatTurns=overlapEnemyAnimation=false;repeatInterval=0;inputBufferSeconds=0;}
 [ContextMenu("Aplicar valores experimentales iniciales")]void Experimental(){experimental=true;stepSeconds=.14f;turnSeconds=.12f;stepInterpolation=DungeonMovementInterpolation.Linear;turnInterpolation=DungeonMovementInterpolation.SmoothStep;continuousMovement=repeatTurns=true;repeatInterval=0;inputBufferSeconds=.22f;overlapEnemyAnimation=true;}
}
