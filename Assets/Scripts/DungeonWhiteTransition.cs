using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// One persistent cover bridges the existing illustration scene. No second city or save system.
[DefaultExecutionOrder(32000)]
public sealed class DungeonWhiteTransition:MonoBehaviour {
 static DungeonWhiteTransition current;
 static bool pendingBackgroundLoad;
 public static bool Active {get{return current!=null;}}
 public static bool CanEnter {get{return !Active&&!pendingBackgroundLoad;}}
 public static float Opacity {get{return current!=null?current.alpha:0;}}
 Material whiteComposite;float alpha;Canvas cover;UnityEngine.UI.Image image;
 float loadTimeoutSeconds=120,readyTimeoutSeconds=30;
 public static void Enter(string path,DigitalKingdomSettings settings,bool dungeon=false){
  if(!CanEnter)return;
  var go=new GameObject("Digital Kingdom - luz de transicion");DontDestroyOnLoad(go);
  current=go.AddComponent<DungeonWhiteTransition>();current.BuildCover();current.StartCoroutine(current.Load(path,settings,dungeon));
 }
 void BuildCover(){
  cover=gameObject.AddComponent<Canvas>();cover.renderMode=RenderMode.ScreenSpaceOverlay;cover.sortingOrder=32767;
  var go=new GameObject("White",typeof(RectTransform),typeof(UnityEngine.UI.Image));go.transform.SetParent(transform,false);image=go.GetComponent<UnityEngine.UI.Image>();image.color=new Color(1,1,1,0);image.raycastTarget=true;
  var r=go.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
 }
 void SetAlpha(float value){alpha=value;if(image!=null)image.color=new Color(1,1,1,value>=1?1:0);}
 IEnumerator Fade(float from,float to,float seconds){float start=Time.realtimeSinceStartup;do{float t=Mathf.Clamp01((Time.realtimeSinceStartup-start)/Mathf.Max(.01f,seconds));SetAlpha(Mathf.Lerp(from,to,t));yield return null;}while(Mathf.Abs(alpha-to)>.0001f);SetAlpha(to);}
 IEnumerator Load(string path,DigitalKingdomSettings settings,bool dungeon){
  DungeonAudio.KeepAcrossScenes();var origin=new DungeonTransitionOrigin();
  float seconds=settings!=null?settings.whiteOutSeconds:1;
  DungeonAudio.FadeForCityTransition(seconds);if(settings!=null)DungeonAudio.PlayClip(settings.teleportSound,.65f);
  yield return Fade(0,1,seconds);
  // The opaque cover is already submitted before scene activation starts.
  yield return new WaitForEndOfFrame();
  // Keep the real origin alive until readiness is confirmed; never synthesize its progress.
  DungeonAudio.KeepAcrossScenes();origin.Suspend();
  Scene destination=default(Scene);bool abandoned=false;
  UnityEngine.Events.UnityAction<Scene,LoadSceneMode> loaded=(scene,mode)=>{if(mode!=LoadSceneMode.Additive)return;destination=scene;if(abandoned)origin.HideDestination(scene);else origin.DestinationLoaded(scene);};
  SceneManager.sceneLoaded+=loaded;
  AsyncOperation load=null;string failure=null;
  if(!Application.CanStreamedLevelBeLoaded(path))failure="La escena de destino no está disponible.";
  else try{load=SceneManager.LoadSceneAsync(path,LoadSceneMode.Additive);}catch(System.Exception e){failure=e.Message;}
  if(load!=null){
   float deadline=Time.realtimeSinceStartup+loadTimeoutSeconds;
   while(!load.isDone&&Time.realtimeSinceStartup<deadline)yield return null;
   if(!load.isDone)failure="La carga de escena superó el plazo de espera.";
   else {
    deadline=Time.realtimeSinceStartup+readyTimeoutSeconds;
    while(!DestinationReady(destination,dungeon)&&Time.realtimeSinceStartup<deadline)yield return null;
    if(!DestinationReady(destination,dungeon))failure="La escena no terminó de preparar sus componentes.";
   }
  }else if(failure==null)failure="No se pudo iniciar la carga de escena.";
  if(failure!=null){
   abandoned=true;Debug.LogWarning("Transición recuperada: "+failure);
   DigitalKingdomTransit.CancelPendingTransition();
   pendingBackgroundLoad=true;
   System.Action discard=()=>{var cleanup=origin.Discard(destination);if(cleanup!=null)cleanup.completed+=ignored=>pendingBackgroundLoad=false;else pendingBackgroundLoad=false;};
   if(load!=null&&!load.isDone){
    load.completed+=ignored=>{SceneManager.sceneLoaded-=loaded;discard();origin.RestoreEnvironment();};
   }else{SceneManager.sceneLoaded-=loaded;discard();}
   origin.Resume();yield return Fade(1,0,.5f);
   DungeonSaveRecoveryNotice.Show("No se pudo completar el viaje. Has vuelto al origen y tu progreso se conserva. "+failure);
   Destroy(gameObject);yield break;
  }
  SceneManager.sceneLoaded-=loaded;
  var unloadOrigin=origin.Commit();if(unloadOrigin!=null)yield return unloadOrigin;
  // Retiring an old Pixel composer clears global shader values. Reapply the destination.
  foreach(var composer in UnityEngine.Object.FindObjectsByType<RetroDungeonWallComposer>())composer.ApplyLiveSettings();
  yield return new WaitForEndOfFrame();yield return null;
  DungeonAudio.SetCityMusic(!dungeon);if(dungeon)DungeonAudio.SetCombat(false);
  yield return Fade(1,0,settings!=null?settings.whiteInSeconds:.8f);
  Destroy(gameObject);
 }
 static bool DestinationReady(Scene scene,bool dungeon){if(!scene.IsValid()||!scene.isLoaded)return false;foreach(var root in scene.GetRootGameObjects()){if(dungeon){foreach(var player in root.GetComponentsInChildren<DungeonPrototype>())if(player.isActiveAndEnabled&&player.SceneTransitionReady&&player.GetComponent<DigitalKingdomController>()==null)return true;}else foreach(var city in root.GetComponentsInChildren<DigitalKingdomController>())if(city.isActiveAndEnabled&&city.Ready&&city.cityCamera!=null&&city.cityCamera.isActiveAndEnabled)return true;}return false;}
 // IMGUI is rendered above overlay canvases on some render paths; cover it as well.
 public static bool PresentOverCRT(RenderTexture texture,bool flip){if(current==null)return false;if(current.whiteComposite==null){var shader=Resources.Load<Shader>("Shaders/WhiteTransition");if(shader==null)return false;current.whiteComposite=new Material(shader);}current.whiteComposite.SetFloat("_Opacity",current.alpha);current.whiteComposite.SetFloat("_FlipY",flip?1:0);Graphics.Blit(texture,(RenderTexture)null,current.whiteComposite);return true;}
 void OnGUI(){if(HelmetCRTWholeFrame.Capturing)return;var matrix=GUI.matrix;var color=GUI.color;int depth=GUI.depth;GUI.matrix=Matrix4x4.identity;GUI.depth=-32767;GUI.color=new Color(1,1,1,alpha);GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);GUI.color=color;GUI.matrix=matrix;GUI.depth=depth;}
 void OnDestroy(){if(whiteComposite!=null)Destroy(whiteComposite);if(current==this)current=null;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetState(){current=null;pendingBackgroundLoad=false;}
}
