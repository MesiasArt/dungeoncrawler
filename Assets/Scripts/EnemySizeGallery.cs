using UnityEngine;
[ExecuteAlways]
public sealed class EnemySizeGallery : MonoBehaviour
{
 public EnemySizeSettings sizes;
 [Tooltip("Guia visual de referencia en pies; no representa pies dibujados ni anatomia real.")]
 public bool animateIdle=true;
 public GhostEnemyVisual.Pose previewPose=GhostEnemyVisual.Pose.Idle;
 [Min(.05f)] public float frameSeconds=.32f;
 [Min(2)] public float spacing=4;
 [Range(3,100)] public int maximumGuideFeet=30;
 [HideInInspector] public Transform[] enemies;
 [HideInInspector] public Renderer[] sprites;
 [HideInInspector] public TextMesh[] names;
 [HideInInspector] public GameObject[] guides;
 [HideInInspector] public Camera previewCamera;
 [Range(2,20)] public float cameraZoom=5.5f;
 static readonly string[] ids=EnemyRoster.ResourceIds;
 static readonly string[] captions=EnemyRoster.Names;
 MaterialPropertyBlock properties;
 Texture2D[,] frames;
 void OnEnable(){frames=null;properties=new MaterialPropertyBlock();}
 void Update(){
  if(sizes==null)sizes=EnemySizeSettings.Current;
  if(sizes==null||enemies==null||sprites==null)return;
  if(frames==null||frames.GetLength(0)!=EnemyRoster.Count){frames=new Texture2D[EnemyRoster.Count,14];for(int k=0;k<EnemyRoster.Count;k++){for(int n=0;n<3;n++)frames[k,n]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/idle_"+n);frames[k,3]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/attack");if(k==5||k==6||k==8)frames[k,12]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/attack_1");frames[k,4]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/hurt");frames[k,5]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/death");if(k==8)frames[k,13]=Resources.Load<Texture2D>("Enemies/Makong/death_1");for(int n=0;n<6;n++)frames[k,6+n]=Resources.Load<Texture2D>("Enemies/"+ids[k]+"/special_"+n);}}
  for(int k=0;k<Mathf.Min(EnemyRoster.Count,enemies.Length);k++){
   if(enemies[k]==null||sprites[k]==null)continue;
   float scale=sizes.ScaleFor(k);enemies[k].localPosition=new Vector3((k-(enemies.Length-1)*.5f)*spacing,0,0);
   Vector2 size=k==8?MakongEnemyVisual.FullCanvas:EnemyRoster.CanvasSize(k),offset=k==8?MakongEnemyVisual.FootOffset:EnemyRoster.CanvasOffset(k);sprites[k].transform.localPosition=new Vector3(offset.x*scale,(size.y*.5f+offset.y)*scale,0);sprites[k].transform.localScale=new Vector3(size.x*scale,size.y*scale,1);
   int tick=animateIdle?Mathf.FloorToInt(Time.realtimeSinceStartup/Mathf.Max(.05f,frameSeconds)):0;
   int specialCount=k==3?6:k==4?3:k==6||k==8?5:0;
   int index=previewPose==GhostEnemyVisual.Pose.Idle?tick%3:previewPose==GhostEnemyVisual.Pose.Attack?((k==5||k==6||k==8)&&tick%2==1?12:3):previewPose==GhostEnemyVisual.Pose.Hurt?4:previewPose==GhostEnemyVisual.Pose.Death?(k==8&&tick%2==1?13:5):(specialCount>0?6+tick%specialCount:3);
   properties.SetTexture("_BaseMap",frames[k,index]!=null?frames[k,index]:frames[k,0]);sprites[k].SetPropertyBlock(properties);
   if(names!=null&&k<names.Length&&names[k]!=null)names[k].text=captions[k]+(k>=4?"\nx":"  x")+scale.ToString("0.00");
  }
  if(guides!=null)for(int i=0;i<guides.Length;i++)if(guides[i]!=null&&guides[i].activeSelf!=(i+1<=maximumGuideFeet))guides[i].SetActive(i+1<=maximumGuideFeet);
  if(previewCamera!=null){previewCamera.orthographicSize=cameraZoom;previewCamera.transform.position=new Vector3(0,cameraZoom-.6f,-14);
   float x=Mathf.Max(-((enemies.Length-1)*spacing*.5f+2),-cameraZoom*previewCamera.aspect+.15f);
   if(guides!=null)foreach(var guide in guides)if(guide!=null){var text=guide.GetComponentInChildren<TextMesh>(true);if(text!=null){text.anchor=TextAnchor.MiddleLeft;text.transform.localPosition=new Vector3(x,text.transform.localPosition.y,.1f);}}
  }
 }
}
