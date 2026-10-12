var checks=new System.Collections.Generic.List<string>();
for(int kind=0;kind<EnemyRoster.Count;kind++){
 string id=EnemyRoster.ResourceIds[kind];for(int i=0;i<3;i++)if(UnityEngine.Resources.Load<UnityEngine.Texture2D>("Enemies/"+id+"/idle_"+i)==null)throw new System.Exception("Missing idle "+id+i);
 foreach(string pose in new[]{"attack","hurt","death"})if(UnityEngine.Resources.Load<UnityEngine.Texture2D>("Enemies/"+id+"/"+pose)==null)throw new System.Exception("Missing pose "+id+pose);
 if(kind<EnemyRoster.Mimic)for(int i=0;i<2;i++)if(UnityEngine.Resources.Load<UnityEngine.Texture2D>("UI/JumpEnemies/"+id+"_"+i)==null)throw new System.Exception("Missing walking "+id+i);
 if(EnemyCharacterCatalog.Current.Find(EnemyRoster.Names[kind])==null)throw new System.Exception("Missing profile "+id);
}
checks.Add("8 visual profiles, 3 idles and attack/hurt/death; 7 species with 2 walking frames each");
foreach(var pair in new[]{new[]{4,3},new[]{6,5}})for(int i=0;i<pair[1];i++)if(UnityEngine.Resources.Load<UnityEngine.Texture2D>("Enemies/"+EnemyRoster.ResourceIds[pair[0]]+"/special_"+i)==null)throw new System.Exception("Missing special frame");
checks.Add("Lanza 3 special frames; Gran Makuba 5 special frames");
var cfg=UnityEngine.Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");var sizes=EnemySizeSettings.Current;int chains=0;
for(int kind=0;kind<EnemyRoster.Mimic;kind++)foreach(float frame in new[]{1f/30,1f/60,1f/240}){
 float extent=UnityEngine.Mathf.Clamp(cfg.referenceEnemyPixels*sizes.ScaleFor(kind)/sizes.ScaleFor(0),cfg.minimumEnemyPixels,cfg.maximumEnemyPixels);
 var m=new EnemyJumpModel(cfg,0,true,extent);double target=m.NextArrival,flight=m.GroundFlightSeconds;bool jumped=false;int combo=0;
 for(double time=0;time<22&&!m.Finished;time+=frame){if(!jumped&&time>=target-flight){m.Jump();jumped=true;}m.TickTo(time);if(m.Combo>combo){combo=m.Combo;m.Jump();}}
 if(!m.Won||m.Combo!=cfg.enemiesToWin)throw new System.Exception("Unreachable "+EnemyRoster.Names[kind]+" extent="+extent+" fps="+1/frame+" combo="+m.Combo+" reason="+m.Reason);chains++;
}
checks.Add(chains+" complete rhythm chains using each real enemy reference size at 30/60/240 FPS");
return string.Join("\n",checks.ToArray());
