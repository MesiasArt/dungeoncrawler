using System;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 bool startMenuOpen;int startupSelection;
 bool saveSlotsOpen,loadSlotMode;int saveSlotSelection,overwriteSlot=-1;string slotName="";string slotNotice="";
 void OpenSaveSlots(bool load){loadSlotMode=load;saveSlotsOpen=true;saveSlotSelection=DungeonSaveStore.ActiveSlot;overwriteSlot=-1;slotNotice="";ReadSlotName();SetPause(true);}
 void ReadSlotName(){try{var data=DungeonSaveStore.Read(saveSlotSelection);slotName=data!=null&&!string.IsNullOrEmpty(data.slotName)?data.slotName:"Slot "+(saveSlotSelection+1);}catch{slotName="Slot "+(saveSlotSelection+1);}}
 void CloseSaveSlots(){saveSlotsOpen=false;overwriteSlot=-1;}
 void ShowStartupMenu(){startMenuOpen=true;startupSelection=0;SetPause(true);}
 void StartFreshFromMenu(){StartPlayClock(0);startMenuOpen=false;saveSlotsOpen=false;SetPause(false);VirtualDungeon.GameStartContext.PrepareNewGame();introductionCompleted=false;ArmRefugeWelcome();StartIntroduction();}
 void DrawStartupMenu(float w,float h){float x=w/2-220,y=h/2-150;NeonDungeonUI.Panel(new Rect(x,y,440,300));NeonDungeonUI.Label(new Rect(x+25,y+20,390,45),"CODEBOUND",true);if(NeonDungeonUI.MenuAction(new Rect(x+35,y+85,370,60),"NUEVA PARTIDA",startupSelection==0))StartFreshFromMenu();if(NeonDungeonUI.MenuAction(new Rect(x+35,y+165,370,60),"CARGAR PARTIDA",startupSelection==1))OpenSaveSlots(true);NeonDungeonUI.Label(new Rect(x+30,y+245,380,35),"F9: elegir una partida guardada",true);}
 bool UpdateSaveSlotInput(){var k=Keyboard.current;if(saveSlotsOpen){if(k==null)return true;if(k.escapeKey.wasPressedThisFrame){CloseSaveSlots();return true;}int delta=k.downArrowKey.wasPressedThisFrame?1:k.upArrowKey.wasPressedThisFrame?-1:0;if(delta!=0){saveSlotSelection=(saveSlotSelection+delta+DungeonSaveStore.SlotCount)%DungeonSaveStore.SlotCount;overwriteSlot=-1;ReadSlotName();}if(k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame)UseSaveSlot();return true;}
  if(startMenuOpen){if(k!=null){if(k.upArrowKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame)startupSelection=1-startupSelection;if(k.f9Key.wasPressedThisFrame)OpenSaveSlots(true);else if(k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame){if(startupSelection==0)StartFreshFromMenu();else OpenSaveSlots(true);}}return true;}
  if(k!=null&&k.f9Key.wasPressedThisFrame&&!battle&&!moving){OpenSaveSlots(true);return true;}
  if(k!=null&&k.f5Key.wasPressedThisFrame&&!battle&&!moving&&!IntroductionActive&&!worldDialogue.Active){OpenSaveSlots(false);return true;}return false;
 }
 void UseSaveSlot(){if(loadSlotMode){try{var data=DungeonSaveStore.Read(saveSlotSelection);if(data==null){slotNotice="Slot vacío.";return;}RestoreDungeonSave(data);startMenuOpen=false;DungeonSaveStore.ActiveSlot=saveSlotSelection;saveSlotsOpen=false;slotNotice="";}catch(Exception e){slotNotice="No se pudo cargar: "+e.Message;}return;}
  if(battle||moving||lost||IntroductionActive||worldDialogue.Active||pendingChests.Count>0){slotNotice="Termina la acción o el diálogo antes de guardar.";return;}
  if(DungeonSaveStore.SlotExists(saveSlotSelection)&&overwriteSlot!=saveSlotSelection){overwriteSlot=saveSlotSelection;slotNotice="Este slot ya tiene una partida. Pulsa GUARDAR otra vez para reemplazarla.";return;}
  try{DungeonSaveStore.Write(CaptureDungeonSave(),saveSlotSelection,slotName);DungeonSaveStore.ActiveSlot=saveSlotSelection;overwriteSlot=-1;slotNotice="Partida guardada en "+slotName+".";if(!cityActive)SystemMessages.Show(SystemMessageCode.Saved,"",0,DigitalIcon.Backup);DungeonAudio.Play(DungeonAudio.Cue.Item);foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(!device.portal)device.ShowSaveResult(true);}catch(Exception e){slotNotice="No se pudo guardar: "+e.Message;}
 }
 void DrawSaveSlots(float w,float h){float x=w/2-350,y=h/2-275;NeonDungeonUI.Panel(new Rect(x,y,700,550));NeonDungeonUI.Label(new Rect(x+25,y+15,650,40),loadSlotMode?"CARGAR PARTIDA":"GUARDAR PARTIDA");for(int i=0;i<DungeonSaveStore.SlotCount;i++){string label="Slot "+(i+1)+" — Vacío";try{var data=DungeonSaveStore.Read(i);if(data!=null){label=SaveSlotSummary(data);}}catch{label="Slot "+(i+1)+" — No se puede leer";}if(GUI.Button(new Rect(x+25,y+65+i*48,650,40),(saveSlotSelection==i?"> ":"")+label)){saveSlotSelection=i;overwriteSlot=-1;ReadSlotName();}}
  NeonDungeonUI.Label(new Rect(x+25,y+405,650,55),slotNotice);if(GUI.Button(new Rect(x+25,y+470,310,45),loadSlotMode?"CARGAR":"GUARDAR"))UseSaveSlot();if(GUI.Button(new Rect(x+365,y+470,310,45),"VOLVER"))CloseSaveSlots();
 }
}
