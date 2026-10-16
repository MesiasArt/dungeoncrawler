using System;
using UnityEngine;
// Destination warnings are mounted on the sliding panel above its central symbol.
public sealed class DungeonDoorZoneSigns:MonoBehaviour {
 public TextMesh front,back;
 Camera view;Font font;Material ink;DungeonDoor door;Func<Color> frontColor,backColor;
 public void Build(DungeonDoor owner,Camera camera,Func<Color> forward,Func<Color> backward){
  if(front!=null)return;door=owner;view=camera;frontColor=forward;backColor=backward;
  font=Resources.Load<Font>("UI/Fonts/CodeboundPixelSpanish")??Resources.Load<Font>("UI/Fonts/ThaleahFat_TTF");
  var shader=Resources.Load<Shader>("Shaders/WorldTextDepth");if(font==null||shader==null)return;
  font.RequestCharactersInTexture("ZONA SEGURA PELIGROSA SIN DATOS",64);
  ink=new Material(shader);ink.mainTexture=font.material.mainTexture;
  front=CreateFace(-1);back=CreateFace(1);LateUpdate();
 }
 TextMesh CreateFace(int side){
  float height=door.panel.localScale.y;float travel=door.panel.position.y-transform.position.y-height*.5f;
  var go=new GameObject(side<0?"Zone warning front":"Zone warning back");go.transform.SetParent(transform,false);go.transform.localPosition=new Vector3(0,height*.76f+travel,side*.28f);go.transform.localRotation=Quaternion.Euler(0,side<0?0:180,0);
  var text=go.AddComponent<TextMesh>();text.font=font;text.fontSize=64;text.characterSize=.12f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.text="ZONA PELIGROSA";go.GetComponent<Renderer>().sharedMaterial=ink;
  var bounds=go.GetComponent<Renderer>().localBounds.size;float scale=Mathf.Min(1,door.doorwayWidth*.8f/Mathf.Max(.01f,bounds.x),.23f/Mathf.Max(.01f,bounds.y));go.transform.localScale=Vector3.one*scale;go.transform.SetParent(door.panel,true);return text;
 }
 void UpdateFace(TextMesh text,Color color,bool facing){if(text==null)return;bool known=color.r!=color.g;text.text=!known?"ZONA SIN DATOS":color.g>color.r?"ZONA SEGURA":"ZONA PELIGROSA";if(!known)color=new Color(1,.75f,.2f);var renderer=text.GetComponent<Renderer>();float distance=view!=null?Vector3.Distance(view.transform.position,renderer.bounds.center):0;color.a=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(door.doorwayWidth*1.5f,door.doorwayWidth*4,distance));text.color=color;
  bool visible=facing&&color.a>.01f;if(visible&&view!=null){var delta=renderer.bounds.center-view.transform.position;visible=!Physics.Raycast(view.transform.position,delta.normalized,Mathf.Max(0,delta.magnitude-.08f));}renderer.enabled=visible;
 }
 void LateUpdate(){if(front==null||back==null||view==null)return;if(font!=null&&ink!=null)ink.mainTexture=font.material.mainTexture;float side=transform.InverseTransformPoint(view.transform.position).z;UpdateFace(front,frontColor(),side<=0);UpdateFace(back,backColor(),side>0);}
 void OnDestroy(){if(ink!=null)Destroy(ink);}
}
