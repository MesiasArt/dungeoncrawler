using System;using System.Collections.Generic;using UnityEngine;
public enum DungeonConditionKind { Item,EnemyDefeated,SwitchActive,EventCompleted,HackUnlocked }
public enum DungeonConditionJoin { All,Any }
public enum DungeonEventActionKind { OpenDoor,CloseDoor,SetSwitch,UnlockConnection,Dialogue,Message,Reward,CompleteEvent,RunEvent }
[Serializable] public sealed class DungeonCondition {public DungeonConditionKind kind;public DungeonMapSettings sourceMap;public string targetId;[Min(1)]public int quantity=1;public bool negate;}
[Serializable] public sealed class DungeonConditionGroup {public DungeonConditionJoin join;public List<DungeonCondition> conditions=new List<DungeonCondition>();}
[Serializable] public sealed class DungeonEventAction {
 public string id=Guid.NewGuid().ToString("N");public DungeonEventActionKind kind;public string targetId;public bool value=true;[Min(1)]public int quantity=1;
 public WorldDialogueLine[] dialogue=new WorldDialogueLine[0];[TextArea]public string message;public DungeonInteractionSettings eventProfile;
}
[Serializable] public sealed class DungeonLegacyInteractionBinding {public string targetId;public DungeonInteractionSettings profile;}
[Serializable] public sealed class DungeonSwitchState {public string id;public bool active;}
[Serializable] public sealed class DungeonEventExecution {public string id;public int nextAction;public bool completed;}
[Serializable] public sealed class DungeonInteractionProgress {
 public List<DungeonBossEventRecord> pendingBossEvents=new List<DungeonBossEventRecord>();
 public List<string> defeatedEnemyIds=new List<string>();
 public List<DungeonSwitchState> switches=new List<DungeonSwitchState>();public List<string> completedEvents=new List<string>(),unlockedConnections=new List<string>(),rewardReceipts=new List<string>();public List<DungeonEventExecution> executions=new List<DungeonEventExecution>();
}
[CreateAssetMenu(menuName="Virtual Dungeon/Interacción y evento")]
public sealed class DungeonInteractionSettings:ScriptableObject {
 public string id=Guid.NewGuid().ToString("N");public bool once=true,continueLegacy=true;public DungeonConditionJoin join;
 public List<DungeonConditionGroup> groups=new List<DungeonConditionGroup>();public List<DungeonEventAction> actions=new List<DungeonEventAction>();
 [TextArea]public string deniedMessage="Todavía no puedes activar este mecanismo.";
}
