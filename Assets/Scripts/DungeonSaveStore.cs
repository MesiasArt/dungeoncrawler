using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public class DungeonSaveData {
 public string slotName,savedUtc;
 public int directorRestRemaining;
 public int version=1;public bool introductionCompleted;
 public Vector2Int cell;public int heading;public PlayerProgressionRecord player;public RefugeProgress refuge;
 public List<EnemyInstanceRecord> enemies=new List<EnemyInstanceRecord>();public string[] triggeredEncounters;
 public List<DungeonEntityState> entityStates=new List<DungeonEntityState>();
 public List<Vector2Int> explored=new List<Vector2Int>();
}
public static class DungeonSaveStore {
 #if UNITY_EDITOR
 public static string editorTestPath;
 #endif
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void ResetTestSlot(){
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
 public static bool SlotExists(int slot){if(File.Exists(SlotPath(slot)))return true;
 #if UNITY_EDITOR
 if(!string.IsNullOrEmpty(editorTestPath))return false;
 #endif
 return slot==0&&File.Exists(LegacyPath);}
 public static bool Exists {get{return SlotExists(ActiveSlot);}}
 public static void Write(DungeonSaveData data){Write(data,ActiveSlot,data.slotName);}
 public static void Write(DungeonSaveData data,int slot,string name){data.slotName=string.IsNullOrWhiteSpace(name)?"Slot "+(slot+1):name.Trim();data.savedUtc=DateTime.UtcNow.ToString("o");var path=SlotPath(slot);string temp=path+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));if(File.Exists(path)){for(int i=4;i>=1;i--){string from=path+".bak"+i,to=path+".bak"+(i+1);if(File.Exists(from))File.Copy(from,to,true);}File.Copy(path,path+".bak1",true);}File.Copy(temp,path,true);File.Delete(temp);}
 public static DungeonSaveData Read(){return Read(ActiveSlot);}
 public static DungeonSaveData Read(int slot){if(!SlotExists(slot))return null;string path=SlotPath(slot);if(!File.Exists(path))path=LegacyPath;var data=JsonUtility.FromJson<DungeonSaveData>(File.ReadAllText(path));if(data==null||data.version!=1||data.player==null)throw new InvalidDataException("Partida incompatible");return data;}
}
