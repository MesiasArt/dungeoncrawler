// Run as an execute_code method body in Unity MCP.
var cfg=ScriptableObject.CreateInstance<EnemyJumpSettings>();var checks=new System.Collections.Generic.List<string>();
Action<EnemyJumpModel> prepare=m=>{typeof(EnemyJumpModel).GetProperty("Y").SetValue(m,EnemyJumpModel.EnemySize+8f,null);typeof(EnemyJumpModel).GetProperty("Velocity").SetValue(m,-200f,null);m.Enemies.Add(160);};
var weak=new EnemyJumpModel(cfg);prepare(weak);weak.Tick(.045f);if(weak.Combo!=1||weak.Velocity>cfg.weakBounceForce)throw new Exception("Weak bounce failed");checks.Add("no press: low bounce");
var high=new EnemyJumpModel(cfg);prepare(high);high.Jump();high.Tick(.045f);if(high.Velocity<cfg.bounceForce*.9f)throw new Exception("Precise press failed");checks.Add("precise press: high bounce");
var grace=new EnemyJumpModel(cfg);prepare(grace);grace.Tick(.045f);grace.Jump();if(grace.Velocity<cfg.bounceForce*.9f)throw new Exception("Contact grace failed");checks.Add("just after contact: high bounce");
var late=new EnemyJumpModel(cfg);prepare(late);late.Tick(.2f);late.Jump();if(late.Velocity>cfg.bounceForce*.5f)throw new Exception("Late press boosted");checks.Add("late press: no boost");
var air=new EnemyJumpModel(cfg);air.Jump();air.Tick(.3f);prepare(air);air.Tick(.045f);if(air.Velocity>cfg.bounceForce*.5f)throw new Exception("Early press boosted");checks.Add("early/ground press: no automatic high bounce");
int wins=0;
for(int seed=0;seed<30;seed++){bool reachable=false;
for(float timing=.64f;timing<=.82f&&!reachable;timing+=.01f){var m=new EnemyJumpModel(cfg,seed);bool first=false;int pressed=-1;
for(int i=0;i<240*40&&!m.Finished;i++){if(!first&&m.Enemies.Count>0&&m.Enemies[0]<EnemyJumpModel.PlayerX+cfg.enemySpeed*timing){m.Jump();first=true;}
if(m.Velocity<0&&m.Y>EnemyJumpModel.EnemySize&&m.Y-EnemyJumpModel.EnemySize<-m.Velocity*.045f&&pressed!=m.Combo){m.Jump();pressed=m.Combo;}m.Tick(1f/240);}
if(m.Won&&m.Damage==20)reachable=true;}
if(reachable)wins++;}
if(wins!=30)throw new Exception("Timed chains: "+wins+"/30");checks.Add("30 seeds: ten timed presses -> victory / 20 damage");
var fail=new EnemyJumpModel(cfg);fail.Enemies.Add(160);fail.Tick(.01f);if(!fail.Finished||fail.Damage!=0)throw new Exception("Side failure damage");checks.Add("side failure: zero damage");

var partial=new EnemyJumpModel(cfg);
for(int i=0;i<3;i++) {
 typeof(EnemyJumpModel).GetProperty("Y").SetValue(partial,EnemyJumpModel.EnemySize+8f,null);
 typeof(EnemyJumpModel).GetProperty("Velocity").SetValue(partial,-200f,null);
 partial.Enemies.Add(160);partial.Tick(.045f);
}
typeof(EnemyJumpModel).GetProperty("Y").SetValue(partial,0f,null);
partial.Enemies.Add(160);partial.Tick(.01f);
if(!partial.Finished || partial.Damage!=6)throw new Exception("Partial damage discarded");
checks.Add("three stomps then failure: six damage"); UnityEngine.Object.DestroyImmediate(cfg);return checks;
