using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public sealed class DungeonMapSnapshot {
 public string mapId;public DungeonInteractionProgress interactions=new DungeonInteractionProgress();
 public Vector2Int cell;public int heading,directorRestRemaining;
 public List<EnemyInstanceRecord> enemies=new List<EnemyInstanceRecord>();
 public string[] triggeredEncounters=new string[0];
 public List<DungeonEntityState> entityStates=new List<DungeonEntityState>(),legacyDoors=new List<DungeonEntityState>();
 public List<Vector2Int> explored=new List<Vector2Int>(),visited=new List<Vector2Int>();
}
// Uses the existing save record and store. Snapshots contain only local floor state.
public static class DungeonMapPersistence {
 [Serializable] sealed class ListCopy<T>{public List<T> items;}
 public static List<T> Copy<T>(List<T> value){if(value==null)return new List<T>();return JsonUtility.FromJson<ListCopy<T>>(JsonUtility.ToJson(new ListCopy<T>{items=value})).items;}
 public static T Copy<T>(T value){return JsonUtility.FromJson<T>(JsonUtility.ToJson(value));}
 public static DungeonMapSettings Resolve(string id,DungeonMapSettings activeMap=null){if(string.IsNullOrEmpty(id))return null;DungeonMapSettings found=null;var seen=new HashSet<DungeonMapSettings>();var queue=new Queue<DungeonMapSettings>(Resources.LoadAll<DungeonMapSettings>(""));if(activeMap!=null)queue.Enqueue(activeMap);while(queue.Count>0){var map=queue.Dequeue();if(map==null||!seen.Add(map))continue;if(map.mapId==id){if(found!=null&&found!=map)throw new InvalidOperationException("mapId duplicado: "+id);found=map;}if(map.entities!=null)foreach(var entity in map.entities.objects)if(entity!=null&&entity.destinationMap!=null)queue.Enqueue(entity.destinationMap);}return found;}
 public static string RequireId(DungeonMapSettings map){if(map==null||string.IsNullOrEmpty(map.mapId))throw new InvalidOperationException("Mapa sin mapId estable");return map.mapId;}
 public static void Normalize(DungeonSaveData data,DungeonMapSettings fallback){
  if(data.interactions==null)data.interactions=new DungeonInteractionProgress();if(data.maps==null)data.maps=new List<DungeonMapSnapshot>();if(data.visited==null)data.visited=new List<Vector2Int>();if(data.explored==null)data.explored=new List<Vector2Int>();if(data.enemies==null)data.enemies=new List<EnemyInstanceRecord>();if(data.entityStates==null)data.entityStates=new List<DungeonEntityState>();if(data.legacyDoors==null)data.legacyDoors=new List<DungeonEntityState>();
  if(string.IsNullOrEmpty(data.activeMapId)){
   var original=fallback;
   if(data.city!=null&&!string.IsNullOrEmpty(data.city.returnMapId))original=Resolve(data.city.returnMapId)??original;
   else if(data.city!=null&&!string.IsNullOrEmpty(data.city.returnMap))foreach(var map in Resources.LoadAll<DungeonMapSettings>(""))if(map.name==data.city.returnMap){original=map;break;}
   data.activeMapId=RequireId(original);
   // Legacy discovery does not prove visits. Only the saved current cell is known.
   data.visited.Clear();if(original.IsFloor(data.cell))data.visited.Add(data.cell);
  }
  data.version=2;
  if(data.city!=null&&string.IsNullOrEmpty(data.city.returnMapId)&&!string.IsNullOrEmpty(data.city.returnMap))foreach(var map in Resources.LoadAll<DungeonMapSettings>(""))if(map.name==data.city.returnMap){data.city.returnMapId=map.mapId;break;}
 }
 public static DungeonMapSnapshot Snapshot(DungeonSaveData data){return Copy(new DungeonMapSnapshot{mapId=data.activeMapId,interactions=data.interactions,cell=data.cell,heading=data.heading,directorRestRemaining=data.directorRestRemaining,enemies=data.enemies,triggeredEncounters=data.triggeredEncounters,entityStates=data.entityStates,legacyDoors=data.legacyDoors,explored=data.explored,visited=data.visited});}
 public static void StoreCurrent(DungeonSaveData data){data.maps.RemoveAll(s=>s!=null&&s.mapId==data.activeMapId);data.maps.Add(Snapshot(data));}
 public static void Apply(DungeonSaveData data,DungeonMapSnapshot snapshot){var s=Copy(snapshot);data.interactions=s.interactions??new DungeonInteractionProgress();data.activeMapId=s.mapId;data.cell=s.cell;data.heading=s.heading;data.directorRestRemaining=s.directorRestRemaining;data.enemies=s.enemies;data.triggeredEncounters=s.triggeredEncounters;data.entityStates=s.entityStates;data.legacyDoors=s.legacyDoors;data.explored=s.explored;data.visited=s.visited;}
 public static DungeonSaveData ForTravel(DungeonSaveData source,DungeonMapSettings destination,Vector2Int cell,int heading,out bool firstVisit){
  var data=Copy(source);Normalize(data,DungeonMapSettings.Current);StoreCurrent(data);string id=RequireId(destination);var saved=data.maps.Find(s=>s!=null&&s.mapId==id);firstVisit=saved==null;Apply(data,saved??new DungeonMapSnapshot{mapId=id});data.cell=cell;data.heading=heading;return data;
 }
}
