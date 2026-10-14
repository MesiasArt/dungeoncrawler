using UnityEngine;
public partial class DungeonPrototype {
 Texture2D characterSheetPortrait,jumpSheetIcon,overclockSheetIcon;
 int characterHackSelection;
 void DrawCharacterCard(float width,float height){
  NeonDungeonUI.ApplyFont();
  const float sheetWidth=1100;float x=(width-sheetWidth)/2,y=(height-630)/2;
  NeonDungeonUI.Panel(new Rect(x,y,sheetWidth,52));NeonDungeonUI.Label(new Rect(x+24,y+10,sheetWidth-48,32),"PAUSA / PERSONAJE");
  float body=y+66;Rect left=new Rect(x,body,292,460),middle=new Rect(x+308,body,360,460),right=new Rect(x+684,body,416,460);
  NeonDungeonUI.Panel(left);NeonDungeonUI.Panel(middle);NeonDungeonUI.Panel(new Rect(right.x,right.y,right.width,258));NeonDungeonUI.Panel(new Rect(right.x,right.y+274,right.width,186));
  NeonDungeonUI.Label(new Rect(left.x+22,body+16,248,30),"EQUIPAMIENTO");NeonDungeonUI.Rule(new Rect(left.x+22,body+52,248,1));
  EnsureEquipment();pauseRow=Mathf.Clamp(pauseRow,0,2);GUI.enabled=!choosingEquipment;
  string[] slots={"GUANTE","CAMISETA","PIES"};
  var small=new GUIStyle(GUI.skin.label){fontSize=18,wordWrap=true};small.normal.textColor=new Color(.65f,.85f,1);
  for(int n=0;n<3;n++){
   int index=EquippedInSlot(n);var item=index>=0?Character.equipment[index]:null;float row=body+68+n*100;
   Rect icon=new Rect(left.x+22,row,72,72);NeonDungeonUI.InventorySlot(icon,pauseRow==n);
   if(item!=null&&item.icon!=null)GUI.DrawTexture(new Rect(icon.x+10,icon.y+10,52,52),item.icon,ScaleMode.ScaleToFit,true);
   GUI.Label(new Rect(left.x+108,row+3,162,25),slots[n],small);GUI.Label(new Rect(left.x+108,row+28,162,44),item!=null?item.name:"VACÍO",small);
   if(GUI.Button(new Rect(left.x+20,row,252,78),GUIContent.none,GUIStyle.none)){pauseRow=n;OpenEquipmentChoice();}
  }
  NeonDungeonUI.Rule(new Rect(left.x+22,body+370,248,1));int selected=EquippedInSlot(pauseRow);var chosen=selected>=0?Character.equipment[selected]:null;
  GUI.Label(new Rect(left.x+22,body+386,248,60),chosen==null?"Ranura vacía":chosen.name+"\nVida +"+chosen.health+"  Fuerza +"+chosen.strength+"\nDefensa +"+chosen.defense+"  Suerte +"+chosen.luck,small);
  NeonDungeonUI.Label(new Rect(middle.x+22,body+16,316,32),"GAMERBOY / LV "+PlayerLevel,true);
  if(characterSheetPortrait==null)characterSheetPortrait=Resources.Load<Texture2D>("UI/CharacterSheet/GamerBoyFullBody");
  if(characterSheetPortrait!=null)GUI.DrawTexture(new Rect(middle.x+22,body+58,316,318),characterSheetPortrait,ScaleMode.ScaleToFit,true);
  else NeonDungeonUI.DialogueAvatar(new Rect(middle.x+50,body+75,260,280));
  GUI.Label(new Rect(middle.x+28,body+385,40,24),"HP",small);GUI.Label(new Rect(middle.x+28,body+408,40,24),"BP",small);GUI.Label(new Rect(middle.x+28,body+429,40,24),"EXP",small);
  CharacterSheetBar(new Rect(middle.x+70,body+391,258,12),hp/(float)MaximumHP,NeonDungeonUI.Pink);
  CharacterSheetBar(new Rect(middle.x+70,body+414,258,10),CurrentBits/(float)MaximumBits,new Color(.12f,.38f,1));
  CharacterSheetBar(new Rect(middle.x+70,body+438,258,5),XPRequired<=0?1:PlayerExperience/(float)XPRequired,new Color(.25f,.95f,.65f));
  NeonDungeonUI.Label(new Rect(right.x+24,body+16,368,30),"ESTADÍSTICAS");NeonDungeonUI.Rule(new Rect(right.x+24,body+52,368,1));
  string[] names={"Vida","Fuerza","Defensa","Suerte"};int[] values={MaximumHP,PlayerStrength,PlayerDefense,PlayerLuck};
  for(int n=0;n<4;n++){float row=body+68+n*39;NeonDungeonUI.PixelIcon(new Rect(right.x+26,row+3,22,22),n,NeonDungeonUI.Pink);NeonDungeonUI.Label(new Rect(right.x+65,row,180,29),names[n]);NeonDungeonUI.Label(new Rect(right.x+280,row,105,29),values[n].ToString(),true);if(n<3)NeonDungeonUI.Solid(new Rect(right.x+65,row+33,320,1),new Color(.12f,.16f,.3f));}
  GUI.Label(new Rect(right.x+65,body+224,320,22),"Crítico "+CombatAccuracySettings.EffectiveBonus(Character.criticalPercent,PlayerLuck)+"%",small);
  NeonDungeonUI.Label(new Rect(right.x+24,body+287,368,26),"HACKS");
  if(jumpSheetIcon==null)jumpSheetIcon=Resources.Load<Texture2D>("UI/GamerboyJump");if(overclockSheetIcon==null)overclockSheetIcon=Resources.Load<Texture2D>("UI/Overclock/frame_1");
  characterHackSelection=Mathf.Clamp(characterHackSelection,0,Mathf.Max(0,availableHackDefinitions.Count-1));
  for(int n=0;n<availableHackDefinitions.Count;n++){
   var h=availableHackDefinitions[n];Rect icon=new Rect(right.x+24+n*64,body+321,56,56);NeonDungeonUI.InventorySlot(icon,characterHackSelection==n);
   var texture=h.ImplementationType==typeof(EnemyJumpHack)?jumpSheetIcon:h.ImplementationType==typeof(OverclockHack)?overclockSheetIcon:null;
   if(texture!=null)GUI.DrawTexture(new Rect(icon.x+8,icon.y+6,40,42),texture,ScaleMode.ScaleToFit,true);else GUI.Label(icon,h.displayName.Substring(0,1),small);
   if(GUI.Button(icon,GUIContent.none,GUIStyle.none))characterHackSelection=n;
  }
  if(availableHackDefinitions.Count>0){var h=availableHackDefinitions[characterHackSelection];GUI.Label(new Rect(right.x+24,body+385,368,26),h.displayName+" / "+h.bitsCost+" Bits",small);GUI.Label(new Rect(right.x+24,body+412,368,40),h.description,small);}else GUI.Label(new Rect(right.x+24,body+328,368,35),"Ninguno desbloqueado",small);
  Rect back=new Rect(width/2-140,y+542,280,44);NeonDungeonUI.Panel(back);NeonDungeonUI.Label(back,"VOLVER",true);if(GUI.Button(back,GUIContent.none,GUIStyle.none))pausePage=0;
  GUI.enabled=true;if(choosingEquipment)DrawEquipmentChoice(width,height);
  var hint=new GUIStyle(GUI.skin.label){fontSize=16,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(x,y+599,sheetWidth,24),"Flechas: equipo / ENTER: cambiar / Clic: HACK / ESC: volver",hint);
 }
 void CharacterSheetBar(Rect r,float amount,Color color){NeonDungeonUI.Solid(r,new Color(.025f,.035f,.075f));NeonDungeonUI.Solid(new Rect(r.x,r.y,r.width*Mathf.Clamp01(amount),r.height),color);}
}
