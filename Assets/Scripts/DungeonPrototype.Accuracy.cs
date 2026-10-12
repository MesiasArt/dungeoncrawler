using UnityEngine;
using System.Collections;
public partial class DungeonPrototype
{
 public CombatAccuracySettings attackAccuracySettings;
 CombatAccuracySettings Accuracy { get { if(attackAccuracySettings==null)attackAccuracySettings=Resources.Load<CombatAccuracySettings>("Combat/CombatAccuracySettings");if(attackAccuracySettings==null)attackAccuracySettings=ScriptableObject.CreateInstance<CombatAccuracySettings>();return attackAccuracySettings; } }
 static bool RollAttackMiss(float percent){return CombatAccuracySettings.Misses(percent,Random.value);}
 IEnumerator ResolveEnemyAttack(int index,GhostEnemyVisual visual,bool grenade,bool spin)
 {
  if(visual is MakongEnemyVisual){var makong=(MakongEnemyVisual)visual;var decision=DecideMakong(index,makong);if(decision!=MakongAction.Normal){yield return RunMakongAction(index,makong,decision);yield break;}spin=false;}
  Bark(grenade||spin?EnemyBarkCategory.Special:EnemyBarkCategory.Attack);
  string name=enemyObjects[index].name;
  var profile=enemyObjects[index].GetComponent<EnemyCombatStats>().snapshot;
  float missPercent=Accuracy.CharacterMissPercent(name,grenade||spin);
  if(Accuracy.enabledLuck&&profile!=null){float chance=grenade||spin?profile.specialMissPercent:profile.normalMissPercent;missPercent=CombatAccuracySettings.EffectiveMiss(chance,profile.luck);}
  bool grenadeMissed=grenade&&RollAttackMiss(missPercent);
  int count=spin?Mathf.Max(1,profile!=null?profile.specialHits:EnemySettings.bigTapakuSpecialHits):1;
  float interval=Mathf.Max(.03f,profile!=null?profile.specialHitInterval:EnemySettings.bigTapakuSpecialHitInterval);
  float firstDelay=spin&&visual is BigTapakuEnemyVisual?Mathf.Max(.05f,((BigTapakuEnemyVisual)visual).specialFrameSeconds):visual is MakongEnemyVisual?MakongSpecialSettings.Current.normalImpactDelay:.05f;
  float duration=0;
  if(grenade)yield return ((SoldierEnemyVisual)visual).ThrowGrenade(view,grenadeMissed);
  else {
   if(visual!=null&&spin)visual.minimumSpecialSeconds=firstDelay+(count-1)*interval+.15f;
   duration=visual!=null?visual.Play(spin?GhostEnemyVisual.Pose.Special:GhostEnemyVisual.Pose.Attack):0;
   yield return new WaitForSeconds(Mathf.Min(firstDelay,duration));
  }
  int damage=Mathf.Max(0,spin?(profile!=null?profile.specialDamage:EnemySettings.bigTapakuSpecialDamage):grenade?(profile!=null?profile.specialDamage:4):enemyObjects[index].GetComponent<EnemyCombatStats>().damage);

  for(int hit=0;hit<count;hit++){
   if(lost||hp<=0)break;
   bool missed=grenade?grenadeMissed:RollAttackMiss(missPercent);
   string hitText=count>1?"golpe "+(hit+1)+"/"+count+", ":"";
   ApplyEnemyHit(index,damage,missed,grenade,hitText);
   if(lost)break;
   if(hit<count-1)yield return new WaitForSeconds(interval);
  }
  yield return new WaitForSeconds(Mathf.Max(.15f,duration-firstDelay-(count-1)*interval));
 }
 void ApplyEnemyHit(int index,int damage,bool missed,bool grenade=false,string hitText="")
 {
  var stats=enemyObjects[index].GetComponent<EnemyCombatStats>();var profile=stats.snapshot;string name=enemyObjects[index].name;
   if(grenade)DungeonAudio.Play(DungeonAudio.Cue.Explosion);
   if(missed)message=name+": "+hitText+"falló.";
   else {
    bool critical=profile!=null&&CombatMath.CriticalRoll(CombatAccuracySettings.EffectiveBonus(profile.criticalPercent,profile.luck),Random.value);
    int hitDamage=CombatMath.Damage(damage,PlayerDefense,1,critical,profile!=null?profile.criticalMultiplier:1,CombatBalanceSettings.Current);
    if(hitDamage>0){if(!grenade)DungeonAudio.Play(DungeonAudio.Cue.PlayerHurt);hp=Mathf.Max(0,hp-hitDamage);view.GetComponent<CombatImpactFeedback>().Play(true);}
    message=name+": "+hitText+(critical?"crítico, ":"")+"pierdes "+hitDamage+" HP.";
    if(hp==0){DungeonAudio.Play(DungeonAudio.Cue.PlayerDeath);lost=true;message="Has caído. Pulsa Reiniciar.";return;}
   }

 }

}
