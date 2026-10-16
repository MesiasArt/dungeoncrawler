#if UNITY_EDITOR
using System;using System.Collections;using System.Collections.Generic;using System.Reflection;using System.IO;using UnityEngine;
public sealed class RefugeWelcomeValidation:MonoBehaviour {
 [Serializable]sealed class Expected {public string[] speakers,texts;}
 const BindingFlags F=BindingFlags.NonPublic|BindingFlags.Instance;DungeonPrototype d;DungeonSaveData original;bool originalMenu;string oldPath;readonly List<string> results=new List<string>();public bool Done{get;private set;}public string Summary{get;private set;}
 object Get(string n){return typeof(DungeonPrototype).GetField(n,F).GetValue(d);}void Set(string n,object v){typeof(DungeonPrototype).GetField(n,F).SetValue(d,v);}object Call(string n,params object[] args){return typeof(DungeonPrototype).GetMethod(n,F).Invoke(d,args);}void Check(bool ok,string text){results.Add((ok?"PASS ":"FAIL ")+text);}
 WorldDialogue Dialogue{get{return (WorldDialogue)Get("worldDialogue");}}void Restore(DungeonSaveData data){d.RestoreDungeonSave(data);Set("startMenuOpen",false);Set("saveSlotsOpen",false);Call("SetPause",false);}
 public void Run(){StartCoroutine(Checks());}
 IEnumerator Checks(){d=UnityEngine.Object.FindFirstObjectByType<DungeonPrototype>();original=d.CaptureDungeonSave();originalMenu=(bool)Get("startMenuOpen");oldPath=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath=Path.GetFullPath("Tests/RefugeWelcomeSave.json");Directory.CreateDirectory("Captures/RefugeWelcome");
 try{
  var clean=DungeonMapPersistence.Copy(original);clean.interactions.completedEvents.RemoveAll(id=>id.StartsWith("refuge.welcome"));clean.interactions.executions.RemoveAll(e=>e.id.StartsWith("refuge.welcome"));Restore(clean);Call("WarpPlayer",new Vector2Int(17,6),0);Check(!(bool)Call("UpdateRefugeWelcome")&&!Dialogue.Active,"Old save without new-game eligibility does not trigger welcome");
  Call("StartFreshFromMenu");Call("WarpPlayer",new Vector2Int(17,6),0);Check(!(bool)Call("UpdateRefugeWelcome"),"Welcome cannot trigger during prologue");((VirtualDungeon.IntroSequence)Get("introduction")).Skip();Set("introductionCompleted",true);Call("EndIntroPresentation",true);Call("ReleaseIntroductionInput");Call("WarpPlayer",new Vector2Int(17,5),0);Check(!(bool)Call("UpdateRefugeWelcome"),"Adjacent corridor cell does not trigger event");var before=d.CaptureDungeonSave();var camera=(Camera)Get("view");var heading=(int)Get("heading");var homeRotation=camera.transform.rotation;
  yield return (IEnumerator)Call("Move",1,0);yield return null;Check((Vector2Int)Get("position")==new Vector2Int(17,6),"Real grid movement reaches confirmed X17/Y6");Check(Dialogue.Active&&(bool)Get("welcomeRunning"),"First arrival automatically starts welcome");var expected=JsonUtility.FromJson<Expected>(File.ReadAllText("Tests/RefugeWelcomeExpected.json"));int n=0;bool turned=false;int guard=0;
  while(Dialogue.Active&&guard++<80){var line=Dialogue.Current;Check(n<expected.texts.Length&&line.speaker==expected.speakers[n]&&line.text==expected.texts[n],"Exact welcome dialogue "+(n+1));Check((bool)Call("MovementInterfaceBlocked"),"Navigation blocked during dialogue "+(n+1));
   if(n==0){Dialogue.CompleteLetters();yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Captures/RefugeWelcome/01-FirstMeeting.png");}
   if(line.presentationCue=="refuge.welcome.look-back"){
    float end=Time.realtimeSinceStartup+5;while((bool)Get("welcomeLooking")&&Time.realtimeSinceStartup<end){turned|=Quaternion.Angle(homeRotation,camera.transform.rotation)>165;Check((Vector2Int)Get("position")==new Vector2Int(17,6)&&(int)Get("heading")==heading,"Look-back preserves grid cell and logical heading");yield return null;}
    Check(turned&&!(bool)Get("welcomeLooking"),"Camera looks behind, holds, then returns");Check(Quaternion.Angle(camera.transform.rotation,homeRotation)<.1f,"Look-back returns camera to forward heading");
   }
   if(n==16)Check(line.pauseBefore>=1,"Residents' silence is a timed pause, not an added spoken line");
   Dialogue.CompleteLetters();Dialogue.Advance(1000);n++;yield return null;
  }
  Check(n==33&&!Dialogue.Active,"All 33 supplied lines finish once");var completed=d.CaptureDungeonSave();Check(completed.interactions.completedEvents.Contains("refuge.welcome"),"Completion stored in existing persistent event state");Check(!(bool)Get("welcomeRunning")&&!(bool)Get("welcomeLooking"),"Event releases its camera/input ownership");Check(Quaternion.Angle(camera.transform.rotation,homeRotation)<.1f,"Welcome restores original grid-facing view");Check(!Dialogue.Active&&!(bool)Call("UpdateRefugeWelcome"),"Standing on trigger cannot repeat completed event");
  bool unchanged=true;foreach(var npc in before.refuge.npcs){var after=completed.refuge.npcs.Find(v=>v.id==npc.id);unchanged&=after!=null&&after.talks==npc.talks;}Check(unchanged,"Individual NPC conversation counters unchanged");
  DungeonSaveStore.Write(completed,0,"Welcome fixture");Restore(DungeonSaveStore.Read(0));Call("WarpPlayer",new Vector2Int(17,6),0);Check(!(bool)Call("UpdateRefugeWelcome")&&!Dialogue.Active,"Save/load does not replay completed welcome");Check(d.CaptureDungeonSave().maps.Find(m=>m.mapId==completed.activeMapId).interactions.completedEvents.Contains("refuge.welcome"),"Per-map snapshot retains completion");
  Call("WarpPlayer",new Vector2Int(17,5),0);Call("WarpPlayer",new Vector2Int(17,6),0);Check(!(bool)Call("UpdateRefugeWelcome"),"Leaving and returning does not replay welcome");
  Restore(before);Call("WarpPlayer",new Vector2Int(17,6),0);Check((bool)Call("UpdateRefugeWelcome")&&Dialogue.Active,"New-game save from before first meeting remains eligible");Restore(completed);Check(!Dialogue.Active&&!(bool)Get("welcomeRunning")&&!(bool)Get("welcomeLooking"),"Load cancels pending welcome and callbacks cleanly");
  Summary=results.FindAll(v=>v.StartsWith("PASS ")).Count+" passed / "+results.FindAll(v=>v.StartsWith("FAIL ")).Count+" failed";
 }finally{DungeonSaveStore.editorTestPath=oldPath;File.WriteAllLines("Tests/RefugeWelcomeResults.txt",results);Restore(original);if(originalMenu)Call("ShowStartupMenu");Done=true;Debug.Log("Welcome: "+Summary);}
 }
}
#endif
