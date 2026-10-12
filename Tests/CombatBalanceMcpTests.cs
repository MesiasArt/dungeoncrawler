var cfg=UnityEngine.ScriptableObject.CreateInstance<CombatBalanceSettings>();int assertions=0;System.Action<bool,string> check=(ok,label)=>{assertions++;if(!ok)throw new System.Exception(label);};
check(CombatMath.Damage(2,7,1,false,1,cfg)==1,"weak hits equipped player");
check(CombatMath.Damage(3,7,1,false,1,cfg)==2,"normal hit");
check(CombatMath.Damage(6,7,1,false,1,cfg)==4,"strong hit");
check(CombatMath.Damage(9,9,1,false,1,cfg)==5,"player vs strong");
check(CombatMath.Damage(9,9,1,true,1.5f,cfg)==7,"critical before final rounding");
check(CombatMath.Damage(9,int.MaxValue,1,false,1,cfg)==1,"high defense minimum");
check(CombatMath.Damage(0,3,1,false,1,cfg)==0,"no attack means zero");check(CombatMath.Damage(9,3,0,false,1,cfg)==0,"failed minigame means zero");
check(CombatMath.Damage(9,3,1,false,1,cfg,true)==0,"explicit immunity");check(CombatMath.Damage(-5,-2,1,false,1,cfg)==0,"negative ATK");check(CombatMath.Damage(9,-3,1,false,1,cfg)==9,"negative DEF clamped");
check(CombatMath.Damage(9,3,float.NaN,false,1,cfg)==0,"NaN quality");check(CombatMath.Damage(int.MaxValue,0,100,true,3,cfg)==int.MaxValue,"overflow clamped");
check(CombatMath.Critical(5,1000,cfg)==30,"crit cap");check(CombatMath.Miss(15,1000,cfg)==5,"miss reduction cap");check(CombatMath.Flee(50,1000,cfg)==85,"flee cap");check(CombatMath.Miss(10,10,cfg)==7.5f,"luck coefficient");
check(!CombatMath.CriticalRoll(0,0),"zero crit");check(CombatMath.CriticalRoll(100,.999f),"100 crit");
int prev=int.MaxValue;for(int defense=0;defense<=100;defense++){int hit=CombatMath.Damage(9,defense,1,false,1,cfg);check(hit<=prev&&hit>=1,"monotonic defense");prev=hit;}
prev=0;for(int atk=1;atk<=100;atk++){int hit=CombatMath.Damage(atk,9,1,false,1,cfg);check(hit>=prev,"monotonic ATK");prev=hit;}
check(CombatMath.JumpQuality(0,0,0,10,cfg)==0,"Jump fail");check(CombatMath.JumpQuality(10,10,0,10,cfg)==2.5f,"perfect quality");check(CombatMath.JumpQuality(10,0,10,10,cfg)<2.5f,"good below perfect");check(CombatMath.Damage(9,9,2.5f,false,1,cfg)==12,"Jump uses ATK DEF");check(CombatMath.Damage(6,9,2.5f,false,1,cfg)==8,"Jump strength matters");check(CombatMath.Damage(9,20,2.5f,false,1,cfg)<12,"Jump defense matters");
UnityEngine.Object.DestroyImmediate(cfg);return "PASS "+assertions+" assertions: minimum, weak/strong, defense/ATK monotonicity, crit bounds, luck caps, immunity, no damage on failure, overflow/NaN, Jump quality and statistics";
