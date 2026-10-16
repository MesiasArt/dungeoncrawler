using UnityEngine;
public partial class DungeonPrototype {
 public PlayerProfileSettings playerProfile;
 public ManualMapProvider mapProvider;
 public PlayerProfileSettings ActivePlayerProfile{get{if(playerProfile==null)playerProfile=PlayerProfileSettings.Current;return playerProfile;}}
 public DungeonMapDefinition CurrentMapDefinition{get{
  // A provider assigned for another floor must not replace a travel destination.
  if(mapProvider!=null&&(mapProvider.map==null||mapProvider.map==ActiveMap))return mapProvider.CreateDefinition(ActiveMap);
  return ManualMapProvider.Capture(ActiveMap);
 }}
 void ApplyPlayerProfileDefaults(){var profile=ActivePlayerProfile;if(profile==null)return;if(characterSettings==null)characterSettings=profile.character;if(playerLevelSettings==null)playerLevelSettings=profile.levels;if(playerBitsSettings==null)playerBitsSettings=profile.bits;if(inventorySettings==null)inventorySettings=profile.inventory;if(playerHackCatalog==null)playerHackCatalog=profile.hacks;}
}
