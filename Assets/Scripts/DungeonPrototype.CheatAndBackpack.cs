using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 public const string LevelUpCheatId="level-up-cheat";
 bool cityBackpackOpen;float cheatPulseUntil,cheatMessageUntil;
 public bool UseLevelUpCheat(int index){
  if(index<0||index>=inventorySettings.items.Length||index>=itemCounts.Length||itemCounts[index]<=0||!inventorySettings.items[index].gainOneLevel||busy||moving||lost||worldDialogue.Active||DungeonWhiteTransition.Active)return false;
  if(PlayerLevel>=Levels.maximumLevel){pauseNotice=message="Ya estás en el nivel máximo.";return false;}
  int before=PlayerLevel;GainPlayerXP(XPRemaining,true);if(PlayerLevel!=before+1)return false;
  itemCounts[index]--;pauseNotice="LEVEL UP! · Nivel "+PlayerLevel;cheatPulseUntil=Time.unscaledTime+.7f;cheatMessageUntil=Time.unscaledTime+2f;DungeonAudio.Play(DungeonAudio.Cue.Item);return true;
 }
 string CityNpcName(DigitalKingdomService service){var location=Array.Find(City.locations,l=>l!=null&&l.service==service);return location!=null?location.npcName:"";}
 string CityGiftFlag(DigitalKingdomLocation location){return "city.welcome.gift.v1:"+location.id;}
 bool CityGiftPending(DigitalKingdomLocation location){CityNoticeSystem.Normalize(cityProgress);return !string.IsNullOrEmpty(location.welcomeGiftId)&&location.welcomeGiftQuantity>0&&!cityProgress.storyFlags.Contains(CityGiftFlag(location));}
 bool GrantCityWelcomeGift(DigitalKingdomLocation location){if(!CityGiftPending(location))return false;if(!AcquireItem(location.welcomeGiftId,location.welcomeGiftQuantity)){worldDialogue.Begin(new[]{new WorldDialogueLine(location.npcName,"Tu mochila está llena. Haz espacio y vuelve; guardaré tu regalo.")});return false;}cityProgress.storyFlags.Add(CityGiftFlag(location));return true;}
 void BeginCityWelcome(DigitalKingdomLocation location){
  var notice=PendingCityNotice(location.id);var lines=new List<WorldDialogueLine>();
  if(!location.uniqueConversation||!cityProgress.conversationsSeen.Contains(location.id))lines.AddRange(location.dialogue??new WorldDialogueLine[0]);
  if(notice!=null)lines.AddRange(notice.dialogue??new WorldDialogueLine[0]);
  bool gift=CityGiftPending(location);if(gift)lines.Add(new WorldDialogueLine(location.npcName,string.IsNullOrEmpty(location.welcomeGiftDialogue)?"Toma este LEVEL UP CHEAT. Te ayudará en el camino.":location.welcomeGiftDialogue));
  worldDialogue.Begin(lines.ToArray(),()=>{if(!cityProgress.conversationsSeen.Contains(location.id))cityProgress.conversationsSeen.Add(location.id);if(notice!=null)CityNoticeSystem.Read(notice,cityProgress);if(gift)GrantCityWelcomeGift(location);});
 }
 public bool OpenCityBackpack(){if(!cityActive||worldDialogue.Active||DungeonWhiteTransition.Active||saveSlotsOpen)return false;cityBackpackOpen=true;pausePage=0;pauseSelection=0;pauseRow=0;choosingEquipment=false;pauseNotice="";RefreshAvailableHacks();return true;}
 void UpdateCityBackpack(){
  if(pausePage!=0){PauseInput();var controller=Gamepad.current;if(controller!=null){if(controller.buttonEast.wasPressedThisFrame){if(choosingEquipment)choosingEquipment=false;else pausePage=0;return;}int delta=controller.dpad.right.wasPressedThisFrame?1:controller.dpad.left.wasPressedThisFrame?-1:controller.dpad.down.wasPressedThisFrame?(pausePage==2?6:1):controller.dpad.up.wasPressedThisFrame?(pausePage==2?-6:-1):0;if(choosingEquipment){var choices=EquipmentChoices();equipmentChoice=(equipmentChoice+delta+choices.Count)%choices.Count;if(controller.buttonSouth.wasPressedThisFrame)ConfirmEquipmentChoice();}else{int count=pausePage==2?18:3;pauseRow=(pauseRow+delta+count)%count;if(controller.buttonSouth.wasPressedThisFrame){if(pausePage==2)UseOutsidePotion(pauseRow);else if(pausePage==1)OpenEquipmentChoice();}}}return;}
  var key=Keyboard.current;var pad=Gamepad.current;
  if((key!=null&&(key.escapeKey.wasPressedThisFrame||key.iKey.wasPressedThisFrame))||(pad!=null&&pad.buttonEast.wasPressedThisFrame)){cityBackpackOpen=false;return;}
  int direction=key!=null&&key.downArrowKey.wasPressedThisFrame||pad!=null&&pad.dpad.down.wasPressedThisFrame?1:key!=null&&key.upArrowKey.wasPressedThisFrame||pad!=null&&pad.dpad.up.wasPressedThisFrame?-1:0;
  pauseSelection=(pauseSelection+direction+3)%3;
  if(key!=null&&key.enterKey.wasPressedThisFrame||pad!=null&&pad.buttonSouth.wasPressedThisFrame)ChooseCityBackpack(pauseSelection);
 }
 void ChooseCityBackpack(int i){if(i==2){cityBackpackOpen=false;return;}pausePage=i==0?2:1;pauseRow=0;choosingEquipment=false;}
 void DrawCityBackpackButton(float height){GUI.enabled=!worldDialogue.Active&&!DungeonWhiteTransition.Active;var r=new Rect(24,height-94,76,70);NeonDungeonUI.Panel(r);GUI.DrawTexture(new Rect(r.x+20,r.y+7,36,36),DigitalIcons.Texture(DigitalIcon.Backpack));var style=new GUIStyle(GUI.skin.label){fontSize=11,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(r.x,r.y+46,r.width,19),"MOCHILA",style);if(GUI.Button(r,GUIContent.none,GUIStyle.none))OpenCityBackpack();GUI.enabled=true;}
 void DrawCityBackpack(float w,float h){
  NeonDungeonUI.Solid(new Rect(0,0,w,h),new Color(0,0,0,.7f));
  if(pausePage==2)DrawInventoryGrid(w,h);else if(pausePage==1)DrawCharacterCard(w,h);else {NeonDungeonUI.Panel(new Rect(w/2-225,h/2-180,450,360));NeonDungeonUI.Label(new Rect(w/2-210,h/2-165,420,50),"MOCHILA",true);var names=new[]{"INVENTARIO","PERSONAJE","VOLVER"};for(int i=0;i<3;i++)if(NeonDungeonUI.MenuAction(new Rect(w/2-180,h/2-95+i*75,360,58),names[i],pauseSelection==i))ChooseCityBackpack(i);}
  DrawCheatPulse(w,h);
 }
 void DrawCheatPulse(float w,float h){float t=(cheatPulseUntil-Time.unscaledTime)/.7f;if(t<=0)return;for(int i=0;i<16;i++){float x=w*.5f-170+i*23,y=h*.5f+60-(1-t)*120;NeonDungeonUI.Solid(new Rect(x,y,4,4),new Color(1,.8f,.15f,t));}}
}
