using System;using System.Collections.Generic;using UnityEngine;
// Future providers can implement this contract without changing grid navigation.
public interface IMapProvider {DungeonMapDefinition CreateDefinition(DungeonMapSettings fallback=null);}
[Serializable]public sealed class DungeonMapOrigin {public string providerId="manual";public int algorithmVersion=1;public bool hasSeed;public int seed;}
public sealed class DungeonMapDefinition {
 readonly string[] rows;readonly DungeonMapOrigin origin;
 public string MapId{get;private set;}public DungeonMapSettings Source{get;private set;}public DungeonRegionSettings Regions{get;private set;}public DungeonEntitySettings Entities{get;private set;}
 public string[] Rows{get{return (string[])rows.Clone();}}public Vector2Int StartCell{get;private set;}public int StartHeading{get;private set;}
 public DungeonMapOrigin Origin{get{return new DungeonMapOrigin{providerId=origin.providerId,algorithmVersion=origin.algorithmVersion,hasSeed=origin.hasSeed,seed=origin.seed};}}
 // Manual snapshots continue using the exact mapId published by Phase 2.
 public string SnapshotKey{get{return origin.providerId=="manual"&&!origin.hasSeed?MapId:MapId+"::"+Uri.EscapeDataString(origin.providerId)+"/v"+origin.algorithmVersion+"/s"+(origin.hasSeed?origin.seed.ToString(System.Globalization.CultureInfo.InvariantCulture):"none");}}
 public DungeonMapDefinition(DungeonMapSettings source,DungeonMapOrigin metadata){
  if(source==null||!source.Valid)throw new ArgumentException("Definición de mapa inválida");if(string.IsNullOrWhiteSpace(source.mapId))throw new ArgumentException("El mapa necesita un mapId estable");if(metadata==null||string.IsNullOrWhiteSpace(metadata.providerId)||metadata.algorithmVersion<1)throw new ArgumentException("Origen/versionado inválido");
  Source=source;MapId=source.mapId;rows=(string[])source.rows.Clone();Regions=source.regions;Entities=source.entities;StartCell=source.startCell;StartHeading=source.startHeading;origin=new DungeonMapOrigin{providerId=metadata.providerId,algorithmVersion=metadata.algorithmVersion,hasSeed=metadata.hasSeed,seed=metadata.seed};
 }
}
