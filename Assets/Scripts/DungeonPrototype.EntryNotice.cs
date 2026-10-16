using System.Collections;using System;using UnityEngine;
public partial class DungeonPrototype {
 IEnumerator ShowDungeonEntryNotice(){
  while(DungeonWhiteTransition.Active)yield return null;
  if(City==null||City.story==null)yield break;
  var route=Array.Find(City.story.destinations,r=>r!=null&&r.map==ActiveMap);
  if(route==null||route.entryDialogue==null||route.entryDialogue.Length==0)yield break;
  string id="dungeon.entry.notice.v1:"+ActiveMap.mapId;
  if(interactionProgress.completedEvents.Contains(id))yield break;
  while(worldDialogue.Active||battle||moving)yield return null;
  worldDialogue.Begin(route.entryDialogue,()=>{if(!interactionProgress.completedEvents.Contains(id))interactionProgress.completedEvents.Add(id);});
 }
}
