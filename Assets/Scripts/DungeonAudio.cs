using UnityEngine;
public sealed class DungeonAudio : MonoBehaviour
{
 public enum Cue { Step,Attack,GhostAttack,HongoAttack,SoldierAttack,EnemyHurt,EnemyDeath,PlayerHurt,PlayerDeath,GrenadeThrow,Explosion,Door,Jump,Stomp,HackWin,HackFail,Item,Encounter,Confirm,Text,TextAdvance,JumpPoint }
 [Range(0,1)] public float masterVolume=.45f;
 static DungeonAudio instance;
 AudioSource source, musicSource;
 public DungeonAudioSettings settings;
 AudioClip dungeonTrack,combatTrack;
 AudioClip[] clips;
 static bool jumpMusicOverride;
 public static AudioClip JumpTrack { get { EnsureInstance();return instance.combatTrack; } }
 public static float MusicVolume { get { EnsureInstance();return instance.settings!=null ? instance.settings.musicVolume : .28f; } }
 public static void SetJumpMusicOverride(bool active)
 { jumpMusicOverride=active;if(!active&&instance==null)return;EnsureInstance();if(instance.musicSource==null)return;if(active) instance.musicSource.Pause();else if(instance.musicSource.clip!=null) instance.musicSource.UnPause(); }
 static void EnsureInstance()
 { if(instance==null) { var root=new GameObject("Dungeon audio");instance=root.AddComponent<DungeonAudio>(); } }
 public static void SetCombat(bool combat)
 {
  EnsureInstance();
  if(jumpMusicOverride) return;
  AudioClip clip=combat ? instance.combatTrack : instance.dungeonTrack;
  instance.musicSource.volume=instance.settings!=null ? instance.settings.musicVolume : .28f;
  if(instance.musicSource.clip==clip) return;
  instance.musicSource.Stop();instance.musicSource.clip=clip;
  if(clip!=null) instance.musicSource.Play();
 }
 static float lastTextSound=-100;
 public static void PlayTextBlip() { EnsureInstance();if(!instance.isActiveAndEnabled||Time.unscaledTime-lastTextSound<.025f)return;lastTextSound=Time.unscaledTime;var clip=instance.clips[(int)Cue.Text];if(clip!=null)instance.source.PlayOneShot(clip,(instance.settings!=null?instance.settings.effectsVolume*instance.settings.dialogueVolume:.045f)); }
 public static void Play(Cue cue)
 {
  EnsureInstance();
  if(!instance.isActiveAndEnabled) return;
  AudioClip clip=instance.clips[(int)cue];
  if(clip!=null) instance.source.PlayOneShot(clip,(instance.settings!=null ? instance.settings.effectsVolume : instance.masterVolume)*(cue==Cue.Step ? .6f : cue==Cue.TextAdvance ? (instance.settings!=null?instance.settings.dialogueAdvanceVolume:.35f) : 1));
 }
 void Awake()
 {
  if(instance!=null && instance!=this) { Destroy(gameObject);return; }
  instance=this;source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;
  settings=Resources.Load<DungeonAudioSettings>("Audio/DungeonAudioSettings");
  dungeonTrack=settings!=null && settings.dungeonMusic!=null ? settings.dungeonMusic : Resources.Load<AudioClip>("Audio/Music/DungeonMusic");
  combatTrack=settings!=null && settings.combatMusic!=null ? settings.combatMusic : Resources.Load<AudioClip>("Audio/Music/CombatMusic");
  musicSource=gameObject.AddComponent<AudioSource>();musicSource.playOnAwake=false;musicSource.spatialBlend=0;musicSource.loop=true;
  string[] names=System.Enum.GetNames(typeof(Cue));clips=new AudioClip[names.Length];
  for(int i=0;i<names.Length;i++) clips[i]=Resources.Load<AudioClip>("Audio/Temporary/"+names[i]);
 }
 void Update() { if(musicSource!=null&&settings!=null)musicSource.volume=settings.musicVolume; }
 void OnDestroy() { if(instance==this) { instance=null;jumpMusicOverride=false;lastTextSound=-100; } }
}
