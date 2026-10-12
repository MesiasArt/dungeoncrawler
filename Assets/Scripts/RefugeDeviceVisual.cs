using System.Collections.Generic;
using UnityEngine;
// Pixel-edged geometry shares the dungeon palette; no imported concept-sheet background.
public sealed class RefugeDeviceVisual : MonoBehaviour {
 public bool portal;public bool transferActive;public string deviceName;
 readonly List<Material> materials=new List<Material>();readonly List<Transform> particles=new List<Transform>();
 Material cyan,rose,metal,dark;TextMesh screen;float saveUntil;bool saveSucceeded;
 Material Ink(Color color){var m=new Material(Resources.Load<Shader>("Shaders/DungeonNeon"));m.SetColor("_BaseColor",color);materials.Add(m);return m;}
 Transform Block(string label,Vector3 position,Vector3 size,Material material){var o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name=label;o.transform.SetParent(transform,false);o.transform.localPosition=position;o.transform.localScale=size;o.GetComponent<Renderer>().sharedMaterial=material;Destroy(o.GetComponent<Collider>());return o.transform;}
 TextMesh Text(string value,Vector3 position,float size){var o=new GameObject("Pixel display");o.transform.SetParent(transform,false);o.transform.localPosition=position;o.transform.localRotation=Quaternion.Euler(0,180,0);var text=o.AddComponent<TextMesh>();text.text=value;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.fontSize=48;text.characterSize=size;text.color=new Color(.1f,.95f,1);return text;}
 public void Build(){cyan=Ink(new Color(0,.8f,1));rose=Ink(new Color(1,.015f,.48f));metal=Ink(new Color(.055f,.065f,.12f));dark=Ink(new Color(.003f,.012f,.03f));
  if(portal)BuildPortal();else BuildTerminal();
 }
 void BuildTerminal(){
  Block("Wall backplate",new Vector3(0,1.55f,0),new Vector3(2.3f,2.8f,.18f),metal);
  Block("Screen housing",new Vector3(0,1.8f,.18f),new Vector3(2,1.9f,.28f),metal);
  Block("Black screen",new Vector3(0,1.8f,.34f),new Vector3(1.64f,1.52f,.035f),dark);
  for(int side=-1;side<=1;side+=2){Block("Cyan screen edge",new Vector3(side*.84f,1.8f,.37f),new Vector3(.055f,1.58f,.04f),cyan);Block("Cyan screen edge",new Vector3(0,1.8f+side*.79f,.37f),new Vector3(1.72f,.055f,.04f),cyan);Block("Pink corner",new Vector3(side*.99f,1.8f+side*.9f,.3f),new Vector3(.18f,.18f,.12f),rose);}
  for(int i=0;i<5;i++)Block("Scanline",new Vector3(0,1.35f+i*.22f,.365f),new Vector3(1.55f,.012f,.012f),metal);
  // Block-built disk icon keeps its edges crisp at any render resolution.
  Block("Disk icon",new Vector3(0,2.03f,.4f),new Vector3(.56f,.52f,.035f),cyan);
  Block("Disk notch",new Vector3(.06f,2.18f,.43f),new Vector3(.24f,.16f,.02f),dark);
  Block("Disk label",new Vector3(0,1.88f,.43f),new Vector3(.35f,.13f,.02f),dark);
  screen=Text("SAVE",new Vector3(0,1.43f,.45f),.1f);
  Block("Lower console",new Vector3(0,.56f,.18f),new Vector3(1.5f,.55f,.32f),metal);
  Block("Pink confirmation button",new Vector3(.45f,.56f,.38f),new Vector3(.22f,.22f,.09f),rose);
  for(int i=0;i<3;i++)Block("Status light",new Vector3(-1.02f,1.35f+i*.32f,.18f),new Vector3(.13f,.2f,.12f),rose);
 }
 void BuildPortal(){
  var cylinder=GameObject.CreatePrimitive(PrimitiveType.Cylinder);cylinder.name="Octagonal transfer pedestal";cylinder.transform.SetParent(transform,false);cylinder.transform.localPosition=Vector3.up*.14f;cylinder.transform.localScale=new Vector3(3.3f,.14f,3.3f);cylinder.GetComponent<Renderer>().sharedMaterial=metal;Destroy(cylinder.GetComponent<Collider>());
  for(int i=0;i<8;i++){float angle=i*Mathf.PI/4;var p=new Vector3(Mathf.Sin(angle)*1.45f,.33f,Mathf.Cos(angle)*1.45f);var rim=Block("Segmented neon rim",p,new Vector3(.95f,.13f,.16f),i%2==0?rose:cyan);rim.localRotation=Quaternion.Euler(0,i*45,0);var plate=Block("Armored rim",p+Vector3.up*.15f,new Vector3(.45f,.17f,.3f),metal);plate.localRotation=rim.localRotation;}
  for(int i=0;i<4;i++){var line=Block("Transfer diamond",new Vector3(0,.32f,0)+Quaternion.Euler(0,i*90+45,0)*Vector3.forward*.48f,new Vector3(.98f,.035f,.055f),cyan);line.localRotation=Quaternion.Euler(0,i*90+45,0);}
  Block("Core",new Vector3(0,.34f,0),new Vector3(.24f,.04f,.24f),rose);
  Block("Activation console",new Vector3(1.65f,.57f,.4f),new Vector3(.45f,1.1f,.45f),metal);
  Block("Pink activation button",new Vector3(1.65f,1.14f,.4f),new Vector3(.3f,.1f,.3f),rose);
  Block("Console display",new Vector3(1.65f,.85f,.64f),new Vector3(.29f,.19f,.035f),cyan);
  for(int i=0;i<12;i++){float angle=i*2.39996f;particles.Add(Block("Transfer pixel",new Vector3(Mathf.Sin(angle)*.65f,.5f+i*.2f,Mathf.Cos(angle)*.65f),new Vector3(.065f,.14f,.065f),i%2==0?cyan:rose));}
 }
 public void ShowSaveResult(bool success){saveSucceeded=success;saveUntil=Time.unscaledTime+2.5f;}
 void Update(){if(portal){for(int i=0;i<particles.Count;i++){particles[i].gameObject.SetActive(transferActive);var p=particles[i].localPosition;p.y=.4f+Mathf.Repeat(Time.time*.5f+i*.23f,2.7f);particles[i].localPosition=p;}}
  else if(screen!=null)screen.text=Time.unscaledTime<saveUntil?(saveSucceeded?"GUARDADO":"ERROR"):"SAVE";
 }
 void OnDestroy(){foreach(var m in materials)if(m!=null)Destroy(m);}
}
