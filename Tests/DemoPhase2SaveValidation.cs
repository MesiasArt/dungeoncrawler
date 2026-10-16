#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DemoPhase2SaveValidation {
 public static void Run(Action<bool,string> check){
  string root=Path.GetFullPath("Temp/DemoPhase2SaveFixtures/"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
  string previous=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath=Path.Combine(root,"slot.json");
  try{
   for(int slot=0;slot<6;slot++){
    var data=Fixture(slot%2+1,slot+1);DungeonSaveStore.Write(data,slot,"Fixture "+slot);
    var read=DungeonSaveStore.Read(slot);check(read.player.coins==slot+1&&read.version==data.version,"Save: slot "+slot+" version "+data.version+" roundtrip");
    check(read.maps.Count==1&&read.maps[0].visited.Count==1,"Save: slot "+slot+" preserves floor snapshot");
   }
   string path=DungeonSaveStore.SlotPath(0);for(int i=10;i<=16;i++)DungeonSaveStore.Write(Fixture(2,i),0,"Rotation");
   for(int i=1;i<=5;i++)check(JsonUtility.FromJson<DungeonSaveData>(File.ReadAllText(path+".bak"+i)).player.coins==16-i,"Save: backup rotation "+i);
   string validBackup=File.ReadAllText(path+".bak1");File.WriteAllText(path,"{\"version\":2,");
   int backup;var recovered=DungeonSaveStore.Read(0,out backup);check(backup==1&&recovered.player.coins==15,"Save: truncated principal falls back to first valid backup");
   check(File.ReadAllText(path)=="{\"version\":2,","Save: summary read does not repair or mutate files");
   string notice;recovered=DungeonSaveStore.Load(0,out notice);check(notice.Contains("respaldo 1")&&notice.Contains("progreso anterior"),"Save: explicit recovery reports source and possible lost recent progress");
   check(DungeonSaveStore.Read(0).player.coins==15&&Directory.GetFiles(root,"slot.json.corrupt-*").Length==1,"Save: load repairs principal and archives damaged bytes");
   check(File.ReadAllText(path+".bak1")==validBackup,"Save: recovery leaves source backup intact");
   File.WriteAllText(path,"{\"version\":99,\"player\":{\"currentHP\":30}}");bool future=false;try{DungeonSaveStore.Read(0);}catch(NotSupportedException){future=true;}check(future&&File.ReadAllText(path).Contains("99"),"Save: future version never silently downgrades to an old backup");
   File.WriteAllText(path,"damaged but locked");using(var locked=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read)){recovered=DungeonSaveStore.Load(0,out notice);check(recovered.player.coins==15&&notice.Contains("No se pudo reparar"),"Save: valid backup remains usable when principal repair is denied");}
   File.WriteAllText(path,"broken");File.WriteAllText(path+".bak1","also broken");recovered=DungeonSaveStore.Read(0,out backup);check(backup==2&&recovered.player.coins==14,"Save: skips invalid recent backup");
   File.Delete(path);check(DungeonSaveStore.SlotExists(0)&&DungeonSaveStore.Read(0,out backup).player.coins==14&&backup==2,"Save: missing principal remains recoverable through slot menu");
   DungeonSaveStore.Load(0,out notice);check(File.Exists(path),"Save: missing principal recreated atomically");
   string stable=File.ReadAllText(path);using(var locked=new FileStream(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){bool failed=false;try{DungeonSaveStore.Write(Fixture(2,99),0,"Locked");}catch(IOException){failed=true;}check(failed,"Save: locked destination fails safely");}
   check(File.ReadAllText(path)==stable&&!Directory.GetFiles(root,"*.tmp-*").Any(),"Save: failed replacement retains original and cleans staged write");
   var invalid=Fixture(2,90);invalid.player.inventory.Add(null);bool rejected=false;try{DungeonSaveStore.Write(invalid,0,"Invalid");}catch(InvalidDataException){rejected=true;}check(rejected&&File.ReadAllText(path)==stable,"Save: invalid inventory rejected before touching destination");
   File.WriteAllText(path+".tmp","{interrupted write");check(DungeonSaveStore.Read(0).player.coins==14,"Save: abandoned temporary write never supersedes committed principal");
   File.WriteAllText(path,JsonUtility.ToJson(invalid));check(DungeonSaveStore.Read(0,out backup)!=null&&backup>0,"Save: structurally invalid principal falls back safely");
   for(int i=1;i<=5;i++)File.WriteAllText(path+".bak"+i,"invalid");bool unreadable=false;try{DungeonSaveStore.Read(0);}catch(InvalidDataException){unreadable=true;}check(unreadable&&File.Exists(path+".bak5"),"Save: no valid candidate fails clearly without deleting evidence");
   File.WriteAllText(DungeonSaveStore.SlotPath(5),"{\"version\":1,\"player\":{\"currentHP\":30},\"heading\":0}");var legacy=DungeonSaveStore.Read(5);check(legacy.version==1&&legacy.player.inventory!=null&&legacy.maps!=null,"Save: minimal version 1 optional fields remain compatible");
   var ui=UnityEngine.Object.FindAnyObjectByType<DungeonSaveRecoveryNotice>();check(ui!=null,"Save: recovery creates a persistent acknowledgement notice");if(ui!=null)UnityEngine.Object.Destroy(ui.gameObject);
  }finally{DungeonSaveStore.editorTestPath=previous;}
 }
 static DungeonSaveData Fixture(int version,int coins){var d=new DungeonSaveData{version=version,activeMapId="fixture.map",player=new PlayerProgressionRecord{coins=coins,currentHP=30}};d.player.inventory.Add(new InventoryItemRecord{itemId="fixture.potion",quantity=2});var map=new DungeonMapSnapshot{mapId="fixture.floor"};map.visited.Add(new Vector2Int(1,2));d.maps.Add(map);return d;}
}
#endif
