using UnityEngine;
using System.Collections;
public partial class DungeonPrototype {
 int MakongCompanion(int index,int atTurn=int.MaxValue){foreach(int other in encounter){if(other==index||enemyObjects[other]==null||enemyLives[other]<=0)continue;var marker=enemyObjects[other].GetComponent<MakongSummonedHelper>();if(marker!=null&&(long)atTurn-marker.summonedOnTurn<MakongSpecialSettings.Current.helperMinimumAgeTurns)continue;var stats=enemyObjects[other].GetComponent<EnemyCombatStats>();var profile=EnemyCharacterCatalog.Current.FindId(stats.speciesId);if(profile!=null&&profile.category>=EnemyRank.Subjefe)continue;return other;}return -1;}
 IEnumerator ResolveMakongSpecial(int index,MakongEnemyVisual visual,bool? swallowOverride=null){
  var settings=MakongSpecialSettings.Current;int companion=MakongCompanion(index);var stats=enemyObjects[index].GetComponent<EnemyCombatStats>();
  bool swallow=companion>=0&&(swallowOverride??(enemyLives[index]<=stats.maximumHP*settings.healBelowFraction));
  Bark(EnemyBarkCategory.Special);
  if(swallow)yield return MakongSwallow(index,companion,visual);else yield return MakongJump(index,visual);
 }
 LineRenderer MakongLine(MakongEnemyVisual visual,string name,Color color,float width,bool loop=false){var root=new GameObject(name);root.transform.SetParent(transform,false);visual.OwnEffect(root);return root.AddComponent<MakongEffect>().Initialize(color,width,loop);}
 IEnumerator MakongJump(int index,MakongEnemyVisual visual){
  var s=MakongSpecialSettings.Current;Vector3 home=visual.transform.position;
  var circle=new GameObject("Makong — aviso de caída");circle.transform.SetParent(transform,false);circle.transform.position=home+Vector3.up*.025f;visual.OwnEffect(circle);var ring=circle.AddComponent<MakongEffect>().InitializeRing();ring.enabled=false;
  try {
   visual.Hold("jump_0");message="MAKONG toma impulso…";yield return new WaitForSeconds(s.windupSeconds);
   visual.Hold("jump_1");
   for(float t=0;t<s.ascentSeconds;t+=Time.deltaTime){visual.transform.position=home+Vector3.up*Mathf.Lerp(0,s.jumpHeight,Mathf.Clamp01(t/s.ascentSeconds));yield return null;}
   visual.transform.position=home+Vector3.up*s.jumpHeight;visual.ShowBody(false);ring.enabled=true;
   for(float t=0;t<s.aboveSeconds;t+=Time.deltaTime){float radius=Mathf.Lerp(s.circleStartRadius,s.circleEndRadius,Mathf.Clamp01(t/s.aboveSeconds));circle.transform.localScale=Vector3.one*radius;yield return null;}
   visual.Hold("jump_2");visual.ShowBody(true);
   for(float t=0;t<s.fallSeconds;t+=Time.deltaTime){visual.transform.position=home+Vector3.up*Mathf.Lerp(s.jumpHeight,0,Mathf.Clamp01(t/s.fallSeconds));yield return null;}
   visual.transform.position=home;ring.enabled=false;visual.Hold("jump_3");
   var stats=enemyObjects[index].GetComponent<EnemyCombatStats>();var profile=stats.snapshot;
   float miss=profile==null?0:CombatAccuracySettings.EffectiveMiss(profile.specialMissPercent,Accuracy.enabledLuck?profile.luck:0);
   ApplyEnemyHit(index,profile!=null?profile.specialDamage:stats.damage,RollAttackMiss(miss));
   view.GetComponent<CombatImpactFeedback>().PlayImpact(s.landingShakeAmplitude,s.landingShakeSeconds);
   yield return new WaitForSeconds(s.landingFrameSeconds);visual.Hold("jump_4");yield return new WaitForSeconds(s.landingFrameSeconds);
  } finally {if(visual!=null){visual.transform.position=home;visual.EndSpecial();}}
 }
 IEnumerator MakongSwallow(int index,int companion,MakongEnemyVisual visual){
  var s=MakongSpecialSettings.Current;var victim=enemyObjects[companion];Vector3 original=victim.transform.position;Vector3 scale=victim.transform.localScale;bool consumed=false;
  visual.FaceCompanion(original,view.transform.right);
  var tongueRoot=new GameObject("Makong — lengua separada");tongueRoot.transform.SetParent(transform,false);visual.OwnEffect(tongueRoot);var tongue=tongueRoot.AddComponent<MakongTongueVisual>();tongue.Initialize();
  float tongueScale=EnemySizeSettings.Current.ScaleFor(EnemyRoster.Makong)*visual.transform.lossyScale.y;
  float side=Vector3.Dot(original-visual.transform.position,view.transform.right)<0?-1:1;
  Vector3 mouth=visual.transform.position+(view.transform.right*side*s.tongueMouthOffset.x+Vector3.up*s.tongueMouthOffset.y)*tongueScale;Vector3 tip=original+Vector3.up*(victim.GetComponent<GhostEnemyVisual>().WorldHeight*.48f);
  try {
   message="MAKONG apunta a su compañero…";
   for(int n=0;n<2;n++){visual.Hold("swallow_prepare_"+n);yield return new WaitForSeconds(s.swallowFrameSeconds);}
   for(int n=0;n<2;n++){visual.Hold("swallow_tongue_"+n);yield return new WaitForSeconds(s.swallowFrameSeconds);}
   visual.Hold("swallow_tongue_2");Vector3 direction=(tip-mouth).normalized;float fullLength=Vector3.Distance(mouth,tip),startLength=Mathf.Min(fullLength,tongue.NativeLength*tongueScale);
   for(float t=0;t<s.tongueExtendSeconds;t+=Time.deltaTime){float length=Mathf.Lerp(startLength,fullLength,Mathf.Clamp01(t/s.tongueExtendSeconds));tongue.SetEndpoints(mouth,mouth+direction*length,view,tongueScale,s.tongueThickness);yield return null;}
   tongue.SetEndpoints(mouth,tip,view,tongueScale,s.tongueThickness);
   for(float t=0;t<s.retractSeconds;t+=Time.deltaTime){float k=Mathf.Clamp01(t/s.retractSeconds);visual.Hold("swallow_tongue_2");Vector3 attached=Vector3.Lerp(tip,mouth,Mathf.SmoothStep(0,1,k));victim.transform.position=original+(attached-tip);tongue.SetEndpoints(mouth,attached,view,tongueScale,s.tongueThickness);yield return null;}
   tongue.Hide();visual.Hold("swallow_eat_0");victim.SetActive(false);
   // The swallowed combatant is one defeat. Existing pending XP/drops still require victory.
   enemyLives[companion]=0;RecordDefeated(companion);
   enemyObjects[companion]=null;respawnAt[companion]=RespawnDeadline(companion);encounter.Remove(companion);encounterHomes.Remove(companion);encounterScales.Remove(companion);consumed=true;Destroy(victim);
   yield return new WaitForSeconds(s.swallowFrameSeconds);
   visual.Hold("swallow_eat_1");int before=enemyLives[index];int maximum=enemyObjects[index].GetComponent<EnemyCombatStats>().maximumHP;
   enemyLives[index]=Mathf.Min(maximum,before+s.Healing(maximum));if(activeEnemy==index)enemyHP=enemyLives[index];message="MAKONG devora a su compañero: +"+(enemyLives[index]-before)+" HP.";
   yield return new WaitForSeconds(s.swallowFrameSeconds*2);
  } finally {if(!consumed&&victim!=null){victim.SetActive(true);victim.transform.position=original;victim.transform.localScale=scale;}if(visual!=null)visual.EndSpecial();}
 }
}
