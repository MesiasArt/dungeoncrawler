using System;using System.Collections.Generic;using UnityEngine;
[Serializable]public class DungeonDecorationPlacement {
 public string id=Guid.NewGuid().ToString("N"),prefabId;public Vector2Int cell;[Range(0,3)]public int heading;
 public Vector3 localOffset;[Range(.25f,1.5f)]public float scale=1;public float yawOffset;public bool enabled=true;
}
[Serializable]public class DecorationZoneRule {
 public string zoneId;public bool enabled=true;[Range(0,1)]public float density=.22f;public DecorationTheme theme;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Decoración/Configuración del mapa")]
public class DungeonDecorationSettings:ScriptableObject {
 public DungeonDecorationCatalog catalog;public DungeonRegionSettings regions;
 public bool enabled=true,automaticEnabled;public int seed=1987;[Range(0,1)]public float density=.20f;
 [Min(0)]public int maximumAutomatic=90;[Min(1)]public int minimumSpacing=3;
 [Tooltip("Favorece maquinaria en bordes de habitaciones amplias; no cambia apariciones de enemigos.")][Range(1,10)]public float roomAccentWeight=5;
 public List<DecorationZoneRule> zones=new List<DecorationZoneRule>();
 public List<DungeonDecorationPlacement> placements=new List<DungeonDecorationPlacement>();
 public List<string> suppressedAutomaticIds=new List<string>();
 [NonSerialized]public int revision;void OnValidate(){revision++;}
 public DungeonDecorationCatalog Catalog {get{return catalog!=null?catalog:DungeonDecorationCatalog.Current;}}
}
