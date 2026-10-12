using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 public PlayerLevelSettings playerLevelSettings;
 PlayerLevelSettings Levels {get {if(playerLevelSettings==null)playerLevelSettings=PlayerLevelSettings.Current;if(playerLevelSettings==null)playerLevelSettings=ScriptableObject.CreateInstance<PlayerLevelSettings>();return playerLevelSettings;}}
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
 public void GainPlayerXP(long amount){int previousLevel=PlayerLevel;int gained=Levels.AddXP(ref playerLevel,ref playerExperience,amount);if(gained<=0)return;PlayerLevelHackUnlocks(previousLevel);int healing=Levels.healFlatHP+Mathf.RoundToInt(MaximumHP*Levels.healMaximumFraction);hp=(int)System.Math.Min(MaximumHP,(long)hp+(long)Mathf.Max(0,healing)*gained);levelNotice="LEVEL UP — GAMERBOY LV "+PlayerLevel;levelNoticeUntil=Time.unscaledTime+4;}
 void RestorePlayerLevel(PlayerProgressionRecord record){playerLevel=Levels.ClampLevel(record.level);playerExperience=System.Math.Max(0,record.experience);Levels.AddXP(ref playerLevel,ref playerExperience,0);encounterXP=0;xpDefeats.Clear();levelNoticeUntil=0;}
 void DrawPlayerLevelNotice(float width){if(Time.unscaledTime>=levelNoticeUntil)return;var style=new GUIStyle(GUI.skin.label){fontSize=16,alignment=TextAnchor.MiddleCenter};style.normal.textColor=NeonDungeonUI.Pink;GUI.Label(new Rect(width/2-190,115,380,30),levelNotice,style);}
}
