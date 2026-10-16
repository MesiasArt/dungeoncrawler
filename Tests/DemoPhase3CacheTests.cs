#if UNITY_EDITOR && DEMO_PHASE3_CACHE
using System;
using System.IO;
using UnityEngine;
public static class DemoPhase3CacheTests {
 public static void Run(Action<bool,string> check){
  var cache=new DungeonSaveSlotCache(d=>d.slotName);long before=DemoPhase3Counters.reads;cache.Open(0);check(DemoPhase3Counters.reads-before==6,"Cache: initial open reads six fixtures once");before=DemoPhase3Counters.reads;
  for(int i=0;i<100;i++)cache.Refresh(.1f);check(DemoPhase3Counters.reads==before,"Cache: repeated GUI refresh does not reread JSON");
  var unchanged=cache.Data(0);var data=DungeonSaveStore.Read(1);data.slotName="Updated slot";DungeonSaveStore.Write(data,1,data.slotName);before=DemoPhase3Counters.reads;cache.Refresh(.2f);
  check(DemoPhase3Counters.reads-before==1&&cache.Name(1)=="Updated slot"&&ReferenceEquals(unchanged,cache.Data(0)),"Cache: a write invalidates only its own slot immediately");
  data=DungeonSaveStore.Read(2);data.slotName="External change with different length";File.WriteAllText(DungeonSaveStore.SlotPath(2),JsonUtility.ToJson(data));before=DemoPhase3Counters.reads;cache.Refresh(1.5f);
  check(DemoPhase3Counters.reads-before==1&&cache.Name(2)==data.slotName,"Cache: metadata polling refreshes an externally changed file");
  File.WriteAllText(DungeonSaveStore.SlotPath(3),"broken");cache.Refresh(3);check(cache.Data(3)==null&&cache.Label(3,false).Contains("No se puede leer"),"Cache: unreadable slot displays the existing error label");
  File.Delete(DungeonSaveStore.SlotPath(4));cache.Refresh(5);check(cache.Data(4)==null&&cache.Label(4,false).Contains("Vacío"),"Cache: externally deleted slot becomes empty");
  data=DungeonSaveStore.Read(0);DungeonSaveStore.Write(data,0,data.slotName);File.WriteAllText(DungeonSaveStore.SlotPath(0),"{");cache.Refresh(7);
  check(cache.Data(0)!=null&&File.ReadAllText(DungeonSaveStore.SlotPath(0))=="{","Cache: backup summaries do not repair the principal");
  string notice;DungeonSaveStore.Load(0,out notice);before=DemoPhase3Counters.reads;cache.Refresh(7.1f);check(DemoPhase3Counters.reads-before==1&&notice.Contains("respaldo"),"Cache: explicit backup recovery invalidates the repaired slot");
  var popup=UnityEngine.Object.FindAnyObjectByType<DungeonSaveRecoveryNotice>();if(popup!=null)UnityEngine.Object.Destroy(popup.gameObject);
  string context=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath=Path.Combine(Path.GetDirectoryName(context),"other-context.json");cache.Refresh(7.2f);check(cache.Data(0)==null&&cache.Name(0)=="Slot 1","Cache: changing the fixture path cannot expose stale slot data");DungeonSaveStore.editorTestPath=context;
 }
}
#endif
