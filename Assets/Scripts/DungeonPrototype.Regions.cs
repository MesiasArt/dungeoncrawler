using System;
using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 public DungeonRegionSettings dungeonRegions;
 public string[] MapRows {get{return (string[])map.Clone();}}
 DungeonRegion fallbackRegion;
 DungeonRegionSettings RegionMap {get{return dungeonRegions!=null?dungeonRegions:enemyZoneSettings!=null||ActiveMap!=null&&!ActiveMap.includeLegacyObjects?null:DungeonRegionSettings.Current;}}
 DungeonRegion RegionAt(Vector2Int cell){if(RegionMap!=null&&RegionMap.regions.Length>0)return RegionMap.At(cell);if(fallbackRegion==null)fallbackRegion=new DungeonRegion{id=Zone.zoneId,displayName="Zona 1",enemies=Zone};return fallbackRegion;}
 bool EnemyMayEnter(Vector2Int cell){if(RefugeNpcBlocks(cell))return false;var r=RegionAt(cell);return r==null||!r.safeArea;}
 sealed class EnemySpawnSlot {
  public DungeonRegion origin;public PlacedEnemyDefinition placement;public bool counts;public string instanceId;
  public bool CanRespawn {get{return placement==null||placement.CanRespawn;}}
 }
 readonly List<EnemySpawnSlot> slots=new List<EnemySpawnSlot>();
 readonly HashSet<string> triggeredEncounters=new HashSet<string>();
 int Population(string zoneId){int count=0;for(int i=0;i<enemyObjects.Count;i++)if(enemyObjects[i]!=null&&slots[i].counts&&slots[i].origin.id==zoneId)count++;return count;}
 static int ArrayIndex(string id){return id=="DevotoCorrompido"?6:Array.IndexOf(EnemyRoster.ResourceIds,id);}
 float RespawnDeadline(int index){return slots[index].CanRespawn?Time.unscaledTime+slots[index].origin.enemies.respawnSeconds:-1;}
 bool ReservedCell(Vector2Int cell){if(ActiveMap!=null&&ActiveMap.entities!=null&&ActiveMap.entities.objects.Exists(e=>e.cell==cell))return true;return RegionMap!=null&&Array.Exists(RegionMap.placedEnemies,e=>e!=null&&e.enabled&&e.cell==cell);}
 bool EligibleJoiner(int index,int primary){return slots[index].placement==null&&slots[index].origin.id==slots[primary].origin.id;}
 void SpawnPlacedEncounters(){if(RegionMap==null)return;foreach(var e in RegionMap.placedEnemies)if(e!=null&&e.enabled&&e.trigger==EncounterTrigger.OnDungeonStart&&e.speciesId!="Mimic")SpawnPlacement(e);}
 bool SpawnPlacement(PlacedEnemyDefinition e){int kind=ArrayIndex(e.speciesId);if(kind<0||triggeredEncounters.Contains(e.id))return false;int before=enemyObjects.Count;AddEnemy(e.cell,visualKind:kind,origin:RegionAt(e.cell),placement:e);if(enemyObjects.Count==before)return false;triggeredEncounters.Add(e.id);return true;}
 // Entry point for a future chest/event. No chest or reward UI is created here.
 public bool TriggerSpecialEncounter(string placementId){if(battle||busy||moving||IntroductionActive||lost||levelComplete||RegionMap==null)return false;var e=Array.Find(RegionMap.placedEnemies,x=>x!=null&&x.enabled&&x.id==placementId&&x.trigger==EncounterTrigger.InteractiveEvent);if(e==null||!SpawnPlacement(e))return false;BeginEncounter(enemyObjects.Count-1);return true;}
 void PopulateRegions(bool onlyMissing=false){if(RegionMap==null||RegionMap.regions.Length==0)return;foreach(var r in RegionMap.regions){if(r==null||!r.CanSpawn)continue;
  bool hasEnemy=false;for(int i=0;i<slots.Count;i++){var slot=slots[i];if(slot.origin.id!=r.id||slot.placement!=null&&slot.placement.trigger==EncounterTrigger.InteractiveEvent)continue;if(enemyObjects[i]!=null||slot.CanRespawn&&respawnAt[i]>=0){hasEnemy=true;break;}}
  // Event-only enemies (including defeated Mimics) never replace a region's population.
  if(onlyMissing&&hasEnemy)continue;int wanted=RegionMap.legacyPlacements?(hasEnemy?0:RegionMap.initialRandomPerZone):RegionMap.initialRandomPerZone;
  for(int i=0;i<wanted;i++){Vector2Int cell;if(!FindSpawn(out cell,false,r)||!SpawnRandom(cell,r))break;}
 }}
 public string[] CaptureTriggeredEncounterIds(){var ids=new string[triggeredEncounters.Count];triggeredEncounters.CopyTo(ids);return ids;}
}
