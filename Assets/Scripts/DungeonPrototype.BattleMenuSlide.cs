using UnityEngine;
public partial class DungeonPrototype {
 bool battleMenuAnimating,battleMenuSwapped;
 float battleMenuSlideStarted,battleMenuSlideX;
 CombatMenu battleMenuDestination;
 int battleMenuDestinationSelection;
 void ChangeBattleMenu(CombatMenu destination,int selection){
  if(battleMenuAnimating)return;
  var ui=BattleUISettings.Current;
  if(ui==null||!ui.useNewArtwork||ui.menuSlideSeconds<=0||combatMenu==destination){combatMenu=destination;selectedCombatAction=selection;return;}
  battleMenuDestination=destination;battleMenuDestinationSelection=selection;battleMenuSlideStarted=Time.unscaledTime;battleMenuSwapped=false;battleMenuAnimating=true;battleMenuSlideX=0;
 }
 void TickBattleMenuSlide(){
  if(!battleMenuAnimating)return;
  if(!battle||lost||worldDialogue.Active||(combatHack!=null&&combatHack.Active)){battleMenuAnimating=false;battleMenuSlideX=0;return;}
  var ui=BattleUISettings.Current;
  if(ui==null){combatMenu=battleMenuDestination;selectedCombatAction=battleMenuDestinationSelection;battleMenuAnimating=false;battleMenuSlideX=0;return;}
  float t=Mathf.Clamp01((Time.unscaledTime-battleMenuSlideStarted)/Mathf.Max(.01f,ui.menuSlideSeconds));
  float distance=Mathf.Max(ui.commandSize.x,ui.hackButtonSize.x)+Mathf.Max(0,ui.commandOffset.x)+ui.menuSlidePadding;
  if(t<.5f)battleMenuSlideX=-distance*Mathf.SmoothStep(0,1,t*2);
  else{
   if(!battleMenuSwapped){combatMenu=battleMenuDestination;selectedCombatAction=battleMenuDestinationSelection;battleMenuSwapped=true;}
   battleMenuSlideX=-distance*(1-Mathf.SmoothStep(0,1,(t-.5f)*2));
  }
  if(t>=1){battleMenuAnimating=false;battleMenuSlideX=0;}
 }
}
