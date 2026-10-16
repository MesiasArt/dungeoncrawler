using System;
using System.IO;

// UI summaries only. Actual loading always reads/validates the store again.
internal sealed class DungeonSaveSlotCache {
 sealed class Slot {
  public DungeonSaveData data;public string label,selected,name;
  public FileInfo[] files;public long[] ticks,lengths;public int revision;
 }
 readonly Slot[] slots=new Slot[DungeonSaveStore.SlotCount];
 readonly Func<DungeonSaveData,string> summarize;
 string context;float nextPoll;bool ready;
 public DungeonSaveSlotCache(Func<DungeonSaveData,string> summarize){this.summarize=summarize;}
 public void Open(float now){context=DungeonSaveStore.ContextKey;for(int i=0;i<slots.Length;i++)Reload(i);ready=true;nextPoll=now+1;}
 public void Refresh(float now){if(!ready||context!=DungeonSaveStore.ContextKey){Open(now);return;}bool poll=now>=nextPoll;if(poll)nextPoll=now+1;for(int i=0;i<slots.Length;i++)if(slots[i].revision!=DungeonSaveStore.SlotRevision(i)||(poll&&Changed(slots[i])))Reload(i);}
 public string Label(int slot,bool selected){return selected?slots[slot].selected:slots[slot].label;}
 public string Name(int slot){return slots[slot].name;}
 internal DungeonSaveData Data(int slot){return slots[slot].data;}
 void Reload(int index){
  var slot=slots[index]??(slots[index]=new Slot());string fallback="Slot "+(index+1);slot.data=null;slot.name=fallback;slot.label=fallback+" — Vacío";
  string[] paths=DungeonSaveStore.CandidatePaths(index);slot.files=new FileInfo[paths.Length];slot.ticks=new long[paths.Length];slot.lengths=new long[paths.Length];for(int i=0;i<paths.Length;i++)slot.files[i]=new FileInfo(paths[i]);Stamp(slot);
  try{slot.data=DungeonSaveStore.Read(index);if(slot.data!=null){slot.label=summarize(slot.data);slot.name=string.IsNullOrEmpty(slot.data.slotName)?fallback:slot.data.slotName;}}
  catch{slot.label=fallback+" — No se puede leer";}
  slot.selected="> "+slot.label;slot.revision=Changed(slot)?-1:DungeonSaveStore.SlotRevision(index);
 }
 static bool Changed(Slot slot){try{for(int i=0;i<slot.files.Length;i++){var file=slot.files[i];file.Refresh();long length=file.Exists?file.Length:-1,ticks=file.Exists?file.LastWriteTimeUtc.Ticks:0;if(length!=slot.lengths[i]||ticks!=slot.ticks[i])return true;}return false;}catch(IOException){return true;}catch(UnauthorizedAccessException){return true;}}
 static void Stamp(Slot slot){try{for(int i=0;i<slot.files.Length;i++){var file=slot.files[i];file.Refresh();slot.lengths[i]=file.Exists?file.Length:-1;slot.ticks[i]=file.Exists?file.LastWriteTimeUtc.Ticks:0;}}catch(IOException){}catch(UnauthorizedAccessException){}}
}
