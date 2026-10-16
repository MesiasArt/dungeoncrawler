using System;using System.Collections.Generic;using UnityEngine;
public partial class DungeonPrototype {
 DungeonInteractionProgress interactionProgress=new DungeonInteractionProgress();readonly HashSet<string> runningInteractions=new HashSet<string>(),interactionAdapterBypass=new HashSet<string>();int interactionEpoch;
 void NormalizeInteractionProgress(){bossEventRunning=false;bossEventRetryAt=0;if(interactionProgress==null)interactionProgress=new DungeonInteractionProgress();if(interactionProgress.pendingBossEvents==null)interactionProgress.pendingBossEvents=new List<DungeonBossEventRecord>();if(interactionProgress.defeatedEnemyIds==null)interactionProgress.defeatedEnemyIds=new List<string>();if(interactionProgress.switches==null)interactionProgress.switches=new List<DungeonSwitchState>();if(interactionProgress.completedEvents==null)interactionProgress.completedEvents=new List<string>();if(interactionProgress.unlockedConnections==null)interactionProgress.unlockedConnections=new List<string>();if(interactionProgress.rewardReceipts==null)interactionProgress.rewardReceipts=new List<string>();if(interactionProgress.executions==null)interactionProgress.executions=new List<DungeonEventExecution>();runningInteractions.Clear();interactionAdapterBypass.Clear();interactionEpoch++;}
 DungeonInteractionProgress ConditionProgress(DungeonMapSettings source){if(source==null||source==ActiveMap)return interactionProgress;var snapshot=floorSnapshots.Find(s=>s!=null&&s.mapId==source.mapId);return snapshot!=null?snapshot.interactions:null;}
 void RefreshDungeonSwitchVisuals(){foreach(var component in GetComponentsInChildren<DungeonSwitch>()){var renderer=component.GetComponentInChildren<Renderer>();if(renderer!=null&&renderer.material.HasProperty("_BaseColor"))renderer.material.SetColor("_BaseColor",IsDungeonSwitchActive(component.id)?Color.green:Color.gray);}}
 public bool IsDungeonSwitchActive(string id){var state=interactionProgress.switches.Find(s=>s.id==id);if(state!=null)return state.active;var entity=ActiveMap?.entities?.objects.Find(e=>e.id==id&&e.kind==DungeonEntityKind.Switch);if(entity!=null)return entity.switchInitiallyActive;foreach(var component in GetComponentsInChildren<DungeonSwitch>())if(component.id==id)return component.initiallyActive;return false;}
 // Named placements retain the fact of a past defeat even after legitimate respawn.
 // Random population is not copied into an unbounded event history.
 void RecordDungeonDefeat(EnemyInstanceRecord record){if(record!=null&&record.defeated&&!string.IsNullOrEmpty(record.placementId)&&!interactionProgress.defeatedEnemyIds.Contains(record.placementId))interactionProgress.defeatedEnemyIds.Add(record.placementId);}
 bool OtherMapSwitchActive(DungeonMapSettings source,DungeonInteractionProgress progress,string id){var saved=progress?.switches?.Find(s=>s.id==id);if(saved!=null)return saved.active;var entity=source?.entities?.objects.Find(e=>e.id==id&&e.kind==DungeonEntityKind.Switch);return entity!=null&&entity.switchInitiallyActive;}
 bool Condition(DungeonCondition condition){if(condition==null)return false;var progress=ConditionProgress(condition.sourceMap);bool yes=false;switch(condition.kind){
  case DungeonConditionKind.Item:int index=Array.FindIndex(inventorySettings.items,i=>i!=null&&i.id==condition.targetId);yes=ownedEquipment.Contains(condition.targetId)?condition.quantity<=1:index>=0&&itemCounts[index]>=Mathf.Max(1,condition.quantity);break;
  case DungeonConditionKind.HackUnlocked:var hack=HackCatalog?.Find(condition.targetId);yes=hack!=null&&HackCatalog.ValidEntry(hack)&&hack.Unlocked(PlayerLevel,eventHackIds);break;
  case DungeonConditionKind.SwitchActive:yes=condition.sourceMap==null||condition.sourceMap==ActiveMap?IsDungeonSwitchActive(condition.targetId):OtherMapSwitchActive(condition.sourceMap,progress,condition.targetId);break;
  case DungeonConditionKind.EventCompleted:yes=progress!=null&&progress.completedEvents!=null&&progress.completedEvents.Contains(condition.targetId);break;
  case DungeonConditionKind.EnemyDefeated:if(progress?.defeatedEnemyIds!=null&&progress.defeatedEnemyIds.Contains(condition.targetId)){yes=true;break;}if(condition.sourceMap==null||condition.sourceMap==ActiveMap){foreach(var record in defeatedEnemies.Values)if(record.defeated&&(record.placementId==condition.targetId||record.instanceId==condition.targetId)){yes=true;break;}}else{var snapshot=floorSnapshots.Find(s=>s.mapId==condition.sourceMap.mapId);yes=snapshot!=null&&snapshot.enemies.Exists(e=>e.defeated&&(e.placementId==condition.targetId||e.instanceId==condition.targetId));}break;
 }return condition.negate?!yes:yes;}
 public bool DungeonConditionsMet(DungeonInteractionSettings profile){if(profile==null)return true;if(profile.groups==null||profile.groups.Count==0)return true;bool all=profile.join==DungeonConditionJoin.All;foreach(var group in profile.groups){if(group==null||group.conditions==null)return false;bool groupAll=group.join==DungeonConditionJoin.All;bool value=groupAll;foreach(var condition in group.conditions){bool term=Condition(condition);if(groupAll&&!term){value=false;break;}if(!groupAll&&term){value=true;break;}}if(all&&!value)return false;if(!all&&value)return true;}return all;}
 bool ConnectionUnlocked(string id){return interactionProgress.unlockedConnections.Contains(id);}
 bool ProfileAdapter(string source,DungeonInteractionSettings profile,Action resume=null){
  if(profile==null||interactionAdapterBypass.Remove(source))return true;
  bool synchronous=true,finished=false;
  bool accepted=StartDungeonEvent(profile,source,success=>{finished=success;if(success&&!synchronous&&profile.continueLegacy&&resume!=null){interactionAdapterBypass.Add(source);resume();}});
  synchronous=false;return accepted&&finished&&profile.continueLegacy;
 }
 void OpenLegacyEventDoor(string id,DungeonDoor target){if(target!=null&&LegacyInteractionAdapter(id,()=>OpenLegacyEventDoor(id,target)))target.OpenDoor();}
 bool LegacyInteractionAdapter(string id,Action resume=null){var binding=ActiveMap!=null?Array.Find(ActiveMap.legacyInteractions??new DungeonLegacyInteractionBinding[0],b=>b!=null&&b.targetId==id):null;return binding==null||ProfileAdapter(id,binding.profile,resume);}
 public bool ActivateDungeonSwitch(DungeonSwitch component){if(component==null||component.owner!=this||battle||moving||lost||pauseOpen||IntroductionActive||worldDialogue.Active||!FacingRefugeNpc(component.cell))return false;
  return StartDungeonEvent(component.interaction,component.id,success=>{if(success&&(component.interaction==null||!component.interaction.actions.Exists(a=>a!=null&&a.kind==DungeonEventActionKind.SetSwitch&&a.targetId==component.id)))SetDungeonSwitch(component.id,true);});
 }
 void SetDungeonSwitch(string id,bool value){var state=interactionProgress.switches.Find(s=>s.id==id);if(state==null){state=new DungeonSwitchState{id=id};interactionProgress.switches.Add(state);}state.active=value;foreach(var component in GetComponentsInChildren<DungeonSwitch>())if(component.id==id){var renderer=component.GetComponentInChildren<Renderer>();if(renderer!=null)renderer.material.color=value?Color.green:Color.gray;}}
 // Checkpoint each action only after it succeeds. Dialogues resume through the existing
 // WorldDialogue callback; restoring a save invalidates callbacks from the previous world.
 public bool RunDungeonEvent(DungeonInteractionSettings profile,string sourceId="event"){return StartDungeonEvent(profile,sourceId,null);}
 bool StartDungeonEvent(DungeonInteractionSettings profile,string sourceId,Action<bool> completed){
  if(profile==null){completed?.Invoke(true);return true;}
  if(battle||moving||lost||IntroductionActive){completed?.Invoke(false);return false;}
  string key=sourceId+"/"+profile.id;
  var state=interactionProgress.executions.Find(s=>s.id==key);
  if(profile.once&&state!=null&&state.completed){completed?.Invoke(true);return true;}
  if(!DungeonConditionsMet(profile)){InteractionMessage(profile.deniedMessage);completed?.Invoke(false);return false;}
  if(runningInteractions.Count>=16||!runningInteractions.Add(key)){InteractionMessage("Ciclo de eventos o ejecuciÃ³n simultÃ¡nea bloqueada.");completed?.Invoke(false);return false;}
  if(state==null){state=new DungeonEventExecution{id=key};interactionProgress.executions.Add(state);}
  if(state.completed){state.completed=false;state.nextAction=0;}
  return RunDungeonEventSteps(profile,key,state,interactionEpoch,completed);
 }
 void InteractionMessage(string text){message=text??"";if(!string.IsNullOrEmpty(text))SystemMessages.Show(SystemMessageCode.Status,custom:text);}
 bool RunDungeonEventSteps(DungeonInteractionSettings profile,string key,DungeonEventExecution state,int epoch,Action<bool> completed){
  if(epoch!=interactionEpoch)return false;
  for(int i=state.nextAction;i<profile.actions.Count;i++){
   var action=profile.actions[i];int next=i+1;
   if(action!=null&&action.kind==DungeonEventActionKind.Dialogue){
    if(worldDialogue.Active||action.dialogue==null||action.dialogue.Length==0)break;
    worldDialogue.Begin(action.dialogue,()=>{if(epoch!=interactionEpoch)return;state.nextAction=next;RunDungeonEventSteps(profile,key,state,epoch,completed);});return true;
   }
   if(action!=null&&action.kind==DungeonEventActionKind.RunEvent&&action.eventProfile!=null){
    return StartDungeonEvent(action.eventProfile,"event",success=>{if(epoch!=interactionEpoch)return;if(success){state.nextAction=next;RunDungeonEventSteps(profile,key,state,epoch,completed);}else{runningInteractions.Remove(key);completed?.Invoke(false);}});
   }
   if(action==null||!ApplyDungeonAction(action,key))break;
   state.nextAction=next;
  }
  bool success=state.nextAction>=profile.actions.Count;
  if(success){state.completed=true;if(!interactionProgress.completedEvents.Contains(profile.id))interactionProgress.completedEvents.Add(profile.id);}else InteractionMessage("El evento quedÃ³ pendiente: revisa sus referencias o el inventario.");
  runningInteractions.Remove(key);completed?.Invoke(success);return success;
 }
 bool ApplyDungeonAction(DungeonEventAction action,string key){DungeonDoor door;var entity=ActiveMap?.entities?.objects.Find(e=>e.id==action.targetId);switch(action.kind){
  case DungeonEventActionKind.OpenDoor:case DungeonEventActionKind.CloseDoor:case DungeonEventActionKind.UnlockConnection:
   if(action.kind==DungeonEventActionKind.UnlockConnection&&FindTransferNode(action.targetId)!=null){if(!ConnectionUnlocked(action.targetId))interactionProgress.unlockedConnections.Add(action.targetId);if(!refuge.unlockedNodes.Contains(action.targetId))refuge.unlockedNodes.Add(action.targetId);RefreshMapEntityStates();if(refugeSettings!=null)RefreshTransferVisuals();return true;}
   if(!mapDoors.TryGetValue(action.targetId,out door)&&!LegacyDoors().TryGetValue(action.targetId,out door)||door==null)return false;
   if(action.kind==DungeonEventActionKind.UnlockConnection&&!ConnectionUnlocked(action.targetId))interactionProgress.unlockedConnections.Add(action.targetId);
   if(action.kind!=DungeonEventActionKind.CloseDoor&&entity!=null&&(entity.cityEntrance&&!CityBossDefeated()||entity.doorKind==DungeonDoorKind.Locked&&!EntityState(entity.id).keyConsumed&&!HasMapKey(entity.requiredKeyId)&&!ConnectionUnlocked(entity.id)))return false;
   if(action.kind==DungeonEventActionKind.CloseDoor&&door.IsPermanentlyOpen)return false;if(action.kind==DungeonEventActionKind.CloseDoor)door.SetClosedImmediately();else door.SetOpenImmediately();if(entity!=null)EntityState(entity.id).opened=door.IsOpen;return true;
  case DungeonEventActionKind.SetSwitch:if(entity==null||entity.kind!=DungeonEntityKind.Switch){var exists=Array.Exists(GetComponentsInChildren<DungeonSwitch>(),s=>s.id==action.targetId);if(!exists)return false;}SetDungeonSwitch(action.targetId,action.value);return true;
  case DungeonEventActionKind.Dialogue:if(worldDialogue.Active)return false;worldDialogue.Begin(action.dialogue??new WorldDialogueLine[0]);return true;
  case DungeonEventActionKind.Message:InteractionMessage(action.message);return true;
  case DungeonEventActionKind.Reward:string receipt=key+"/"+action.id;if(interactionProgress.rewardReceipts.Contains(receipt))return true;if(!AcquireItem(action.targetId,action.quantity))return false;interactionProgress.rewardReceipts.Add(receipt);return true;
  case DungeonEventActionKind.CompleteEvent:if(string.IsNullOrEmpty(action.targetId))return false;if(!interactionProgress.completedEvents.Contains(action.targetId))interactionProgress.completedEvents.Add(action.targetId);return true;
  case DungeonEventActionKind.RunEvent:return action.eventProfile!=null&&RunDungeonEvent(action.eventProfile,"event");
 }return false;}
 bool LegacyEntityReady(DungeonMapEntity e){if(e.kind==DungeonEntityKind.Door)return !(e.cityEntrance&&!CityBossDefeated()||e.doorKind==DungeonDoorKind.Locked&&!EntityState(e.id).keyConsumed&&!HasMapKey(e.requiredKeyId)&&!ConnectionUnlocked(e.id));if(e.kind==DungeonEntityKind.Chest){if(EntityState(e.id).opened||pendingChests.Contains(e.id))return false;if(!string.IsNullOrEmpty(e.guardPlacementId)){int guard=slots.FindIndex(s=>s.placement!=null&&s.placement.id==e.guardPlacementId);if(guard>=0&&enemyObjects[guard]!=null)return false;if(guard<0){bool dead=false;foreach(var record in defeatedEnemies.Values)if(record.placementId==e.guardPlacementId)dead=true;if(!dead)return false;}}}return true;}
}
