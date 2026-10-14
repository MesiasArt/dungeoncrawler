using UnityEngine;
public partial class DungeonPrototype {
 void DrawInventoryGrid(float width,float height){float x=width/2-310,y=height/2-240;Rect panel=new Rect(x,y,620,385);NeonDungeonUI.Panel(panel);NeonDungeonUI.Label(new Rect(x+28,y+17,410,30),"INVENTARIO");int used=InventoryCellsUsed;var cells=InventoryCells();NeonDungeonUI.Label(new Rect(x+490,y+17,100,30),used+" / "+18,true);NeonDungeonUI.Rule(new Rect(x+28,y+56,564,1));int page=pauseRow/18;
 for(int row=0;row<3;row++)for(int col=0;col<6;col++){int index=page*18+row*6+col;Rect r=new Rect(x+28+col*96,y+69+row*91,83,80);bool selected=pauseRow==index;NeonDungeonUI.InventorySlot(r,selected);if(index<cells.Count){var cell=cells[index];if(cell.potion>=0){int potion=cell.potion;if(inventorySettings.items[potion].keyItem)NeonDungeonUI.Label(r,"LLAVE",true);else NeonDungeonUI.PotionIcon(new Rect(r.x+25,r.y+10,32,43),potion%4);var number=new GUIStyle(GUI.skin.label){fontSize=16,alignment=TextAnchor.LowerRight};GUI.Label(new Rect(r.x+6,r.y+53,r.width-13,21),"x"+itemCounts[potion],number);}else{var item=Character.equipment[cell.equipment];if(item.icon!=null)GUI.DrawTexture(new Rect(r.x+10,r.y+10,r.width-20,r.height-20),item.icon,ScaleMode.ScaleToFit,true);else NeonDungeonUI.Label(r,item.name,true);}}
 if(GUI.Button(r,GUIContent.none,GUIStyle.none)){if(pauseRow==index)UseOutsidePotion(index);else{pauseRow=index;pauseNotice="";}}}
 string description="Casilla vacía";if(pauseRow>=0&&pauseRow<cells.Count){var cell=cells[pauseRow];if(cell.potion>=0){var item=inventorySettings.items[cell.potion];description=item.displayName+(item.keyItem?" — abre su puerta": " — recupera "+item.healHP+" HP");}else description=Character.equipment[cell.equipment].name+" — ENTER: equipar";}
 var small=new GUIStyle(GUI.skin.label){fontSize=17,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(x+28,y+340,564,24),description,small);
 NeonDungeonUI.Panel(new Rect(x+40,y+394,540,42));GUI.Label(new Rect(x+50,y+399,520,30),"HP "+hp+" / "+MaximumHP+"   "+pauseNotice,small);
 Rect back=new Rect(x+40,y+444,540,46);NeonDungeonUI.Panel(back);NeonDungeonUI.Label(back,"VOLVER",true);if(GUI.Button(back,GUIContent.none,GUIStyle.none))pausePage=0;
 GUI.Label(new Rect(x,y+495,620,28),"Flechas: elegir / ENTER: usar / ESC: volver",small);
 }
}
