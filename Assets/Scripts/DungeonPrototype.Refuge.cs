using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 readonly HashSet<string> disappearingBodies=new HashSet<string>();
 RefugeSettings refugeSettings;RefugeProgress refuge=new RefugeProgress();DungeonDoor refugeDoor;
 readonly WorldDialogue worldDialogue=new WorldDialogue();readonly Dictionary<string,GameObject> refugeActors=new Dictionary<string,GameObject>();
 bool devotoApproachLatched,refugeExitLooking;
 Quaternion refugeExitViewRotation;string refugeExitLastNpc;
 bool transferMenu,devotoLowLifeSpoken;int transferSelection;List<TransferNodeDefinition> transferChoices=new List<TransferNodeDefinition>();
 RefugeNpcState NpcState(string id){var state=refuge.npcs.Find(n=>n.id==id);if(state==null){state=new RefugeNpcState{id=id};refuge.npcs.Add(state);}return state;}
 void CreateRefuge(){refugeSettings=RefugeSettings.Current;if(refugeSettings==null)return;refuge.narrativeVersion=1;
  foreach(var npc in refugeSettings.npcs){var root=new GameObject("NPC placeholder — "+npc.displayName);root.transform.SetParent(transform,false);root.transform.position=World(npc.cell)-Vector3.up*1.5f;
   int visualKind=Array.IndexOf(EnemyRoster.ResourceIds,npc.visualSpeciesId);
   if(visualKind>=0){EnemyRoster.AddVisual(root,visualKind);root.AddComponent<DungeonNpcFacing>().viewCamera=view;root.transform.localScale=Vector3.one*.75f;}
   else{var body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.transform.SetParent(root.transform,false);body.transform.localPosition=Vector3.up*.9f;body.transform.localScale=new Vector3(.65f,.9f,.65f);body.GetComponent<Renderer>().material=Glow(npc.placeholderColor);Destroy(body.GetComponent<Collider>());}
   refugeActors[npc.id]=root;}
  if(LegacyObjectEnabled("refuge.save"))RefugeDevice(refugeSettings.saveCell,"TERMINAL DE RESTAURACIÓN",new Color(0,.8f,.4f),false);
  foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id))RefugeDevice(node.cell,"NODO "+node.displayName,new Color(.7f,.15f,1),true);
  if(!LegacyObjectEnabled("refuge.exit"))return;
  var doorRoot=new GameObject("Refugio — salida a Zona 1");doorRoot.transform.SetParent(transform,false);doorRoot.transform.position=new Vector3(refugeSettings.exitDoorCell.x*Cell,0,refugeSettings.exitDoorCell.y*Cell);
  refugeDoor=doorRoot.AddComponent<DungeonDoor>();refugeDoor.settings=exitDoorSettings;refugeDoor.closeAfterPassage=true;refugeDoor.doorwayWidth=Cell;refugeDoor.interactionDistance=Cell*.5f+.25f;refugeDoor.openingTravel=corridorHeight+.2f;
  var panel=Box("Refuge door panel",doorRoot.transform.position+Vector3.up*corridorHeight*.5f,new Vector3(Cell-.15f,corridorHeight,.18f),wall);panel.transform.SetParent(doorRoot.transform,true);refugeDoor.panel=panel.transform;refugeDoor.blocker=panel.GetComponent<Collider>();refugeDoor.Initialize();refugeDoor.Passed+=()=>{message="Zona 1";};
 }
 void RefugeDevice(Vector2Int cell,string name,Color color,bool portal){
  var root=new GameObject(name);root.transform.SetParent(transform,false);root.transform.position=World(cell)-Vector3.up*1.5f;
  if(!portal){Vector2Int wallDirection=Vector2Int.left;int nearest=100;
   foreach(var direction in directions)for(int distance=1;distance<=6;distance++){if(Open(cell+direction*distance))continue;if(distance<nearest){nearest=distance;wallDirection=direction;}break;}
   if(nearest<100)root.transform.position+=new Vector3(wallDirection.x,0,wallDirection.y)*(nearest*Cell-Cell*.5f-.18f);
   root.transform.rotation=Quaternion.LookRotation(new Vector3(-wallDirection.x,0,-wallDirection.y));
  }
  var visual=root.AddComponent<RefugeDeviceVisual>();visual.portal=portal;visual.deviceName=name;visual.Build();
 }
 bool RefugeNpcBlocks(Vector2Int cell){if(MapNpcBlocks(cell))return true;if(refugeSettings==null)return false;foreach(var npc in refugeSettings.npcs)if(npc.cell==cell&&refugeActors.ContainsKey(npc.id)&&refugeActors[npc.id].activeSelf)return true;return false;}
 void WarpPlayer(Vector2Int cell,int direction=0){if(!Open(cell))return;ResetConversationCamera();position=cell;heading=Mathf.Clamp(direction,0,3);view.transform.position=World(cell);view.transform.rotation=Quaternion.Euler(0,heading*90,0);RevealVisitedCell(cell);glitch=.15f;moving=false;}
 void BeginNpcTalk(string id){GameObject focusActor;if(refugeActors.TryGetValue(id,out focusActor))FocusConversationOn(focusActor.transform);if(id=="centinela"&&!refuge.refugeTragedyTriggered&&!refuge.sentinelTeleportSeen){worldDialogue.Begin(refugeSettings.sentinelTeleportDialogue,()=>refuge.sentinelTeleportSeen=true);return;}var npc=Array.Find(refugeSettings.npcs,n=>n.id==id);if(npc==null)return;var state=NpcState(id);if(refuge.refugeTragedyTriggered){if(id=="devoto"||state.lastWordsHeard)return;var bodyLines=new List<WorldDialogueLine>();if(!refuge.firstBodyDiscovered){bodyLines.AddRange(refugeSettings.tragedyBodyDialogue);refuge.firstBodyDiscovered=true;}bodyLines.Add(new WorldDialogueLine(npc.displayName,npc.lastWords));worldDialogue.Begin(bodyLines.ToArray(),()=>{state.lastWordsHeard=true;StartCoroutine(DisappearRefugeBody(id));});}else{if(npc.conversations==null||npc.conversations.Length==0)return;int index=Mathf.Min(state.talks,npc.conversations.Length-1);worldDialogue.Begin(new[]{new WorldDialogueLine(npc.displayName,npc.conversations[index])},()=>state.talks++);}}
 IEnumerator DisappearRefugeBody(string id){GameObject obj;if(!refugeActors.TryGetValue(id,out obj))yield break;disappearingBodies.Add(id);yield return BlinkUntilGone(obj);if(obj!=null)obj.SetActive(false);disappearingBodies.Remove(id);}
 void TryOpenRefugeExit(){
  if(refugeDoor==null||!DoorInteractionAvailable(refugeDoor)||worldDialogue.Active)return;
  if(refugeSettings==null||refuge.exitDialogueCompleted||refuge.refugeTragedyTriggered){refugeDoor.OpenDoor();return;}
  refugeExitViewRotation=view.transform.rotation;refugeExitLastNpc=null;refugeExitLooking=true;
  worldDialogue.Begin(refugeSettings.exitDialogue,()=>{refuge.exitDialogueCompleted=true;refugeExitLooking=false;view.transform.rotation=refugeExitViewRotation;refugeDoor.OpenDoor();},ReceiveDeparturePackage);
 }
 void ReceiveDeparturePackage(WorldDialogueLine line){
  if(string.IsNullOrEmpty(line.giftId)||line.giftQuantity<=0||refuge.exitDialogueCompleted)return;
  if(refuge.departureGiftIds==null)refuge.departureGiftIds=new List<string>();if(refuge.departureGiftIds.Contains(line.giftId))return;
  if(AcquireItem(line.giftId,line.giftQuantity)){refuge.departureGiftIds.Add(line.giftId);if(itemReceipts.Count>1)itemReceipts.RemoveRange(0,itemReceipts.Count-1);}
 }
 void LookAtRefugeSpeaker(){
  if(!refugeExitLooking||!worldDialogue.Active)return;
  var line=worldDialogue.Current;if(line.speaker!="GAMERBOY")refugeExitLastNpc=line.speaker;
  var npc=Array.Find(refugeSettings.npcs,n=>n.displayName==refugeExitLastNpc);if(npc==null)return;
  GameObject actor;if(!refugeActors.TryGetValue(npc.id,out actor)||!actor.activeSelf)return;
  var visual=actor.GetComponent<GhostEnemyVisual>();Vector3 target=actor.transform.position+Vector3.up*(visual!=null?visual.WorldHeight*.7f:1.5f);
  Vector3 delta=target-view.transform.position;if(delta.sqrMagnitude>.01f)view.transform.rotation=Quaternion.Slerp(view.transform.rotation,Quaternion.LookRotation(delta),1-Mathf.Exp(-5*Time.unscaledDeltaTime));
  UpdateRefugePresentation();
 }
 bool NearRefugeCell(Vector2Int cell){return (position-cell).sqrMagnitude<=1;}
 bool FacingRefugeNpc(Vector2Int cell){
  Vector2Int delta=cell-position;if(delta.sqrMagnitude!=1)return false;
  return delta==directions[heading];
 }
 bool UpdateRefugeInput(){if(UpdateNodeDecisionInput())return true;if(worldDialogue.Active){LookAtRefugeSpeaker();var k=Keyboard.current;if(ConfirmPressed()||(k!=null&&(k.eKey.wasPressedThisFrame||k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame)))worldDialogue.Advance(Layout.lettersPerSecond);return true;}
  if(transferMenu){var k=Keyboard.current;if(k!=null){if(k.escapeKey.wasPressedThisFrame){transferMenu=false;return true;}if(k.upArrowKey.wasPressedThisFrame)transferSelection=(transferSelection+transferChoices.Count-1)%transferChoices.Count;if(k.downArrowKey.wasPressedThisFrame)transferSelection=(transferSelection+1)%transferChoices.Count;if(k.enterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame){TravelToNode(transferChoices[transferSelection].id);transferMenu=false;}}return true;}
  if(refugeSettings==null||IntroductionActive||battle||moving||lost||pauseOpen)return false;
  UpdateRefugePresentation();RefreshTransferVisuals();TryGrantRefugeReward();CheckRefugeDiscovery();CheckDevotoApproach();if(worldDialogue.Active)return true;
  var keys=Keyboard.current;if(keys==null||!keys.eKey.wasPressedThisFrame)return false;
  if(RefugeTerminalAvailable()){UseRefugeTerminal();return true;}
  foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id)&&position==node.cell&&FacingRefugeTransfer(node.id)){RegisterTransferNode(node.id);return true;}
  if(refuge.refugeTragedyTriggered&&!refuge.devotoDefeated&&FacingRefugeNpc(refugeSettings.devotoCell)){BeginDevotoConversation();return true;}
  foreach(var npc in refugeSettings.npcs)if(FacingRefugeNpc(npc.cell)&&refugeActors[npc.id].activeSelf){BeginNpcTalk(npc.id);return true;}
  if(DoorInteractionAvailable(refugeDoor)){TryOpenRefugeExit();return true;}return false;
 }
 void UseRefugeTerminal(){BeginSaveTerminalNarrative();}
 public void RegisterTransferNode(string id){ExamineTransferNode(id);}
 public bool TravelToNode(string id){if(!refuge.unlockedNodes.Contains(id)||battle||lost)return false;var node=FindTransferNode(id);if(node==null||!Open(node.cell))return false;WarpPlayer(node.cell,node.arrivalHeading);var crt=view.GetComponent<HelmetCRTController>();if(crt!=null)crt.TriggerContext("Teleport",.4f);devotoApproachLatched=false;transferMenu=false;if(refugeSettings!=null)UpdateRefugePresentation();DiscoverRefugeReturn(id);return true;}
 void UpdateRefugePresentation(){foreach(var npc in refugeSettings.npcs){var root=refugeActors[npc.id];var state=NpcState(npc.id);if(disappearingBodies.Contains(npc.id))continue;root.SetActive(!state.lastWordsHeard&&!(refuge.refugeTragedyTriggered&&npc.id=="devoto"));if(!root.activeSelf)continue;var visual=root.GetComponent<GhostEnemyVisual>();
   if(visual!=null){root.transform.rotation=Quaternion.Euler(0,view.transform.eulerAngles.y,0);if(refuge.refugeTragedyTriggered&&visual.CurrentPose!=GhostEnemyVisual.Pose.Death)visual.Play(GhostEnemyVisual.Pose.Death);}
   else{var body=root.transform.GetChild(0);body.localRotation=refuge.refugeTragedyTriggered?Quaternion.Euler(0,0,90):Quaternion.identity;body.localPosition=Vector3.up*(refuge.refugeTragedyTriggered?.25f:.9f);}
   var label=root.GetComponentInChildren<TextMesh>();if(label!=null)label.transform.rotation=view.transform.rotation;}}
 void EnsureDevotoEvent(){if(refugeSettings==null||!refuge.refugeTragedyTriggered||refuge.devotoDefeated)return;var placement=Array.Find(RegionMap.placedEnemies,e=>e.id==refugeSettings.devotoPlacementId);if(placement==null)return;if(enemyObjects.Exists(o=>o!=null&&o.GetComponent<EnemyCombatStats>().placementId==placement.id))return;var configured=JsonUtility.FromJson<PlacedEnemyDefinition>(JsonUtility.ToJson(placement));configured.fixedLevel=refugeSettings.devotoLevel;SpawnPlacement(configured);}
 void CheckDevotoApproach(){
  if(refugeSettings==null||!refugeSettings.automaticDevotoEncounter||!refuge.refugeTragedyTriggered||refuge.devotoDefeated||worldDialogue.Active||battle||moving||IntroductionActive||pauseOpen||lost)return;
  Vector2Int delta=refugeSettings.devotoCell-position;
  Vector2Int facing=heading==0?Vector2Int.up:heading==1?Vector2Int.right:heading==2?Vector2Int.down:Vector2Int.left;
  bool approach=position.y<refugeSettings.refugePassageRow&&delta.sqrMagnitude>0&&delta.sqrMagnitude<=refugeSettings.devotoApproachRange*refugeSettings.devotoApproachRange&&Vector2.Dot(((Vector2)delta).normalized,(Vector2)facing)>.7f;
  if(!approach){devotoApproachLatched=false;return;}
  if(devotoApproachLatched)return;devotoApproachLatched=true;BeginDevotoConversation();
 }
 void BeginDevotoConversation(){if(refuge.devotoDefeated)return;if(refuge.devotoConfrontationSeen){StartDevotoCombat();return;}worldDialogue.Begin(refuge.refugeTeleportActivatedByDevotee?refugeSettings.devotoDevoteeDialogue:refugeSettings.devotoPlayerDialogue,()=>{refuge.devotoConfrontationSeen=true;StartDevotoCombat();});}
 void StartDevotoCombat(){EnsureDevotoEvent();int i=enemyObjects.FindIndex(o=>o!=null&&o.GetComponent<EnemyCombatStats>().placementId==refugeSettings.devotoPlacementId);if(i>=0)BeginEncounter(i);}
 void OnRefugeEnemyDefeated(int index){if(refugeSettings==null||enemyObjects[index].GetComponent<EnemyCombatStats>().placementId!=refugeSettings.devotoPlacementId)return;refuge.devotoDefeated=true;TryGrantRefugeReward();worldDialogue.Begin(new[]{new WorldDialogueLine("DEVOTO","Yo también... tenía miedo...")});}
 void TryGrantRefugeReward(){if(refugeSettings==null||!refuge.devotoDefeated||refuge.rewardObtained)return;if(ownedEquipment.Contains(refugeSettings.rewardId)||AcquireEquipment(refugeSettings.rewardId))refuge.rewardObtained=true;}
 void RefugeLowLifeBark(int index){if(refugeSettings==null||devotoLowLifeSpoken||enemyObjects[index].GetComponent<EnemyCombatStats>().placementId!=refugeSettings.devotoPlacementId||enemyLives[index]>enemyObjects[index].GetComponent<EnemyCombatStats>().maximumHP*.25f)return;devotoLowLifeSpoken=true;Bark(EnemyBarkCategory.LowHealth);}
 void DrawWorldDialogue(float height){var line=worldDialogue.Current;if(line==null)return;if((line.textPanel&&line.speaker!="GAMERBOY")||line.speaker=="SISTEMA"||line.speaker=="TERMINAL DE RESTAURACIÓN"||line.speaker=="NODO DE TRANSFERENCIA"){var panel=new Rect(340,height-150,600,120);NeonDungeonUI.Message(panel,worldDialogue.Visible(Layout.lettersPerSecond));if(worldDialogue.Ready(Layout.lettersPerSecond))NeonDungeonUI.ContinueIndicator(panel);return;}if(line.speaker=="GAMERBOY")NeonDungeonUI.DialogueAvatar(Layout.Resolve(Layout.dialogueAvatar,0,height));else if(refugeSettings!=null){var npc=Array.Find(refugeSettings.npcs,n=>n.displayName==line.speaker);if(npc!=null&&npc.portrait!=null)GUI.DrawTexture(Layout.Resolve(Layout.dialogueAvatar,0,height),npc.portrait,ScaleMode.ScaleToFit,true);}Rect rect=Layout.Resolve(Layout.dialogue,0,height);bool wide=line.speaker!="GAMERBOY"&&line.text.Length>130;if(wide)rect=new Rect(320,height-220,640,190);NeonDungeonUI.Dialogue(rect,line.speaker,worldDialogue.Visible(Layout.lettersPerSecond),!wide&&Layout.preservePanelAspect,worldDialogue.Ready(Layout.lettersPerSecond));}
 void DrawRefugePrompt(float width,float height){if(DrawNodeDecision(width,height))return;if(transferMenu){NeonDungeonUI.Panel(new Rect(width/2-220,height/2-160,440,320));for(int i=0;i<transferChoices.Count;i++)if(GUI.Button(new Rect(width/2-190,height/2-125+i*55,380,45),(transferSelection==i?"> ":"")+transferChoices[i].displayName)){TravelToNode(transferChoices[i].id);transferMenu=false;}return;}if(refugeSettings==null||worldDialogue.Active||IntroductionActive||battle||lost||pauseOpen)return;string prompt=null;if(RefugeTerminalAvailable())prompt="[E] Terminal de restauración";else foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id)&&position==node.cell&&FacingRefugeTransfer(node.id)){prompt="[E] Nodo "+node.displayName;break;}if(prompt==null&&refuge.refugeTragedyTriggered&&!refuge.devotoDefeated&&FacingRefugeNpc(refugeSettings.devotoCell))prompt="[E] Hablar";if(prompt==null)foreach(var npc in refugeSettings.npcs)if(FacingRefugeNpc(npc.cell)&&refugeActors[npc.id].activeSelf){prompt="[E] Hablar";break;}if(prompt==null&&DoorInteractionAvailable(refugeDoor))prompt="[E] Abrir salida";if(prompt!=null)GUI.Box(new Rect(width/2-190,height-155,380,45),prompt);}
}
