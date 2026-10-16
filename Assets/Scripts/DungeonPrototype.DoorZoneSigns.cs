using UnityEngine;
public partial class DungeonPrototype {
 Vector2Int DoorWarningBeyond(Vector2Int cell,Vector2Int direction){var initial=RegionAt(cell);for(int i=1;i<=2;i++){var next=cell+direction*i;if(!Open(next))break;var region=RegionAt(next);if(region!=null&&region!=initial)return next;}return cell;}
 void ConfigureDoorZoneSigns(){
  foreach(var door in NavigationDoors()){
   if(door==null||door.panel==null||door.GetComponent<DungeonCityPortal>()!=null)continue;door.closeAfterPassage=!door.IsPermanentlyOpen;
   Vector3 center=door.transform.position,dir=door.transform.forward;
   Vector3 before=center-dir*Cell*.5f,after=center+dir*Cell*.5f;
   var from=new Vector2Int(Mathf.RoundToInt(before.x/Cell),Mathf.RoundToInt(before.z/Cell));
   var to=new Vector2Int(Mathf.RoundToInt(after.x/Cell),Mathf.RoundToInt(after.z/Cell));
   if(RegionAt(from)!=null&&RegionAt(from)==RegionAt(to)){var step=new Vector2Int(Mathf.RoundToInt(dir.x),Mathf.RoundToInt(dir.z));to=DoorWarningBeyond(to,step);from=DoorWarningBeyond(from,-step);}
   DungeonMapEntity definition=null;if(ActiveMap!=null&&ActiveMap.entities!=null)foreach(var e in ActiveMap.entities.objects){DungeonDoor actual;if(e.kind==DungeonEntityKind.Door&&mapDoors.TryGetValue(e.id,out actual)&&actual==door){definition=e;break;}}
   if(definition!=null&&definition.destinationMap!=null&&definition.destinationMap!=ActiveMap){var target=definition.destinationMap;var region=(target.regions!=null?target.regions.At(definition.destinationCell):null);bool safe=region!=null&&!region.CanSpawn;var result=safe?new Color(.25f,1,.45f):new Color(1,.22f,.25f);var signs=door.gameObject.AddComponent<DungeonDoorZoneSigns>();signs.Build(door,view,()=>result,()=>ZoneLabelColor(from));continue;}
   bool city=definition!=null&&(definition.cityEntrance||definition.returnToDigitalKingdom);
   var sign=door.GetComponent<DungeonDoorZoneSigns>()??door.gameObject.AddComponent<DungeonDoorZoneSigns>();
   sign.Build(door,view,()=>city?new Color(.25f,1,.45f):ZoneLabelColor(to),()=>ZoneLabelColor(from));
  }
 }
}
