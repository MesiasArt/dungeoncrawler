using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public class InventoryItemRecord {public string itemId;public int quantity;}
[Serializable] public class PlayerProgressionRecord {
 public bool hasPlayTime;public double playedSeconds;
 public SavePointReturnRecord lastSavePoint;
 public int level=1;public long experience;
 public string[] eventHackIds=new string[0];
 public int coins;public List<InventoryItemRecord> pendingCombatDrops=new List<InventoryItemRecord>();
 public bool hasBits;public int currentBits;
 public int currentHP;public List<InventoryItemRecord> inventory=new List<InventoryItemRecord>();
 public string[] equipmentIds=new string[3];
}
public partial class DungeonPrototype {
 List<int> InventoryEquipment(){EnsureEquipment();var result=new List<int>();for(int i=0;i<Character.equipment.Length;i++)if(Character.equipment[i]!=null&&ownedEquipment.Contains(Character.equipment[i].id)&&!equipped[i])result.Add(i);return result;}
 public bool AcquireEquipment(string itemId) {
  EnsureEquipment();var item=Array.Find(Character.equipment,e=>e!=null&&e.id==itemId);
  if(item==null||ownedEquipment.Contains(itemId)||InventoryCellsUsed>=18)return false;
  ownedEquipment.Add(itemId);NotifyReceived(itemId,1);return true;
 }
 struct InventoryCell {public int potion,equipment;}
 List<InventoryCell> InventoryCells(){var cells=new List<InventoryCell>();if(itemCounts!=null)for(int i=0;i<itemCounts.Length;i++)if(itemCounts[i]>0)cells.Add(new InventoryCell{potion=i,equipment=-1});foreach(int i in InventoryEquipment())cells.Add(new InventoryCell{potion=-1,equipment=i});return cells;}
 public bool AcquireItem(string id,int quantity=1){if(quantity<=0)return false;if(Array.Exists(Character.equipment,e=>e!=null&&e.id==id))return quantity==1&&AcquireEquipment(id);int i=Array.FindIndex(inventorySettings.items,e=>e!=null&&e.id==id);if(i<0||itemCounts[i]==0&&InventoryCellsUsed>=18||itemCounts[i]>int.MaxValue-quantity)return false;itemCounts[i]+=quantity;NotifyReceived(id,quantity);return true;}
 int InventoryCellsUsed {get{int count=0;if(itemCounts!=null)foreach(int n in itemCounts)if(n>0)count++;EnsureEquipment();foreach(var id in ownedEquipment){int i=Array.FindIndex(Character.equipment,e=>e!=null&&e.id==id);if(i>=0&&!equipped[i])count++;}return count;}}
 public bool EquipItem(string id) {
  EnsureEquipment();int index=Array.FindIndex(Character.equipment,e=>e!=null&&e.id==id);
  if(index<0||!ownedEquipment.Contains(id))return false;
  int previous=EquippedInSlot(Character.equipment[index].SlotIndex);
  if(previous==index)return true;
  equippedIds[Character.equipment[index].SlotIndex]=id;EnsureEquipment();hp=Mathf.Min(hp,MaximumHP);return true;
 }
 public bool UnequipSlot(int slot) {
  int index=EquippedInSlot(slot);if(index<0)return true;if(InventoryCellsUsed>=18)return false;
  equippedIds[slot]=null;EnsureEquipment();hp=Mathf.Min(hp,MaximumHP);return true;
 }
 public PlayerProgressionRecord CapturePlayerProgression() {
  EnsureEquipment();var result=new PlayerProgressionRecord{hasPlayTime=true,playedSeconds=CurrentPlayedSeconds,lastSavePoint=lastSavePoint==null?null:DungeonMapPersistence.Copy(lastSavePoint),level=PlayerLevel,experience=playerExperience,eventHackIds=new List<string>(eventHackIds).ToArray(),currentHP=hp,hasBits=true,currentBits=CurrentBits,coins=coins,pendingCombatDrops=new List<InventoryItemRecord>(pendingCombatDrops)};
  foreach(var id in ownedEquipment)result.inventory.Add(new InventoryItemRecord{itemId=id,quantity=1});
  for(int i=0;i<itemCounts.Length;i++)if(itemCounts[i]>0)result.inventory.Add(new InventoryItemRecord{itemId=inventorySettings.items[i].id,quantity=itemCounts[i]});
  for(int slot=0;slot<3;slot++){int i=EquippedInSlot(slot);result.equipmentIds[slot]=i<0?null:Character.equipment[i].id;}
  return result;
 }
 readonly Dictionary<int,EnemyInstanceRecord> defeatedEnemies=new Dictionary<int,EnemyInstanceRecord>();
 void RecordDefeated(int index){var s=enemyObjects[index].GetComponent<EnemyCombatStats>();defeatedEnemies[index]=new EnemyInstanceRecord{instanceId=s.instanceId,speciesId=s.speciesId,zoneId=s.zoneId,placementId=s.placementId,category=s.category,level=s.level,currentHP=0,cell=enemies[index],defeated=true,stats=s.snapshot};CaptureExplorationState(defeatedEnemies[index]);RecordDungeonDefeat(defeatedEnemies[index]);}
 public List<EnemyInstanceRecord> CaptureEnemyProgression() {
  var result=new List<EnemyInstanceRecord>();for(int i=0;i<enemyObjects.Count;i++)if(enemyObjects[i]!=null&&!combatSummons.Contains(i)){var s=enemyObjects[i].GetComponent<EnemyCombatStats>();result.Add(new EnemyInstanceRecord{instanceId=s.instanceId,speciesId=s.speciesId,zoneId=s.zoneId,placementId=s.placementId,category=s.category,level=s.level,currentHP=enemyLives[i],cell=enemies[i],alerted=alerted[i],stats=s.snapshot});}for(int i=0;i<result.Count;i++)CaptureExplorationState(result[i]);foreach(var pair in defeatedEnemies){if(combatSummons.Contains(pair.Key))continue;var record=pair.Value;record.respawnRemaining=Mathf.Max(0,respawnAt[pair.Key]-Time.unscaledTime);result.Add(record);}return result;
 }
}
