using System;using System.Collections.Generic;using UnityEngine;
[Serializable]public sealed class CityNoticeRule {
 public string id,locationId;public bool requireAllLocationsVisited;
 public string[] requiredFlags=new string[0],blockedFlags=new string[0],requiredVisits=new string[0],grantFlags=new string[0];
 public WorldDialogueLine[] dialogue=new WorldDialogueLine[0];
}
[Serializable]public sealed class CityDungeonDestination {public string id,displayName,scenePath,requiredFlag;public DungeonMapSettings map;public bool allowEntryWithoutNode=true,includeLegacyNodes;public string entryNodeName="Entrada";public WorldDialogueLine[] entryDialogue=new WorldDialogueLine[0];}
[CreateAssetMenu(menuName="Virtual Dungeon/Digital Kingdom/Historia y avisos")]
public sealed class DigitalKingdomStorySettings:ScriptableObject {
 public string badge="NEW",guideQuestion="Preguntar por los dungeons",travelPrefix="Viajar a ";
 public CityNoticeRule[] notices=new CityNoticeRule[0];public CityDungeonDestination[] destinations=new CityDungeonDestination[0];
 public WorldDialogueLine[] noDestinationDialogue=new WorldDialogueLine[0];
}
// Rules contain stable IDs and only use the city state already serialized in the save.
public static class CityNoticeSystem {
 public static void Normalize(DigitalKingdomProgress state){if(state.visitedLocations==null)state.visitedLocations=new List<string>();if(state.storyFlags==null)state.storyFlags=new List<string>();if(state.readNotices==null)state.readNotices=new List<string>();}
 public static bool Available(CityNoticeRule rule,DigitalKingdomProgress state,DigitalKingdomLocation[] visible){Normalize(state);if(rule==null||string.IsNullOrEmpty(rule.id)||state.readNotices.Contains(rule.id))return false;foreach(var flag in rule.requiredFlags)if(!state.storyFlags.Contains(flag))return false;foreach(var flag in rule.blockedFlags)if(state.storyFlags.Contains(flag))return false;foreach(var visit in rule.requiredVisits)if(!state.visitedLocations.Contains(visit))return false;if(rule.requireAllLocationsVisited)foreach(var location in visible)if(location!=null&&location.available&&state.unlockedLocations.Contains(location.id)&&!state.visitedLocations.Contains(location.id))return false;return true;}
 public static void Read(CityNoticeRule rule,DigitalKingdomProgress state){Normalize(state);if(state.readNotices.Contains(rule.id))return;state.readNotices.Add(rule.id);foreach(var flag in rule.grantFlags)if(!state.storyFlags.Contains(flag))state.storyFlags.Add(flag);}
}
