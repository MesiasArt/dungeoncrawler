#if UNITY_EDITOR && DEMO_PHASE3_CACHE
using System;
using System.IO;
using UnityEngine;
public static class DemoPhase3EditValidation {
 public static string Run(){string previous=DungeonSaveStore.editorTestPath;string root=Path.GetFullPath("Temp/Phase3EditFixtures/"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int passed=0;
  Action<bool,string> check=(ok,label)=>{if(!ok)throw new Exception(label);passed++;File.AppendAllText("phase3-edit-results.txt","PASS: "+label+"\n");};
  try{
   DungeonSaveStore.editorTestPath=null;bool blocked=false;try{DungeonSaveStore.SlotPath(0);}catch(InvalidOperationException){blocked=true;}check(blocked,"Edit Mode: default personal path is blocked in isolated tests");DungeonSaveStore.editorTestPath=Path.Combine(root,"slot.json");
   for(int i=0;i<6;i++)DungeonSaveStore.Write(new DungeonSaveData{player=new PlayerProgressionRecord{currentHP=30}},i,"Editor fixture "+i);
   var cache=new DungeonSaveSlotCache(d=>d.slotName);cache.Open(0);check(cache.Data(5)!=null&&cache.Label(5,true)=="> Editor fixture 5","Edit Mode: six slots and selected labels are preserved");
   var before=cache.Data(1);cache.Refresh(.1f);check(ReferenceEquals(before,cache.Data(1)),"Edit Mode: unchanged summaries reuse the same data");
   var record=DungeonSaveStore.Read(1);DungeonSaveStore.Write(record,1,"Rewritten editor fixture");cache.Refresh(.2f);check(cache.Name(1)=="Rewritten editor fixture","Edit Mode: own writes invalidate without waiting for metadata polling");
   record=DungeonSaveStore.Read(2);record.slotName="External editor fixture";File.WriteAllText(DungeonSaveStore.SlotPath(2),JsonUtility.ToJson(record));cache.Refresh(2);check(cache.Name(2)==record.slotName,"Edit Mode: external writes refresh through metadata");
   File.WriteAllText(DungeonSaveStore.SlotPath(3),"invalid");cache.Refresh(4);check(cache.Label(3,false).Contains("No se puede leer"),"Edit Mode: corrupt fixture is reported without throwing in GUI");
   foreach(string path in DungeonSaveStore.CandidatePaths(0))check(Path.GetFullPath(path).StartsWith(root,StringComparison.OrdinalIgnoreCase),"Edit Mode: candidate path remains inside fixture directory");
  }finally{DungeonSaveStore.editorTestPath=previous;}
  return "PASS: "+passed+" Edit Mode cache checks";
 }
}
#endif
