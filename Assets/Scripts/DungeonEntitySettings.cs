using System;
using System.Collections.Generic;
using UnityEngine;
public enum DungeonEntityKind { NPC, Chest, Door, Savepoint, Teleport, Switch }
public enum DungeonChestKind { Normal, Mimic, KeyItem }
public enum DungeonDoorKind { Normal, Locked, Transition }
[Serializable] public class DungeonMapEntity {
 public DungeonInteractionSettings interaction;public bool switchInitiallyActive;
 public string id=Guid.NewGuid().ToString("N"),displayName="Objeto";
 public DungeonEntityKind kind;public Vector2Int cell;[Range(0,3)]public int heading;
 public RefugeSettings npcCatalog;public string npcId;public WorldDialogueLine[] dialogue=new WorldDialogueLine[0];
 public DungeonChestKind chestKind;public string contentId;[Min(1)]public int quantity=1;public string mimicPlacementId,guardPlacementId;
 public bool cityEntrance;
 public bool doorInitiallyOpen;public DungeonDoorKind doorKind;public string requiredKeyId,destinationRegionId;public DungeonMapSettings destinationMap;public Vector2Int destinationCell;public DungeonDoorSettings doorSettings;
 public bool heal=true,save=true,initiallyActive,requiresActivation=true;
 public List<string> revealedRegionIds=new List<string>();
 public List<string> destinations=new List<string>();
 }
[Serializable]public class DungeonEntityState {public string id;public bool opened;}
[CreateAssetMenu(menuName="Virtual Dungeon/Objetos del plano")]
public class DungeonEntitySettings:ScriptableObject {
 public List<DungeonMapEntity> objects=new List<DungeonMapEntity>();
 public List<string> Validate(DungeonMapSettings map,DungeonRegionSettings regions){var errors=new List<string>();var ids=new HashSet<string>();var cells=new Dictionary<Vector2Int,DungeonMapEntity>();
  foreach(var e in objects){if(e==null)continue;if(string.IsNullOrEmpty(e.id)||!ids.Add(e.id))errors.Add("ID vacío o duplicado: "+e.displayName);
   if(!map.IsFloor(e.cell))errors.Add(e.displayName+": casilla fuera del suelo "+e.cell);
   if(cells.ContainsKey(e.cell))errors.Add("Objetos incompatibles en "+e.cell);else cells[e.cell]=e;
   if(regions!=null&&Array.Exists(regions.placedEnemies,p=>p!=null&&p.enabled&&p.cell==e.cell&&!(e.kind==DungeonEntityKind.Chest&&e.chestKind==DungeonChestKind.Mimic&&p.id==e.mimicPlacementId)))errors.Add(e.displayName+": casilla ocupada por un enemigo");
   if(e.kind==DungeonEntityKind.NPC&&(e.npcCatalog==null||Array.Find(e.npcCatalog.npcs,n=>n!=null&&n.id==e.npcId)==null))errors.Add(e.displayName+": NPC sin referencia válida");
   if(e.kind==DungeonEntityKind.Chest&&e.chestKind==DungeonChestKind.Mimic&&(regions==null||Array.Find(regions.placedEnemies,p=>p!=null&&p.id==e.mimicPlacementId&&p.trigger==EncounterTrigger.InteractiveEvent)==null))errors.Add(e.displayName+": falta encuentro Mimic");
   if(e.kind==DungeonEntityKind.Chest&&e.chestKind!=DungeonChestKind.Mimic&&!string.IsNullOrEmpty(e.contentId)){var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var character=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");if(!(inventory!=null&&Array.Exists(inventory.items,i=>i.id==e.contentId))&&!(character!=null&&Array.Exists(character.equipment,i=>i.id==e.contentId)))errors.Add(e.displayName+": contenido desconocido");}
   if(e.kind==DungeonEntityKind.Door&&e.doorKind==DungeonDoorKind.Locked&&string.IsNullOrEmpty(e.requiredKeyId))errors.Add(e.displayName+": falta llave");
   if(e.kind==DungeonEntityKind.Door&&e.doorKind==DungeonDoorKind.Transition&&!e.cityEntrance){var destination=e.destinationMap!=null?e.destinationMap:map;if(!destination.IsFloor(e.destinationCell))errors.Add(e.displayName+": destino inválido");}
   if(e.kind==DungeonEntityKind.Teleport)foreach(var id in e.revealedRegionIds)if(regions==null||!Array.Exists(regions.regions,r=>r!=null&&r.id==id))errors.Add(e.displayName+": zona de revelación inexistente "+id);
   foreach(var id in e.destinations)if(!objects.Exists(o=>o!=null&&o.kind==DungeonEntityKind.Teleport&&o.id==id)&&!(RefugeSettings.Current!=null&&Array.Exists(RefugeSettings.Current.nodes,n=>n!=null&&n.id==id)))errors.Add(e.displayName+": destino inexistente "+id);
  }return errors;
 }
}
