using System;using UnityEngine;
public enum DecorationMount { Wall, Floor, Ceiling, Room, Door }
public enum DecorationTheme { General, Safe, Corrupt }
[Serializable]public class DungeonDecorationDefinition {
 public string id,displayName;public GameObject prefab;public DecorationMount mount;public DecorationTheme theme;
 [Min(0)]public float weight=1;[Min(.05f)]public float depth=.25f;public bool cornerOnly;[Tooltip("Altura relativa al techo; el resto usa dimensiones estilizadas propias.")]public bool doorwayFrame;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Decoración/Catálogo")]
public class DungeonDecorationCatalog:ScriptableObject {
 public DungeonDecorationDefinition[] entries=new DungeonDecorationDefinition[0];
 public DungeonDecorationDefinition Find(string id){return Array.Find(entries,e=>e!=null&&e.id==id);}
 public static DungeonDecorationCatalog Current {get{return Resources.Load<DungeonDecorationCatalog>("Decorations/Catalog");}}
}
