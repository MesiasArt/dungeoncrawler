using UnityEngine;
public enum DialogueCorruptionLevel { Normal,Light,Moderate,Controlled }
[CreateAssetMenu(menuName="Codebound/Dialogue corruption")]
public sealed class DialogueCorruptionSettings:ScriptableObject {
 public bool enabledEffects=true,allowFlicker=true,allowTypingPauses=true;
 [Range(0,1)]public float intensity=.65f;
 [Range(0,.4f)]public float lightChance=.045f,moderateChance=.10f,controlledChance=.16f;
 [Range(.04f,.4f)]public float recoverySeconds=.12f;
 [Range(0,.3f)]public float flickerStrength=.1f;
 [Range(0,.2f)]public float typingPauseSeconds=.07f;
 public static DialogueCorruptionSettings Current {get{return Resources.Load<DialogueCorruptionSettings>("UI/DialogueCorruptionSettings");}}
 public float Chance(DialogueCorruptionLevel level){return level==DialogueCorruptionLevel.Light?lightChance:level==DialogueCorruptionLevel.Moderate?moderateChance:level==DialogueCorruptionLevel.Controlled?controlledChance:0;}
}
public static class DialogueCorruptionEffects {
 // Presentation-only hash: never consumes UnityEngine.Random or combat RNG.
 static uint Hash(uint value){unchecked{value^=value>>16;value*=0x7feb352d;value^=value>>15;value*=0x846ca68b;return value^(value>>16);}}
 public static string Display(string source,WorldDialogueLine line,float age,DialogueCorruptionSettings settings){
  if(settings==null||!settings.enabledEffects||settings.intensity<=0||line==null||line.corruption==DialogueCorruptionLevel.Normal)return source;
  float chance=(line.characterChance>=0?line.characterChance:settings.Chance(line.corruption))*settings.intensity;
  float recovery=line.recoverySeconds>0?line.recoverySeconds:settings.recoverySeconds;int tick=Mathf.FloorToInt(age/Mathf.Max(.04f,recovery));
  if(tick%3==2)return source;var chars=source.ToCharArray();bool tag=false;
  for(int i=0;i<chars.Length;i++){if(chars[i]=='<'){tag=true;continue;}if(tag){if(chars[i]=='>')tag=false;continue;}if(Hash((uint)(i*7919+tick*104729))%10000>=chance*10000)continue;
   // These replacements retain the original advance in Codebound Pixel Spanish.
   switch(char.ToUpperInvariant(chars[i])){case 'A':chars[i]='4';break;case 'E':chars[i]='3';break;case 'I':chars[i]='!';break;case 'O':chars[i]='0';break;case 'S':chars[i]='5';break;case 'T':chars[i]='?';break;case 'B':chars[i]='8';break;}
  }return new string(chars);
 }
}
