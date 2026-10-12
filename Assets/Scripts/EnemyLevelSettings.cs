using System;
using UnityEngine;
[Serializable] public class EnemyGrowth {
 [Min(0)] public float health=.12f, strength=.07f, defense=.06f, luck=.03f;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Progresion de enemigos")]
public sealed class EnemyLevelSettings:ScriptableObject {
 public EnemyGrowth growth=new EnemyGrowth();
 public static EnemyLevelSettings Current { get { return Resources.Load<EnemyLevelSettings>("Combat/EnemyLevelSettings"); } }
 public static int Scale(int value,float growth,int level) {
  double v=Math.Max(0,value)*(1d+(double)(decimal)Math.Max(0,growth)*(Math.Max(1,level)-1d));
  return (int)Math.Min(int.MaxValue,Math.Floor(v+.5d));
 }
 public EnemyStatSnapshot Evaluate(EnemyCharacterDefinition source,int level) {
  var g=source.overrideGrowth?source.growth:growth;
  return new EnemyStatSnapshot {speciesId=source.id,displayName=source.character,level=Math.Max(1,level),
   maximumHP=Math.Max(1,Scale(source.health,g.health,level)),damage=Scale(source.strength,g.strength,level),
   defense=Scale(source.defense,g.defense,level),luck=Scale(source.luck,g.luck,level),
   specialDamage=Scale(source.specialDamage,g.strength,level),specialHits=Math.Max(1,source.specialHits),
   specialHitInterval=source.specialHitInterval,normalMissPercent=source.normalMissPercent,
   specialMissPercent=source.specialMissPercent,criticalPercent=source.criticalPercent,criticalMultiplier=source.criticalMultiplier};
 }
}
[Serializable] public class EnemyStatSnapshot {
 public string speciesId,displayName;public int level=1,maximumHP,damage,defense,luck,specialDamage,specialHits;
 public float specialHitInterval,normalMissPercent,specialMissPercent,criticalPercent,criticalMultiplier;
}
[Serializable] public class EnemyInstanceRecord {
 public string instanceId,speciesId,zoneId,placementId;public PlacedEnemyCategory category;public int level,currentHP;public Vector2Int cell;
 public bool alerted,defeated;public float respawnRemaining;
 public EnemyStatSnapshot stats;
}
