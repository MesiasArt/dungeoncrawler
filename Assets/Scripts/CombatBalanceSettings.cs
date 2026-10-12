using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Balance de combate")]
public sealed class CombatBalanceSettings:ScriptableObject {
 [Min(0)] public float defenseCoefficient=10;
 [Min(1)] public int minimumHitDamage=1;
 [Range(0,100)] public float maximumCriticalPercent=30;
 [Range(1,3)] public float maximumCriticalMultiplier=2;
 [Min(0)] public float criticalPerLuck=.5f,missReductionPerLuck=.25f,fleePerLuck=.5f;
 [Range(0,100)] public float maximumMissReduction=10,maximumFleePercent=85;
 [Range(.1f,5)] public float jumpMaximumQuality=2.5f;
 [Range(0,1)] public float jumpGoodWeight=.85f;
 static CombatBalanceSettings current;
 public static CombatBalanceSettings Current {get{if(current==null)current=Resources.Load<CombatBalanceSettings>("Combat/CombatBalanceSettings");if(current==null)current=CreateInstance<CombatBalanceSettings>();return current;}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void ResetBalance(){current=null;}
}
