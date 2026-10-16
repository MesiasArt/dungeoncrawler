#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DemoPhase2TransitionValidation {
 const string Missing="Assets/DemoPhase2Fixtures/MissingReady.unity",Delayed="Assets/DemoPhase2Fixtures/Delayed.unity";
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 static void SetTimeouts(float ready=1.5f,float load=120){var transition=UnityEngine.Object.FindAnyObjectByType<DungeonWhiteTransition>();typeof(DungeonWhiteTransition).GetField("readyTimeoutSeconds",F).SetValue(transition,ready);typeof(DungeonWhiteTransition).GetField("loadTimeoutSeconds",F).SetValue(transition,load);}
 static IEnumerator Settled(){float until=Time.realtimeSinceStartup+15;while((DungeonWhiteTransition.Active||!DungeonWhiteTransition.CanEnter)&&Time.realtimeSinceStartup<until)yield return null;if(DungeonWhiteTransition.Active||!DungeonWhiteTransition.CanEnter)throw new Exception("Transition did not recover within test deadline");yield return null;}
 static IEnumerator Dismiss(){var notice=UnityEngine.Object.FindAnyObjectByType<DungeonSaveRecoveryNotice>();if(notice!=null)UnityEngine.Object.Destroy(notice.gameObject);yield return null;}
 static bool SameProgress(DungeonSaveData before,DungeonSaveData after){return before.activeMapId==after.activeMapId&&before.cell==after.cell&&before.heading==after.heading&&before.player.currentHP==after.player.currentHP&&before.player.currentBits==after.player.currentBits&&before.player.coins==after.player.coins&&before.player.level==after.player.level&&before.player.experience==after.player.experience&&JsonUtility.ToJson(before.interactions)==JsonUtility.ToJson(after.interactions)&&before.enemies.Count==after.enemies.Count;}
 public static IEnumerator Run(Action<bool,string> check,DungeonMapSettings pixelMap){
  var fast=UnityEngine.Object.Instantiate(DigitalKingdomSettings.Current);fast.whiteOutSeconds=.05f;fast.whiteInSeconds=.05f;
  var source=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();var before=source.CaptureDungeonSave();var scene=source.gameObject.scene;
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),source.mapSettings,"Assets/Scenes/Phase2NotPresent.unity",fast);SetTimeouts();yield return Settled();
  check(source!=null&&source.gameObject.activeInHierarchy&&SceneManager.GetActiveScene()==scene&&SameProgress(before,source.CaptureDungeonSave()),"Transition: invalid scene returns to the same live origin without progress loss");
  check(DungeonSaveRecoveryNotice.Active&&DungeonWhiteTransition.Opacity==0,"Transition: invalid scene removes white cover and displays acknowledgement");yield return Dismiss();
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),source.mapSettings,Missing,fast);SetTimeouts(.25f);yield return Settled();
  check(source!=null&&source.RetroView.isActiveAndEnabled&&SameProgress(before,source.CaptureDungeonSave()),"Transition: missing readiness times out and restores original camera and state");
  check(SceneManager.sceneCount==1&&UnityEngine.Object.FindObjectsByType<DungeonPrototype>(FindObjectsSortMode.None).Length==1,"Transition: failed destination is unloaded without duplicate controllers");yield return Dismiss();
  var originalPresentation=source.digitalKingdomSettings;var unavailable=UnityEngine.Object.Instantiate(fast);unavailable.scenePath="Assets/Scenes/Phase2NotPresent.unity";unavailable.requiredBossId="";source.digitalKingdomSettings=unavailable;
  check(source.EnterDigitalKingdom(),"Transition: real dungeon entry starts through its existing interaction method");SetTimeouts();yield return Settled();
  var dialogue=(WorldDialogue)typeof(DungeonPrototype).GetField("worldDialogue",F).GetValue(source);var state=source.CaptureDungeonSave();
  check(!state.city.inCity&&!dialogue.Active&&Time.timeScale==1&&source.RetroView.cullingMask!=0,"Transition: failed city entry clears only the uncommitted destination dialogue and restores exploration");source.digitalKingdomSettings=originalPresentation;UnityEngine.Object.Destroy(unavailable);yield return Dismiss();
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),source.mapSettings,Delayed,fast);SetTimeouts();yield return Settled();
  var next=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();check(source==null&&next!=null&&SceneManager.GetActiveScene().path==Delayed&&SameProgress(before,next.CaptureDungeonSave()),"Transition: delayed readiness succeeds with the original progress");
  check(SceneManager.sceneCount==1&&!DungeonSaveRecoveryNotice.Active,"Transition: normal success unloads origin without warning");
  var data=next.CaptureDungeonSave();data.city=data.city??new DigitalKingdomProgress();data.city.inCity=true;data.city.firstVisitCompleted=true;
  DigitalKingdomTransit.Enter(data,next.mapSettings,"Assets/Scenes/DigitalKingdom.unity",fast);SetTimeouts();yield return Settled();
  var city=UnityEngine.Object.FindAnyObjectByType<DigitalKingdomController>();check(next==null&&city!=null&&city.Ready&&city.cityCamera.isActiveAndEnabled,"Transition: normal Digital Kingdom entry completes with a live city camera");
  check(city.stateHost.CaptureDungeonSave().player.currentHP==before.player.currentHP&&SceneManager.sceneCount==1,"Transition: city entry retains player progress and retires origin");
  var cityOrigin=city;var cityBefore=city.stateHost.CaptureDungeonSave();DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(cityBefore),city.stateHost.mapSettings,Missing,fast);SetTimeouts(.25f);yield return Settled();
  check(cityOrigin!=null&&cityOrigin.Ready&&cityOrigin.gameObject.activeInHierarchy&&cityOrigin.cityCamera.isActiveAndEnabled&&SameProgress(cityBefore,cityOrigin.stateHost.CaptureDungeonSave()),"Transition: city rollback retains its actual model and camera");
  check(Time.timeScale==0&&!AudioListener.pause,"Transition: city rollback restores original pause and audible music");yield return Dismiss();
  bool first;data=DungeonMapPersistence.ForTravel(city.stateHost.CaptureDungeonSave(),pixelMap,pixelMap.startCell,pixelMap.startHeading,out first);
  data.city.inCity=false;DigitalKingdomTransit.Explore(data,pixelMap,"Assets/Scenes/RetroDungeonExperiment.unity",fast,first);SetTimeouts(3);yield return Settled();yield return null;yield return new WaitForEndOfFrame();
  source=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();var composer=source.GetComponent<RetroDungeonWallComposer>();check(city==null&&composer!=null&&composer.ElementCount>0&&source.mapSettings==pixelMap,"Transition: city to Pixel succeeds with its visual composer");
  before=source.CaptureDungeonSave();var profile=composer.settings.crtProfile;int hash=composer.CompositionHash;float fog=Shader.GetGlobalFloat("_RetroFogEnabled");
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),pixelMap,Missing,fast);SetTimeouts(.25f);yield return Settled();yield return new WaitForEndOfFrame();
  check(source!=null&&source.GetComponent<RetroDungeonWallComposer>()==composer&&composer.CompositionHash==hash&&SameProgress(before,source.CaptureDungeonSave()),"Transition: Pixel rollback retains the same visual hierarchy and progress");
  check(source.RetroView.GetComponent<HelmetCRTController>().settings==profile&&source.RetroView.GetComponents<DungeonFloorReflection>().Length==2&&Mathf.Approximately(Shader.GetGlobalFloat("_RetroFogEnabled"),fog),"Transition: Pixel rollback restores CRT, reflections and atmosphere");yield return Dismiss();
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),pixelMap,"Assets/DemoPhase2Fixtures/PixelUnready.unity",fast);SetTimeouts(.25f);yield return Settled();
  check(source!=null&&source.gameObject.activeInHierarchy&&SameProgress(before,source.CaptureDungeonSave()),"Transition: a camera alone does not count as a fully initialized Pixel destination");yield return Dismiss();
  ScreenCapture.CaptureScreenshot("phase2-transition-recovered.png");yield return new WaitForEndOfFrame();yield return null;
  DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),pixelMap,"Assets/Scenes/RetroDungeonExperiment.unity",fast);SetTimeouts(3);yield return Settled();yield return new WaitForEndOfFrame();
  next=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();check(source==null&&next.GetComponent<RetroDungeonWallComposer>().CompositionHash==hash&&UnityEngine.Object.FindObjectsByType<RetroDungeonWallComposer>(FindObjectsSortMode.None).Length==1,"Transition: Pixel to Pixel commits with one composer and identical visual configuration");
  before=next.CaptureDungeonSave();DigitalKingdomTransit.Explore(DungeonMapPersistence.Copy(before),pixelMap,Missing,fast);SetTimeouts(.25f,0);yield return Settled();
  check(next!=null&&next.gameObject.activeInHierarchy&&SameProgress(before,next.CaptureDungeonSave())&&SceneManager.sceneCount==1,"Transition: load deadline restores origin and cleans up the late destination");yield return Dismiss();
  check(UnityEngine.Object.FindObjectsByType<DungeonAudio>(FindObjectsSortMode.None).Length==1&&DigitalKingdomTransit.ReturnData==null&&DigitalKingdomTransit.CityData==null,"Transition: one audio host and no stale pending transit after rollback");
  yield return SceneManager.LoadSceneAsync("Assets/Scenes/SampleScene.unity");yield return null;yield return null;
  check(!string.IsNullOrEmpty(DungeonSaveStore.editorTestPath)&&DungeonSaveStore.editorTestPath.Contains("DemoPhase2Fixtures"),"Transition: Pixel retirement restores the isolated save override");
  UnityEngine.Object.Destroy(fast);
 }
}
#endif
