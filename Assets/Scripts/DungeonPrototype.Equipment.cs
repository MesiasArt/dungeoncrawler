using UnityEngine;
public partial class DungeonPrototype {
 bool choosingEquipment;int equipmentChoice;
 readonly System.Collections.Generic.HashSet<string> ownedEquipment=new System.Collections.Generic.HashSet<string>();
 // IDs are authoritative. The bool array is only a view for existing UI code.
 string[] equippedIds;
 void EnsureEquipment(){
  if(equippedIds==null){equippedIds=new string[3];for(int slot=0;slot<3;slot++)if(Character.initialEquipmentIds!=null&&slot<Character.initialEquipmentIds.Length){string id=Character.initialEquipmentIds[slot];var item=System.Array.Find(Character.equipment,e=>e!=null&&e.id==id&&e.SlotIndex==slot);if(item!=null&&!string.IsNullOrEmpty(id)){equippedIds[slot]=id;ownedEquipment.Add(id);}}}
  if(equipped==null||equipped.Length!=Character.equipment.Length)equipped=new bool[Character.equipment.Length];else System.Array.Clear(equipped,0,equipped.Length);
  for(int i=0;i<equipped.Length;i++){var item=Character.equipment[i];equipped[i]=item!=null&&ownedEquipment.Contains(item.id)&&equippedIds[item.SlotIndex]==item.id;}
 }
 int EquippedInSlot(int slot){EnsureEquipment();for(int i=0;i<equipped.Length;i++)if(equipped[i]&&Character.equipment[i]!=null&&Character.equipment[i].SlotIndex==slot)return i;return -1;}
 System.Collections.Generic.List<int> EquipmentChoices(){var choices=new System.Collections.Generic.List<int>{-1};for(int i=0;i<Character.equipment.Length;i++)if(Character.equipment[i]!=null&&Character.equipment[i].SlotIndex==pauseRow&&ownedEquipment.Contains(Character.equipment[i].id))choices.Add(i);return choices;}
 void OpenEquipmentChoice(){choosingEquipment=true;equipmentChoice=0;var choices=EquipmentChoices();int current=EquippedInSlot(pauseRow);equipmentChoice=Mathf.Max(0,choices.IndexOf(current));}
 void ConfirmEquipmentChoice(){var choices=EquipmentChoices();equipmentChoice=Mathf.Clamp(equipmentChoice,0,choices.Count-1);int item=choices[equipmentChoice];if(item<0)UnequipSlot(pauseRow);else EquipItem(Character.equipment[item].id);choosingEquipment=false;}
 void DrawEquipmentChoice(float width,float height){var choices=EquipmentChoices();float x=width/2-225,y=height/2-165;NeonDungeonUI.Solid(new Rect(0,0,width,height),new Color(0,0,0,.85f));NeonDungeonUI.Panel(new Rect(x,y,450,330));NeonDungeonUI.Label(new Rect(x+24,y+14,400,35),"ELEGIR EQUIPO / RANURA "+(pauseRow+1));int first=Mathf.Max(0,equipmentChoice-3);for(int n=0;n<5&&first+n<choices.Count;n++){int choice=first+n,index=choices[choice];var e=index>=0?Character.equipment[index]:null;Rect r=new Rect(x+24,y+57+n*43,400,38);NeonDungeonUI.Label(r,(choice==equipmentChoice?"> ":"")+(e==null?"Quitar equipo / vacío":e.name));if(e!=null&&e.icon!=null)GUI.DrawTexture(new Rect(r.xMax-35,r.y,32,32),e.icon,ScaleMode.ScaleToFit,true);if(GUI.Button(r,GUIContent.none,GUIStyle.none)){equipmentChoice=choice;ConfirmEquipmentChoice();}}if(GUI.Button(new Rect(x+125,y+282,200,32),"CANCELAR"))choosingEquipment=false;}
}
