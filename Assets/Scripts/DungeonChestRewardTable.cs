using System;using System.Collections.Generic;using UnityEngine;
[Serializable]public sealed class DungeonChestRewardEntry {public string itemId;[Min(1)]public int minimum=1,maximum=1;[Min(0)]public int weight=1;}
[CreateAssetMenu(menuName="Virtual Dungeon/Cofres/Tabla de recompensas")]
public sealed class DungeonChestRewardTable:ScriptableObject {
 public string id=Guid.NewGuid().ToString("N");public int seedSalt;
 [Tooltip("Se comprueban las condiciones, sin ejecutar las acciones del perfil.")]public DungeonInteractionSettings conditions;
 public InventoryItemRecord[] guaranteed=new InventoryItemRecord[0];
 [Range(0,32)]public int randomRolls=1;public DungeonChestRewardEntry[] secondary=new DungeonChestRewardEntry[0];
 public List<InventoryItemRecord> Roll(string mapId,DungeonMapEntity chest){
  var result=new List<InventoryItemRecord>();Add(result,chest.contentId,chest.quantity);
  foreach(var item in guaranteed??new InventoryItemRecord[0])if(item!=null)Add(result,item.itemId,item.quantity);
  // Story/key chests never depend on the random table.
  if(chest.chestKind==DungeonChestKind.KeyItem)return result;
  uint hash=2166136261;foreach(char c in (mapId??"")+"/"+chest.id+"/"+id)hash=unchecked((hash^c)*16777619);var random=new System.Random(unchecked((int)hash)^seedSalt);
  double total=0;foreach(var entry in secondary??new DungeonChestRewardEntry[0])if(entry!=null&&entry.weight>0&&!string.IsNullOrEmpty(entry.itemId))total+=entry.weight;
  for(int n=0;n<Mathf.Clamp(randomRolls,0,32)&&total>0;n++){double pick=random.NextDouble()*total;foreach(var entry in secondary){if(entry==null||entry.weight<=0||string.IsNullOrEmpty(entry.itemId))continue;pick-=entry.weight;if(pick>=0)continue;int min=Math.Max(1,entry.minimum),max=Math.Max(min,entry.maximum);long quantity=min+(long)(random.NextDouble()*((long)max-min+1));Add(result,entry.itemId,(int)Math.Min(int.MaxValue,quantity));break;}}
  return result;
 }
 static void Add(List<InventoryItemRecord> target,string id,int quantity){if(!string.IsNullOrEmpty(id)&&quantity>0)target.Add(new InventoryItemRecord{itemId=id,quantity=quantity});}
 public List<string> Validate(CombatInventorySettings inventory,PlayerCharacterSettings character){
  var errors=new List<string>();if(string.IsNullOrEmpty(id))errors.Add("Tabla de cofre sin ID");if(randomRolls<0||randomRolls>32)errors.Add("Tiradas fuera de 0–32");
  foreach(var row in guaranteed??new InventoryItemRecord[0])if(row==null||row.quantity<1||!Known(row.itemId,inventory,character))errors.Add("Recompensa garantizada inválida");
  bool weighted=false;foreach(var row in secondary??new DungeonChestRewardEntry[0]){if(row==null||row.minimum<1||row.maximum<row.minimum||row.weight<0||!Known(row.itemId,inventory,character)){errors.Add("Entrada secundaria inválida");continue;}weighted|=row.weight>0;var item=Array.Find(inventory.items,i=>i!=null&&i.id==row.itemId);if(item!=null&&item.keyItem)errors.Add("Las llaves deben ser garantizadas: "+row.itemId);if(character!=null&&Array.Exists(character.equipment,e=>e!=null&&e.id==row.itemId)&&row.maximum!=1)errors.Add("Equipo no apilable: "+row.itemId);}
  if(randomRolls>0&&!weighted)errors.Add("Tabla aleatoria sin entradas con peso positivo");return errors;
 }
 static bool Known(string id,CombatInventorySettings inventory,PlayerCharacterSettings character){return !string.IsNullOrEmpty(id)&&(inventory!=null&&Array.Exists(inventory.items,i=>i!=null&&i.id==id)||character!=null&&Array.Exists(character.equipment,e=>e!=null&&e.id==id));}
}
