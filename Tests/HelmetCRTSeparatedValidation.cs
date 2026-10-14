using System;
using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HelmetCRTSeparatedValidation:MonoBehaviour {
 public HelmetCRTController controller;
 static Color[] Read(RenderTexture rt){var old=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false,true);t.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);t.Apply();var pixels=t.GetPixels();Destroy(t);RenderTexture.active=old;return pixels;}
 static float Difference(Color a,Color b){return Mathf.Max(Mathf.Abs(a.r-b.r),Mathf.Abs(a.g-b.g),Mathf.Abs(a.b-b.b));}
 void OnGUI(){
  GUI.matrix=Matrix4x4.identity;GUI.color=Color.white;
  GUI.DrawTexture(new Rect(20,20,240,100),Texture2D.whiteTexture);
  GUI.color=Color.black;GUI.Label(new Rect(30,30,210,40),"NATIVE TEXT / UI ZERO",new GUIStyle(GUI.skin.label){fontSize=22,normal={textColor=Color.black}});
  GUI.color=Color.white;GUI.Box(new Rect(300,20,240,100),"NATIVE BOX");GUI.Button(new Rect(20,150,240,70),"NATIVE BUTTON");
  GUI.color=new Color(1,.3f,.1f,.5f);GUI.DrawTexture(new Rect(300,150,240,70),Texture2D.whiteTexture);GUI.color=Color.white;
  GUILayout.BeginArea(new Rect(20,250,300,180));GUILayout.Label("LAYOUT TEXT");GUILayout.Button("LAYOUT BUTTON");GUILayout.Toggle(true,"NATIVE TOGGLE");GUILayout.HorizontalSlider(.5f,0,1);GUILayout.EndArea();
 }
 IEnumerator Start(){
  Application.runInBackground=true;var settings=controller.settings;var frame=controller.GetComponent<HelmetCRTWholeFrame>();
  for(int i=0;i<35;i++)yield return new WaitForEndOfFrame();
  string failure=null;int opaque=0,mismatch=0,transparent=0,holes=0,worldChanged=0,enemyPixels=0,enemyOverlap=0;double maskY=0;
  Color[] cleanUI=null,zero=null;
  try {
   if(frame==null||frame.LastOutput==null||frame.UIFrame==null||frame.WorldFrame==null)throw new Exception("Separate frame targets were not produced");
   bool displayReady=false;foreach(var camera in Camera.allCameras)if(camera.enabled&&camera.targetTexture==null&&camera.targetDisplay==controller.GetComponent<Camera>().targetDisplay)displayReady=true;
   if(!displayReady)throw new Exception("No camera renders to the display while CRT is separated");
   cleanUI=Read(frame.UIFrame);zero=Read(frame.LastOutput);var cleanWorld=Read(frame.WorldFrame);var processedWorld=(RenderTexture)typeof(HelmetCRTWholeFrame).GetField("output",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(frame);var processed=Read(processedWorld);
   var helper=controller.GetComponent<HelmetCRTLayerMask>();var maskTexture=(RenderTexture)typeof(HelmetCRTLayerMask).GetField("mask",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(helper);var mask=Read(maskTexture);
   for(int i=0;i<zero.Length;i++){
    if(cleanUI[i].a>.995f){opaque++;if(Difference(zero[i],cleanUI[i])>.025f)mismatch++;}
    if(cleanUI[i].a<.001f){transparent++;var expected=Color.Lerp(cleanWorld[i],processed[i],Mathf.Lerp(settings.backgroundIntensity,settings.enemyIntensity,mask[i].r));if(Difference(zero[i],expected)>.025f)holes++;}
    if(mask[i].r>.99f){enemyPixels++;maskY+=(i/maskTexture.width+.5)/maskTexture.height;if(cleanWorld[i].r>.8f&&cleanWorld[i].g<.5f&&cleanWorld[i].b<.3f)enemyOverlap++;}
    if(Difference(cleanWorld[i],processed[i])>.03f)worldChanged++;
   }
   if(opaque<1000||mismatch>0||transparent<1000||holes>0||worldChanged<100)throw new Exception("UI zero failed: opaque="+opaque+" mismatch="+mismatch+" transparent="+transparent+" holes="+holes+" world CRT pixels="+worldChanged);
   var actor=GameObject.Find("Off-center enemy");var expectedY=controller.GetComponent<Camera>().WorldToViewportPoint(actor.transform.position).y;
   if(enemyPixels<100||Math.Abs(maskY/enemyPixels-expectedY)>.03||enemyOverlap<enemyPixels*.95)throw new Exception("Enemy mask orientation/coverage failed: "+enemyOverlap+"/"+enemyPixels+", Y="+maskY/enemyPixels+" expected "+expectedY);
  }catch(Exception e){failure=e.ToString();}
  if(failure!=null){Finish(false,failure);yield break;}
  settings.uiIntensity=1;
  for(int i=0;i<5;i++)yield return new WaitForEndOfFrame();
  try {
   var strong=Read(frame.LastOutput);int changed=0;for(int i=0;i<strong.Length;i++)if(cleanUI[i].a>.995f&&Difference(strong[i],zero[i])>.03f)changed++;
   if(changed<100)throw new Exception("UI intensity one did not process the native controls");
   settings.uiIntensity=0;settings.brightness=.8f;
  }catch(Exception e){Finish(false,e.ToString());yield break;}
  for(int i=0;i<5;i++)yield return new WaitForEndOfFrame();
  try {
   var dim=Read(frame.LastOutput);var currentUI=Read(frame.UIFrame);int bad=0;for(int i=0;i<dim.Length;i++)if(currentUI[i].a>.995f&&Difference(dim[i],currentUI[i]*.8f)>.025f)bad++;
   if(bad>0)throw new Exception("Brightness mismatch across UI: "+bad);
   Finish(true,"PASS: native IMGUI text, textures, Box, Button, GUILayout and Toggle captured separately; UI=0 exact ("+opaque+" opaque pixels), transparent background holes="+holes+"; world CRT changed "+worldChanged+" pixels; UI=1 works; brightness uniform; ready for Play/Stop restoration check");
  }catch(Exception e){Finish(false,e.ToString());}
 }
 void Finish(bool success,string report){
  File.WriteAllText(Path.Combine(Application.dataPath,"../separated-results.txt"),report);
  var rt=controller.GetComponent<HelmetCRTWholeFrame>().LastOutput;if(rt!=null){var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Application.dataPath,"../separated-preview.png"),image.EncodeToPNG());RenderTexture.active=old;}
#if UNITY_EDITOR
  SessionState.SetBool("CRTSeparated.Success",success);SessionState.SetBool("CRTSeparated.Done",true);EditorApplication.isPlaying=false;
#endif
 }
}
