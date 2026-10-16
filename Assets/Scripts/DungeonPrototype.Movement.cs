using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 public DungeonMovementSettings movementSettings;
 int pendingMovement;float pendingMovementUntil,nextHeldMovement;bool enemyNavigationRunning;
 DungeonMovementSettings Movement {get{return movementSettings!=null?movementSettings:DungeonMovementSettings.Current;}}
 bool ExperimentalMovement {get{return Movement!=null&&Movement.experimental;}}
 void ClearMovementInput(){pendingMovement=0;nextHeldMovement=Time.unscaledTime;}
 bool MovementInterfaceBlocked(){return DungeonWhiteTransition.Active||makongMusicTransition||cityActive||battle||lost||levelComplete||IntroductionActive||pauseOpen||worldDialogue.Active||transferMenu||nodeDecision||mapRevealActive||saveSlotsOpen||startMenuOpen;}
 #if UNITY_EDITOR
 public System.Func<bool,int> navigationTestInput;
#endif
 int ReadNavigation(bool press){
#if UNITY_EDITOR
  if(navigationTestInput!=null)return navigationTestInput(press);
#endif
 var k=Keyboard.current;var pad=Gamepad.current;
  if(k!=null){if(press?(k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame):(k.aKey.isPressed||k.leftArrowKey.isPressed))return 1;if(press?(k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame):(k.dKey.isPressed||k.rightArrowKey.isPressed))return 2;if(press?(k.wKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame):(k.wKey.isPressed||k.upArrowKey.isPressed))return 3;if(press?(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame):(k.sKey.isPressed||k.downArrowKey.isPressed))return 4;}
  if(pad!=null){if(press?pad.dpad.left.wasPressedThisFrame:pad.dpad.left.isPressed)return 1;if(press?pad.dpad.right.wasPressedThisFrame:pad.dpad.right.isPressed)return 2;if(press?pad.dpad.up.wasPressedThisFrame:pad.dpad.up.isPressed)return 3;if(press?pad.dpad.down.wasPressedThisFrame:pad.dpad.down.isPressed)return 4;}return 0;
 }
 void PollMovementInput(){
  if(MovementInterfaceBlocked()){ClearMovementInput();return;}
  int pressed=ReadNavigation(true);if(pressed!=0){if(!moving)StartNavigation(pressed);else if(Movement.inputBufferSeconds>0){pendingMovement=pressed;pendingMovementUntil=Time.unscaledTime+Movement.inputBufferSeconds;}return;}
  if(!moving)ContinueNavigation();
 }
 void ContinueNavigation(){if(MovementInterfaceBlocked()||moving)return;int command=0;if(pendingMovement!=0&&Time.unscaledTime<=pendingMovementUntil)command=pendingMovement;pendingMovement=0;
  if(command==0&&Time.unscaledTime>=nextHeldMovement){int held=ReadNavigation(false);if(held>=3&&Movement.continuousMovement||held>0&&held<3&&Movement.repeatTurns)command=held;}
  if(command!=0)StartNavigation(command);
 }
 void StartNavigation(int command){nextHeldMovement=Time.unscaledTime+Movement.repeatInterval;StartCoroutine(Move(command==3?1:command==4?-1:0,command==1?-1:command==2?1:0));}
 IEnumerator AnimateEnemyNavigation(){enemyNavigationRunning=true;yield return EnemyTurn();enemyNavigationRunning=false;if(!moving&&!MovementInterfaceBlocked())TryEncounter();}
}
