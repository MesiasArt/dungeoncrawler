var d=UnityEngine.Object.FindFirstObjectByType<DungeonPrototype>();var f=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
typeof(DungeonPrototype).GetMethod("ProcessIntroductionInput",f).Invoke(d,new object[]{false,true,0f});
var enemies=(System.Collections.Generic.List<UnityEngine.GameObject>)typeof(DungeonPrototype).GetField("enemyObjects",f).GetValue(d);
var hongo=enemies.Find(e=>e!=null && e.GetComponent<HongoEnemyVisual>()!=null);
typeof(DungeonPrototype).GetField("battle",f).SetValue(d,true);typeof(DungeonPrototype).GetField("activeEnemy",f).SetValue(d,enemies.IndexOf(hongo));typeof(DungeonPrototype).GetField("enemyHP",f).SetValue(d,32);
var hack=d.GetComponent<EnemyJumpHack>();hack.SetOpponent(hongo);hack.Begin(delegate(int damage) {UnityEngine.Debug.Log("JUMP_HIGH_RESULT damage="+damage);});
double first=hack.Model.NextArrival,interval=hack.Model.BeatSeconds*hack.settings.beatsPerEnemy;
int frame=-1,pressed=-1,seen=0;bool started=false,release=false;float peak=0;
UnityEditor.EditorApplication.CallbackFunction driver=null;
driver=delegate() {
 if(!UnityEditor.EditorApplication.isPlaying || !hack.Active || hack.Model.Finished) {
  UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());
  if(hack.Active)UnityEngine.Debug.Log("JUMP_HIGH_TEST combo="+hack.Model.Combo+" grade="+hack.Model.LastGrade+" error="+hack.Model.LastLandingError+" peak="+peak+" reason="+hack.Model.Reason);
  UnityEditor.EditorApplication.update-=driver;return;
 }
 if(frame==UnityEngine.Time.frameCount)return;frame=UnityEngine.Time.frameCount;
 peak=UnityEngine.Mathf.Max(peak,hack.Model.Y);
 if(release) {UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState());release=false;return;}
 double now=hack.Rhythm.SongTime;
 bool press=!started && now>=first-hack.Model.GroundFlightSeconds;
 if(press)started=true;
 if(started && hack.Model.Velocity<0 && pressed!=hack.Model.Combo && now>=first+interval*hack.Model.Combo-.04) {press=true;pressed=hack.Model.Combo;}
 if(press) {UnityEngine.InputSystem.InputSystem.QueueStateEvent(UnityEngine.InputSystem.Keyboard.current,new UnityEngine.InputSystem.LowLevel.KeyboardState(UnityEngine.InputSystem.Key.Space));release=true;}
 if(seen!=hack.Model.Combo) {seen=hack.Model.Combo;UnityEngine.Debug.Log("JUMP_HIGH_STOMP "+seen+" "+hack.Model.LastGrade+" error="+hack.Model.LastLandingError);}
};UnityEditor.EditorApplication.update+=driver;return "Live Hongo test with taller two-beat opening jump started.";