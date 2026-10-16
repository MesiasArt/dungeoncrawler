using System.Collections;using UnityEngine;
public partial class DungeonPrototype {
 const string WelcomeEligibility="refuge.welcome.eligible";
 bool welcomeRunning,welcomeLooking;Coroutine welcomeLookRoutine;
 Quaternion welcomeCameraRotation;float welcomeCameraFov;
 void ArmRefugeWelcome(){if(ActiveMap!=DungeonMapSettings.Current||refugeSettings==null)return;if(!interactionProgress.completedEvents.Contains(WelcomeEligibility))interactionProgress.completedEvents.Add(WelcomeEligibility);}
 bool UpdateRefugeWelcome(){
  if(welcomeLooking)return true;if(welcomeRunning)return false;
  var settings=RefugeWelcomeSettings.Current;if(settings==null||settings.dialogue==null||ActiveMap!=DungeonMapSettings.Current||refugeSettings==null||refuge.refugeTragedyTriggered||!introductionCompleted||IntroductionActive||moving||battle||lost||levelComplete||pauseOpen||startMenuOpen||saveSlotsOpen||worldDialogue.Active||position!=settings.triggerCell||!interactionProgress.completedEvents.Contains(WelcomeEligibility)||interactionProgress.completedEvents.Contains(settings.dialogue.id))return false;
  ClearMovementInput();ResetConversationCamera();welcomeCameraRotation=view.transform.rotation;welcomeCameraFov=view.fieldOfView;welcomeRunning=true;worldDialogue.LineStarted+=WelcomeLineStarted;
  bool accepted=StartDungeonEvent(settings.dialogue,"refuge.welcome.trigger",success=>CancelRefugeWelcome());if(!accepted)CancelRefugeWelcome();return accepted;
 }
 void WelcomeLineStarted(WorldDialogueLine line){if(!welcomeRunning||line==null||line.presentationCue!="refuge.welcome.look-back")return;welcomeLooking=true;ClearMovementInput();ResetConversationCamera();welcomeLookRoutine=StartCoroutine(WelcomeLookBehind());}
 IEnumerator WelcomeLookBehind(){
  var settings=RefugeWelcomeSettings.Current;var forward=Quaternion.Euler(0,heading*90,0);var from=view.transform.rotation;var behind=Quaternion.Euler(0,heading*90+180,0);view.fieldOfView=welcomeCameraFov;
  float duration=Mathf.Max(.1f,settings.turnSeconds);for(float t=0;t<duration;t+=Time.unscaledDeltaTime){if(pauseOpen){yield return null;t-=Time.unscaledDeltaTime;continue;}view.transform.rotation=Quaternion.Slerp(from,behind,Mathf.SmoothStep(0,1,t/duration));yield return null;}view.transform.rotation=behind;
  for(float t=0;t<settings.lookHoldSeconds;t+=Time.unscaledDeltaTime){if(pauseOpen)t-=Time.unscaledDeltaTime;yield return null;}
  for(float t=0;t<duration;t+=Time.unscaledDeltaTime){if(pauseOpen){yield return null;t-=Time.unscaledDeltaTime;continue;}view.transform.rotation=Quaternion.Slerp(behind,forward,Mathf.SmoothStep(0,1,t/duration));yield return null;}
  view.transform.rotation=forward;welcomeLooking=false;welcomeLookRoutine=null;ClearMovementInput();
 }
 void CancelRefugeWelcome(){
  worldDialogue.LineStarted-=WelcomeLineStarted;if(welcomeLookRoutine!=null){StopCoroutine(welcomeLookRoutine);welcomeLookRoutine=null;}
  if(welcomeRunning&&view!=null){ResetConversationCamera();view.transform.rotation=welcomeCameraRotation;view.fieldOfView=welcomeCameraFov;}
  welcomeRunning=welcomeLooking=false;ClearMovementInput();
 }
}
