// Run as an execute_code method body in Unity MCP.
var cfg=UnityEngine.ScriptableObject.CreateInstance<EnemyJumpSettings>();
var checks=new System.Collections.Generic.List<string>();
if(EnemyJumpModel.Judge(.02,.045f,.105f)!=EnemyJumpModel.LandingGrade.Perfect || EnemyJumpModel.Judge(-.07,.045f,.105f)!=EnemyJumpModel.LandingGrade.Good || EnemyJumpModel.Judge(.13,.045f,.105f)!=EnemyJumpModel.LandingGrade.Miss) throw new System.Exception("Judgement windows");
checks.Add("PERFECT / GOOD / MISS and early/late windows");
foreach(float gravity in new float[]{2000,3000}) foreach(float frame in new float[]{1f/240,1f/60,1f/30}) foreach(float offset in new float[]{-.12f,0,.18f}) {
 cfg.gravity=gravity;cfg.jumpForce=gravity==3000 ? 1020 : 820;cfg.musicOffsetSeconds=offset;
 var m=new EnemyJumpModel(cfg,0,true);double first=m.NextArrival;
 double flight=m.GroundFlightSeconds;
 bool jumped=false;int priorCombo=0;
 for(double time=0;time<20 && !m.Finished;time+=frame) {
  if(!jumped && time>=first-flight) { m.Jump();jumped=true; }
  m.TickTo(time);
  if(m.Combo>priorCombo) { priorCombo=m.Combo;m.Jump(); }
 }
 if(!m.Won || m.Damage!=20) throw new System.Exception("Rhythm chain failed gravity="+gravity+" frame="+frame+" offset="+offset+" combo="+m.Combo+" reason="+m.Reason+" error="+m.LastLandingError);
}
checks.Add("18 chains: 30/60/240 FPS, offsets and both gravity settings, 20 damage");
cfg.musicOffsetSeconds=0;var idle=new EnemyJumpModel(cfg,0,true);idle.TickTo(5);
if(!idle.Finished || idle.Won || idle.Combo!=0) throw new System.Exception("Automatic jump or no MISS");
checks.Add("No input: no auto jump; missed enemy fails");
cfg.gravity=2000;cfg.jumpForce=820;
foreach(double delay in new double[]{0,.07,.15}) {
 var m=new EnemyJumpModel(cfg,0,true);double target=m.NextArrival;
 double flight=m.GroundFlightSeconds;
 bool jumped=false;
 for(double time=0;time<5 && !m.Finished && m.Combo==0;time+=1d/240) {
  if(!jumped && time>=target-flight+delay) { m.Jump();jumped=true; }
  m.TickTo(time);
 }
 var expected=delay==0 ? EnemyJumpModel.LandingGrade.Perfect : delay<.1 ? EnemyJumpModel.LandingGrade.Good : EnemyJumpModel.LandingGrade.Miss;
 if(m.LastGrade!=expected) throw new System.Exception("Actual landing grade failed: "+delay+" "+m.LastGrade);
}
checks.Add("Real collision landings: PERFECT, GOOD and MISS");
UnityEngine.Object.DestroyImmediate(cfg);return checks;
