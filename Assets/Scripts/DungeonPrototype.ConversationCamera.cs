using UnityEngine;
public partial class DungeonPrototype {
 Transform conversationFocus;bool conversationCameraOwned,conversationLooking;float conversationOriginalFov;Quaternion conversationOriginalRotation;
 void FocusConversationOn(Transform target){conversationFocus=target;}
 Transform CurrentConversationSpeaker(){
  var line=worldDialogue.Current;if(line==null)return null;
  if(refugeSettings!=null){var npc=System.Array.Find(refugeSettings.npcs,n=>n.displayName==line.speaker);GameObject actor;if(npc!=null&&refugeActors.TryGetValue(npc.id,out actor)&&actor.activeSelf)return actor.transform;}
  if(ActiveMap!=null&&ActiveMap.entities!=null)foreach(var entity in ActiveMap.entities.objects){if(entity.kind!=DungeonEntityKind.NPC)continue;var npc=entity.npcCatalog!=null?System.Array.Find(entity.npcCatalog.npcs,n=>n.id==entity.npcId):null;GameObject actor;if((npc!=null&&npc.displayName==line.speaker||entity.displayName==line.speaker)&&mapEntityObjects.TryGetValue(entity.id,out actor)&&actor.activeSelf)return actor.transform;}
  return conversationFocus!=null&&conversationFocus.gameObject.activeInHierarchy?conversationFocus:null;
 }
 void TickConversationCamera(){
  if(view==null||pauseOpen)return;
  Transform target=worldDialogue.Active?CurrentConversationSpeaker():null;
  bool focus=Layout.zoomDuringConversation&&worldDialogue.Active&&target!=null&&!battle&&!IntroductionActive;
  if(focus){
   if(!conversationCameraOwned){conversationOriginalFov=view.fieldOfView;conversationOriginalRotation=refugeExitLooking?refugeExitViewRotation:view.transform.rotation;conversationCameraOwned=true;}
   conversationLooking=true;conversationFocus=target;
   float k=1-Mathf.Exp(-5*Time.unscaledDeltaTime/Mathf.Max(.1f,Layout.conversationZoomSeconds));view.fieldOfView=Mathf.Lerp(view.fieldOfView,Mathf.Min(conversationOriginalFov,Layout.conversationFieldOfView),k);
   if(!refugeExitLooking){var visual=target.GetComponent<GhostEnemyVisual>();Vector3 aim=target.position+Vector3.up*(visual!=null?visual.WorldHeight*.65f:1.2f);if((aim-view.transform.position).sqrMagnitude>.01f)view.transform.rotation=Quaternion.Slerp(view.transform.rotation,Quaternion.LookRotation(aim-view.transform.position),k);}
  }else if(conversationCameraOwned){
   if(battle){ResetConversationCamera();return;}
   if(conversationLooking){if(!moving)view.transform.rotation=conversationOriginalRotation;conversationLooking=false;conversationFocus=null;}
   view.fieldOfView=Mathf.Lerp(view.fieldOfView,conversationOriginalFov,1-Mathf.Exp(-5*Time.unscaledDeltaTime/Mathf.Max(.1f,Layout.conversationZoomSeconds)));
   if(Mathf.Abs(view.fieldOfView-conversationOriginalFov)<.05f){view.fieldOfView=conversationOriginalFov;conversationCameraOwned=false;}
  }else conversationFocus=null;
 }
 void ResetConversationCamera(){if(conversationCameraOwned&&view!=null){view.fieldOfView=conversationOriginalFov;view.transform.rotation=conversationOriginalRotation;}conversationCameraOwned=conversationLooking=false;conversationFocus=null;}
}
