using System;
using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 [Min(1)]public float itemNoticeSeconds=4;
 sealed class ItemReceipt {public string text;public float expires;}
 readonly List<ItemReceipt> itemReceipts=new List<ItemReceipt>();float lastItemNoticeSound=-100;
 void NotifyReceived(string id,int quantity){if(quantity<=0)return;var item=inventorySettings!=null?Array.Find(inventorySettings.items,e=>e!=null&&e.id==id):null;var equipment=Array.Find(Character.equipment,e=>e!=null&&e.id==id);string name=item!=null?item.displayName:equipment!=null?equipment.name:id;
  if(quantity>1&&name.StartsWith("Poción "))name=name.Replace("Poción ","Pociones ").Replace("pequeña","pequeñas").Replace("mediana","medianas").Replace("grande","grandes");
  SystemMessages.Show(SystemMessageCode.Received,name,quantity,item!=null?item.digitalIcon:DigitalIcon.Granted);
  itemReceipts.Add(new ItemReceipt{text="+"+quantity+" "+name,expires=Time.unscaledTime+itemNoticeSeconds});if(itemReceipts.Count>8)itemReceipts.RemoveAt(0);
  if(Time.unscaledTime-lastItemNoticeSound>.08f){DungeonAudio.Play(DungeonAudio.Cue.Item);lastItemNoticeSound=Time.unscaledTime;}
 }
 void DrawItemReceipts(float width,float height){itemReceipts.RemoveAll(n=>n.expires<=Time.unscaledTime);}

 bool LookingToward(Vector3 point){Vector3 delta=point-view.transform.position;delta.y=0;if(delta.sqrMagnitude<.01f)return false;var dir=directions[heading];return Vector3.Dot(delta.normalized,new Vector3(dir.x,0,dir.y))>.7f;}
 bool DoorInteractionAvailable(DungeonDoor door){return door!=null&&door.CanInteract(view.transform.position)&&LookingToward(door.transform.position);}
 bool RefugeTerminalAvailable(){if(refugeSettings==null||!LegacyObjectEnabled("refuge.save")||!NearRefugeCell(refugeSettings.saveCell))return false;foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(!device.portal&&device.deviceName=="TERMINAL DE RESTAURACIÓN")return LookingToward(device.transform.position);return false;}
 bool FacingTransferConsole(Vector3 basePosition){return LookingToward(basePosition+new Vector3(1.65f,.57f,.4f));}
 bool FacingRefugeTransfer(string id){foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(device.portal&&refugeSettings!=null&&Array.Exists(refugeSettings.nodes,n=>n.id==id&&device.deviceName=="NODO "+n.displayName))return FacingTransferConsole(device.transform.position);return false;}
 bool FacingMapEntity(DungeonMapEntity entity){GameObject root;if(!mapEntityObjects.TryGetValue(entity.id,out root))return false;return LookingToward(root.transform.position);}
}
