using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
public static class MakongBalanceSimulation {
 sealed class Helper {public EnemyStatSnapshot stats;public int hp,born;}
 struct Result {public bool won;public int turns,received,healed,summoned,consumed,potions;}
 static int[] Player(int level,int variant){var c=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");var inv=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var lv=PlayerLevelSettings.Current;int[] v={inv.maximumHP+lv.Growth(lv.healthGrowth,level),c.strength+lv.Growth(lv.attackGrowth,level),c.defense+lv.Growth(lv.defenseGrowth,level),c.luck+lv.Growth(lv.luckGrowth,level)};for(int n=0;n<3;n++){string id=n<variant?MakongBalanceSetup.EquipmentIds[n]:c.initialEquipmentIds[n];var e=Array.Find(c.equipment,item=>item!=null&&item.id==id);v[0]+=e.health;v[1]+=e.strength;v[2]+=e.defense;v[3]+=e.luck;}return v;}
 public static int[] PlayerStats(int level,int variant){return Player(level,variant);}
 static int Hit(EnemyStatSnapshot source,int defense,bool special,System.Random rng){var config=CombatBalanceSettings.Current;float miss=CombatMath.Miss(special?source.specialMissPercent:source.normalMissPercent,source.luck,config);if(rng.NextDouble()<miss/100)return 0;bool crit=rng.NextDouble()<CombatMath.Critical(source.criticalPercent,source.luck,config)/100;return CombatMath.Damage(special?source.specialDamage:source.damage,defense,1,crit,source.criticalMultiplier,config);}
 static Result Fight(int level,int equipment,int performance,int stock,int seed,bool directBoss){
  var rng=new System.Random(seed);var cfg=CombatBalanceSettings.Current;var tacticsSettings=MakongSpecialSettings.Current;var levels=EnemyLevelSettings.Current;var charSettings=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");var accuracy=Resources.Load<CombatAccuracySettings>("Combat/CombatAccuracySettings");var inv=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var over=Resources.Load<OverclockSettings>("Combat/OverclockSettings");var jump=Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");var player=Player(level,equipment);int hp=Mathf.Max(1,(int)(player[0]*(.6+rng.NextDouble()*.4)));var boss=levels.Evaluate(EnemyCharacterCatalog.Current.FindId("Makong"),10);int bossHP=boss.maximumHP;var helpers=new List<Helper>();var state=new MakongTactics();int[] potions=stock==0?new[]{3,2,1}:new[]{1,1,0};Result result=new Result();
  for(int round=1;round<=120&&hp>0;round++){
   result.turns=round;Helper target=null;foreach(var h in helpers)if(h.hp>0&&(!directBoss||bossHP<=0)){target=h;break;}
   int chosenPotion=-1;if(hp<player[0]*.35){int missing=player[0]-hp,best=0;for(int n=0;n<Math.Min(potions.Length,inv.items.Length);n++)if(potions[n]>0&&inv.items[n].healHP<=missing&&inv.items[n].healHP>best){best=inv.items[n].healHP;chosenPotion=n;}if(chosenPotion<0)for(int n=0;n<Math.Min(potions.Length,inv.items.Length);n++)if(potions[n]>0){chosenPotion=n;break;}}
   if(chosenPotion>=0){hp=Math.Min(player[0],hp+inv.items[chosenPotion].healHP);potions[chosenPotion]--;result.potions++;}
   else {
    bool normal=round%3==0;bool clock=!normal&&round%3==1;float quality=1;bool missed=false,critical=false;
    if(normal){missed=rng.NextDouble()<CombatMath.Miss(accuracy.CharacterMissPercent("GAMERBOY",false),player[3],cfg)/100;critical=!missed&&rng.NextDouble()<CombatMath.Critical(charSettings.criticalPercent,player[3],cfg)/100;}
    else if(clock){float success=performance==0?.35f:performance==1?.65f:.9f;quality=rng.NextDouble()<success?over.successQuality:over.failureQuality;float miss=accuracy.HackMissPercent("OverclockHack",true);missed=rng.NextDouble()<CombatMath.Miss(miss,player[3],cfg)/100;}
    else {double fraction=(performance==0?.35:performance==1?.65:.95)+(rng.NextDouble()-.5)*.15;int combo=Mathf.Clamp((int)Math.Round(jump.enemiesToWin*fraction),0,jump.enemiesToWin);int perfect=(int)(combo*(performance==0?.3:performance==1?.6:.9));quality=CombatMath.JumpQuality(combo,perfect,combo-perfect,jump.enemiesToWin,cfg);}
    int damage=missed?0:CombatMath.Damage(player[1],target!=null?target.stats.defense:boss.defense,quality,critical,charSettings.criticalMultiplier,cfg);if(target!=null)target.hp=Math.Max(0,target.hp-damage);else bossHP=Math.Max(0,bossHP-damage);
   }
   helpers.RemoveAll(h=>h.hp<=0);if(bossHP<=0&&helpers.Count==0){result.won=true;break;}
   if(bossHP>0){bool edible=helpers.Exists(h=>state.Turn+1-h.born>=tacticsSettings.helperMinimumAgeTurns);var action=state.Decide(new MakongContext{hp=bossHP,maximumHP=boss.maximumHP,helpers=helpers.Count,participants=helpers.Count+1,edibleHelper=edible,canSummon=true},tacticsSettings,()=>rng.NextDouble());
    if(action==MakongAction.Swallow){var victim=helpers.Find(h=>state.Turn-h.born>=tacticsSettings.helperMinimumAgeTurns);if(victim!=null){helpers.Remove(victim);int before=bossHP;bossHP=Math.Min(boss.maximumHP,bossHP+tacticsSettings.Healing(boss.maximumHP));result.healed+=bossHP-before;result.consumed++;}}
    else if(action==MakongAction.Summon){int amount=Math.Min(2-helpers.Count,rng.NextDouble()<tacticsSettings.twoHelpersProbability?2:1);var valid=new List<SpeciesSpawnWeight>();foreach(var row in tacticsSettings.helperSpecies){var p=EnemyCharacterCatalog.Current.FindId(row.speciesId);if(row.weight>0&&p!=null&&!p.reservedForEvents&&p.category==EnemyRank.Enemigo)valid.Add(row);}var weights=new double[valid.Count];for(int i=0;i<weights.Length;i++)weights[i]=valid[i].weight;
     for(int i=0;i<amount;i++){int pick=EnemyZoneSettings.Select(weights,rng.NextDouble());int helperLevel=rng.Next(tacticsSettings.helperMinimumLevel,tacticsSettings.helperMaximumLevel+1);var stats=levels.Evaluate(EnemyCharacterCatalog.Current.FindId(valid[pick].speciesId),Math.Min(9,helperLevel));helpers.Add(new Helper{stats=stats,hp=stats.maximumHP,born=state.Turn});result.summoned++;}}
    else {int damage=Hit(boss,player[2],action==MakongAction.Earthquake,rng);hp=Math.Max(0,hp-damage);result.received+=damage;}
   }
   foreach(var helper in helpers){if(bossHP>0&&helper.born==state.Turn)continue;int damage=Hit(helper.stats,player[2],false,rng);hp=Math.Max(0,hp-damage);result.received+=damage;if(hp<=0)break;}
  }
  return result;
 }
 static string N(double value){return value.ToString("F2",CultureInfo.InvariantCulture);}
 public static string Run(int trials=300){var stats=new List<string>{"level,equipment,HP,ATK,DEF,LUK"};var rows=new List<string>{"level,equipment,performance,potions_stock,strategy,trials,win_percent,turns_mean,damage_received_mean,makong_healing_mean,helpers_summoned_mean,helpers_consumed_mean,potions_used_mean"};for(int level=8;level<=11;level++)for(int eq=0;eq<4;eq++){var v=Player(level,eq);stats.Add(level+","+eq+","+string.Join(",",Array.ConvertAll(v,x=>x.ToString())));for(int perf=0;perf<3;perf++)for(int stock=0;stock<2;stock++)for(int strategy=0;strategy<2;strategy++){double wins=0,turns=0,damage=0,healing=0,summons=0,consumes=0,pots=0;for(int n=0;n<trials;n++){var r=Fight(level,eq,perf,stock,104729+level*100000+eq*10000+perf*2000+stock*1000+strategy*500+n,strategy==1);wins+=r.won?1:0;turns+=r.turns;damage+=r.received;healing+=r.healed;summons+=r.summoned;consumes+=r.consumed;pots+=r.potions;}
 rows.Add(level+","+eq+","+perf+","+stock+","+strategy+","+trials+","+N(wins/trials*100)+","+N(turns/trials)+","+N(damage/trials)+","+N(healing/trials)+","+N(summons/trials)+","+N(consumes/trials)+","+N(pots/trials));}}
 System.IO.File.WriteAllLines("Tests/MakongBalanceStats.csv",stats);System.IO.File.WriteAllLines("Tests/MakongBalanceSimulation.csv",rows);return "Simulados "+((rows.Count-1)*trials)+" combates reproducibles en "+(rows.Count-1)+" escenarios";}
}
