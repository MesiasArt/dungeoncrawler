using System;
using UnityEngine;
[Serializable]public class CombatDropEntry {public string itemId;[Range(0,100)]public float chancePercent;[Min(1)]public int quantity=1;}
[CreateAssetMenu(menuName="Virtual Dungeon/Recompensas de combate")]
public class CombatDropSettings:ScriptableObject {
 public bool enabled=true;[Min(0)]public int minimumCoinsPerEnemy=2,maximumCoinsPerEnemy=6;
 [Header("Monedas adicionales por fuerza del enemigo")]
 [Min(0)]public int coinsPerExtraLevel=2;
 [Min(1)]public int hpPerExtraCoin=10,attackPerExtraCoin=3,defensePerExtraCoin=2;
 public int StrengthCoinBonus(int level,int health,int attack,int defense){return Mathf.Max(0,level-1)*Mathf.Max(0,coinsPerExtraLevel)+Mathf.Max(0,health)/Mathf.Max(1,hpPerExtraCoin)+Mathf.Max(0,attack)/Mathf.Max(1,attackPerExtraCoin)+Mathf.Max(0,defense)/Mathf.Max(1,defensePerExtraCoin);}
 public CombatDropEntry[] potions=new CombatDropEntry[0];
 public static CombatDropSettings Current {get{return Resources.Load<CombatDropSettings>("Combat/CombatDropSettings");}}
}
