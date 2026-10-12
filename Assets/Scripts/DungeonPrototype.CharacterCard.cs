using UnityEngine;
public partial class DungeonPrototype {
 void DrawCharacterCard(float width,float height){
  float x=width/2-180,y=(height-650)/2;
  NeonDungeonUI.Panel(new Rect(x,y,360,58));NeonDungeonUI.Label(new Rect(x+24,y+12,290,32),"PAUSA / PERSONAJE");
  NeonDungeonUI.Panel(new Rect(x,y+63,360,160));NeonDungeonUI.DialogueAvatar(new Rect(x+16,y+78,145,130));
  NeonDungeonUI.Label(new Rect(x+182,y+85,165,58),"LV "+PlayerLevel+"\nGAMERBOY");NeonDungeonUI.Label(new Rect(x+182,y+149,160,30),"HP "+hp+" / "+MaximumHP);
  NeonDungeonUI.Solid(new Rect(x+182,y+181,150,12),new Color(.13f,.03f,.15f));NeonDungeonUI.Solid(new Rect(x+182,y+181,150*Mathf.Clamp01(hp/(float)MaximumHP),12),NeonDungeonUI.Pink);
  NeonDungeonUI.Solid(new Rect(x+182,y+214,150,3),new Color(.04f,.08f,.15f));NeonDungeonUI.Solid(new Rect(x+182,y+214,150*(XPRequired==0?1:Mathf.Clamp01(PlayerExperience/(float)XPRequired)),3),NeonDungeonUI.Cyan);
  var xpStyle=new GUIStyle(GUI.skin.label){fontSize=10};GUI.Label(new Rect(x+182,y+198,168,14),XPRequired==0?"XP / NIVEL MÁXIMO":"XP "+PlayerExperience+" / "+XPRequired+"  (faltan "+XPRemaining+")",xpStyle);
  NeonDungeonUI.Panel(new Rect(x,y+228,360,202));NeonDungeonUI.Label(new Rect(x+24,y+239,300,28),"ESTADÍSTICAS");NeonDungeonUI.Rule(new Rect(x+25,y+271,310,1));
  string[] names={"Vida","Fuerza","Defensa","Suerte"};int[] values={MaximumHP,PlayerStrength,PlayerDefense,PlayerLuck};
  for(int i=0;i<4;i++){float row=y+281+i*34;NeonDungeonUI.PixelIcon(new Rect(x+28,row+3,22,22),i,NeonDungeonUI.Pink);NeonDungeonUI.Label(new Rect(x+68,row,110,28),names[i]);NeonDungeonUI.Label(new Rect(x+181,row,55,28),values[i].ToString());if(i<3)NeonDungeonUI.Solid(new Rect(x+68,row+31,265,1),new Color(.12f,.16f,.3f));if(i==3){var small=new GUIStyle(GUI.skin.label){fontSize=13,alignment=TextAnchor.MiddleRight};GUI.Label(new Rect(x+222,row,110,28),"(crítico "+CombatAccuracySettings.EffectiveBonus(Character.criticalPercent,PlayerLuck)+"%)",small);}}
  NeonDungeonUI.Panel(new Rect(x,y+435,360,150));NeonDungeonUI.Label(new Rect(x+24,y+446,300,28),"EQUIPAMIENTO");NeonDungeonUI.Rule(new Rect(x+25,y+477,310,1));
  EnsureEquipment();pauseRow=Mathf.Clamp(pauseRow,0,2);
  GUI.enabled=!choosingEquipment;
  for(int n=0;n<3;n++){int i=EquippedInSlot(n);var e=i>=0?Character.equipment[i]:null;Rect slot=new Rect(x+36+n*103,y+489,82,70);NeonDungeonUI.InventorySlot(slot,pauseRow==n);if(e!=null&&e.icon!=null)GUI.DrawTexture(new Rect(slot.x+15,slot.y+8,52,42),e.icon,ScaleMode.ScaleToFit,true);var label=new GUIStyle(GUI.skin.label){fontSize=11,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(slot.x,slot.y+49,82,18),e!=null?"EQUIPADO":"VACÍO",label);if(GUI.Button(slot,GUIContent.none,GUIStyle.none)){pauseRow=n;OpenEquipmentChoice();}}
  int selected=EquippedInSlot(pauseRow);var item=selected>=0?Character.equipment[selected]:null;var detail=new GUIStyle(GUI.skin.label){fontSize=10,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(x+15,y+559,330,13),item==null?"Ranura "+(pauseRow+1)+" / sin equipo":item.name+"  HP +"+item.health+" F +"+item.strength+" D +"+item.defense+" S +"+item.luck,detail);

  Rect back=new Rect(x,y+592,360,45);NeonDungeonUI.Panel(back);NeonDungeonUI.Label(back,"VOLVER",true);if(GUI.Button(back,GUIContent.none,GUIStyle.none))pausePage=0;
  GUI.enabled=true;
  if(choosingEquipment)DrawEquipmentChoice(width,height);
  var hint=new GUIStyle(GUI.skin.label){fontSize=10,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(x,y+638,360,18),"Flechas: elegir equipo / ENTER: equipar / ESC: volver",hint);
 }
}
