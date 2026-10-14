$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$source = Get-Content (Join-Path $projectRoot 'Assets/Scripts/DungeonMapPersistence.cs') -Raw
$start = $source.IndexOf(' public static DungeonMapSettings Resolve(')
$end = $source.IndexOf(' public static string RequireId', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Resolver source not found' }
$resolver = $source.Substring($start, $end-$start)
# Execute the production resolver with a controlled resource inventory, without starting Unity.
$harness = @'
using System;
using System.Collections.Generic;
public class DungeonMapSettings {
 public string mapId;
 public DungeonEntitySettings entities;
}
public class DungeonEntitySettings { public DungeonMapEntity[] objects; }
public class DungeonMapEntity { public DungeonMapSettings destinationMap; }
public static class Resources {
 public static DungeonMapSettings[] maps=new DungeonMapSettings[0];
 public static T[] LoadAll<T>(string path) { return (T[])(object)maps; }
}
public static class MapResolutionRegression {
__RESOLVER__
 static void Check(bool value,string label) { if(!value)throw new Exception(label); }
 public static string Run() {
  var lab=new DungeonMapSettings{mapId="map-9fbf29f9bfe637542adebbd7dbc0d76c"};
  Check(Resolve(lab.mapId)==null,"Scene-only map must reproduce the original failure");
  Check(Resolve(lab.mapId,lab)==lab,"Active scene-only laboratory map must resolve");
  Check(Resolve("missing",lab)==null,"Unknown IDs must not fall back to the active map");
  var other=new DungeonMapSettings{mapId="other"};
  lab.entities=new DungeonEntitySettings{objects=new[]{new DungeonMapEntity{destinationMap=other}}};
  other.entities=new DungeonEntitySettings{objects=new[]{new DungeonMapEntity{destinationMap=lab}}};
  Check(Resolve("other",lab)==other,"Linked maps and cyclic references must resolve");
  Resources.maps=new[]{lab};
  Check(Resolve(lab.mapId,lab)==lab,"Resource and active references to one asset are not duplicates");
  Check(Resolve("other")==other,"Existing resource-only callers must still resolve linked maps");
  Resources.maps=new[]{new DungeonMapSettings{mapId=lab.mapId}};
  bool duplicate=false;
  try { Resolve(lab.mapId,lab); } catch(InvalidOperationException) { duplicate=true; }
  Check(duplicate,"Different assets with one ID must remain rejected");
  return "PASS: 7 map-resolution regression checks";
 }
}
'@
Add-Type -TypeDefinition $harness.Replace('__RESOLVER__',$resolver)
[MapResolutionRegression]::Run()
$saveSource=Get-Content (Join-Path $projectRoot 'Assets/Scripts/DungeonPrototype.Save.cs') -Raw
if(!$saveSource.Contains('DungeonMapPersistence.Resolve(data.activeMapId,ActiveMap)')) {
 throw 'Save restoration must pass the active scene map'
}
Write-Output 'PASS: save restoration passes the active map'
