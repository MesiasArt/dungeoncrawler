using UnityEngine;
[System.Serializable]
public class CharacterAttackAccuracy
{
 public string character;
 [Range(0,100)] public float normalMissPercent=10;
 [Range(0,100)] public float specialMissPercent=25;
 public CharacterAttackAccuracy(string name,float normal,float special){character=name;normalMissPercent=normal;specialMissPercent=special;}
}
[System.Serializable]
public class HackAttackAccuracy
{
 [Tooltip("Nombre de la clase del HACK, por ejemplo EnemyJumpHack. JUMP siempre queda excluido.")]
 public string hackType;
 public bool affectedByLuck=true;
 [Range(0,100)] public float missPercent=15;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Combat Luck")]
public sealed class CombatAccuracySettings : ScriptableObject
{
 public bool enabledLuck=true;
 [Tooltip("Porcentajes de fallo: 0 nunca falla, 100 siempre falla. Cada golpe de un multi-hit tiene su propia tirada.")]
 public CharacterAttackAccuracy[] characters={new CharacterAttackAccuracy("GAMERBOY",10,20),new CharacterAttackAccuracy("TAPAKU",10,20),new CharacterAttackAccuracy("HONGO",10,20),new CharacterAttackAccuracy("SOLDADO",12,25),new CharacterAttackAccuracy("BIG TAPAKU",15,25)};
 [Tooltip("Activa solo los HACK deseados. JUMP no puede fallar por suerte.")]
 public HackAttackAccuracy[] hacks=new HackAttackAccuracy[0];
 public bool unlistedHacksUseLuck;
 [Range(0,100)] public float unlistedHackMissPercent=15;
 public float CharacterMissPercent(string character,bool special){if(!enabledLuck)return 0;foreach(var profile in characters??new CharacterAttackAccuracy[0])if(profile!=null&&string.Equals(profile.character,character,System.StringComparison.OrdinalIgnoreCase))return Mathf.Clamp(special?profile.specialMissPercent:profile.normalMissPercent,0,100);return 0;}
 public float HackMissPercent(string hackType,bool eligible){if(!enabledLuck||!eligible||hackType==nameof(EnemyJumpHack))return 0;foreach(var rule in hacks??new HackAttackAccuracy[0])if(rule!=null&&rule.hackType==hackType)return rule.affectedByLuck?Mathf.Clamp(rule.missPercent,0,100):0;return unlistedHacksUseLuck?Mathf.Clamp(unlistedHackMissPercent,0,100):0;}
 public static float EffectiveMiss(float baseline,int luck){return CombatMath.Miss(baseline,luck,CombatBalanceSettings.Current);}
 public static float EffectiveBonus(float baseline,int luck){return CombatMath.Critical(baseline,luck,CombatBalanceSettings.Current);}
 public static float EffectiveFlee(float baseline,int luck){return CombatMath.Flee(baseline,luck,CombatBalanceSettings.Current);}
 public static bool Misses(float percent,float roll){return percent>=100||(percent>0&&roll<Mathf.Clamp(percent,0,100)*.01f);}
}
