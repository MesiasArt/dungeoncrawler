using System;using System.Collections.Generic;using UnityEngine;using UnityEditor;
public static class DrpgPhase5FixtureBuilder {
 const string Folder="Assets/Resources/DrpgPhase5Tests";
 static T Asset<T>(string name) where T:ScriptableObject{var path=Folder+"/"+name+".asset";var a=AssetDatabase.LoadAssetAtPath<T>(path);if(a==null){a=ScriptableObject.CreateInstance<T>();AssetDatabase.CreateAsset(a,path);}return a;}
 [MenuItem("Editor/DRPG/Fase 5/Preparar mapas independientes")]
 public static string Build(){
  if(!AssetDatabase.IsValidFolder(Folder))AssetDatabase.CreateFolder("Assets/Resources","DrpgPhase5Tests");
  var a=Asset<DungeonMapSettings>("FloorA");var b=Asset<DungeonMapSettings>("FloorB");var zone=Asset<EnemyZoneSettings>("Enemies");EditorUtility.CopySerialized(EnemyZoneSettings.Current,zone);zone.maximumEnemies=20;zone.minimumLevel=zone.maximumLevel=1;
  foreach(var m in new[]{a,b}){m.mapId=m==a?"phase5.fixture.a":"phase5.fixture.b";m.rows=new[]{"#####################","#...................#","#...................#","#...................#","#.........#.........#","#.........#.........#","#.........#.........#","#...................#","#...................#","#####################"};m.startCell=new Vector2Int(1,1);m.startHeading=0;m.includeLegacyObjects=false;m.entities=Asset<DungeonEntitySettings>(m.name+"Objects");m.entities.objects.Clear();m.regions=Asset<DungeonRegionSettings>(m.name+"Regions");m.regions.legacyPlacements=false;m.regions.initialRandomPerZone=0;var region=new DungeonRegion{id=m.mapId+".wild",displayName="Prueba de exploracion",enemies=zone};var safe=new DungeonRegion{id=m.mapId+".safe",displayName="Prueba segura",safeArea=true,spawnsEnabled=false};for(int y=0;y<m.rows.Length;y++)for(int x=0;x<m.rows[y].Length;x++)if(m.rows[y][x]=='.')(y==8?safe:region).cells.Add(new Vector2Int(x,y));m.regions.regions=new[]{region,safe};m.regions.placedEnemies=new PlacedEnemyDefinition[0];}
  a.regions.placedEnemies=new[]{
   new PlacedEnemyDefinition{id="phase5.agent",speciesId="Tapaku",cell=new Vector2Int(3,3),randomLevel=false,countsTowardLimit=false,exploration=new EnemyExplorationSettings{profile=EnemyExplorationProfile.Patrol,persistence=EnemyExplorationPersistence.TimedRespawn,respawnSeconds=.1f,patrol=new[]{new Vector2Int(3,3),new Vector2Int(6,3)}}},
   new PlacedEnemyDefinition{id="phase5.reset",speciesId="Hongo",cell=new Vector2Int(13,3),randomLevel=false,countsTowardLimit=false,holdPosition=true,exploration=new EnemyExplorationSettings{profile=EnemyExplorationProfile.Static,persistence=EnemyExplorationPersistence.ResetOnReentry}},
   new PlacedEnemyDefinition{id="phase5.permanent",speciesId="Tapaku",cell=new Vector2Int(18,6),randomLevel=false,countsTowardLimit=false,holdPosition=true,exploration=new EnemyExplorationSettings{profile=EnemyExplorationProfile.Static,persistence=EnemyExplorationPersistence.PermanentDefeat}}
  };
  a.entities.objects.Add(new DungeonMapEntity{id="phase5.door",displayName="Puerta de prueba",kind=DungeonEntityKind.Door,cell=new Vector2Int(3,4),heading=0});
  a.entities.objects.Add(new DungeonMapEntity{id="phase5.to.b",displayName="Piso B",kind=DungeonEntityKind.Door,doorKind=DungeonDoorKind.Transition,cell=new Vector2Int(19,1),destinationMap=b,destinationCell=new Vector2Int(1,1)});
  b.entities.objects.Add(new DungeonMapEntity{id="phase5.to.a",displayName="Piso A",kind=DungeonEntityKind.Door,doorKind=DungeonDoorKind.Transition,cell=new Vector2Int(19,1),destinationMap=a,destinationCell=new Vector2Int(1,1)});
  foreach(var m in new[]{a,b}){EditorUtility.SetDirty(m);EditorUtility.SetDirty(m.entities);EditorUtility.SetDirty(m.regions);}EditorUtility.SetDirty(zone);AssetDatabase.SaveAssets();return "Phase 5 independent floors ready";
 }
}
