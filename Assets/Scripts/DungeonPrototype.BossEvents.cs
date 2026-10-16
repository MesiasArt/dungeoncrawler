using System;using UnityEngine;
public partial class DungeonPrototype {
 bool bossEventRunning;float bossEventRetryAt;
 void SignalBossState(int index,bool defeated){
  if(index<0||index>=slots.Count||enemyObjects[index]==null)return;var placement=slots[index].placement;var profile=placement!=null?placement.bossProfile:null;if(profile==null)return;var stats=enemyObjects[index].GetComponent<EnemyCombatStats>();if(!MakongLegacyBossAdapter.Accepts(profile,stats))return;
  string source="boss:"+(string.IsNullOrEmpty(placement.id)?stats.instanceId:placement.id)+":"+profile.id;
  foreach(var phase in profile.phases??new DungeonBossPhase[0])if(MakongLegacyBossAdapter.PhaseReached(phase,enemyLives[index],stats.maximumHP))QueueBossEvent(source+":"+phase.id,source,phase.onEntered);
  if(defeated)QueueBossEvent(source+":victory",source,profile.onVictory);
 }
 void QueueBossEvent(string receipt,string source,DungeonInteractionSettings target){
  if(interactionProgress.completedEvents.Contains(receipt))return;interactionProgress.completedEvents.Add(receipt);
  if(target!=null)interactionProgress.pendingBossEvents.Add(new DungeonBossEventRecord{id=receipt,profileId=target.id,sourceId=source+":"+receipt});
 }
 DungeonInteractionSettings ResolveBossEvent(string id){
  DungeonInteractionSettings found=null;foreach(var profile in Resources.LoadAll<DungeonInteractionSettings>(""))if(profile.id==id){if(found!=null&&found!=profile)return null;found=profile;}
  if(RegionMap!=null)foreach(var placement in RegionMap.placedEnemies??new PlacedEnemyDefinition[0]){var boss=placement!=null?placement.bossProfile:null;if(boss==null)continue;if(boss.onVictory!=null&&boss.onVictory.id==id)found=boss.onVictory;foreach(var phase in boss.phases??new DungeonBossPhase[0])if(phase!=null&&phase.onEntered!=null&&phase.onEntered.id==id)found=phase.onEntered;}
  return found;
 }
 bool UpdateBossEvents(){
  if(bossEventRunning)return worldDialogue.Active;if(battle||busy||moving||lost||cityActive||pauseOpen||worldDialogue.Active||IntroductionActive||Time.unscaledTime<bossEventRetryAt||interactionProgress.pendingBossEvents.Count==0)return false;
  var pending=interactionProgress.pendingBossEvents[0];var profile=ResolveBossEvent(pending.profileId);if(profile==null){bossEventRetryAt=Time.unscaledTime+2;return false;}
  int epoch=interactionEpoch;bossEventRunning=true;
  StartDungeonEvent(profile,pending.sourceId,success=>{if(epoch!=interactionEpoch)return;bossEventRunning=false;if(success)interactionProgress.pendingBossEvents.Remove(pending);else bossEventRetryAt=Time.unscaledTime+2;});
  return worldDialogue.Active||bossEventRunning;
 }
}
