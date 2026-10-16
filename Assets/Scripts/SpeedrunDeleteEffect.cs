using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Temporary materials and frozen sprite animation are restored even on cancellation.
public sealed class SpeedrunDeleteEffect:MonoBehaviour {
 sealed class Surface {public Renderer renderer;public Material[] originals,replacements;}
 readonly List<Surface> surfaces=new List<Surface>();
 GhostEnemyVisual visual;bool wasEnabled;Camera cameraView;Bounds bounds;float progress;bool running;
 public IEnumerator Play(Camera camera,float seconds){
  cameraView=camera;visual=GetComponent<GhostEnemyVisual>();if(visual!=null){wasEnabled=visual.enabled;visual.enabled=false;}
  var shader=Resources.Load<Shader>("Shaders/SpeedrunDelete");bool first=true;
  foreach(var renderer in GetComponentsInChildren<Renderer>()){
   if(!renderer.enabled)continue;if(first){bounds=renderer.bounds;first=false;}else bounds.Encapsulate(renderer.bounds);
   if(shader==null)continue;var original=renderer.sharedMaterials;var temporary=new Material[original.Length];
   for(int i=0;i<original.Length;i++){temporary[i]=new Material(shader);var source=original[i];Texture texture=source!=null&&source.HasProperty("_BaseMap")?source.GetTexture("_BaseMap"):source!=null?source.mainTexture:null;temporary[i].SetTexture("_BaseMap",texture!=null?texture:Texture2D.whiteTexture);}
   surfaces.Add(new Surface{renderer=renderer,originals=original,replacements=temporary});renderer.sharedMaterials=temporary;
  }
  running=true;DungeonAudio.Play(DungeonAudio.Cue.Confirm);
  try{for(float elapsed=0;elapsed<seconds;elapsed+=Time.unscaledDeltaTime){progress=Mathf.Clamp01(elapsed/seconds);foreach(var surface in surfaces)foreach(var material in surface.replacements)material.SetFloat("_Progress",progress);yield return null;}progress=1;}
  finally{Restore();}
 }
 void Restore(){running=false;foreach(var surface in surfaces){if(surface.renderer!=null)surface.renderer.sharedMaterials=surface.originals;foreach(var material in surface.replacements)if(material!=null)Destroy(material);}surfaces.Clear();if(visual!=null)visual.enabled=wasEnabled;visual=null;}
 void OnDisable(){Restore();}void OnDestroy(){Restore();}
 void OnGUI(){if(!running||cameraView==null)return;var matrix=GUI.matrix;var color=GUI.color;int depth=GUI.depth;GUI.matrix=Matrix4x4.identity;GUI.depth=-35;HelmetCRTLayerMask.BeginGUI();
  Vector3 a=cameraView.WorldToScreenPoint(bounds.min),b=cameraView.WorldToScreenPoint(bounds.max);if(a.z<=0||b.z<=0){GUI.matrix=matrix;GUI.depth=depth;return;}
  var r=Rect.MinMaxRect(Mathf.Min(a.x,b.x)-8,Screen.height-Mathf.Max(a.y,b.y)-8,Mathf.Max(a.x,b.x)+8,Screen.height-Mathf.Min(a.y,b.y)+8);
  Color cyan=new Color(.1f,.95f,1,1-progress*.4f);if(progress<.6f){NeonDungeonUI.Outline(r,cyan,2);NeonDungeonUI.Solid(new Rect(r.x,r.y+r.height*Mathf.Clamp01((progress-.12f)/.4f),r.width,3),cyan);var style=new GUIStyle(GUI.skin.label){fontSize=Mathf.Max(10,Mathf.RoundToInt(Screen.height/60f)),alignment=TextAnchor.MiddleCenter};style.normal.textColor=cyan;GUI.Label(new Rect(r.x,r.y-22,r.width,22),progress<.2f?"TARGET LOCKED":"SYSTEM SCAN  0101",style);}
  else {float t=(progress-.6f)/.4f;for(int i=0;i<32;i++){float u=((i*37)%101)/100f,v=((i*59)%97)/96f;float x=r.x+r.width*u+(u-.5f)*t*r.width*.9f,y=r.y+r.height*v-(.2f+v)*t*60;float size=Mathf.Max(2,Screen.height/170f)*(1-t*.7f);NeonDungeonUI.Solid(new Rect(x,y,size,size),new Color(i%2==0?.1f:1,i%2==0?1:.1f,1,1-t));}}
  GUI.matrix=matrix;GUI.color=color;GUI.depth=depth;
 }
}
