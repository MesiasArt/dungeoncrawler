using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public class DungeonSaveData {
 public DigitalKingdomProgress city;
 public DungeonInteractionProgress interactions=new DungeonInteractionProgress();
 public string activeMapId;
 public List<DungeonMapSnapshot> maps=new List<DungeonMapSnapshot>();
 public List<Vector2Int> visited=new List<Vector2Int>();
 public List<DungeonEntityState> legacyDoors=new List<DungeonEntityState>();
 public string slotName,savedUtc;
 public int directorRestRemaining;
 public int version=1;public bool introductionCompleted;
 public Vector2Int cell;public int heading;public PlayerProgressionRecord player;public RefugeProgress refuge;
 public List<EnemyInstanceRecord> enemies=new List<EnemyInstanceRecord>();public string[] triggeredEncounters;
 public List<DungeonEntityState> entityStates=new List<DungeonEntityState>();
 public List<Vector2Int> explored=new List<Vector2Int>();
}
public static class DungeonSaveStore {
 static readonly int[] revisions=new int[SlotCount];
 internal static int SlotRevision(int slot){return revisions[slot];}
 internal static string ContextKey {get{
  #if UNITY_EDITOR
  return editorTestPath??"";
  #else
  return "";
  #endif
 }}
 internal static string[] CandidatePaths(int slot){var paths=new List<string>();string path=SlotPath(slot);paths.Add(path);for(int i=1;i<=5;i++)paths.Add(path+".bak"+i);if(slot==0&&AllowLegacy)paths.Add(LegacyPath);return paths.ToArray();}
 #if UNITY_EDITOR
 public static string editorTestPath;
 #endif
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void ResetTestSlot(){
  Array.Clear(revisions,0,revisions.Length);
 #if UNITY_EDITOR
 editorTestPath=null;
 #endif
 }
 public const int SlotCount=6;
 public static int ActiveSlot {get{return Mathf.Clamp(PlayerPrefs.GetInt("Codebound.ActiveSaveSlot",0),0,SlotCount-1);}set{PlayerPrefs.SetInt("Codebound.ActiveSaveSlot",Mathf.Clamp(value,0,SlotCount-1));PlayerPrefs.Save();}}
 static string LegacyPath {get{return Path.Combine(Application.persistentDataPath,"Codebound-save-v1.json");}}
 public static string SlotPath(int slot){if(slot<0||slot>=SlotCount)throw new ArgumentOutOfRangeException("slot");
 #if UNITY_EDITOR
 if(!string.IsNullOrEmpty(editorTestPath))return slot==0?editorTestPath:editorTestPath+".slot-"+(slot+1)+".json";
 #endif
 return Path.Combine(Application.persistentDataPath,"Codebound-slot-"+(slot+1)+"-v1.json");}
 public static string SavePath {get{return SlotPath(ActiveSlot);}}
 public static bool SlotExists(int slot){string path=SlotPath(slot);if(File.Exists(path))return true;for(int i=1;i<=5;i++)if(File.Exists(path+".bak"+i))return true;
 #if UNITY_EDITOR
 if(!string.IsNullOrEmpty(editorTestPath))return false;
 #endif
 return slot==0&&File.Exists(LegacyPath);}
 public static bool Exists {get{return SlotExists(ActiveSlot);}}
 public static void Write(DungeonSaveData data){if(data==null)throw new ArgumentNullException("data");Write(data,ActiveSlot,data.slotName);}
 public static void Write(DungeonSaveData data,int slot,string name){
  Validate(data);data.slotName=string.IsNullOrWhiteSpace(name)?"Slot "+(slot+1):name.Trim();data.savedUtc=DateTime.UtcNow.ToString("o");string path=SlotPath(slot);
  // Validate and flush a sibling temporary file before touching the current save.
  string temp=PrepareTemporary(path,JsonUtility.ToJson(data,true));
  try{
   if(File.Exists(path)){
    DungeonSaveData previous;bool valid=TryRead(path,out previous,true);
    if(valid){for(int i=4;i>=1;i--){string from=path+".bak"+i,to=path+".bak"+(i+1);if(File.Exists(from))File.Copy(from,to,true);}File.Copy(path,path+".bak1",true);}
    else PreserveDamaged(path);
    File.Replace(temp,path,null);
   }else File.Move(temp,path);
  }finally{if(File.Exists(temp))File.Delete(temp);}revisions[slot]++;
 }
 public static DungeonSaveData Read(){return Read(ActiveSlot);}
 public static DungeonSaveData Read(int slot){int backup;return Read(slot,out backup);}
 // Reading summaries is deliberately read-only. Only an explicit load repairs the primary.
 public static DungeonSaveData Read(int slot,out int backup){
  backup=0;string path=SlotPath(slot);DungeonSaveData data;bool found=File.Exists(path);
  if(TryRead(path,out data,true))return data;
  for(int i=1;i<=5;i++){string candidate=path+".bak"+i;found|=File.Exists(candidate);if(TryRead(candidate,out data)){backup=i;return data;}}
  if(!found&&slot==0&&AllowLegacy){found|=File.Exists(LegacyPath);if(TryRead(LegacyPath,out data,true))return data;}
  if(found)throw new InvalidDataException("La partida y sus respaldos no se pueden leer. Los archivos se han conservado.");return null;
 }
 static bool AllowLegacy {get{
  #if UNITY_EDITOR
  if(!string.IsNullOrEmpty(editorTestPath))return false;
  #endif
  return true;
 }}
 public static DungeonSaveData Load(int slot,out string notice){
  int backup;var data=Read(slot,out backup);notice="";if(data==null||backup==0)return data;
  notice="Partida recuperada del respaldo "+backup+". Puede contener progreso anterior al último guardado.";
  string path=SlotPath(slot),temp=null;
  try{temp=PrepareTemporary(path,JsonUtility.ToJson(data,true));if(File.Exists(path)){PreserveDamaged(path);File.Replace(temp,path,null);}else File.Move(temp,path);revisions[slot]++;}
  catch(IOException){notice+=" No se pudo reparar el archivo principal; el respaldo sigue intacto.";}
  catch(UnauthorizedAccessException){notice+=" No se pudo reparar el archivo principal; el respaldo sigue intacto.";}
  finally{if(temp!=null&&File.Exists(temp))try{File.Delete(temp);}catch(IOException){}}
  DungeonSaveRecoveryNotice.Show(notice);return data;
 }
 static void PreserveDamaged(string path){File.Copy(path,path+".corrupt-"+DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")+"-"+Guid.NewGuid().ToString("N"),false);}
 static string PrepareTemporary(string path,string json){
  string parent=Path.GetDirectoryName(path);if(!string.IsNullOrEmpty(parent))Directory.CreateDirectory(parent);
  string temp=path+".tmp-"+Guid.NewGuid().ToString("N");
  try{byte[] bytes=new System.Text.UTF8Encoding(false).GetBytes(json);using(var stream=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}ReadFile(temp);return temp;}
  catch{if(File.Exists(temp))File.Delete(temp);throw;}
 }
 static bool TryRead(string path,out DungeonSaveData data,bool preserveFuture=false){data=null;if(!File.Exists(path))return false;try{data=ReadFile(path);return true;}catch(NotSupportedException){if(preserveFuture)throw;return false;}catch(ArgumentException){return false;}catch(InvalidDataException){return false;}catch(IOException){return false;}catch(UnauthorizedAccessException){return false;}}
 static DungeonSaveData ReadFile(string path){string json=File.ReadAllText(path).Trim();if(!json.StartsWith("{")||!json.EndsWith("}"))throw new InvalidDataException("JSON incompleto");var data=JsonUtility.FromJson<DungeonSaveData>(json);if(data!=null&&data.version>2)throw new NotSupportedException("Esta partida requiere una versión más reciente del juego; sus archivos no se han modificado.");Validate(data);return data;}
 static void Validate(DungeonSaveData data){
  if(data==null||(data.version!=1&&data.version!=2)||data.player==null)throw new InvalidDataException("Partida incompatible");
  if(data.version==2&&string.IsNullOrEmpty(data.activeMapId))throw new InvalidDataException("Partida sin identificador de mapa");
  if(data.heading<0||data.heading>3||double.IsNaN(data.player.playedSeconds)||double.IsInfinity(data.player.playedSeconds))throw new InvalidDataException("Estado de jugador inválido");
  // Missing optional collections in older saves are empty, never null during restoration.
  if(data.player.inventory==null)data.player.inventory=new List<InventoryItemRecord>();
  foreach(var item in data.player.inventory)if(item==null||string.IsNullOrEmpty(item.itemId)||item.quantity<0)throw new InvalidDataException("Inventario inválido");
  if(data.player.pendingCombatDrops==null)data.player.pendingCombatDrops=new List<InventoryItemRecord>();
  foreach(var item in data.player.pendingCombatDrops)if(item==null||string.IsNullOrEmpty(item.itemId)||item.quantity<0)throw new InvalidDataException("Recompensa inválida");
  if(data.maps==null)data.maps=new List<DungeonMapSnapshot>();
  var ids=new HashSet<string>();foreach(var map in data.maps){if(map==null||string.IsNullOrEmpty(map.mapId)||!ids.Add(map.mapId))throw new InvalidDataException("Snapshot de mapa inválido");if(map.enemies==null)map.enemies=new List<EnemyInstanceRecord>();if(map.entityStates==null)map.entityStates=new List<DungeonEntityState>();if(map.legacyDoors==null)map.legacyDoors=new List<DungeonEntityState>();ValidateEntries(map.enemies,map.entityStates,map.legacyDoors);}
  if(data.enemies==null)data.enemies=new List<EnemyInstanceRecord>();if(data.entityStates==null)data.entityStates=new List<DungeonEntityState>();if(data.legacyDoors==null)data.legacyDoors=new List<DungeonEntityState>();ValidateEntries(data.enemies,data.entityStates,data.legacyDoors);
  if(data.explored==null)data.explored=new List<Vector2Int>();if(data.visited==null)data.visited=new List<Vector2Int>();
 }
 static void ValidateEntries(List<EnemyInstanceRecord> enemies,List<DungeonEntityState> entities,List<DungeonEntityState> doors){foreach(var enemy in enemies)if(enemy==null)throw new InvalidDataException("Enemigo inválido");foreach(var entity in entities)if(entity==null)throw new InvalidDataException("Entidad inválida");foreach(var door in doors)if(door==null)throw new InvalidDataException("Puerta inválida");}
}
