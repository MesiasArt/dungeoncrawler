using System.Collections.Generic;
public partial class DungeonPrototype {
 bool ChestRewardConditionsMet(DungeonMapEntity chest){return chest.chestRewards==null||DungeonConditionsMet(chest.chestRewards.conditions);}
 bool CollectChestRewards(DungeonMapEntity chest){
  var state=EntityState(chest.id);if(state.opened)return true;
  if(!state.chestRewardsRolled){state.chestRewards=chest.chestRewards.Roll(ActiveMap.mapId,chest);state.chestRewardCursor=0;state.chestRewardsRolled=true;}
  if(state.chestRewards==null)state.chestRewards=new List<InventoryItemRecord>();
  while(state.chestRewardCursor<state.chestRewards.Count){var row=state.chestRewards[state.chestRewardCursor];if(row==null||!AcquireItem(row.itemId,row.quantity))return false;state.chestRewardCursor++;}
  return true;
 }
}
