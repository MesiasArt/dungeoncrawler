using System;using System.Collections.Generic;using UnityEngine;
public static class DrpgPhase6Diagnostics {
 public static List<string> Validate(DungeonMapSettings map){var errors=new List<string>();if(map==null)return errors;var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var character=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");var tableIds=new Dictionary<string,DungeonChestRewardTable>();var bossIds=new Dictionary<string,DungeonBossProfile>();
  if(map.entities!=null)foreach(var chest in map.entities.objects){var table=chest!=null?chest.chestRewards:null;if(table==null)continue;if(chest.kind!=DungeonEntityKind.Chest||chest.chestKind==DungeonChestKind.Mimic)errors.Add(chest.id+": tabla solo permitida en cofres no Mimic");foreach(var error in table.Validate(inventory,character))errors.Add(chest.id+": "+error);DungeonChestRewardTable other;if(tableIds.TryGetValue(table.id??"",out other)&&other!=table)errors.Add("ID de tabla duplicado: "+table.id);tableIds[table.id??""]=table;}
  if(map.regions!=null)foreach(var placement in map.regions.placedEnemies){var boss=placement!=null?placement.bossProfile:null;if(boss==null)continue;foreach(var error in boss.Validate(placement.speciesId))errors.Add(placement.id+": "+error);DungeonBossProfile other;if(bossIds.TryGetValue(boss.id??"",out other)&&other!=boss)errors.Add("ID de jefe duplicado: "+boss.id);bossIds[boss.id??""]=boss;}
  return errors;
 }
}
