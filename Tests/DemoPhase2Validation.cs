#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public sealed class DemoPhase2Validation:MonoBehaviour {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static string FixturePath {get{return Path.GetFullPath("Temp/DemoPhase2Fixtures/slot.json");}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)] static void Guard(){Directory.CreateDirectory(Path.GetDirectoryName(FixturePath));DungeonSaveStore.editorTestPath=FixturePath;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Launch(){if(FindAnyObjectByType<DemoPhase2Validation>()!=null)return;var go=new GameObject("Phase 2 isolated validation");DontDestroyOnLoad(go);go.AddComponent<DemoPhase2Validation>();}
 void Check(bool condition,string description){File.AppendAllText("phase2-results.txt",(condition?"PASS: ":"FAIL: ")+description+"\n");if(!condition)throw new Exception(description);}
 IEnumerator Start(){yield return RunGuarded();}
 IEnumerator RunGuarded(){var stack=new System.Collections.Generic.Stack<IEnumerator>();stack.Push(Run());Exception failure=null;while(stack.Count>0){bool next=false;var steps=stack.Peek();try{next=steps.MoveNext();}catch(Exception e){failure=e;}if(failure!=null)break;if(!next){stack.Pop();continue;}var nested=steps.Current as IEnumerator;if(nested!=null){stack.Push(nested);continue;}yield return steps.Current;}if(failure!=null)File.AppendAllText("phase2-results.txt","FAIL: "+failure+"\n");SessionState.SetBool("DemoPhase2.Done",true);SessionState.SetBool("DemoPhase2.Success",failure==null);EditorApplication.isPlaying=false;}
 IEnumerator Ready(){float until=Time.realtimeSinceStartup+30;while(Time.realtimeSinceStartup<until){var d=FindAnyObjectByType<DungeonPrototype>();if(d!=null&&d.RetroView!=null&&d.LaboratoryReady)yield break;yield return null;}throw new Exception("Dungeon initialization timeout");}
 IEnumerator Run(){
  yield return Ready();yield return null;yield return new WaitForEndOfFrame();
  #if DEMO_PHASE3_BENCHMARK
  if(File.Exists("phase3-label")){yield return DemoPhase3Benchmark.Run(Check);yield break;}
  #endif
  if(File.Exists("phase2-ui-only")){
   DungeonSaveRecoveryNotice.Show("Partida recuperada del respaldo 2. Puede contener progreso anterior al último guardado. No se pudo reparar el archivo principal; el respaldo sigue intacto.");
   yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("phase2-recovery-notice.png");yield return new WaitForEndOfFrame();yield return null;
   var keyboard=UnityEngine.InputSystem.Keyboard.current;bool created=keyboard==null;if(created)keyboard=UnityEngine.InputSystem.InputSystem.AddDevice<UnityEngine.InputSystem.Keyboard>();
   UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard,new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.Enter));yield return null;yield return null;
   UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard,new UnityEngine.InputSystem.LowLevel.KeyboardState());if(created)UnityEngine.InputSystem.InputSystem.RemoveDevice(keyboard);
   Check(!DungeonSaveRecoveryNotice.Active,"Recovery notice acknowledges Enter without loading or saving a personal slot");yield break;
  }
  var dungeon=FindAnyObjectByType<DungeonPrototype>();var composer=dungeon.GetComponent<RetroDungeonWallComposer>();var settings=composer.settings;var map=dungeon.mapSettings;
  var pixelMap=map;int hash=composer.CompositionHash;Check(composer.ElementCount>0,"Pixel baseline contains generated visual layers");
  for(int i=0;i<3;i++){
   typeof(DungeonPrototype).GetMethod("RestartNewGame",F).Invoke(dungeon,null);yield return null;yield return Ready();yield return null;yield return new WaitForEndOfFrame();
   dungeon=FindAnyObjectByType<DungeonPrototype>();composer=dungeon.GetComponent<RetroDungeonWallComposer>();
   Check(composer!=null&&composer.settings==settings&&dungeon.mapSettings==map,"Pixel restart "+i+" retains map and composer configuration");
   Check(composer.ElementCount>0&&composer.CompositionHash==hash,"Pixel restart "+i+" rebuilds identical visual composition");
   Check(FindObjectsByType<DungeonPrototype>(FindObjectsSortMode.None).Length==1&&FindObjectsByType<RetroDungeonWallComposer>(FindObjectsSortMode.None).Length==1,"Pixel restart "+i+" has one controller and composer");
   Check(dungeon.RetroView.GetComponents<DungeonFloorReflection>().Length==2,"Pixel restart "+i+" restores floor and ceiling reflections");
   Check(dungeon.RetroView.GetComponent<HelmetCRTController>().settings==settings.crtProfile,"Pixel restart "+i+" preserves CRT profile");
   var audio=FindAnyObjectByType<DungeonAudio>();Check(FindObjectsByType<DungeonAudio>(FindObjectsSortMode.None).Length==1&&typeof(DungeonAudio).GetField("mapAudio",F).GetValue(audio)==settings.audioProfile,"Pixel restart "+i+" preserves one audio host and map playlist");
  }
  ScreenCapture.CaptureScreenshot("phase2-pixel-restart.png");yield return new WaitForEndOfFrame();yield return null;
  yield return SceneManager.LoadSceneAsync("Assets/Scenes/SampleScene.unity");yield return Ready();yield return null;
  dungeon=FindAnyObjectByType<DungeonPrototype>();map=dungeon.mapSettings;
  typeof(DungeonPrototype).GetMethod("RestartNewGame",F).Invoke(dungeon,null);yield return null;yield return Ready();yield return null;
  dungeon=FindAnyObjectByType<DungeonPrototype>();Check(dungeon.mapSettings==map&&dungeon.GetComponent<RetroDungeonWallComposer>()==null,"Normal dungeon restart retains map without injecting Pixel presentation");
  Check(FindObjectsByType<DungeonPrototype>(FindObjectsSortMode.None).Length==1&&dungeon.RetroView.GetComponents<DungeonFloorReflection>().Length==2,"Normal dungeon restart has one controller and both reflections");
  #if DEMO_PHASE2_SAVE
  DemoPhase2SaveValidation.Run(Check);
  DungeonSaveStore.editorTestPath=FixturePath;
  var live=dungeon.CaptureDungeonSave();live.introductionCompleted=true;
  for(int slot=0;slot<6;slot++){live.version=slot%2+1;DungeonSaveStore.Write(live,slot,"Live fixture");var copy=DungeonSaveStore.Read(slot);dungeon.RestoreDungeonSave(copy);yield return null;Check(dungeon.CaptureDungeonSave().activeMapId==live.activeMapId&&dungeon.PlayerLevel==live.player.level,"Save: live restoration slot "+slot+" version "+live.version);}
  #endif
  #if DEMO_PHASE2_TRANSITION
  yield return DemoPhase2TransitionValidation.Run(Check,pixelMap);
  #endif
 }
}
#endif
