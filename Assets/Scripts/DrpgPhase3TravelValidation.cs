#if UNITY_EDITOR
using System;using System.Collections;using System.Reflection;using System.Collections.Generic;using System.IO;using UnityEngine;
public sealed class DrpgPhase3TravelValidation:MonoBehaviour {
 const BindingFlags F=BindingFlags.NonPublic|BindingFlags.Instance;DungeonPrototype d;List<string> results=new List<string>();
 void Awake(){DontDestroyOnLoad(gameObject);}public void Run(){StartCoroutine(Checks());}
 void Check(bool ok,string text){results.Add((ok?"PASS ":"FAIL ")+text);}
 IEnumerator Wait(DungeonMapSettings map){float end=Time.realtimeSinceStartup+20;while(Time.realtimeSinceStartup<end){var w=UnityEngine.Object.FindFirstObjectByType<DungeonPrototype>();if(w!=null&&w.mapSettings==map&&typeof(DungeonPrototype).GetField("view",F).GetValue(w)!=null){d=w;d.enabled=false;yield return null;yield break;}yield return null;}throw new Exception("Floor timeout");}
 void Travel(DungeonMapSettings map,Vector2Int cell){typeof(DungeonPrototype).GetMethod("TravelMapDoor",F).Invoke(d,new object[]{new DungeonMapEntity{destinationMap=map,destinationCell=cell,doorKind=DungeonDoorKind.Transition}});}
 IEnumerator Checks(){d=UnityEngine.Object.FindFirstObjectByType<DungeonPrototype>();var original=d.CaptureDungeonSave();var main=DungeonMapSettings.Current;var floor=Resources.Load<DungeonMapSettings>("DrpgPhase2Tests/FloorB");
  try{d.enabled=false;var data=d.CaptureDungeonSave();data.introductionCompleted=true;d.RestoreDungeonSave(data);Travel(floor,new Vector2Int(1,1));yield return Wait(floor);data=d.CaptureDungeonSave();data.explored.Add(new Vector2Int(3,1));data.explored.Add(new Vector2Int(3,3));data.entityStates.Find(s=>s.id=="test-phase2-chest").opened=true;data.entityStates.Find(s=>s.id=="test-phase2-door").opened=true;d.RestoreDungeonSave(data);Check(d.VisibleMapMarkers().Exists(m=>m.id=="test-phase2-chest"&&m.symbol==DungeonMapSymbol.ChestOpened),"Floor B displays its opened chest");Travel(main,original.cell);yield return Wait(main);Check(!d.VisibleMapMarkers().Exists(m=>m.id.StartsWith("test-phase2-")),"Returning A removes B symbols");Travel(floor,new Vector2Int(1,1));yield return Wait(floor);Check(d.VisibleMapMarkers().Exists(m=>m.id=="test-phase2-chest"&&m.symbol==DungeonMapSymbol.ChestOpened)&&d.VisibleMapMarkers().Exists(m=>m.id=="test-phase2-door"&&m.symbol==DungeonMapSymbol.DoorOpen),"Returning B rebuilds symbols from restored snapshot");}
  finally{File.WriteAllLines("Tests/DrpgPhase3TravelResults.txt",results);if(d!=null){d.RestoreDungeonSave(original);d.enabled=true;}}
 }
}
#endif
