using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(20000)]
public sealed class CodeboundSystemMessages:MonoBehaviour {
 public SystemMessageSettings settings;
 sealed class Notice {public SystemMessageCode code;public string text;public DigitalIcon icon;public bool hack,warning;public float start,duration;}
 static float Now {get{return (float)Time.realtimeSinceStartupAsDouble;}}
 Notice current,status;bool statusVisible;
 public bool SuppressPanel {get;set;}
 public void SetStatus(string text){statusVisible=!string.IsNullOrWhiteSpace(text);if(!statusVisible)return;if(status==null||status.text!=text)status=new Notice{code=SystemMessageCode.Status,text=text,start=Now,duration=float.PositiveInfinity};}
 readonly Queue<Notice> pending=new Queue<Notice>();
 public string CurrentText {get{return current!=null?current.text:null;}}
 public SystemMessageCode CurrentCode {get{return current!=null?current.code:SystemMessageCode.Received;}}
 public int PresentedCount {get;private set;}
 public static CodeboundSystemMessages For(GameObject owner){var value=owner.GetComponent<CodeboundSystemMessages>();return value!=null?value:owner.AddComponent<CodeboundSystemMessages>();}
 void Awake(){settings=SystemMessageSettings.Current;}
 public void Show(SystemMessageCode code,string name="",int amount=0,DigitalIcon icon=DigitalIcon.None,string custom=null){
  if(settings!=null&&!settings.enabledMessages)return;
  bool hack=code>=SystemMessageCode.HackInitialized&&code<=SystemMessageCode.HackInterrupted||code>=SystemMessageCode.SpeedrunStart&&code<=SystemMessageCode.CheatLevelUp;bool warning=code==SystemMessageCode.BossWarning||code==SystemMessageCode.DeniedRoot;
  string template=!string.IsNullOrWhiteSpace(custom)?custom:settings!=null?settings.Text(code):SystemMessageSettings.DefaultText(code);
  var notice=new Notice{code=code,text=template.Replace("{name}",name??"").Replace("{amount}",amount.ToString()),icon=icon,hack=hack,warning=warning,duration=settings!=null?(hack?settings.hackSeconds:warning?settings.warningSeconds:settings.noticeSeconds):1.7f};
  if(hack||current==null||Now>=current.start+current.duration){if(hack)pending.Clear();Present(notice);}else if(pending.Count<8)pending.Enqueue(notice);
 }
 void Present(Notice notice){current=notice;current.start=Now;PresentedCount++;if(settings==null||settings.sounds)DungeonAudio.Play(DungeonAudio.Cue.Confirm);}
 void Update(){if(current!=null&&Now>=current.start+current.duration){current=null;if(pending.Count>0)Present(pending.Dequeue());}}
 void OnGUI(){var displayed=current??(statusVisible?status:null);if(displayed==null||SuppressPanel)return;HelmetCRTLayerMask.BeginGUI();
  var matrix=GUI.matrix;var color=GUI.color;
  float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f),width=Screen.width/scale,height=Screen.height/scale;
  GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));GUI.depth=-150;NeonDungeonUI.ApplyFont();
  float age=Now-displayed.start;float alpha=Mathf.Clamp01(age/.06f)*Mathf.Clamp01((displayed.duration-age)/.18f);
  var layout=Resources.Load<DungeonUILayout>("UI/DungeonUILayout");
  Rect r=layout!=null?layout.Resolve(layout.message,width,height):new Rect(width/2-245,height-118,490,98);
  var owner=GetComponent<DungeonPrototype>();var ui=BattleUISettings.Current;bool newBattle=owner!=null&&owner.LaboratoryBattle&&ui!=null&&ui.useNewArtwork;
  if(newBattle)r=new Rect(ui.message.x,height+ui.message.y,ui.message.width,ui.message.height);
  if(age<.07f)r.x+=Mathf.Sin(age*400)*3;
  GUI.color=new Color(1,1,1,alpha);if(newBattle)BattleUIArt.Draw(r,"SystemMessage");else{NeonDungeonUI.Solid(r,new Color(.005f,.012f,.035f,.97f));NeonDungeonUI.Outline(r,NeonDungeonUI.Cyan,1);NeonDungeonUI.Solid(new Rect(r.x,r.y,3,r.height),NeonDungeonUI.Pink);}
  var title=new GUIStyle(GUI.skin.label){fontSize=14};title.normal.textColor=NeonDungeonUI.Cyan;GUI.Label(new Rect(r.x+22,r.y+13,r.width-20,18),"SYSTEM MESSAGE",title);
  Color tint=displayed.code==SystemMessageCode.HackFailed||displayed.code==SystemMessageCode.HackInterrupted||displayed.code==SystemMessageCode.Denied||displayed.code==SystemMessageCode.DeniedRed||displayed.warning?new Color(1,.2f,.35f):new Color(.45f,1,.7f);
  float inset=displayed.icon!=DigitalIcon.None?47:13;if(displayed.icon!=DigitalIcon.None)GUI.DrawTexture(new Rect(r.x+10,r.y+29,28,28),DigitalIcons.Texture(displayed.icon),ScaleMode.ScaleToFit,true);
  var body=new GUIStyle(GUI.skin.label){fontSize=16,wordWrap=true,padding=new RectOffset(0,0,0,0)};body.normal.textColor=tint;
  int count=Mathf.Clamp(Mathf.FloorToInt(age*(settings!=null?settings.charactersPerSecond:160)),0,displayed.text.Length);string visible=displayed.text.Substring(0,count);GUI.Label(new Rect(r.x+inset+7,r.y+34,r.width-inset-27,r.height-42),visible,body);
  for(float y=r.y+2;y<r.yMax;y+=4)NeonDungeonUI.Solid(new Rect(r.x+1,y,r.width-2,1),new Color(.2f,.45f,.8f,.035f*alpha));
  GUI.matrix=matrix;GUI.color=color;GUI.depth=-150;
 }
}
