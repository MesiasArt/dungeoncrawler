using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DigitalDoorAccess { Automatic, Cyan, Red, Green, Blue, Yellow, Root }
// Cosmetic child of the existing DungeonDoor: no colliders or persistence of its own.
public sealed class DigitalDoorVisual:MonoBehaviour {
 public DigitalDoorAccess access;public bool bossGate,effects=true,requiresToken=true;
 DungeonDoor door;Transform backHologram,backRing,backWarning;Transform hologram,ring,warning;TextMesh warningText;Vector3 warningHome;bool built,unlocked,wasOpen,burst;float unlockStarted=-100;
 readonly List<Material> owned=new List<Material>();readonly List<Transform> fragments=new List<Transform>();
 Color color;Material metal,glow;float width,height;
 void Start(){if(!built){var owner=GetComponent<DungeonDoor>();if(owner!=null&&owner.panel!=null){owner.Initialize();Build(owner,access,bossGate,effects,requiresToken);}}}
 public static DigitalDoorAccess AccessFor(string key){return key=="key.red"?DigitalDoorAccess.Red:key=="key.green"?DigitalDoorAccess.Green:key=="key.master"?DigitalDoorAccess.Root:DigitalDoorAccess.Cyan;}
 public static DigitalIcon TokenIcon(DigitalDoorAccess access){return access==DigitalDoorAccess.Root?DigitalIcon.RootKey:access==DigitalDoorAccess.Red?DigitalIcon.RedToken:access==DigitalDoorAccess.Green?DigitalIcon.GreenToken:access==DigitalDoorAccess.Yellow?DigitalIcon.YellowToken:DigitalIcon.BlueToken;}
 public void Build(DungeonDoor owner,DigitalDoorAccess style,bool boss,bool animate,bool tokenRequired=true){if(built)return;built=true;door=owner;requiresToken=tokenRequired;access=style;bossGate=boss;effects=animate;color=DigitalIcons.ColorFor(TokenIcon(access));if(access==DigitalDoorAccess.Cyan)color=new Color(.05f,.6f,1);
  width=door.doorwayWidth;height=door.panel.localScale.y;
  metal=Material(Resources.Load<Shader>("Shaders/DungeonMetalToon")??Shader.Find("Universal Render Pipeline/Unlit"),new Color(.035f,.06f,.12f));metal.SetTexture("_BaseMap",Texture2D.whiteTexture);if(metal.HasProperty("_ContinuousWallGrid"))metal.SetFloat("_ContinuousWallGrid",0);if(metal.HasProperty("_ContinuousFloorGrid"))metal.SetFloat("_ContinuousFloorGrid",0);
  glow=Material(Resources.Load<Shader>("Shaders/MakongEffect")??Shader.Find("Universal Render Pipeline/Unlit"),color);
  for(int side=-1;side<=1;side+=2){Cube(transform,new Vector3(side*(width*.5f-.14f),height*.5f,0),new Vector3(.28f,height,.32f),metal);Cube(transform,new Vector3(side*(width*.5f-.14f),height*.5f,-.18f),new Vector3(.055f,height*.8f,.025f),glow);Cube(transform,new Vector3(side*(width*.5f-.38f),height*.5f,0),new Vector3(.12f,height*.88f,.22f),metal);}
  Cube(transform,new Vector3(0,height-.12f,0),new Vector3(width,.24f,.35f),metal);
  for(int n=0;n<5;n++)Cube(transform,new Vector3((n-2)*.19f,height-.12f,-.2f),new Vector3(.12f,.07f,.025f),glow);
  for(int side=-1;side<=1;side+=2){Cube(door.panel,new Vector3(side*.46f,0,-.6f),new Vector3(.018f,.92f,.1f),glow);for(int n=0;n<3;n++)Cube(door.panel,new Vector3(side*.27f,(n-1)*.22f,-.6f),new Vector3(.3f,.012f,.1f),glow);}
  var icon=new GameObject("Security hologram");hologram=icon.transform;hologram.SetParent(transform,false);hologram.localPosition=new Vector3(0,height*.53f,-.2f);
  var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);quad.name="Lock pixels";quad.transform.SetParent(hologram,false);quad.transform.localScale=Vector3.one*(bossGate?1.15f:.85f);RemoveCollider(quad);var material=Material(Resources.Load<Shader>("Shaders/EnemySprite"),new Color(color.r*2,color.g*2,color.b*2,1));material.SetTexture("_BaseMap",DigitalIcons.Texture(requiresToken?DigitalIcon.Lock:DigitalIcon.Unlocked));quad.GetComponent<Renderer>().sharedMaterial=material;
  ring=new GameObject("Access ring").transform;ring.SetParent(hologram,false);for(int n=0;n<24;n++){float a=n*Mathf.PI/12;var pixel=Cube(ring,new Vector3(Mathf.Cos(a)*.65f,Mathf.Sin(a)*.65f,0),new Vector3(.11f,.04f,.03f),glow);pixel.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg+90);}
  if(bossGate){var crown=GameObject.CreatePrimitive(PrimitiveType.Quad);crown.name="Root crown";crown.transform.SetParent(hologram,false);crown.transform.localPosition=Vector3.up*.85f;crown.transform.localScale=Vector3.one*.5f;RemoveCollider(crown);var cm=Material(Resources.Load<Shader>("Shaders/EnemySprite"),Color.white);cm.SetTexture("_BaseMap",DigitalIcons.Texture(DigitalIcon.RootKey));crown.GetComponent<Renderer>().sharedMaterial=cm;
   warning=new GameObject("WARNING hologram").transform;warning.SetParent(transform,false);warning.localPosition=new Vector3(0,height-.55f,-.22f);warningHome=warning.localPosition;warningText=warning.gameObject.AddComponent<TextMesh>();warningText.text="! WARNING";warningText.anchor=TextAnchor.MiddleCenter;warningText.alignment=TextAlignment.Center;warningText.fontSize=64;warningText.characterSize=.13f;warningText.color=new Color(1,.08f,.2f);var font=Resources.Load<Font>("UI/Fonts/ThaleahFat_TTF");if(font!=null){warningText.font=font;var fm=Material(Resources.Load<Shader>("Shaders/WorldTextDepth"),Color.white);fm.mainTexture=font.material.mainTexture;warningText.GetComponent<Renderer>().sharedMaterial=fm;}}
  // Mirror the front artwork as a complete face, keeping text readable from behind.
  backHologram=BackFace(hologram);backRing=backHologram.Find("Access ring");
  if(warning!=null)backWarning=BackFace(warning);
  var details=new List<Transform>();
  foreach(Transform child in transform)if(child.name=="Digital door detail"&&child.localPosition.z<-.1f)details.Add(child);
  foreach(var child in details)BackFace(child);
  details.Clear();
  foreach(Transform child in door.panel)if(child.name=="Digital door detail"&&child.localPosition.z<0)details.Add(child);
  foreach(var child in details)BackFace(child);
  SyncImmediate(door.IsOpen);
 }
 Transform BackFace(Transform front){var back=Instantiate(front.gameObject,front.parent,false).transform;back.name=front.name+" (back)";var p=front.localPosition;back.localPosition=new Vector3(-p.x,p.y,-p.z);back.localRotation=Quaternion.Euler(0,180,0)*front.localRotation;return back;}
 void SetHologramsVisible(bool visible){hologram.gameObject.SetActive(visible);if(backHologram!=null)backHologram.gameObject.SetActive(visible);if(warning!=null)warning.gameObject.SetActive(visible);if(backWarning!=null)backWarning.gameObject.SetActive(visible);}
 Material Material(Shader shader,Color tint){var material=new Material(shader??Shader.Find("Universal Render Pipeline/Unlit"));material.SetColor("_BaseColor",tint);if(material.HasProperty("_Color"))material.SetColor("_Color",tint);owned.Add(material);return material;}
 Transform Cube(Transform parent,Vector3 position,Vector3 scale,Material material){var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);cube.name="Digital door detail";cube.transform.SetParent(parent,false);cube.transform.localPosition=position;cube.transform.localScale=scale;cube.GetComponent<Renderer>().sharedMaterial=material;RemoveCollider(cube);return cube.transform;}
 void RemoveCollider(GameObject root){var c=root.GetComponent<Collider>();if(c!=null){c.enabled=false;Destroy(c);}}
 public void SyncImmediate(bool open){if(!built)return;wasOpen=open;unlocked=open;burst=false;foreach(var fragment in fragments)if(fragment!=null)Destroy(fragment.gameObject);fragments.Clear();SetHologramsVisible(!open);}
 public IEnumerator Unlock(){if(!built||unlocked)yield break;unlocked=true;burst=effects;unlockStarted=Time.time;if(effects){for(int n=0;n<18;n++){float a=n*Mathf.PI*2/18;var bit=Cube(transform,new Vector3(Mathf.Cos(a)*.4f,height*.53f+Mathf.Sin(a)*.4f,-.24f),Vector3.one*.055f,glow);fragments.Add(bit);}for(float elapsed=0;elapsed<.35f;elapsed+=Time.deltaTime){if(ring!=null)ring.Rotate(0,0,Time.deltaTime*720);if(backRing!=null)backRing.Rotate(0,0,Time.deltaTime*720);foreach(var fragment in fragments)if(fragment!=null)fragment.localPosition+=((fragment.localPosition-new Vector3(0,height*.53f,0)).normalized+Vector3.up*.2f)*Time.deltaTime*2;yield return null;}}SetHologramsVisible(false);foreach(var bit in fragments)if(bit!=null)Destroy(bit.gameObject);fragments.Clear();burst=false;}
 void Update(){if(!built||door==null)return;if(door.IsOpen!=wasOpen&&!door.IsOpening)SyncImmediate(door.IsOpen);if(!unlocked&&effects){if(ring!=null)ring.Rotate(0,0,Time.deltaTime*(bossGate?24:12));if(backRing!=null)backRing.Rotate(0,0,Time.deltaTime*(bossGate?24:12));}if(warning!=null&&warning.gameObject.activeSelf){float t=Time.unscaledTime;bool glitch=effects&&(t%7<.10f||burst);warning.localPosition=warningHome+(glitch?Vector3.right*Mathf.Sin(t*130)*.055f:Vector3.zero);warningText.color=glitch?new Color(1,.25f,.4f,.5f):new Color(1,.08f,.2f);if(backWarning!=null){backWarning.localPosition=new Vector3(-warning.localPosition.x,warning.localPosition.y,-warning.localPosition.z);backWarning.GetComponent<TextMesh>().color=warningText.color;}}}
 void OnDestroy(){foreach(var material in owned)if(material!=null)Destroy(material);}
}
