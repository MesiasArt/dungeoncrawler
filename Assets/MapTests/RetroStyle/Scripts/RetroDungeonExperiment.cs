using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[Serializable]public class RetroDecoration {
 public string label;public Vector2Int cell;public Texture2D texture;[Range(.1f,1)]public float heightCells=.7f;
 [NonSerialized]Color32[] pixels;public Color32[] Pixels {get{if(pixels==null)pixels=texture.GetPixels32();return pixels;}}
}
public enum RetroSurfaceStyle { DrawnBlocks, NeonTextures }
public enum RetroExperimentMode { Pseudo3DDrawn, Current3DReference }
public sealed class RetroDungeonExperiment:MonoBehaviour {
 public DungeonMapSettings map;
 public RetroDecoration[] decorations=new RetroDecoration[0];
 public Camera referenceCamera;public GameObject referenceGeometry;public Transform[] referenceDoorPanels;public string[] referenceDoorIds;public Transform[] referenceSprites;
 [Header("Presentación experimental")]
 public RetroExperimentMode mode=RetroExperimentMode.Pseudo3DDrawn;
 public RetroSurfaceStyle surfaceStyle=RetroSurfaceStyle.DrawnBlocks;public Texture2D neonWallTexture,neonFloorTexture;[Range(1,6)]public int neonTilesPerCell=3;
 public Vector2Int internalResolution=new Vector2Int(256,192);public bool integerScale=true;
 [Range(1,10)]public int maximumDistance=10,depthLevels=10;
 [Range(1,4)]public int outlineThickness=2;
 [Range(0,.25f)]public float transitionSeconds=0;
 [Range(0,.3f)]public float shadowContrast=.16f;
 public Color wallColor=new Color(.09f,.28f,.65f),wallHighlight=new Color(.18f,.49f,.88f),floorColor=new Color(.48f,.08f,.29f),ceilingColor=new Color(.32f,.045f,.23f),doorColor=new Color(.085f,.17f,.36f),outlineColor=new Color(.008f,.012f,.035f),background=new Color(.01f,.012f,.026f);
 [Tooltip("Anchos de piezas en profundidades .5,1.5,2.5...; reducir progresivamente.")]
 public float[] perspectiveWidths={1.08f,.36f,.215f,.154f,.120f,.098f,.083f,.072f,.064f,.057f,.052f};
 public bool acceptInput=true;public bool showControls=true,showMiniMap=true;
 public Vector2Int Position {get;private set;}public int Heading {get;private set;}
 public RetroPerspectiveCanvas Canvas {get;private set;}public RenderTexture ReferenceTexture {get;private set;}
 public Texture CurrentImage {get{return mode==RetroExperimentMode.Pseudo3DDrawn?(Texture)Canvas.Image:ReferenceTexture;}}
 public int DoorVersion {get;private set;}
 public static readonly Vector2Int[] Directions={Vector2Int.up,Vector2Int.right,Vector2Int.down,Vector2Int.left};
 RawImage screenImage,transitionImage;
 readonly HashSet<string> openDoors=new HashSet<string>();Texture2D previousImage;bool dirty=true;double changeTime;int lastWidth,lastHeight;RetroExperimentMode appliedMode;
 public long ApproximateImageBytes {get{return Canvas.BufferBytes+(ReferenceTexture!=null?ReferenceTexture.width*(long)ReferenceTexture.height*8:0);}}
 void Awake(){var sentinel=GetComponent<DungeonPrototype>();if(sentinel==null)sentinel=gameObject.AddComponent<DungeonPrototype>();sentinel.enabled=false;sentinel.hideFlags=HideFlags.HideInInspector;CreatePresentation();Position=map.startCell;Heading=map.startHeading;Canvas=new RetroPerspectiveCanvas(256,192);appliedMode=(RetroExperimentMode)(-1);Refresh();}
 void OnValidate(){internalResolution.x=Mathf.Clamp(internalResolution.x,128,1280);internalResolution.y=Mathf.Clamp(internalResolution.y,96,960);dirty=true;}
 public bool DoorOpen(string id){return openDoors.Contains(id);}
 DungeonMapEntity DoorBetween(Vector2Int a,Vector2Int b){if(map.entities==null)return null;foreach(var e in map.entities.objects)if(e.kind==DungeonEntityKind.Door){var next=e.cell+Directions[e.heading];if(e.cell==a&&next==b||e.cell==b&&next==a)return e;}return null;}
 public bool CanStep(int step){var next=Position+Directions[Heading]*step;var door=DoorBetween(Position,next);return map.IsFloor(next)&&(door==null||DoorOpen(door.id));}
 public bool Step(int step){if(step==0||!CanStep(step))return false;Position+=Directions[Heading]*Math.Sign(step);dirty=true;Refresh();return true;}
 public void Turn(int direction){Heading=(Heading+direction+4)%4;dirty=true;Refresh();}
 public void SetPose(Vector2Int cell,int heading){if(!map.IsFloor(cell))throw new ArgumentException("Pose fuera de suelo");Position=cell;Heading=(heading+4)%4;dirty=true;Refresh();}
 public bool ToggleFacingDoor(){var door=DoorBetween(Position,Position+Directions[Heading]);if(door==null)return false;SetDoor(door.id,!DoorOpen(door.id));return true;}
 public void SetDoor(string id,bool open){if(open)openDoors.Add(id);else openDoors.Remove(id);DoorVersion++;dirty=true;Refresh();}
 public void SetSurfaceStyle(RetroSurfaceStyle value){surfaceStyle=value;dirty=true;Refresh();}
 public void SetMode(RetroExperimentMode value){mode=value;dirty=true;Refresh();}
 public void SetResolution(int width,int height){internalResolution=new Vector2Int(width,height);dirty=true;Refresh();}
 void Update(){if(acceptInput&&Keyboard.current!=null){var k=Keyboard.current;if(k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame)Turn(-1);else if(k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame)Turn(1);else if(k.wKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame)Step(1);else if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame)Step(-1);if(k.eKey.wasPressedThisFrame)ToggleFacingDoor();if(k.nKey.wasPressedThisFrame)SetSurfaceStyle(surfaceStyle==RetroSurfaceStyle.NeonTextures?RetroSurfaceStyle.DrawnBlocks:RetroSurfaceStyle.NeonTextures);if(k.tabKey.wasPressedThisFrame)SetMode(mode==RetroExperimentMode.Pseudo3DDrawn?RetroExperimentMode.Current3DReference:RetroExperimentMode.Pseudo3DDrawn);}
  var pad=Gamepad.current;if(acceptInput&&pad!=null){if(pad.dpad.left.wasPressedThisFrame)Turn(-1);else if(pad.dpad.right.wasPressedThisFrame)Turn(1);else if(pad.dpad.up.wasPressedThisFrame)Step(1);else if(pad.dpad.down.wasPressedThisFrame)Step(-1);if(pad.buttonSouth.wasPressedThisFrame)ToggleFacingDoor();}
  UpdatePresentation();if(dirty||mode!=appliedMode||internalResolution.x!=lastWidth||internalResolution.y!=lastHeight)Refresh();
 }
 public void Refresh(){if(Canvas==null||referenceCamera==null)return;int w=Mathf.Clamp(internalResolution.x,128,1280),h=Mathf.Clamp(internalResolution.y,96,960);if(w!=lastWidth||h!=lastHeight){Canvas.Resize(w,h);if(previousImage!=null)Destroy(previousImage);previousImage=new Texture2D(w,h,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};lastWidth=w;lastHeight=h;}
  if(mode==RetroExperimentMode.Pseudo3DDrawn){if(transitionSeconds>0){previousImage.SetPixels32(Canvas.Image.GetPixels32());previousImage.Apply(false,false);}Canvas.Compose(this);}
  bool reference=mode==RetroExperimentMode.Current3DReference;referenceGeometry.SetActive(reference);referenceCamera.gameObject.SetActive(reference);referenceCamera.enabled=reference;int rw=w*3,rh=h*3;if(reference&&(ReferenceTexture==null||ReferenceTexture.width!=rw||ReferenceTexture.height!=rh)){if(ReferenceTexture!=null){referenceCamera.targetTexture=null;ReferenceTexture.Release();Destroy(ReferenceTexture);}ReferenceTexture=new RenderTexture(rw,rh,24,RenderTextureFormat.ARGB32){name="Independent 3D reference",antiAliasing=1,filterMode=FilterMode.Point};ReferenceTexture.Create();}
  referenceCamera.targetTexture=reference?ReferenceTexture:null;referenceCamera.aspect=w/(float)h;referenceCamera.transform.position=new Vector3(Position.x*3,1.5f,Position.y*3);referenceCamera.transform.rotation=Quaternion.Euler(0,Heading*90,0);
  if(referenceDoorPanels!=null)for(int i=0;i<referenceDoorPanels.Length;i++){var panel=referenceDoorPanels[i];Vector3 p=panel.localPosition;p.y=DoorOpen(referenceDoorIds[i])?3.3f:1.5f;panel.localPosition=p;}
  if(referenceSprites!=null)foreach(var sprite in referenceSprites)if(sprite!=null)sprite.rotation=Quaternion.Euler(0,Heading*90,0);
  appliedMode=mode;dirty=false;changeTime=Time.realtimeSinceStartupAsDouble;
 }
 void CreatePresentation(){var root=new GameObject("2D presentation — screen overlay",typeof(RectTransform),typeof(UnityEngine.Canvas));root.transform.SetParent(transform,false);var ui=root.GetComponent<UnityEngine.Canvas>();ui.renderMode=RenderMode.ScreenSpaceOverlay;ui.pixelPerfect=true;ui.sortingOrder=10;var image=new GameObject("Composition",typeof(RectTransform),typeof(RawImage));image.transform.SetParent(root.transform,false);screenImage=image.GetComponent<RawImage>();screenImage.raycastTarget=false;var old=new GameObject("Brief transition",typeof(RectTransform),typeof(RawImage));old.transform.SetParent(root.transform,false);transitionImage=old.GetComponent<RawImage>();transitionImage.raycastTarget=false;}
 void PlaceImage(RawImage image,Rect r){var t=image.rectTransform;t.anchorMin=t.anchorMax=Vector2.zero;t.pivot=Vector2.zero;t.anchoredPosition=new Vector2(r.x,Screen.height-r.yMax);t.sizeDelta=r.size;}
 void UpdatePresentation(){if(screenImage==null||Canvas==null)return;Rect r=ImageRect();PlaceImage(screenImage,r);PlaceImage(transitionImage,r);screenImage.texture=CurrentImage;float alpha=mode==RetroExperimentMode.Pseudo3DDrawn&&transitionSeconds>0?Mathf.Clamp01(1-(float)((Time.realtimeSinceStartupAsDouble-changeTime)/transitionSeconds)):0;transitionImage.texture=previousImage;transitionImage.color=new Color(1,1,1,alpha);transitionImage.enabled=alpha>0;}
 public Rect ImageRect(){float scale=Mathf.Min(Screen.width/(float)lastWidth,(Screen.height-(showControls?85:0))/(float)lastHeight);if(integerScale&&scale>=1)scale=Mathf.Floor(scale);return new Rect((Screen.width-lastWidth*scale)*.5f,(Screen.height-(showControls?85:0)-lastHeight*scale)*.5f,lastWidth*scale,lastHeight*scale);}
 void OnGUI(){if(Canvas==null||RenderTexture.active==ReferenceTexture&&ReferenceTexture!=null)return;GUI.depth=-100;GUI.color=Color.white;
  if(showControls){float y=Screen.height-80;GUI.Label(new Rect(16,y,Screen.width-32,22),"CODEBOUND / EXPERIMENTO INDEPENDIENTE — "+(mode==RetroExperimentMode.Pseudo3DDrawn?"C · PSEUDO-3D / "+(surfaceStyle==RetroSurfaceStyle.NeonTextures?"NEÓN":"BLOQUES")+"":"A · MATERIALES ACTUALES EN MAPA DE PRUEBA"));if(GUI.Button(new Rect(16,y+26,180,27),"C · Pseudo-3D"))SetMode(RetroExperimentMode.Pseudo3DDrawn);if(GUI.Button(new Rect(204,y+26,180,27),"A · Referencia 3D"))SetMode(RetroExperimentMode.Current3DReference);if(GUI.Button(new Rect(394,y+26,110,27),"256 × 192"))SetResolution(256,192);if(GUI.Button(new Rect(512,y+26,110,27),"426 × 240"))SetResolution(426,240);GUI.Label(new Rect(640,y+20,Screen.width-650,50),"W/S: avanzar/retroceder · A/D: girar\nE: puerta · N: neón/bloques · TAB: 2D/3D · Mando: cruceta/A");}
  if(showMiniMap){float size=5;float x=Screen.width-map.rows[0].Length*size-10,y=10;for(int row=0;row<map.rows.Length;row++)for(int col=0;col<map.rows[row].Length;col++){GUI.color=Position==new Vector2Int(col,row)?Color.yellow:map.rows[row][col]=='.'?new Color(.08f,.32f,.48f,.85f):new Color(.01f,.02f,.07f,.9f);GUI.DrawTexture(new Rect(x+col*size,y+(map.rows.Length-1-row)*size,size-1,size-1),Texture2D.whiteTexture);}GUI.color=Color.white;}
 }
 public void SaveImage(string path){if(mode==RetroExperimentMode.Pseudo3DDrawn)System.IO.File.WriteAllBytes(path,Canvas.Image.EncodeToPNG());else{var active=RenderTexture.active;RenderTexture.active=ReferenceTexture;var copy=new Texture2D(ReferenceTexture.width,ReferenceTexture.height,TextureFormat.RGBA32,false);copy.ReadPixels(new Rect(0,0,copy.width,copy.height),0,0);copy.Apply();System.IO.File.WriteAllBytes(path,copy.EncodeToPNG());RenderTexture.active=active;Destroy(copy);}}
 void OnDestroy(){if(referenceCamera!=null)referenceCamera.targetTexture=null;if(ReferenceTexture!=null){ReferenceTexture.Release();Destroy(ReferenceTexture);}if(previousImage!=null)Destroy(previousImage);if(Canvas!=null)Canvas.Dispose();}
}
