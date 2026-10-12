using UnityEngine;
public enum EnemyRank { Enemigo, Fuerte, Subjefe, Jefe }
[System.Serializable] public class EnemyCharacterDefinition {
 public string id;
 public string character="NUEVO ENEMIGO";
 public bool overrideGrowth;public EnemyGrowth growth=new EnemyGrowth();
 public EnemyRank category;
 public bool reservedForEvents;
 [Min(1)] public int health=10;
 [Min(0)] public int strength=2,defense,luck;
 [Range(0,100)] public float normalMissPercent=10,specialMissPercent=25,criticalPercent;
 [Min(1)] public float criticalMultiplier=1.5f;
 [Min(0)] public int specialDamage=1;
 [Min(1)] public int specialHits=1;
 [Min(.03f)] public float specialHitInterval=.2f;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Estadisticas de enemigos")]
public class EnemyCharacterCatalog:ScriptableObject {
 public EnemyCharacterDefinition[] enemies=new EnemyCharacterDefinition[0];
 public EnemyCharacterDefinition FindId(string id){foreach(var e in enemies)if(e!=null&&e.id==id)return e;return null;}
 public EnemyCharacterDefinition Find(string character){foreach(var e in enemies)if(e!=null&&string.Equals(e.character,character,System.StringComparison.OrdinalIgnoreCase))return e;return null;}
 static EnemyCharacterCatalog cached;
 public static EnemyCharacterCatalog Current {get {if(cached==null)cached=Resources.Load<EnemyCharacterCatalog>("Combat/EnemyCharacterCatalog");return cached;}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void ResetCatalog(){cached=null;}
}
