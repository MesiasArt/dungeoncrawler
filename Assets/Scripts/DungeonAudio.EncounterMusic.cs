using UnityEngine;
// Extends the existing single music source. Transitions never overlap two songs.
public sealed partial class DungeonAudio {
 Object musicOwner;CorruptedEncounterPresentationSettings musicProfile;bool encounterMusic,bossMusic,requestedCombat;
 AudioClip pendingMusic;bool pendingSwitch,fadeActive;float envelope=1,fadeStart,fadeTarget,fadeElapsed,fadeSeconds,pendingGain=1,contextGain=1,lineMusicGain=1;
 public static void PlayClip(AudioClip clip,float volume){if(clip==null)return;EnsureInstance();if(instance.isActiveAndEnabled)instance.source.PlayOneShot(clip,Mathf.Clamp01(volume)*(instance.settings!=null?instance.settings.effectsVolume:instance.masterVolume));}
 public static void BeginEncounterAmbient(Object owner,CorruptedEncounterPresentationSettings profile,bool defeated=false){if(owner==null||profile==null)return;EnsureInstance();var a=instance;a.musicOwner=owner;a.musicProfile=profile;a.encounterMusic=true;a.bossMusic=false;a.lineMusicGain=1;a.contextGain=profile.ambientGain*(defeated?profile.defeatGain:1);a.RequestMusic(profile.dialogueMusic,a.contextGain,defeated?profile.defeatFadeSeconds:profile.explorationFadeSeconds);}
 public static void BeginEncounterBoss(Object owner,CorruptedEncounterPresentationSettings profile){if(owner==null||profile==null)return;EnsureInstance();var a=instance;a.musicOwner=owner;a.musicProfile=profile;a.encounterMusic=true;a.bossMusic=true;a.lineMusicGain=1;a.contextGain=profile.battleGain;a.RequestMusic(profile.battleMusic!=null?profile.battleMusic:a.combatTrack,a.contextGain,profile.battleFadeSeconds);}
 public static void SetEncounterLineGain(Object owner,float gain){if(instance==null||!instance.encounterMusic||instance.musicOwner!=owner||instance.bossMusic)return;instance.lineMusicGain=Mathf.Clamp01(gain);instance.RequestMusic(instance.musicProfile.dialogueMusic,instance.contextGain*instance.lineMusicGain,.16f);}
 public static void EndEncounterMusic(Object owner,bool combat=false){if(instance==null||!instance.encounterMusic||instance.musicOwner!=owner)return;instance.ReleaseEncounterMusic(combat);}
 void ReleaseEncounterMusic(bool combat){float seconds=musicProfile!=null?musicProfile.restoreFadeSeconds:.5f;encounterMusic=bossMusic=false;musicOwner=null;musicProfile=null;requestedCombat=combat;RequestMusic(BaseTrack(combat),1,seconds);}
 void SelectBaseMusic(bool combat){requestedCombat=combat;if(transitionSilence)return;if(encounterMusic&&musicOwner!=null)return;if(encounterMusic){ReleaseEncounterMusic(combat);return;}RequestMusic(BaseTrack(combat),1,0);}
 void RequestMusic(AudioClip clip,float gain,float seconds){
  gain=Mathf.Clamp01(gain);if(pendingSwitch&&pendingMusic==clip){pendingGain=gain;return;}
  if(!pendingSwitch&&musicSource.clip==clip){musicSource.loop=titleTrack!=null||cityMusic||encounterMusic||Tracks(requestedCombat).Count<=1;if(clip!=null&&!musicSource.isPlaying&&!jumpMusicOverride)musicSource.Play();if((fadeActive&&Mathf.Approximately(fadeTarget,gain))||(!fadeActive&&Mathf.Approximately(envelope,gain)))return;FadeEnvelope(gain,seconds);return;}
  pendingMusic=clip;pendingGain=gain;pendingSwitch=true;
  if(seconds<=0||!musicSource.isPlaying||envelope<=.001f)SwitchMusic(seconds<=0?0:.35f);else FadeEnvelope(0,seconds);
 }
 void FadeEnvelope(float target,float seconds){fadeStart=envelope;fadeTarget=target;fadeElapsed=0;fadeSeconds=Mathf.Max(0,seconds);fadeActive=seconds>0;if(!fadeActive)envelope=target;ApplyMusicVolume();}
 void SwitchMusic(float fadeIn){musicSource.Stop();musicSource.clip=pendingMusic;musicSource.loop=titleTrack!=null||cityMusic||encounterMusic||Tracks(requestedCombat).Count<=1;pendingSwitch=false;envelope=0;if(musicSource.clip!=null&&!jumpMusicOverride)musicSource.Play();FadeEnvelope(pendingGain,fadeIn);}
 void ApplyMusicVolume(){if(musicSource!=null)musicSource.volume=(settings!=null?settings.musicVolume:.28f)*musicGain*envelope;}
 void TickMusic(){
  float target=menuDucked?(settings!=null?settings.menuMusicMultiplier:.7f):1;musicGain=Mathf.Lerp(musicGain,target,1-Mathf.Exp(-8*Time.unscaledDeltaTime));
  if(encounterMusic&&musicOwner==null)ReleaseEncounterMusic(false);
  if(!jumpMusicOverride&&fadeActive){fadeElapsed+=Time.unscaledDeltaTime;float t=fadeSeconds<=0?1:Mathf.Clamp01(fadeElapsed/fadeSeconds);envelope=Mathf.Lerp(fadeStart,fadeTarget,t);if(t>=1){fadeActive=false;if(pendingSwitch)SwitchMusic(.35f);}}
  TickPlaylist();ApplyMusicVolume();
 }
}
