using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// One persistent cover bridges the existing illustration scene. No second city or save system.
[DefaultExecutionOrder(32000)]
public sealed class DungeonWhiteTransition:MonoBehaviour {
 static DungeonWhiteTransition current;
 public static bool Active {get{return current!=null;}}
 public static float Opacity {get{return current!=null?current.alpha:0;}}
 Material whiteComposite;float alpha;Canvas cover;UnityEngine.UI.Image image;
 public static void Enter(string path,DigitalKingdomSettings settings,bool dungeon=false){
  if(Active)return;
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
  float seconds=settings!=null?settings.whiteOutSeconds:1;
  DungeonAudio.FadeForCityTransition(seconds);if(settings!=null)DungeonAudio.PlayClip(settings.teleportSound,.65f);
  yield return Fade(0,1,seconds);
  // The opaque cover is already submitted before scene activation starts.
  yield return new WaitForEndOfFrame();
  AsyncOperation load=null;try{load=SceneManager.LoadSceneAsync(path,LoadSceneMode.Single);}catch(System.Exception e){Debug.LogException(e);}
  if(load==null){DungeonAudio.SetCityMusic(false);yield return Fade(1,0,.5f);Destroy(gameObject);yield break;}
  while(!load.isDone)yield return null;
  if(dungeon){DungeonPrototype player=null;while(player==null||player.RetroView==null){player=Object.FindFirstObjectByType<DungeonPrototype>();yield return null;}}else{DigitalKingdomController city=null;while(city==null||!city.Ready){city=Object.FindFirstObjectByType<DigitalKingdomController>();yield return null;}}
  yield return new WaitForEndOfFrame();yield return null;
  DungeonAudio.SetCityMusic(!dungeon);if(dungeon)DungeonAudio.SetCombat(false);
  yield return Fade(1,0,settings!=null?settings.whiteInSeconds:.8f);
  Destroy(gameObject);
 }
 // IMGUI is rendered above overlay canvases on some render paths; cover it as well.
 public static bool PresentOverCRT(RenderTexture texture,bool flip){if(current==null)return false;if(current.whiteComposite==null){var shader=Resources.Load<Shader>("Shaders/WhiteTransition");if(shader==null)return false;current.whiteComposite=new Material(shader);}current.whiteComposite.SetFloat("_Opacity",current.alpha);current.whiteComposite.SetFloat("_FlipY",flip?1:0);Graphics.Blit(texture,(RenderTexture)null,current.whiteComposite);return true;}
 void OnGUI(){if(HelmetCRTWholeFrame.Capturing)return;var matrix=GUI.matrix;var color=GUI.color;int depth=GUI.depth;GUI.matrix=Matrix4x4.identity;GUI.depth=-32767;GUI.color=new Color(1,1,1,alpha);GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);GUI.color=color;GUI.matrix=matrix;GUI.depth=depth;}
 void OnDestroy(){if(whiteComposite!=null)Destroy(whiteComposite);if(current==this)current=null;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetState(){current=null;}
}
