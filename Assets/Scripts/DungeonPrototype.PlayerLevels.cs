using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 public PlayerLevelSettings playerLevelSettings;
 PlayerLevelSettings Levels {get {if(playerLevelSettings==null)playerLevelSettings=ActivePlayerProfile!=null&&ActivePlayerProfile.levels!=null?ActivePlayerProfile.levels:PlayerLevelSettings.Current;if(playerLevelSettings==null)playerLevelSettings=ScriptableObject.CreateInstance<PlayerLevelSettings>();return playerLevelSettings;}}
 int playerLevel=1;long playerExperience;
 int encounterXP,encounterXPLevel=1;
 readonly HashSet<EnemyCombatStats> xpDefeats=new HashSet<EnemyCombatStats>();
 float levelNoticeUntil;string levelNotice;
 public int PlayerLevel {get{return Levels.ClampLevel(playerLevel);}}
 public long PlayerTotalExperience {get{long total=playerExperience;for(int i=1;i<PlayerLevel;i++)total+=Levels.RequiredXP(i);return total;}}
 public long PlayerExperience {get{return playerExperience;}}
 public int XPRequired {get{return Levels.RequiredXP(PlayerLevel);}}
 public long XPRemaining {get{return Mathf.Max(0,XPRequired)-(long)System.Math.Min(XPRequired,playerExperience);}}
 public int[] PlayerStatValues {get{return new[]{MaximumHP,PlayerStrength,PlayerDefense,PlayerLuck};}}
 void BeginEncounterXP(){encounterXP=0;encounterXPLevel=PlayerLevel;xpDefeats.Clear();}
 void RecordEnemyXP(EnemyCombatStats enemy){if(enemy==null||enemy.experienceRecorded||!xpDefeats.Add(enemy))return;enemy.experienceRecorded=true;var category=enemy.category;var profile=EnemyCharacterCatalog.Current!=null?EnemyCharacterCatalog.Current.FindId(enemy.speciesId):null;if(profile!=null){if(profile.category==EnemyRank.Jefe)category=PlacedEnemyCategory.Boss;else if(category==PlacedEnemyCategory.Normal&&profile.category!=EnemyRank.Enemigo)category=PlacedEnemyCategory.Elite;}encounterXP=(int)System.Math.Min(int.MaxValue,(long)encounterXP+Levels.Reward(enemy.level,encounterXPLevel,category));}
 void AwardEncounterXP(){int amount=encounterXP;encounterXP=0;xpDefeats.Clear();if(amount<=0)return;GainPlayerXP(amount);}
 public void GainPlayerXP(long amount){GainPlayerXP(amount,false);}
 void GainPlayerXP(long amount,bool cheat){int previousLevel=PlayerLevel;int gained=Levels.AddXP(ref playerLevel,ref playerExperience,amount);if(gained<=0)return;PlayerLevelHackUnlocks(previousLevel);int healing=Levels.healFlatHP+Mathf.RoundToInt(MaximumHP*Levels.healMaximumFraction);hp=(int)System.Math.Min(MaximumHP,(long)hp+(long)Mathf.Max(0,healing)*gained);levelNotice="LEVEL UP — GAMERBOY LV "+PlayerLevel;levelNoticeUntil=0;SystemMessages.Show(cheat?SystemMessageCode.CheatLevelUp:SystemMessageCode.LevelUp,"",0,cheat?DigitalIcon.LevelUpCheat:DigitalIcon.SystemRepair);}
 void RestorePlayerLevel(PlayerProgressionRecord record){playerLevel=Levels.ClampLevel(record.level);playerExperience=System.Math.Max(0,record.experience);Levels.AddXP(ref playerLevel,ref playerExperience,0);encounterXP=0;xpDefeats.Clear();levelNoticeUntil=0;}
 void DrawExperienceBar(Rect r,bool showRemaining){
  float progress=XPRequired<=0?1:Mathf.Clamp01(PlayerExperience/(float)XPRequired);
  NeonDungeonUI.Solid(r,new Color(.035f,.06f,.12f));
  NeonDungeonUI.Solid(new Rect(r.x,r.y,r.width*progress,r.height),NeonDungeonUI.Cyan);
  var style=new GUIStyle(GUI.skin.label){fontSize=14,alignment=TextAnchor.MiddleCenter};
  style.normal.textColor=Color.white;
  if(showRemaining)GUI.Label(new Rect(r.x,r.yMax+2,r.width,18),XPRequired<=0?"NIVEL MÁXIMO":"XP "+PlayerExperience+" / "+XPRequired+(showRemaining?" · Faltan "+XPRemaining:""),style);
 }
 void DrawPlayerLevelNotice(float width){if(Time.unscaledTime>=levelNoticeUntil)return;var style=new GUIStyle(GUI.skin.label){fontSize=19,alignment=TextAnchor.MiddleCenter};style.normal.textColor=NeonDungeonUI.Pink;GUI.Label(new Rect(width/2-190,115,380,30),levelNotice,style);}
}
