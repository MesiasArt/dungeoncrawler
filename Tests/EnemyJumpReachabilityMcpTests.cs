// Execute this C# snippet with Unity MCP execute_code in Edit or Play Mode.
var cfg=ScriptableObject.CreateInstance<EnemyJumpSettings>(); var victories=0;
for(int seed=0;seed<30;seed++) { bool reachable=false; for(float timing=.68f;timing<=.83f && !reachable;timing+=.01f) { var m=new EnemyJumpModel(cfg,seed); bool jumped=false;
for(int f=0;f<240*30 && !m.Finished;f++) { if(!jumped && m.Enemies.Count>0 && m.Enemies[0]<EnemyJumpModel.PlayerX+cfg.enemySpeed*timing) {m.Jump(); jumped=true;} if(m.Velocity<0 && m.Y>EnemyJumpModel.EnemySize && m.Y-EnemyJumpModel.EnemySize<-m.Velocity*.04f) m.Jump(); m.Tick(1f/240); }
if(m.Won) reachable=true; } if(reachable) victories++;
} UnityEngine.Object.DestroyImmediate(cfg); if(victories!=30) throw new Exception("Unreachable chain: "+victories+"/30"); return new {randomSeeds=30,victories=victories};