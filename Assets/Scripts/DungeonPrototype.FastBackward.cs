using System;
using UnityEngine;
[Serializable] public sealed class SavePointReturnRecord {public string mapId,savePointId;public Vector2Int cell;public int heading;}
public partial class DungeonPrototype {
 SavePointReturnRecord lastSavePoint;
 void RememberSavePoint(string id){if(cityDataOnly)return;lastSavePoint=new SavePointReturnRecord{mapId=DungeonMapPersistence.RequireId(ActiveMap),savePointId=id,cell=position,heading=heading};}
 bool UseFastBackward(int index){
  if(battle||moving||busy||lost||IntroductionActive||worldDialogue.Active||pendingChests.Count>0){pauseNotice="Solo se puede usar fuera del combate y de eventos.";return false;}
  if(index<0||index>=itemCounts.Length||itemCounts[index]<=0)return false;
  if(lastSavePoint==null){pauseNotice="Primero utiliza un Save Point.";return false;}
  var destination=DungeonMapPersistence.Resolve(lastSavePoint.mapId,ActiveMap);
  if(destination==null||!destination.IsFloor(lastSavePoint.cell)){pauseNotice="El Save Point ya no está disponible.";return false;}
  var source=CaptureDungeonSave();
  var snapshot=source.maps.Find(m=>m.mapId==lastSavePoint.mapId);
  if(snapshot!=null&&snapshot.enemies.Exists(e=>!e.defeated&&e.currentHP>0&&e.cell==lastSavePoint.cell)){pauseNotice="El punto de retorno está ocupado.";return false;}
  itemCounts[index]--;var target=lastSavePoint.cell;int facing=lastSavePoint.heading;
  if(!cityDataOnly&&destination==ActiveMap){ResumeLoadedExploration();ClearMovementInput();WarpPlayer(target,facing);message="Fast Backward: regreso al Save Point.";}
  else {var data=CaptureDungeonSave();bool first;data=DungeonMapPersistence.ForTravel(data,destination,target,facing,out first);if(data.city!=null)data.city.inCity=false;if(cityDataOnly)DigitalKingdomTransit.Travel(data,destination,City.fallbackDungeonScene,null);else ReplaceFloor(destination,data,first);}
  DungeonAudio.Play(DungeonAudio.Cue.Confirm);return true;
 }
}
