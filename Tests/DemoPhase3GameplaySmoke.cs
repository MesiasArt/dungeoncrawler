#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class DemoPhase3GameplaySmoke {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static object Get(DungeonPrototype d,string name){return typeof(DungeonPrototype).GetField(name,F).GetValue(d);}
 static void Call(DungeonPrototype d,string name,params object[] args){typeof(DungeonPrototype).GetMethod(name,F).Invoke(d,args);}
 static IEnumerator Settled(DungeonPrototype d){float end=Time.realtimeSinceStartup+25;yield return new WaitForSecondsRealtime(1);while(d.LaboratoryBusy&&Time.realtimeSinceStartup<end)yield return null;if(d.LaboratoryBusy)throw new Exception("Combat did not settle within 25 seconds");}
 public static IEnumerator Run(Action<bool,string> check){
  yield return SceneManager.LoadSceneAsync("Assets/Scenes/SampleScene.unity");yield return null;yield return null;
  var d=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();float end=Time.realtimeSinceStartup+20;while(!d.LaboratoryReady&&Time.realtimeSinceStartup<end)yield return null;
  var state=d.CaptureDungeonSave();state.introductionCompleted=true;state.interactions.completedEvents.Add("phase3.fixture.completed");state.interactions.rewardReceipts.Add("phase3.fixture.reward");d.RestoreDungeonSave(state);
  yield return DemoPhase3MaskComparison.Run(d);
  var map=d.mapSettings??DungeonMapSettings.Current;var destination=Resources.Load<DungeonMapSettings>("DrpgPhase2Tests/FloorB");bool first;
  if(destination!=null){var travel=DungeonMapPersistence.ForTravel(d.CaptureDungeonSave(),destination,destination.startCell,0,out first);var back=DungeonMapPersistence.ForTravel(travel,map,state.cell,state.heading,out first);check(back.interactions.completedEvents.Contains("phase3.fixture.completed")&&back.interactions.rewardReceipts.Contains("phase3.fixture.reward")&&back.explored.Count==state.explored.Count,"Gameplay: map roundtrip preserves explored cells, completed event and reward receipt");}
  var lab=d.gameObject.AddComponent<CombatLaboratory>();lab.speciesId="Hongo";lab.enemyLevel=10;lab.gamerLevel=10;lab.companions=0;yield return null;yield return null;lab.Fight();yield return Settled(d);
  check(d.LaboratoryBattle,"Gameplay: laboratory enters real combat on the existing scene");
  Call(d,"Action",0);check(d.LaboratoryBusy,"Gameplay: ATTACK starts its existing turn coroutine");yield return Settled(d);check(d.LaboratoryBattle,"Gameplay: ATTACK returns to an actionable combat state");
  Call(d,"Action",1);yield return new WaitForSecondsRealtime(1);check(Get(d,"combatMenu").ToString()=="Hacks","Gameplay: HACK submenu opens");Call(d,"BackToCombatMenu");
  d.RestoreBits();typeof(DungeonPrototype).GetField("selectedHackId",F).SetValue(d,"hack.jump");var jump=d.GetComponent<EnemyJumpHack>();Call(d,"BeginTargetHack",jump);check(jump.Active&&d.LaboratoryBusy,"Gameplay: Jump activates its real UI session and input lock");
  jump.Model.TickTo(100);yield return Settled(d);check(!jump.Active&&!d.LaboratoryBusy,"Gameplay: Jump failure resolves and returns control to combat");
  d.RestoreBits();typeof(DungeonPrototype).GetField("selectedHackId",F).SetValue(d,"hack.overclock");var over=d.GetComponent<OverclockHack>();Call(d,"BeginTargetHack",over);check(over.Active,"Gameplay: Overclock activates");double timeout=over.Model.Started+over.Model.Duration+1;over.TickSession(timeout);over.TickSession(timeout+10);yield return new WaitForSecondsRealtime(2.2f);yield return Settled(d);check(!over.Active&&!d.LaboratoryBusy,"Gameplay: Overclock result returns control to combat");
  lab.Fight();yield return Settled(d);Call(d,"Action",2);check(Get(d,"combatMenu").ToString()=="Items","Gameplay: ITEM submenu opens");var counts=(int[])Get(d,"itemCounts");counts[0]=1;typeof(DungeonPrototype).GetField("hp",F).SetValue(d,Mathf.Max(1,(int)typeof(DungeonPrototype).GetProperty("MaximumHP",F).GetValue(d)-10));Call(d,"Action",0);check(counts[0]==0,"Gameplay: potion consumes exactly one fixture item");yield return Settled(d);
  Call(d,"FinishSuccessfulFlee");yield return Settled(d);check(!d.LaboratoryBattle&&d.RetroView.enabled&&Time.timeScale==1,"Gameplay: successful flee restores exploration and camera");
  // Pure collision regression uses runtime defaults; it does not change the approved hack profile.
  var cfg=ScriptableObject.CreateInstance<EnemyJumpSettings>();try{var model=new EnemyJumpModel(cfg);typeof(EnemyJumpModel).GetProperty("Y").SetValue(model,EnemyJumpModel.EnemySize+8f);typeof(EnemyJumpModel).GetProperty("Velocity").SetValue(model,-200f);model.Enemies.Add(160);model.Jump();model.Tick(.045f);check(model.Combo==1&&model.Velocity>=cfg.bounceForce*.9f,"Gameplay: precise stomp still produces a boosted bounce");}finally{UnityEngine.Object.Destroy(cfg);}
 }
}
#endif
