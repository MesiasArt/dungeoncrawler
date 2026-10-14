using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 public PlayerHackCatalog playerHackCatalog;
 PlayerHackCatalog HackCatalog {get{if(playerHackCatalog==null)playerHackCatalog=PlayerHackCatalog.Current;return playerHackCatalog;}}
 readonly List<PlayerHackDefinition> availableHackDefinitions=new List<PlayerHackDefinition>();
 readonly Dictionary<System.Type,CombatHack> hackImplementations=new Dictionary<System.Type,CombatHack>();
 readonly HashSet<string> eventHackIds=new HashSet<string>();
 readonly HashSet<string> notifiedHackIds=new HashSet<string>();
 readonly Queue<string> hackUnlockNotices=new Queue<string>();
 string selectedHackId;
 float hackNoticeUntil;string hackNotice;
 CombatHack HackImplementation(PlayerHackDefinition definition){var type=definition.ImplementationType;if(type==null)return null;CombatHack hack;if(!hackImplementations.TryGetValue(type,out hack)||hack==null){hack=GetComponent(type) as CombatHack;if(hack==null)hack=gameObject.AddComponent(type) as CombatHack;hackImplementations[type]=hack;}return hack!=null&&hack.isActiveAndEnabled?hack:null;}
 void RefreshAvailableHacks(){availableHackDefinitions.Clear();var implementations=new List<CombatHack>();var catalog=HackCatalog;if(catalog!=null)foreach(var h in catalog.hacks??new PlayerHackDefinition[0])if(catalog.ValidEntry(h)&&h.Unlocked(PlayerLevel,eventHackIds)){var impl=HackImplementation(h);if(impl!=null){availableHackDefinitions.Add(h);implementations.Add(impl);}}availableHacks=implementations.ToArray();if(combatMenu==CombatMenu.Hacks)selectedCombatAction=Mathf.Clamp(selectedCombatAction,0,availableHacks.Length);}
 bool CanExecuteHack(CombatHack hack){RefreshAvailableHacks();if(!string.IsNullOrEmpty(selectedHackId)){for(int i=0;i<availableHacks.Length;i++)if(availableHackDefinitions[i].id==selectedHackId&&availableHacks[i]==hack)return true;return false;}for(int i=0;i<availableHacks.Length;i++)if(availableHacks[i]==hack){selectedHackId=availableHackDefinitions[i].id;return true;}return false;}
 void InitializeHackAvailability(){hackUnlockNotices.Clear();hackNoticeUntil=0;hackNotice=null;selectedHackId=null;notifiedHackIds.Clear();RefreshAvailableHacks();foreach(var h in availableHackDefinitions)notifiedHackIds.Add(h.id);}
 void RestoreHackAvailability(PlayerProgressionRecord record){eventHackIds.Clear();foreach(var id in record.eventHackIds??new string[0])if(!string.IsNullOrEmpty(id))eventHackIds.Add(id);if(!cityDataOnly)InitializeHackAvailability();}
 void PlayerLevelHackUnlocks(int oldLevel){RefreshAvailableHacks();foreach(var h in availableHackDefinitions)if(h.unlockMode==HackUnlockMode.PlayerLevel&&h.minimumLevel>oldLevel&&notifiedHackIds.Add(h.id))hackUnlockNotices.Enqueue(h.displayName);}
 public string[] AvailableHackIds {get{RefreshAvailableHacks();return availableHackDefinitions.ConvertAll(h=>h.id).ToArray();}}
 void DrawHackUnlockNotice(float width){if(Time.unscaledTime>=hackNoticeUntil){if(hackUnlockNotices.Count==0)return;hackNotice="NEW HACK UNLOCKED — "+hackUnlockNotices.Dequeue();hackNoticeUntil=Time.unscaledTime+4;}var style=new GUIStyle(GUI.skin.label){fontSize=18,alignment=TextAnchor.MiddleCenter};style.normal.textColor=NeonDungeonUI.Cyan;GUI.Label(new Rect(width/2-270,148,540,28),hackNotice,style);}
}
