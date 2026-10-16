using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 readonly HashSet<string> pendingDoorKeys=new HashSet<string>();
 bool ConsumeDoorKey(string id){int index=System.Array.FindIndex(inventorySettings.items,item=>item!=null&&item.id==id);if(index>=0&&itemCounts[index]>0){itemCounts[index]--;return true;}if(ownedEquipment.Remove(id)){for(int slot=0;slot<equippedIds.Length;slot++)if(equippedIds[slot]==id)equippedIds[slot]=null;EnsureEquipment();return true;}return false;}
 void OnMapDoorOpened(DungeonMapEntity entity,DungeonDoor door){var state=EntityState(entity.id);if(pendingDoorKeys.Remove(entity.id)&&!state.keyConsumed){if(!ConsumeDoorKey(entity.requiredKeyId)){door.SetClosedImmediately();state.opened=false;return;}state.keyConsumed=true;}state.opened=door.IsOpen;if(state.keyConsumed)door.KeepOpenPermanently();}
 bool RestorePermanentKeyDoor(DungeonMapEntity entity,DungeonDoor door){var state=EntityState(entity.id);if(entity.doorKind!=DungeonDoorKind.Locked)return false;
  // Old saves kept the used key and only stored opened. Migrate that successful unlock once.
  if(!state.keyConsumed&&state.opened&&!ConnectionUnlocked(entity.id)){if(HasMapKey(entity.requiredKeyId))ConsumeDoorKey(entity.requiredKeyId);state.keyConsumed=true;}
  if(!state.keyConsumed)return false;state.opened=true;pendingDoorKeys.Remove(entity.id);door.KeepOpenPermanently();return true;
 }
}
