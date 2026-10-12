using System;
using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Progresión de GamerBoy")]
public class PlayerLevelSettings : ScriptableObject {
 [Min(1)] public int maximumLevel=50;
 [Header("Crecimiento aditivo por nivel; bases en Personaje e Inventario")]
 [Min(0)] public float healthGrowth=2,attackGrowth=.45f,defenseGrowth=.30f,luckGrowth=.15f;
 [Header("Curva: round(coeficiente × nivel ^ exponente)")]
 [Min(1)] public float xpCoefficient=12;
 [Min(.01f)] public float xpExponent=1.6f;
 [Header("XP por enemigo")]
 [Min(0)] public float xpPerEnemyLevel=5,normalMultiplier=1,eliteMultiplier=1.5f,bossMultiplier=3;
 [Header("Diferencia enemigo menos jugador")]
 [Min(1)] public int higherBonusStartsAt=5,lowerModerateStartsAt=3,lowerSevereStartsAt=5,lowerZeroStartsAt=8;
 [Min(0)] public float higherFivePlus=1.2f,higherOneToFour=1.1f,equal=1,lowerOneToTwo=.7f,lowerThreeToFour=.35f,lowerFiveToSeven=.1f,lowerEightPlus=0;
 [Header("Recuperación por subida (0 = sin curación)")]
 [Range(0,1)] public float healMaximumFraction=0;
 [Min(0)] public int healFlatHP=0;
 public static PlayerLevelSettings Current { get { return Resources.Load<PlayerLevelSettings>("Combat/PlayerLevelSettings"); } }
 public int ClampLevel(int level){return Mathf.Clamp(level,1,Mathf.Max(1,maximumLevel));}
 public int Growth(float coefficient,int level){return Round((double)(Math.Max(0,(decimal)coefficient)*(ClampLevel(level)-1)));}
 static int Round(double value){return (int)Math.Min(int.MaxValue,Math.Max(0,Math.Floor(value+.5)));}
 public int RequiredXP(int level){return ClampLevel(level)>=maximumLevel?0:Math.Max(1,Round((double)(decimal)Math.Max(1,xpCoefficient)*Math.Pow(ClampLevel(level),(double)(decimal)Math.Max(.01f,xpExponent))));}
 public float DifferenceMultiplier(int enemyLevel,int playerLevel){int d=Math.Max(1,enemyLevel)-ClampLevel(playerLevel);return Mathf.Max(0,d>=Math.Max(1,higherBonusStartsAt)?higherFivePlus:d>=1?higherOneToFour:d==0?equal:-d<Math.Max(2,lowerModerateStartsAt)?lowerOneToTwo:-d<Math.Max(lowerModerateStartsAt+1,lowerSevereStartsAt)?lowerThreeToFour:-d<Math.Max(lowerSevereStartsAt+1,lowerZeroStartsAt)?lowerFiveToSeven:lowerEightPlus);}
 public int Reward(int enemyLevel,int playerLevel,PlacedEnemyCategory category){float mult=category==PlacedEnemyCategory.Boss?bossMultiplier:category==PlacedEnemyCategory.Elite?eliteMultiplier:normalMultiplier;return Round((double)((decimal)Math.Max(0,xpPerEnemyLevel)*Math.Max(1,enemyLevel)*(decimal)Math.Max(0,mult)*(decimal)DifferenceMultiplier(enemyLevel,playerLevel)));}
 public int AddXP(ref int level,ref long experience,long amount){level=ClampLevel(level);experience=Math.Max(0,experience);experience=amount>0?experience+Math.Min(amount,long.MaxValue-experience):experience;int before=level;while(level<maximumLevel){int required=RequiredXP(level);if(experience<required)break;experience-=required;level++;}if(level>=maximumLevel)experience=0;return level-before;}
}
