using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
[UnityEngine.Scripting.Preserve]
public sealed class OverclockHack:CombatHack {
 public OverclockSettings settings;
 public UnityEvent onPulse=new UnityEvent(),onSuccess=new UnityEvent(),onFailure=new UnityEvent();
 public override string DisplayName {get{return "OVERCLOCK";}}
 public override bool Active {get{return Model!=null&&!Model.Finished;}}
 public OverclockModel Model {get;private set;}
 public static double ClockNow {get{return System.Diagnostics.Stopwatch.GetTimestamp()/(double)System.Diagnostics.Stopwatch.Frequency;}}
 public override float AttackQuality(int performanceScore){return completedQuality;}
 Action<int> completed;
 InputAction keyboardAction,padAction;
 Key activeKey;OverclockPadButton activePad;
 bool keyboardArmed,padArmed,delivered;
 double pulseAt=-100,resultUntil;
 float completedQuality,sessionSuccess,sessionFailure;
 string result;
 HackDungeonBackdrop backdrop;
 JumpParallaxBackdrop arenaBackground;
 readonly Texture2D[] playerFrames=new Texture2D[8];
 Texture2D opponentFrame;
 int opponentKind;
 double shotStarted;
 public int VisualFrame {get{return Model!=null&&Model.Finished?8:Mathf.Clamp(1+Mathf.FloorToInt((Model!=null?Model.Charge:0)*7),1,7);}}
 public void SetOpponent(GameObject opponent){var visual=opponent!=null?opponent.GetComponent<GhostEnemyVisual>():null;opponentKind=visual!=null?visual.Kind:0;opponentFrame=Resources.Load<Texture2D>("UI/JumpEnemies/"+EnemyRoster.ResourceIds[opponentKind]+"_0")??Resources.Load<Texture2D>("Enemies/"+EnemyRoster.ResourceIds[opponentKind]+"/idle_0");}
 void PrepareVisuals(){for(int i=0;i<8;i++)if(playerFrames[i]==null)playerFrames[i]=Resources.Load<Texture2D>("UI/Overclock/frame_"+(i+1));arenaBackground=GetComponent<JumpParallaxBackdrop>()??gameObject.AddComponent<JumpParallaxBackdrop>();arenaBackground.Prepare();if(opponentFrame==null)SetOpponent(null);}

 public void BeginSession(Action<int> callback,double now){
  if(Active||ClockNow<resultUntil)return;
  if(settings==null)settings=OverclockSettings.Current;if(settings==null)settings=ScriptableObject.CreateInstance<OverclockSettings>();
  completed=callback;delivered=false;completedQuality=0;result=null;resultUntil=0;pulseAt=-100;
  sessionSuccess=Mathf.Max(0,settings.successQuality);sessionFailure=Mathf.Max(0,settings.failureQuality);
  Model=new OverclockModel(now,settings.durationSeconds,settings.requiredPresses);
  activeKey=settings.keyboardKey==Key.None?Key.Space:settings.keyboardKey;activePad=settings.gamepadButton;
  PrepareVisuals();EnableInput();
  backdrop=GetComponent<HackDungeonBackdrop>()??gameObject.AddComponent<HackDungeonBackdrop>();backdrop.Capture(GetComponentInChildren<Camera>());
 }
 public override void Begin(Action<int> callback){BeginSession(callback,ClockNow);}
 string PadPath {get{switch(activePad){case OverclockPadButton.East:return "buttonEast";case OverclockPadButton.West:return "buttonWest";case OverclockPadButton.North:return "buttonNorth";case OverclockPadButton.LeftShoulder:return "leftShoulder";case OverclockPadButton.RightShoulder:return "rightShoulder";default:return "buttonSouth";}}}
 string KeyLabel {get{return activeKey==Key.Space?"ESPACIO":KeyboardControl!=null?KeyboardControl.displayName:activeKey.ToString();}}
 string PadLabel {get{if(PadControl!=null)return PadControl.displayName;switch(activePad){case OverclockPadButton.East:return "B / ○";case OverclockPadButton.West:return "X / □";case OverclockPadButton.North:return "Y / △";case OverclockPadButton.LeftShoulder:return "LB / L1";case OverclockPadButton.RightShoulder:return "RB / R1";default:return "A / ×";}}}
 ButtonControl KeyboardControl {get{return Keyboard.current!=null?Keyboard.current[activeKey]:null;}}
 ButtonControl PadControl {get{return Gamepad.current!=null?Gamepad.current[PadPath] as ButtonControl:null;}}
 void EnableInput(){
  DisableInput();keyboardArmed=KeyboardControl==null||!KeyboardControl.isPressed;padArmed=PadControl==null||!PadControl.isPressed;
  keyboardAction=new InputAction("OverclockKeyboard",InputActionType.Button,"<Keyboard>/"+activeKey);
  padAction=new InputAction("OverclockGamepad",InputActionType.Button,"<Gamepad>/"+PadPath);
  keyboardAction.performed+=ctx=>{if(keyboardArmed){keyboardArmed=false;RegisterPress(ClockNow);}};keyboardAction.canceled+=ctx=>keyboardArmed=true;
  padAction.performed+=ctx=>{if(padArmed){padArmed=false;RegisterPress(ClockNow);}};padAction.canceled+=ctx=>padArmed=true;
  keyboardAction.wantsInitialStateCheck=true;padAction.wantsInitialStateCheck=true;
  keyboardAction.Enable();padAction.Enable();
 }
 void DisableInput(){if(keyboardAction!=null){keyboardAction.Disable();keyboardAction.Dispose();keyboardAction=null;}if(padAction!=null){padAction.Disable();padAction.Dispose();padAction=null;}}
 public bool RegisterPress(double now){if(!Active)return false;bool valid=Model.Press(now);if(valid){pulseAt=ClockNow;if(settings.playSounds)DungeonAudio.Play(settings.pulseSound);onPulse.Invoke();}if(Model!=null&&Model.Finished)Finish();return valid;}
 public void TickSession(double now){if(!Active)return;Model.Tick(now);if(Model.Finished)Finish();}
 void Update(){if(!Active)return;if(KeyboardControl==null||!KeyboardControl.isPressed)keyboardArmed=true;if(PadControl==null||!PadControl.isPressed)padArmed=true;TickSession(ClockNow);}
 void Finish(){
  if(delivered||Model==null||!Model.Finished)return;delivered=true;bool won=Model.Success;completedQuality=won?sessionSuccess:sessionFailure;
  result=won?"MAXIMUM POWER":"OVERLOAD FAILED";shotStarted=ClockNow;resultUntil=shotStarted+Mathf.Clamp(settings.resultDisplaySeconds,.2f,2f);
  DisableInput();if(backdrop!=null)backdrop.Restore();
  if(settings.playSounds)DungeonAudio.Play(won?settings.successSound:settings.failureSound);
  var callback=completed;completed=null;
  // Report performance once. Only DungeonPrototype resolves damage through CombatMath.
  if(callback!=null)callback(1);
  if(won)onSuccess.Invoke();else onFailure.Invoke();
 }
 void OnDisable(){DisableInput();if(backdrop!=null)backdrop.Restore();completed=null;Model=null;resultUntil=0;}
 void OnDestroy(){DisableInput();}
 void Fill(Rect r,Color c){GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=Color.white;}
 void OnGUI(){
  if(!Active&&ClockNow>=resultUntil)return;
  if(!Active){foreach(var other in GetComponents<CombatHack>())if(other!=this&&other.Active){resultUntil=0;return;}}
  var matrix=GUI.matrix;var color=GUI.color;var content=GUI.contentColor;var background=GUI.backgroundColor;int depth=GUI.depth;bool enabled=GUI.enabled;
  GUI.depth=-25;WristHackScreen.DrawFrame();WristHackScreen.ApplyContentMatrix();GUI.enabled=true;
  float charge=Model!=null?Model.Charge:0;float pulse=Mathf.Clamp01(1-(float)(ClockNow-pulseAt)/.12f)*settings.feedbackStrength;
  float shake=pulse*(2+charge*5);float zoom=1+pulse*.012f;
  GUI.matrix=GUI.matrix*Matrix4x4.TRS(new Vector3(Mathf.Sin((float)ClockNow*127)*shake,Mathf.Cos((float)ClockNow*103)*shake,0),Quaternion.identity,new Vector3(zoom,zoom,1));
  Fill(new Rect(0,0,1000,570),new Color(.006f,.009f,.027f));
  var title=new GUIStyle(GUI.skin.label){fontSize=34,alignment=TextAnchor.MiddleLeft};title.normal.textColor=WristHackScreen.Text;
  GUI.Label(new Rect(48,12,650,44),"OVERCLOCK.exe",title);
  var clock=new GUIStyle(title){fontSize=48,alignment=TextAnchor.MiddleRight};GUI.Label(new Rect(780,10,172,50),Model!=null?Model.Remaining.ToString("0.00")+" s":"0.00 s",clock);
  Rect bar=new Rect(50,70,675,22);Fill(new Rect(bar.x-3,bar.y-3,bar.width+6,bar.height+6),NeonDungeonUI.Pink);Fill(bar,new Color(.015f,.025f,.07f));
  for(int i=0;i<20;i++){float begin=i/20f,end=Mathf.Min(charge,(i+1)/20f);if(end>begin)Fill(new Rect(bar.x+bar.width*begin,bar.y+2,bar.width*(end-begin)-1,bar.height-4),Color.Lerp(NeonDungeonUI.Cyan,NeonDungeonUI.Pink,begin));}
  var info=new GUIStyle(title){fontSize=29,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(745,60,200,40),Mathf.RoundToInt(charge*100)+"%",info);
  Rect arena=new Rect(27,120,946,340);if(arenaBackground!=null)arenaBackground.Draw(arena,0);
  GUI.BeginGroup(arena);
  float ground=EnemyJumpLayout.GroundY;
  var pose=playerFrames[VisualFrame-1];float playerHeight=100;float playerWidth=pose!=null?playerHeight*pose.width/pose.height:125;
  Rect player=new Rect(150,ground-playerHeight,playerWidth,playerHeight);
  float enemyHeight=140;var jumpSettings=Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");
  if(jumpSettings!=null){enemyHeight=jumpSettings.referenceEnemyPixels;var sizes=EnemySizeSettings.Current;if(jumpSettings.matchEnemyReferenceSize&&sizes!=null)enemyHeight*=sizes.ScaleFor(opponentKind)/sizes.ScaleFor(0);enemyHeight=Mathf.Clamp(enemyHeight,Mathf.Min(jumpSettings.minimumEnemyPixels,jumpSettings.maximumEnemyPixels),Mathf.Max(jumpSettings.minimumEnemyPixels,jumpSettings.maximumEnemyPixels));}
  float enemyWidth=opponentFrame!=null?enemyHeight*opponentFrame.width/opponentFrame.height:enemyHeight;
  Rect enemy=new Rect(745-enemyWidth*.5f,ground-enemyHeight,enemyWidth,enemyHeight);
  float shotPhase=Model!=null&&Model.Finished?(float)((ClockNow-shotStarted)/Mathf.Clamp(settings.resultDisplaySeconds,.2f,2f)):0;
  GUI.color=Color.white;if(pose!=null)GUI.DrawTexture(player,pose,ScaleMode.StretchToFill,true);
  if(opponentFrame!=null){GUI.color=Model!=null&&Model.Finished&&shotPhase>.43f&&shotPhase<.7f?new Color(1.6f,1.6f,1.6f):Color.white;GUI.DrawTexture(enemy,opponentFrame,ScaleMode.StretchToFill,true);GUI.color=Color.white;}
  Vector2 hand=new Vector2(player.x+player.width*.94f,ground-playerHeight*.5f);
  if(Model!=null&&Model.Finished)OverclockEnergyVisual.Beam(hand,new Vector2(enemy.x+enemy.width*.35f,enemy.y+enemy.height*.53f),charge,shotPhase,ClockNow);
  else if(charge>0)OverclockEnergyVisual.Orb(hand,Mathf.Lerp(5,30,charge)*(1+pulse*.08f),ClockNow);
  GUI.EndGroup();
  GUI.Label(new Rect(40,470,920,38),Active?"CARGA: ["+KeyLabel+"] / MANDO ["+PadLabel+"]":result,info);
  var hint=new GUIStyle(info){fontSize=20};GUI.Label(new Rect(50,516,900,30),Active?"Pulsa rápido para aumentar la energía.":"DISPARO DE ENERGÍA",hint);
  if(pulse>0)Fill(new Rect(0,0,1000,570),new Color(1,.1f,.5f,pulse*.07f));
  GUI.matrix=matrix;GUI.color=color;GUI.contentColor=content;GUI.backgroundColor=background;GUI.depth=depth;GUI.enabled=enabled;
 }
}
