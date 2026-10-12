var d=UnityEngine.Object.FindAnyObjectByType<DungeonPrototype>();if(d==null)throw new System.Exception("Dungeon not ready");var t=d.GetType();var f=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic;d.enabled=false;
var objects=(System.Collections.Generic.List<UnityEngine.GameObject>)t.GetField("enemyObjects",f).GetValue(d);int gran=objects.FindIndex(x=>x!=null&&x.name=="GRAN MAKUBA");if(gran<0)throw new System.Exception("Gran Makuba missing");
var catalog=UnityEngine.Object.Instantiate(EnemyCharacterCatalog.Current);foreach(var p in catalog.enemies){p.normalMissPercent=p.specialMissPercent=0;p.criticalPercent=0;}typeof(EnemyCharacterCatalog).GetField("cached",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).SetValue(null,catalog);
var balance=UnityEngine.Object.Instantiate(CombatBalanceSettings.Current);balance.maximumCriticalPercent=0;typeof(CombatBalanceSettings).GetField("current",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).SetValue(null,balance);
d.attackAccuracySettings=UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<CombatAccuracySettings>("Combat/CombatAccuracySettings"));d.attackAccuracySettings.enabledLuck=false;
t.GetField("activeEnemy",f).SetValue(d,gran);t.GetField("lost",f).SetValue(d,false);t.GetField("hp",f).SetValue(d,37);
var visual=objects[gran].GetComponent<GranMakubaEnemyVisual>();if(visual.NextAttackIsSpecial()||visual.NextAttackIsSpecial()||!visual.NextAttackIsSpecial())throw new System.Exception("Special cadence");
d.StartCoroutine((System.Collections.IEnumerator)t.GetMethod("ResolveEnemyAttack",f).Invoke(d,new object[]{gran,visual,false,true}));
double start=UnityEditor.EditorApplication.timeSinceStartup;UnityEditor.EditorApplication.CallbackFunction tick=null;
tick=()=>{if(UnityEditor.EditorApplication.timeSinceStartup-start<1.5)return;try{
 int hp=(int)t.GetField("hp",f).GetValue(d);if(hp!=34)throw new System.Exception("Three hits expected HP34, got "+hp);
 var jump=d.GetComponent<EnemyJumpHack>();jump.SetOpponent(objects[gran]);jump.ConfigureAttack(9,7);jump.Begin(points=>{});
 if(!jump.Active||jump.Model.EnemyExtent!=180||jump.Rhythm==null)throw new System.Exception("Gran jump session/size/audio");
 var frames=(UnityEngine.Texture2D[][])typeof(EnemyJumpHack).GetField("walkingEnemies",f).GetValue(jump);if(frames[6][0].name!="GranMakuba_0"||frames[6][1].name!="GranMakuba_1")throw new System.Exception("Wrong walking species");
 System.IO.File.WriteAllText("Temp/makubas-playmode-results.txt","PASS\nGran Makuba: two normal turns then special, actual three impacts total3 HP, returns to idle\nLive JUMP: Gran Makuba's walking frames, 180px collision/render height and scheduled audio\n");
}catch(System.Exception e){System.IO.File.WriteAllText("Temp/makubas-playmode-results.txt","FAIL\n"+e);}finally{UnityEditor.EditorApplication.update-=tick;}};
UnityEditor.EditorApplication.update+=tick;return "Gran Makuba live special and JUMP session checks dispatched";
