#if UNITY_EDITOR && DEMO_PHASE3_RENDER_ELISION
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using UnityEngine;
public static class DemoPhase3RenderElisionTests {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
 [Serializable] class Result {public int pixelsPerComparison,comparisons,changedPixels;public bool unequalMaskRetained,nearbyUnequalRetained,defaultUsesBlackMask;}
 static Color32[] Read(RenderTexture rt){var old=RenderTexture.active;try{RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false,true);try{image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();return image.GetPixels32();}finally{UnityEngine.Object.Destroy(image);}}finally{RenderTexture.active=old;}}
 public static IEnumerator Run(DungeonPrototype dungeon,Action<bool,string> check){
  var view=dungeon.RetroView;var whole=view.GetComponent<HelmetCRTWholeFrame>();var controller=view.GetComponent<HelmetCRTController>();var original=controller.settings;var fixture=UnityEngine.Object.Instantiate(original);var mask=view.GetComponent<HelmetCRTLayerMask>()??view.gameObject.AddComponent<HelmetCRTLayerMask>();
  var enemy=GameObject.CreatePrimitive(PrimitiveType.Cube);enemy.name="Elision pixel fixture";enemy.transform.position=view.transform.position+view.transform.forward*.7f;enemy.transform.localScale=Vector3.one*.18f;enemy.GetComponent<Collider>().enabled=false;enemy.AddComponent<EnemyCombatStats>();
  Material composite=null;RenderTexture reference=null,candidate=null;
  try{
   controller.settings=fixture;fixture.backgroundIntensity=fixture.enemyIntensity=1;
   for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();
   var active=(Material)typeof(HelmetCRTWholeFrame).GetField("composite",F).GetValue(whole);check(active!=null,"Render elision: final composition is active");
   var report=new Result{defaultUsesBlackMask=active.GetTexture("_EnemyMask")==Texture2D.blackTexture};check(report.defaultUsesBlackMask,"Render elision: equal weights skip the geometry mask");
   typeof(HelmetCRTLayerMask).GetMethod("Refresh",F).Invoke(mask,null);var realMask=mask.Build(view,whole.WorldFrame.width,whole.WorldFrame.height);bool visible=false;foreach(var p in Read(realMask))if(p.r>128){visible=true;break;}check(visible,"Render elision: reference mask includes a visible enemy");
   composite=new Material(active);reference=new RenderTexture(whole.WorldFrame.width,whole.WorldFrame.height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);candidate=new RenderTexture(reference.descriptor);reference.Create();candidate.Create();
   bool srgb=GL.sRGBWrite;try{GL.sRGBWrite=false;foreach(float weight in new[]{0f,.37f,1f}){
    var intensity=active.GetVector("_Intensity");intensity.x=intensity.y=weight;composite.SetVector("_Intensity",intensity);composite.SetTexture("_EnemyMask",realMask);Graphics.Blit(whole.WorldFrame,reference,composite);composite.SetTexture("_EnemyMask",Texture2D.blackTexture);Graphics.Blit(whole.WorldFrame,candidate,composite);var a=Read(reference);var b=Read(candidate);int changed=0;for(int i=0;i<a.Length;i++)if(!a[i].Equals(b[i]))changed++;report.pixelsPerComparison=a.Length;report.comparisons++;report.changedPixels+=changed;check(changed==0,"Render elision: full composition is pixel-identical at equal intensity "+weight);
   }}finally{GL.sRGBWrite=srgb;}
   fixture.backgroundIntensity=1;fixture.enemyIntensity=.1f;for(int i=0;i<3;i++)yield return new WaitForEndOfFrame();report.unequalMaskRetained=active.GetTexture("_EnemyMask") is RenderTexture;check(report.unequalMaskRetained,"Render elision: unequal intensities still render the enemy mask");
   report.nearbyUnequalRetained=HelmetCRTWholeFrame.NeedsEnemyMask(1,1-.000001f);check(report.nearbyUnequalRetained,"Render elision: no approximate comparison discards distinct settings");File.WriteAllText("phase3-elision-pixels.json",JsonUtility.ToJson(report,true));
  }finally{controller.settings=original;UnityEngine.Object.Destroy(fixture);UnityEngine.Object.Destroy(enemy);if(composite!=null)UnityEngine.Object.Destroy(composite);foreach(var rt in new[]{reference,candidate})if(rt!=null){rt.Release();UnityEngine.Object.Destroy(rt);}}
  yield return null;
 }
}
#endif
