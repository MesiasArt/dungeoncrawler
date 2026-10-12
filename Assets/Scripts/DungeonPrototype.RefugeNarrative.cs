using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 bool nodeDecision;int nodeDecisionSelection;
 void MigrateRefugeNarrative(){
  if(refuge.narrativeVersion>=1)return;
  // Older saves registered the refuge directly: preserve that history as player activation.
  refuge.refugeTeleportActivatedByPlayer=refuge.unlockedNodes.Contains("node.refuge");
  refuge.zone2TeleportActivated=refuge.unlockedNodes.Contains("node.zone2");
  if(refuge.refugeTragedyTriggered&&!refuge.refugeTeleportActivatedByPlayer){refuge.refugeTeleportActivatedByDevotee=true;if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");}
  refuge.narrativeVersion=1;
 }
 void BeginSaveTerminalNarrative(){
  if(!refuge.savePointIntroSeen){worldDialogue.Begin(refugeSettings.savePointIntro,()=>{refuge.savePointIntroSeen=true;CompleteTerminalSave(false);});return;}
  CompleteTerminalSave(true);
 }
 void CompleteTerminalSave(bool showConfirmation){
  hp=MaximumHP;TryGrantRefugeReward();refuge.narrativeVersion=1;bool saved=SaveDungeonGame();
  foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(!device.portal)device.ShowSaveResult(saved);
  if(!saved||showConfirmation){var lines=new List<WorldDialogueLine>{new WorldDialogueLine("SISTEMA",saved?"Integridad restaurada. Partida guardada.":"Integridad restaurada. No se pudo guardar la partida.")};if(refuge.devotoDefeated)lines.Add(new WorldDialogueLine("SISTEMA","Usuarios activos en el refugio: 1."));worldDialogue.Begin(lines.ToArray());}
 }
 void ExamineTransferNode(string id){
  if(refugeSettings==null||worldDialogue.Active||nodeDecision||battle||lost)return;
  var node=Array.Find(refugeSettings.nodes,n=>n.id==id);if(node==null)return;
  if(refuge.unlockedNodes.Contains(id)){OpenTransferChoices(id);return;}
  if(id=="node.refuge"){worldDialogue.Begin(refugeSettings.refugeNodeIntro,()=>{nodeDecision=true;nodeDecisionSelection=0;});return;}
  if(node.triggersRefugeTragedy){ActivateZoneTwoNode(id);return;}
  refuge.unlockedNodes.Add(id);worldDialogue.Begin(new[]{new WorldDialogueLine("SISTEMA","Nodo de transferencia registrado: "+node.displayName+".")},()=>OpenTransferChoices(id));
 }
 void ChooseRefugeNode(bool activate){
  nodeDecision=false;if(!activate){worldDialogue.Begin(refugeSettings.refugeNodeDecline);return;}
  if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");
  refuge.refugeTeleportActivatedByPlayer=true;refuge.narrativeVersion=1;RefreshTransferVisuals();
  worldDialogue.Begin(refugeSettings.refugeNodeActivate,()=>OpenTransferChoices("node.refuge",false));
 }
 void ActivateZoneTwoNode(string id){
  if(!refuge.unlockedNodes.Contains(id))refuge.unlockedNodes.Add(id);
  bool playerActivated=refuge.refugeTeleportActivatedByPlayer;
  if(!playerActivated){refuge.refugeTeleportActivatedByDevotee=true;if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");}
  refuge.zone2TeleportActivated=true;refuge.narrativeVersion=1;
  if(!refuge.refugeTragedyTriggered){refuge.refugeTragedyTriggered=true;UpdateRefugePresentation();EnsureDevotoEvent();}
  RefreshTransferVisuals();worldDialogue.Begin(playerActivated?refugeSettings.zoneTwoPlayerDialogue:refugeSettings.zoneTwoDevoteeDialogue,()=>OpenTransferChoices(id));
 }
 void OpenTransferChoices(string origin,bool showEmpty=true){
  transferChoices.Clear();foreach(var target in refugeSettings.nodes)if(target.id!=origin&&refuge.unlockedNodes.Contains(target.id))transferChoices.Add(target);
  transferSelection=0;transferMenu=transferChoices.Count>0;
  if(!transferMenu&&showEmpty)worldDialogue.Begin(new[]{new WorldDialogueLine("SISTEMA","Ningún otro nodo disponible.")});
 }
 void DiscoverRefugeReturn(string id){if(id!="node.refuge"||!refuge.refugeTragedyTriggered||refuge.refugeTragedyDiscovered)return;refuge.refugeTragedyDiscovered=true;worldDialogue.Begin(refugeSettings.tragedyArrivalDialogue);}
 void CheckRefugeDiscovery(){if(refugeSettings==null||worldDialogue.Active)return;if(refuge.refugeTragedyTriggered&&!refuge.refugeTragedyDiscovered&&RegionAt(position)!=null&&RegionAt(position).id==refugeSettings.regionId)DiscoverRefugeReturn("node.refuge");}
 void RefreshTransferVisuals(){foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(device.portal&&device.deviceName.StartsWith("NODO ")){var node=Array.Find(refugeSettings.nodes,n=>device.deviceName=="NODO "+n.displayName);device.transferActive=node!=null&&refuge.unlockedNodes.Contains(node.id);}}
 bool UpdateNodeDecisionInput(){if(!nodeDecision)return false;var k=Keyboard.current;if(k==null)return true;if(k.upArrowKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame)nodeDecisionSelection=1-nodeDecisionSelection;if(k.escapeKey.wasPressedThisFrame)ChooseRefugeNode(false);else if(k.enterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame)ChooseRefugeNode(nodeDecisionSelection==0);return true;}
 bool DrawNodeDecision(float width,float height){if(!nodeDecision)return false;NeonDungeonUI.Panel(new Rect(width/2-220,height/2-90,440,180));for(int i=0;i<2;i++)if(GUI.Button(new Rect(width/2-195,height/2-65+i*65,390,48),(nodeDecisionSelection==i?"> ":"")+(i==0?"ACTIVAR NODO":"DEJARLO APAGADO")))ChooseRefugeNode(i==0);return true;}
}
