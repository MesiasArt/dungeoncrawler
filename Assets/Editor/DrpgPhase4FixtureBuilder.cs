using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// Independent assets only. Never edits the published dungeon or personal saves.
public static class DrpgPhase4FixtureBuilder {
 const string Folder="Assets/Resources/DrpgPhase4Tests";
 static T Asset<T>(string name) where T:ScriptableObject {var path=Folder+"/"+name+".asset";var asset=AssetDatabase.LoadAssetAtPath<T>(path);if(asset==null){asset=ScriptableObject.CreateInstance<T>();AssetDatabase.CreateAsset(asset,path);}return asset;}
 static DungeonEventAction Action(DungeonEventActionKind kind,string id,string target="",int quantity=1){return new DungeonEventAction{id=id,kind=kind,targetId=target,quantity=quantity};}
 static DungeonInteractionSettings Profile(string name){var p=Asset<DungeonInteractionSettings>(name);p.id="phase4."+name;p.once=true;p.continueLegacy=false;p.groups.Clear();p.actions.Clear();EditorUtility.SetDirty(p);return p;}
 [MenuItem("Editor/DRPG/Fase 4/Preparar mapas independientes")]
 public static string Build(){
  if(!AssetDatabase.IsValidFolder(Folder))AssetDatabase.CreateFolder("Assets/Resources","DrpgPhase4Tests");
  var a=Asset<DungeonMapSettings>("FloorA");var b=Asset<DungeonMapSettings>("FloorB");
  Setup(a,"phase4.fixture.a",new[]{"#########","#.......#","#.......#","#.......#","#.......#","#.......#","#########"});
  Setup(b,"phase4.fixture.b",new[]{"#######","#.....#","#.....#","#.....#","#######"});
  a.eventIds=new[]{"phase4.story","phase4.nested.done"};b.eventIds=new[]{"phase4.remote"};
  string item=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings").items[0].id;
  var sw=Profile("SwitchGate");sw.actions.Add(Action(DungeonEventActionKind.SetSwitch,"switch","phase4.switch"));sw.actions.Add(Action(DungeonEventActionKind.OpenDoor,"open","phase4.door"));sw.actions.Add(Action(DungeonEventActionKind.Reward,"gift",item,2));sw.actions.Add(Action(DungeonEventActionKind.CompleteEvent,"story","phase4.story"));
  var shortcut=Profile("Shortcut");shortcut.actions.Add(Action(DungeonEventActionKind.UnlockConnection,"unlock","phase4.shortcut"));shortcut.actions.Add(Action(DungeonEventActionKind.UnlockConnection,"node","phase4.node"));
  var sequence=Profile("Sequence");sequence.actions.Add(Action(DungeonEventActionKind.Reward,"before",item));var line=Action(DungeonEventActionKind.Dialogue,"dialogue");line.dialogue=new[]{new WorldDialogueLine("TEST","Primero el diálogo; después la segunda recompensa.")};sequence.actions.Add(line);sequence.actions.Add(Action(DungeonEventActionKind.Reward,"after",item));
  var child=Profile("Child");var childLine=Action(DungeonEventActionKind.Dialogue,"line");childLine.dialogue=new[]{new WorldDialogueLine("TEST","Evento narrativo anidado.")};child.actions.Add(childLine);child.actions.Add(Action(DungeonEventActionKind.CompleteEvent,"complete","phase4.nested.done"));
  var parent=Profile("Parent");var nested=Action(DungeonEventActionKind.RunEvent,"child");nested.eventProfile=child;parent.actions.Add(nested);parent.actions.Add(Action(DungeonEventActionKind.Reward,"gift",item));
  var npc=Profile("NpcAdapter");npc.continueLegacy=true;var intro=Action(DungeonEventActionKind.Dialogue,"intro");intro.dialogue=new[]{new WorldDialogueLine("TEST","Diálogo del perfil antes del NPC original.")};npc.actions.Add(intro);
  var chest=Profile("ChestAdapter");chest.continueLegacy=true;chest.groups.Add(new DungeonConditionGroup{conditions=new List<DungeonCondition>{new DungeonCondition{kind=DungeonConditionKind.SwitchActive,targetId="phase4.switch"}}});
  var remote=Profile("Remote");remote.actions.Add(Action(DungeonEventActionKind.SetSwitch,"remote","phase4.remote.switch"));remote.actions.Add(Action(DungeonEventActionKind.CompleteEvent,"complete","phase4.remote"));
  a.entities.objects=new List<DungeonMapEntity>{
   new DungeonMapEntity{id="phase4.switch",displayName="Interruptor de prueba",kind=DungeonEntityKind.Switch,cell=new Vector2Int(2,1),interaction=sw},
   new DungeonMapEntity{id="phase4.door",displayName="Puerta de prueba",kind=DungeonEntityKind.Door,cell=new Vector2Int(4,2)},
   new DungeonMapEntity{id="phase4.shortcut",displayName="Atajo bloqueado",kind=DungeonEntityKind.Door,doorKind=DungeonDoorKind.Locked,requiredKeyId="key.green",cell=new Vector2Int(4,4)},
   new DungeonMapEntity{id="phase4.chest",displayName="Cofre condicionado",kind=DungeonEntityKind.Chest,cell=new Vector2Int(5,1),contentId=item,quantity=3,interaction=chest},
   new DungeonMapEntity{id="phase4.npc",displayName="NPC de prueba",kind=DungeonEntityKind.NPC,cell=new Vector2Int(6,3),npcCatalog=RefugeSettings.Current,npcId=RefugeSettings.Current.npcs[0].id,dialogue=new[]{new WorldDialogueLine("NPC","Diálogo original conservado.")},interaction=npc},
   new DungeonMapEntity{id="phase4.node",displayName="Nodo de prueba",kind=DungeonEntityKind.Teleport,cell=new Vector2Int(7,5)},
   new DungeonMapEntity{id="phase4.connection",displayName="Piso B",kind=DungeonEntityKind.Door,doorKind=DungeonDoorKind.Transition,cell=new Vector2Int(7,1),destinationMap=b,destinationCell=new Vector2Int(1,1)},
   new DungeonMapEntity{id="phase4.sequence",displayName="Secuencia",kind=DungeonEntityKind.Savepoint,cell=new Vector2Int(1,5),interaction=sequence,save=false,heal=false},
   new DungeonMapEntity{id="phase4.parent",displayName="Evento anidado",kind=DungeonEntityKind.Savepoint,cell=new Vector2Int(3,5),interaction=parent,save=false,heal=false},
   new DungeonMapEntity{id="phase4.unlock",displayName="Desbloquear atajo",kind=DungeonEntityKind.Switch,cell=new Vector2Int(5,5),interaction=shortcut}
  };
  b.entities.objects=new List<DungeonMapEntity>{new DungeonMapEntity{id="phase4.remote.switch",displayName="Interruptor remoto",kind=DungeonEntityKind.Switch,cell=new Vector2Int(2,1),interaction=remote},new DungeonMapEntity{id="phase4.return",displayName="Piso A",kind=DungeonEntityKind.Door,doorKind=DungeonDoorKind.Transition,cell=new Vector2Int(5,3),destinationMap=a,destinationCell=new Vector2Int(1,1)}};
  foreach(var map in new[]{a,b}){EditorUtility.SetDirty(map);EditorUtility.SetDirty(map.entities);EditorUtility.SetDirty(map.regions);}
  AssetDatabase.SaveAssets();return "Fixtures A/B preparados sin modificar el mapa principal";
 }
 static void Setup(DungeonMapSettings map,string id,string[] rows){map.mapId=id;map.rows=rows;map.startCell=new Vector2Int(1,1);map.startHeading=1;map.includeLegacyObjects=false;map.entities=Asset<DungeonEntitySettings>(map.name+"Objects");map.regions=Asset<DungeonRegionSettings>(map.name+"Regions");map.regions.legacyPlacements=false;map.regions.initialRandomPerZone=0;var zone=new DungeonRegion{id=id+".safe",displayName="Fixture seguro",safeArea=true,spawnsEnabled=false};for(int y=0;y<rows.Length;y++)for(int x=0;x<rows[y].Length;x++)if(rows[y][x]=='.')zone.cells.Add(new Vector2Int(x,y));map.regions.regions=new[]{zone};map.regions.placedEnemies=new PlacedEnemyDefinition[0];}
}
