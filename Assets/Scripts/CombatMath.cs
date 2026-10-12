using System;
public static class CombatMath {
 static double Safe(float value,double fallback=0){return float.IsNaN(value)||float.IsInfinity(value)?fallback:value;}
 static float Clamp(double value,double min,double max){return (float)Math.Max(min,Math.Min(max,value));}
 public static int Damage(int attack,int defense,float quality,bool critical,float criticalMultiplier,CombatBalanceSettings config,bool immune=false){
  if(immune||attack<=0||Safe(quality)<=0)return 0;
  double coefficient=Math.Max(0,Safe(config.defenseCoefficient));
  double raw=attack*Math.Min(100,Safe(quality))/(1+Math.Max(0,defense)*coefficient/100d);
  if(critical)raw*=Math.Max(1,Math.Min(Safe(config.maximumCriticalMultiplier,1),Safe(criticalMultiplier,1)));
  long rounded=(long)Math.Floor(Math.Min(int.MaxValue,raw)+.5d);
  return (int)Math.Max(Math.Max(1,config.minimumHitDamage),rounded);
 }
 public static float Miss(float baseline,int luck,CombatBalanceSettings config){return Clamp(Safe(baseline)-Math.Min(Math.Max(0,Safe(config.maximumMissReduction)),Math.Max(0,luck)*Math.Max(0,Safe(config.missReductionPerLuck))),0,100);}
 public static float Critical(float baseline,int luck,CombatBalanceSettings config){return Clamp(Safe(baseline)+Math.Max(0,luck)*Math.Max(0,Safe(config.criticalPerLuck)),0,Clamp(Safe(config.maximumCriticalPercent),0,100));}
 public static float Flee(float baseline,int luck,CombatBalanceSettings config){return Clamp(Safe(baseline)+Math.Max(0,luck)*Math.Max(0,Safe(config.fleePerLuck)),0,Clamp(Safe(config.maximumFleePercent),0,100));}
 public static float JumpQuality(int combo,int perfect,int good,int target,CombatBalanceSettings config){int hits=Math.Max(0,combo);int scored=Math.Min(hits,Math.Max(0,perfect)+Math.Max(0,good));double points=Math.Max(0,perfect)+Math.Max(0,good)*Clamp(Safe(config.jumpGoodWeight),0,1)+Math.Max(0,hits-scored);return Clamp(points/Math.Max(1,target),0,1)*Clamp(Safe(config.jumpMaximumQuality),.1,5);}
 public static bool CriticalRoll(float percent,float roll){return roll<Clamp(Safe(percent),0,100)*.01f;}
}
