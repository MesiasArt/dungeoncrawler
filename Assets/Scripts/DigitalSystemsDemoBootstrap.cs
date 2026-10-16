using UnityEngine;
// An isolated scene using the actual dungeon controller, camera, inventory and door interactions.
public sealed class DigitalSystemsDemoBootstrap:MonoBehaviour {
 public DungeonMapSettings demoMap;
 #if UNITY_EDITOR
 string previousSavePath;
 #endif
 void Awake(){
  #if UNITY_EDITOR
  previousSavePath=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath=System.IO.Path.Combine(Application.temporaryCachePath,"CodeboundDigitalSystemsDemo/save.json");System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DungeonSaveStore.editorTestPath));
  #endif
  var world=gameObject.AddComponent<DungeonPrototype>();world.mapSettings=demoMap;world.dungeonRegions=demoMap.regions;
  var data=new DungeonSaveData{version=2,activeMapId=demoMap.mapId,cell=demoMap.startCell,heading=0,introductionCompleted=true,player=new PlayerProgressionRecord{currentHP=30,level=8,hasBits=true,currentBits=100}};
  foreach(var item in Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings").items)if(item.keyItem||item.healHP>0)data.player.inventory.Add(new InventoryItemRecord{itemId=item.id,quantity=3});
  typeof(DungeonPrototype).GetField("incomingMapTravel",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(world,data);
 }
 void OnDestroy(){
  #if UNITY_EDITOR
  DungeonSaveStore.editorTestPath=previousSavePath;
  #endif
 }
}
