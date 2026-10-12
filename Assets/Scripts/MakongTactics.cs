using System;
public enum MakongAction { Normal, Earthquake, Swallow, Summon }
public struct MakongContext { public int hp,maximumHP,helpers,participants;public bool edibleHelper,canSummon; }
// Per-encounter state. It contains decisions only; all damage and actors stay in DungeonPrototype.
public sealed class MakongTactics {
 public int Turn {get;private set;}public int Swallows {get;private set;}
 public int EarthquakeReadyAt {get;private set;}public int SwallowReadyAt {get;private set;}public int SummonReadyAt {get;private set;}
 public MakongAction Previous {get;private set;}public string Reason {get;private set;}
 int lastSpecial=-100;
 public bool CanUse(MakongAction action,MakongContext c,MakongSpecialSettings s,int atTurn){
  if(action==MakongAction.Normal)return true;
  if(atTurn-lastSpecial<=Math.Max(0,s.normalTurnsBetweenSpecials))return false;
  if(action==MakongAction.Earthquake)return atTurn>=EarthquakeReadyAt&&Previous!=action;
  if(action==MakongAction.Swallow)return c.hp<(double)Math.Max(1,c.maximumHP)*s.healBelowFraction&&c.edibleHelper&&Swallows<Math.Max(0,s.maximumSwallows)&&atTurn>=SwallowReadyAt&&Previous!=MakongAction.Summon;
  return c.canSummon&&c.hp>=(double)Math.Max(1,c.maximumHP)*s.minimumSummonHPFraction&&c.helpers<Math.Min(2,Math.Max(0,s.maximumHelpers))&&c.participants<3&&atTurn>=SummonReadyAt&&Previous!=MakongAction.Swallow&&Previous!=action;
 }
 public MakongAction Decide(MakongContext c,MakongSpecialSettings s,Func<double> random){
  Turn++;MakongAction chosen=MakongAction.Normal;int priority=int.MinValue;
  foreach(var action in new[]{MakongAction.Swallow,MakongAction.Summon,MakongAction.Earthquake}){
   bool allowed=CanUse(action,c,s,Turn);double roll=random();float chance=action==MakongAction.Swallow?s.swallowProbability:action==MakongAction.Summon?s.summonProbability:s.earthquakeProbability;
   int rank=action==MakongAction.Swallow?s.swallowPriority:action==MakongAction.Summon?s.summonPriority:s.earthquakePriority;
   if(allowed&&roll<Math.Max(0,Math.Min(1,chance))&&rank>priority){chosen=action;priority=rank;}
  }
  Reason=chosen==MakongAction.Normal?"Condiciones, enfriamientos o probabilidad: ataque normal":chosen==MakongAction.Swallow?"HP bajo, ayudante elegible, usos y enfriamiento disponibles":chosen==MakongAction.Summon?"Faltan ayudantes y hay espacio en el encuentro":"Terremoto disponible y elegido por probabilidad/prioridad";
  Commit(chosen,s);return chosen;
 }
 public bool TryManual(MakongAction action,MakongContext context,MakongSpecialSettings settings){if(!CanUse(action,context,settings,Turn+1))return false;Turn++;Reason="Prueba manual válida";Commit(action,settings);return true;}
 public void Commit(MakongAction action,MakongSpecialSettings s){Previous=action;if(action==MakongAction.Normal)return;lastSpecial=Turn;
  if(action==MakongAction.Earthquake)EarthquakeReadyAt=Turn+Math.Max(0,s.earthquakeCooldownTurns)+1;
  if(action==MakongAction.Swallow){Swallows++;SwallowReadyAt=Turn+Math.Max(0,s.swallowCooldownTurns)+1;}
  if(action==MakongAction.Summon)SummonReadyAt=Turn+Math.Max(0,s.summonCooldownTurns)+1;
 }
}
