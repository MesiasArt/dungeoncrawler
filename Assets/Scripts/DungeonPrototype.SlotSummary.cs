using System;
using UnityEngine;
public partial class DungeonPrototype {
 double playedSeconds,playClockStarted;bool playClockRunning;
 double CurrentPlayedSeconds {get{return playedSeconds+(playClockRunning?Math.Max(0,Time.realtimeSinceStartupAsDouble-playClockStarted):0);}}
 void StartPlayClock(double value){playedSeconds=double.IsNaN(value)||double.IsInfinity(value)?0:Math.Max(0,value);playClockStarted=Time.realtimeSinceStartupAsDouble;playClockRunning=true;}
 string SaveSlotSummary(DungeonSaveData data){return DescribeSaveSlot(data,ActiveMap);}
 public static string DescribeSaveSlot(DungeonSaveData data,DungeonMapSettings fallback=null){
  string zone="Zona desconocida",place="Dungeon";
  if(data.city!=null&&data.city.inCity){zone="Digital Kingdom";place="Ciudad";foreach(var location in DigitalKingdomSettings.Current.locations)if(location.id==data.city.locationId){place=location.displayName;break;}}
  else {var savedMap=DungeonMapPersistence.Resolve(data.activeMapId,fallback)??DungeonMapSettings.Current;if(savedMap!=null){place=savedMap==DungeonMapSettings.Current?"Dungeon":savedMap.name;var regions=savedMap.regions!=null?savedMap.regions:savedMap==DungeonMapSettings.Current?DungeonRegionSettings.Current:null;if(regions!=null){var region=regions.At(data.cell);if(region!=null){zone=region.displayName;int split=zone.IndexOf(" — ",StringComparison.Ordinal);if(split>=0){place=zone.Substring(split+3);zone=zone.Substring(0,split);}}}}}
  string time="Sin registro";if(data.player!=null&&data.player.hasPlayTime){double seconds=data.player.playedSeconds;if(double.IsNaN(seconds)||double.IsInfinity(seconds))seconds=0;long minutes=(long)(Math.Min(360000000,Math.Max(0,seconds))/60);time=(minutes/60).ToString("00")+":"+(minutes%60).ToString("00");}
  return zone+" — "+place+" — "+time;
 }
}
