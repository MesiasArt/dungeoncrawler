#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using System;using System.Reflection;using UnityEngine.InputSystem;using UnityEngine.InputSystem.LowLevel;
public class MovementValidation:MonoBehaviour {
 public void Profile(string label){StartCoroutine(Sample(label));}
 IEnumerator Sample(string label){int old=Application.targetFrameRate,vs=QualitySettings.vSyncCount;QualitySettings.vSyncCount=0;foreach(int cap in new[]{60,120}){Application.targetFrameRate=cap;yield return new WaitForSecondsRealtime(.5f);float sum=0,max=0;for(int i=0;i<120;i++){yield return null;float dt=Time.unscaledDeltaTime;sum+=dt;max=Mathf.Max(max,dt);}Directory.CreateDirectory("Tests");File.AppendAllText("Tests/MovementPerformance.txt",label+" cap="+cap+" fps="+(120/sum).ToString("F1")+" meanMs="+(sum/120*1000).ToString("F2")+" maxMs="+(max*1000).ToString("F2")+"\n");}Application.targetFrameRate=old;QualitySettings.vSyncCount=vs;Debug.Log("Movement profile complete: "+label);}
 public void Checks(){StartCoroutine(CheckMovement());}
 IEnumerator CheckMovement(){
  var d=GetComponent<DungeonPrototype>();var t=typeof(DungeonPrototype);var f=BindingFlags.NonPublic|BindingFlags.Instance;
  var original=d.CaptureDungeonSave();var configured=d.movementSettings;var settings=ScriptableObject.CreateInstance<DungeonMovementSettings>();d.movementSettings=settings;d.navigationTestInput=pressed=>0;
  int cap=Application.targetFrameRate,vs=QualitySettings.vSyncCount;float capture=Time.captureDeltaTime;QualitySettings.vSyncCount=0;d.enabled=false;
  var results="";
  try {
   foreach(int fps in new[]{60,120}){
    Application.targetFrameRate=fps;Time.captureDeltaTime=1f/fps;var data=JsonUtility.FromJson<DungeonSaveData>(JsonUtility.ToJson(original));data.enemies.Clear();data.refuge.refugeTragedyTriggered=false;data.introductionCompleted=true;
    var map=(string[])t.GetField("map",f).GetValue(d);Vector2Int start=Vector2Int.zero;int face=0;bool found=false;var dirs=new[]{Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left};
    for(int y=1;y<map.Length-1&&!found;y++)for(int x=1;x<map[0].Length-1&&!found;x++)for(int h=0;h<4&&!found;h++){
     var c=new Vector2Int(x,y);var region=DungeonRegionSettings.Current.At(c);bool valid=region!=null&&region.safeArea;for(int n=0;n<=4;n++){var at=c+dirs[h]*n;valid&=(bool)t.GetMethod("Open",f).Invoke(d,new object[]{at})&&!(bool)t.GetMethod("RefugeNpcBlocks",f).Invoke(d,new object[]{at});var m=DungeonMapSettings.Current;if(m.entities.objects.Exists(e=>e.cell==at))valid=false;}if(valid){start=c;face=h;found=true;}
    }
    data.cell=start;data.heading=face;d.RestoreDungeonSave(data);t.GetField("startMenuOpen",f).SetValue(d,false);yield return new WaitForSecondsRealtime(.6f);var camera=(Camera)t.GetField("view",f).GetValue(d);var move=t.GetMethod("Move",f);float before=Time.time;
    yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));float duration=Time.time-before;var position=(Vector2Int)t.GetField("position",f).GetValue(d);var world=(Vector3)t.GetMethod("World",f).Invoke(d,new object[]{position});bool aligned=(camera.transform.position-world).sqrMagnitude<.000001f;
    while((bool)t.GetField("enemyNavigationRunning",f).GetValue(d))yield return null;
    before=Time.time;yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{0,1}));float turn=Time.time-before;int heading=(int)t.GetField("heading",f).GetValue(d);bool rotation=Quaternion.Angle(camera.transform.rotation,Quaternion.Euler(0,heading*90,0))<.001f;
    while((bool)t.GetField("enemyNavigationRunning",f).GetValue(d))yield return null;
    t.GetMethod("WarpPlayer",f).Invoke(d,new object[]{start,face});d.navigationTestInput=pressed=>pressed?0:3;
    for(int n=0;n<200;n++){t.GetMethod("PollMovementInput",f).Invoke(d,null);yield return null;var at=(Vector2Int)t.GetField("position",f).GetValue(d);if((at-start).sqrMagnitude>=9)break;}
    d.navigationTestInput=pressed=>0;while((bool)t.GetField("moving",f).GetValue(d)||(bool)t.GetField("enemyNavigationRunning",f).GetValue(d))yield return null;
    var end=(Vector2Int)t.GetField("position",f).GetValue(d);world=(Vector3)t.GetMethod("World",f).Invoke(d,new object[]{end});results+="start="+start+" end="+end+" blocked="+t.GetMethod("MovementInterfaceBlocked",f).Invoke(d,null)+" ";bool continuous=(end-start).sqrMagnitude>=9&&end==start+dirs[face]*Mathf.RoundToInt(Vector2Int.Distance(start,end));
    bool finalAligned=(camera.transform.position-world).sqrMagnitude<.000001f;
    results+="cap="+fps+" step="+duration.ToString("F3")+" turn="+turn.ToString("F3")+" aligned="+aligned+" rotation="+rotation+" heldContinuous="+continuous+" finalAligned="+finalAligned+"\n";
    // Opposite turn queued during an active turn executes once and preserves heading.
    int initialHeading=(int)t.GetField("heading",f).GetValue(d);var active=d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{0,1}));
    t.GetField("pendingMovement",f).SetValue(d,1);t.GetField("pendingMovementUntil",f).SetValue(d,Time.unscaledTime+1f);yield return active;
    while((bool)t.GetField("moving",f).GetValue(d)||(bool)t.GetField("enemyNavigationRunning",f).GetValue(d))yield return null;
    results+="bufferOnce="+((int)t.GetField("heading",f).GetValue(d)==initialHeading)+"\n";
   }
  } finally {d.navigationTestInput=null;d.StopAllCoroutines();t.GetField("enemyNavigationRunning",f).SetValue(d,false);d.movementSettings=configured;d.RestoreDungeonSave(original);d.enabled=true;Application.targetFrameRate=cap;QualitySettings.vSyncCount=vs;Time.captureDeltaTime=capture;Destroy(settings);Directory.CreateDirectory("Tests");File.WriteAllText("Tests/MovementControls.txt",results);Debug.Log("Movement checks finished\n"+results);}
 }

 public void Constraints(){StartCoroutine(CheckConstraints());}
 IEnumerator CheckConstraints(){
  var d=GetComponent<DungeonPrototype>();var t=typeof(DungeonPrototype);var f=BindingFlags.Instance|BindingFlags.NonPublic;var snapshot=d.CaptureDungeonSave();var configured=d.movementSettings;var settings=ScriptableObject.CreateInstance<DungeonMovementSettings>();d.movementSettings=settings;d.enabled=false;d.navigationTestInput=p=>0;
  var results="";
  try {
   var data=JsonUtility.FromJson<DungeonSaveData>(JsonUtility.ToJson(snapshot));data.enemies.Clear();data.introductionCompleted=true;data.refuge.refugeTragedyTriggered=false;d.RestoreDungeonSave(data);t.GetField("startMenuOpen",f).SetValue(d,false);
   var move=t.GetMethod("Move",f);var warp=t.GetMethod("WarpPlayer",f);var map=DungeonMapSettings.Current;var dirs=new[]{Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left};Vector2Int wall=Vector2Int.zero;int face=0;bool found=false;
   for(int y=1;y<map.rows.Length-1&&!found;y++)for(int x=1;x<map.rows[0].Length-1&&!found;x++)for(int h=0;h<4&&!found;h++){var c=new Vector2Int(x,y);if(map.IsFloor(c)&&!map.IsFloor(c+dirs[h])){wall=c;face=h;found=true;}}
   warp.Invoke(d,new object[]{wall,face});yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));results+="wallBlocked="+((Vector2Int)t.GetField("position",f).GetValue(d)==wall)+"\n";
   var entity=map.entities.objects.Find(e=>e.cell==new Vector2Int(15,33)&&e.kind==DungeonEntityKind.Door);var doors=(System.Collections.Generic.Dictionary<string,DungeonDoor>)t.GetField("mapDoors",f).GetValue(d);var door=doors[entity.id];warp.Invoke(d,new object[]{entity.cell,entity.heading});door.SetClosedImmediately();yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));results+="closedDoorBlocked="+((Vector2Int)t.GetField("position",f).GetValue(d)==entity.cell)+"\n";
   door.SetOpenImmediately();yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));results+="openDoorPassed="+((Vector2Int)t.GetField("position",f).GetValue(d)==entity.cell+dirs[entity.heading])+"\n";while((bool)t.GetField("enemyNavigationRunning",f).GetValue(d))yield return null;
   var chest=map.entities.objects.Find(e=>e.kind==DungeonEntityKind.Chest&&e.chestKind==DungeonChestKind.Normal);for(int h=0;h<4;h++){var c=chest.cell-dirs[h];if(!map.IsFloor(c))continue;warp.Invoke(d,new object[]{c,h});yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));results+="chestBlocked="+((Vector2Int)t.GetField("position",f).GetValue(d)==c)+"\n";break;}
   var record=snapshot.enemies.Find(e=>!e.defeated);if(record!=null){data.enemies.Add(record);d.RestoreDungeonSave(data);t.GetField("startMenuOpen",f).SetValue(d,false);for(int h=0;h<4;h++){var c=record.cell-dirs[h];if(!map.IsFloor(c)||(bool)t.GetMethod("RefugeNpcBlocks",f).Invoke(d,new object[]{c}))continue;warp.Invoke(d,new object[]{c,h});yield return d.StartCoroutine((IEnumerator)move.Invoke(d,new object[]{1,0}));results+="enemyEncounter="+t.GetField("battle",f).GetValue(d)+" playerDidNotEnterEnemyCell="+((Vector2Int)t.GetField("position",f).GetValue(d)==c)+"\n";break;}}
  }finally{d.StopAllCoroutines();t.GetField("enemyNavigationRunning",f).SetValue(d,false);d.navigationTestInput=null;d.movementSettings=configured;d.RestoreDungeonSave(snapshot);d.enabled=true;Destroy(settings);File.WriteAllText("Tests/MovementConstraints.txt",results);Debug.Log("Movement constraints completed\n"+results);}
 }

 public void OriginalChecks(){StartCoroutine(CheckOriginalControls());}
 IEnumerator CheckOriginalControls(){
  var d=GetComponent<DungeonPrototype>();var t=typeof(DungeonPrototype);var f=BindingFlags.NonPublic|BindingFlags.Instance;var snapshot=d.CaptureDungeonSave();var prior=d.movementSettings;var config=ScriptableObject.CreateInstance<DungeonMovementSettings>();config.RestoreOriginal();d.movementSettings=config;
  var keyboard=InputSystem.AddDevice<Keyboard>();int cap=Application.targetFrameRate,vs=QualitySettings.vSyncCount;float capture=Time.captureDeltaTime;d.enabled=false;QualitySettings.vSyncCount=0;string results="";
  Action<Key?> tick=key=>{if(key.HasValue)InputSystem.QueueStateEvent(keyboard,key.Value==Key.None?new KeyboardState():new KeyboardState(key.Value));InputSystem.Update();keyboard.MakeCurrent();t.GetMethod("Update",f).Invoke(d,null);};
  try {
   var data=JsonUtility.FromJson<DungeonSaveData>(JsonUtility.ToJson(snapshot));data.enemies.Clear();data.introductionCompleted=true;data.refuge.refugeTragedyTriggered=false;var start=new Vector2Int(15,1);int face=1;
   var plan=DungeonMapSettings.Current;if(!plan.IsFloor(start)||!plan.IsFloor(start+Vector2Int.right))throw new Exception("Fixture de refugio inválida");
   foreach(int fps in new[]{60,120}){
    Application.targetFrameRate=fps;Time.captureDeltaTime=1f/fps;data.cell=start;data.heading=face;d.RestoreDungeonSave(data);t.GetField("startMenuOpen",f).SetValue(d,false);tick(Key.None);
    // Short press, then release: one exact step.
    tick(Key.W);yield return null;tick(Key.None);for(int i=0;i<fps;i++){yield return null;tick(null);}bool shortStep=(Vector2Int)t.GetField("position",f).GetValue(d)==start+Vector2Int.right;
    t.GetMethod("WarpPlayer",f).Invoke(d,new object[]{start,face});tick(Key.D);yield return null;tick(Key.None);for(int i=0;i<fps;i++){yield return null;tick(null);}bool shortTurn=(int)t.GetField("heading",f).GetValue(d)==2;
    t.GetMethod("WarpPlayer",f).Invoke(d,new object[]{start,face});tick(Key.W);for(int i=0;i<fps;i++){yield return null;tick(null);}tick(Key.None);for(int i=0;i<fps/2;i++){yield return null;tick(null);}bool heldStep=(Vector2Int)t.GetField("position",f).GetValue(d)==start+Vector2Int.right;
    t.GetMethod("WarpPlayer",f).Invoke(d,new object[]{start,face});tick(Key.D);for(int i=0;i<fps;i++){yield return null;tick(null);}tick(Key.None);for(int i=0;i<fps/2;i++){yield return null;tick(null);}bool heldTurn=(int)t.GetField("heading",f).GetValue(d)==2;
    t.GetMethod("WarpPlayer",f).Invoke(d,new object[]{start,face});tick(Key.W);yield return null;tick(Key.D);yield return null;tick(Key.S);yield return null;tick(Key.A);yield return null;tick(Key.None);for(int i=0;i<fps;i++){yield return null;tick(null);}bool rapid=(Vector2Int)t.GetField("position",f).GetValue(d)==start+Vector2Int.right&&(int)t.GetField("heading",f).GetValue(d)==face;
    var camera=(Camera)t.GetField("view",f).GetValue(d);var p=(Vector2Int)t.GetField("position",f).GetValue(d);var world=(Vector3)t.GetMethod("World",f).Invoke(d,new object[]{p});bool aligned=(camera.transform.position-world).sqrMagnitude<.000001f&&Quaternion.Angle(camera.transform.rotation,Quaternion.Euler(0,face*90,0))<.001f;
    bool pending=(int)t.GetField("pendingMovement",f).GetValue(d)==0;
    results+="clock="+fps+" shortStep="+shortStep+" shortTurn90="+shortTurn+" holdOnlyOneStep="+heldStep+" holdOnlyOneTurn="+heldTurn+" rapidInputsIgnoredWhileMoving="+rapid+" releaseNoPending="+pending+" exactAlignment="+aligned+"\n";
   }
  }finally{InputSystem.RemoveDevice(keyboard);d.StopAllCoroutines();d.movementSettings=prior;d.RestoreDungeonSave(snapshot);d.enabled=true;Application.targetFrameRate=cap;QualitySettings.vSyncCount=vs;Time.captureDeltaTime=capture;Destroy(config);Directory.CreateDirectory("Tests");File.WriteAllText("Tests/MovementOriginalRestored.txt",results);Debug.Log("Original controls verified\n"+results);}
 }

}
#endif
