using System;using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Mapas/Proveedor manual")]
public sealed class ManualMapProvider:ScriptableObject,IMapProvider {
 public DungeonMapSettings map;
 public DungeonMapDefinition CreateDefinition(DungeonMapSettings fallback=null){return Capture(map!=null?map:fallback);}
 public static DungeonMapDefinition Capture(DungeonMapSettings source){return new DungeonMapDefinition(source,new DungeonMapOrigin());}
}
