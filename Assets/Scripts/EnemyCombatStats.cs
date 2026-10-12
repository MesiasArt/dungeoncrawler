using UnityEngine;
public class EnemyCombatStats:MonoBehaviour {
 [Min(1)] public int maximumHP=10;
 [Min(0)] public int damage=3,defense,luck;
 public string instanceId,speciesId,zoneId,placementId;public PlacedEnemyCategory category;public int level=1;
 [System.NonSerialized] public bool experienceRecorded; // Per life: a genuine respawn creates a new component.
 public EnemyStatSnapshot snapshot;
 public string CombatLabel {get{return (snapshot!=null?snapshot.displayName:gameObject.name)+" — LV. "+level;}}
 public void Initialize(EnemyCharacterDefinition profile,int assignedLevel,EnemyLevelSettings settings) {
  snapshot=settings.Evaluate(profile,assignedLevel);instanceId=System.Guid.NewGuid().ToString("N");speciesId=snapshot.speciesId;level=snapshot.level;
  maximumHP=snapshot.maximumHP;damage=snapshot.damage;defense=snapshot.defense;luck=snapshot.luck;
 }
}
