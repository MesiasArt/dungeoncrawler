using System;using UnityEngine;
public enum DialogueAudiovisualCue { Default,Observed,Distress,Quiet,DramaticCut }
[Serializable]public sealed class CorruptionAudiovisualLevel {
 public AudioClip clip;[Range(0,1)]public float volume=.2f,intensity=.12f;[Range(.1f,.3f)]public float seconds=.18f;
}
[CreateAssetMenu(menuName="Codebound/Corrupted encounter presentation")]
public sealed class CorruptedEncounterPresentationSettings:ScriptableObject {
 [Header("Existing music player")]
 public AudioClip dialogueMusic,battleMusic;
 [Min(0)]public float explorationFadeSeconds=1.5f,battleFadeSeconds=.25f,defeatFadeSeconds=.8f,restoreFadeSeconds=.65f,dramaticPauseSeconds=.4f;
 [Range(0,1)]public float ambientGain=.65f,battleGain=1,defeatGain=.28f;
 [Header("Dialogue-triggered interference")]
 public bool audioEffects=true,screenEffects=true;
 [Min(.1f)]public float effectCooldown=.65f;
 [Range(0,.2f)]public float synchronizationDelay=.06f;
 public CorruptionAudiovisualLevel light=new CorruptionAudiovisualLevel{volume=.18f,intensity=.06f,seconds=.1f};
 public CorruptionAudiovisualLevel moderate=new CorruptionAudiovisualLevel{volume=.24f,intensity=.14f,seconds=.18f};
 public CorruptionAudiovisualLevel strong=new CorruptionAudiovisualLevel{volume=.30f,intensity=.26f,seconds=.26f};
 public CorruptionAudiovisualLevel Level(DialogueCorruptionLevel level){return level==DialogueCorruptionLevel.Light?light:level==DialogueCorruptionLevel.Moderate?moderate:strong;}
}
