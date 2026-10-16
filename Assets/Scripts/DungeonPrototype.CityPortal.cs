using UnityEngine;
public partial class DungeonPrototype {
 internal bool CityPortalReady {get{return CityBossDefeated();}}
 bool TryEnterCityPortal(Vector3 from,Vector3 to){
  if(ActiveMap==null||ActiveMap.entities==null)return false;
  foreach(var e in ActiveMap.entities.objects){DungeonDoor door;
   if((!e.cityEntrance&&!e.returnToDigitalKingdom)||!mapDoors.TryGetValue(e.id,out door)||door==null||!door.IsOpen||!door.CrossesPassage(from,to)||door.transform.InverseTransformPoint(to).z<=0)continue;
   if(e.returnToDigitalKingdom)return ReturnFromKingdomDoor();
   if(!CityBossDefeated()){message="La salida espera la derrota de Makong.";return true;}if(!door.IsPermanentlyOpen)door.SetClosedImmediately();EntityState(e.id).opened=door.IsOpen;
   return EnterDigitalKingdomAt(e.cell,(e.heading+2)%4);
  }return false;
 }
}
