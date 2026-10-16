using System.Collections.Generic;
using UnityEngine;
public sealed partial class DungeonAudio {
 AudioClip titleTrack;
 public static void SetTitleTrack(AudioClip track){if(track==null&&instance==null)return;EnsureInstance();instance.titleTrack=track;instance.baseContextKnown=false;instance.SelectBaseMusic(false);}
 DungeonAudioSettings mapAudio;Object mapAudioOwner;
 public static void SetMapPlaylist(Object owner,DungeonAudioSettings profile){EnsureInstance();var a=instance;a.mapAudioOwner=owner;a.mapAudio=profile;a.baseContextKnown=false;if(!a.cityMusic&&!a.transitionSilence)a.SelectBaseMusic(a.requestedCombat);}
 public static void ClearMapPlaylist(Object owner){if(instance==null||instance.mapAudioOwner!=owner)return;instance.mapAudio=null;instance.mapAudioOwner=null;instance.baseContextKnown=false;}
 bool cityMusic,transitionSilence,baseContextKnown,baseCombat;AudioClip baseSelected,lastExploration,lastCombat;
 readonly System.Random playlistRandom=new System.Random();
 public static void FadeForCityTransition(float seconds){EnsureInstance();instance.transitionSilence=true;instance.RequestMusic(null,0,seconds);}
 public static void SetCityMusic(bool active){EnsureInstance();var a=instance;bool changed=a.cityMusic!=active||a.transitionSilence;a.cityMusic=active;a.transitionSilence=false;if(changed||active){a.baseContextKnown=false;a.RequestMusic(a.BaseTrack(false),1,a.settings!=null?a.settings.cityMusicFadeSeconds:1);}}
 List<AudioClip> Tracks(bool combat){var result=new List<AudioClip>();var profile=mapAudioOwner!=null&&mapAudio!=null?mapAudio:settings;var list=profile!=null?(combat?profile.normalCombatPlaylist:profile.explorationPlaylist):null;if(list!=null)foreach(var clip in list)if(clip!=null&&!result.Contains(clip))result.Add(clip);if(result.Count==0){var old=combat?combatTrack:dungeonTrack;if(old!=null)result.Add(old);}return result;}
 AudioClip NextTrack(bool combat){var list=Tracks(combat);if(list.Count==0)return null;var previous=combat?lastCombat:lastExploration;int i=list.IndexOf(previous);if(list.Count>1){if(settings!=null&&settings.shufflePlaylist){var candidates=list.FindAll(c=>c!=previous);return candidates[playlistRandom.Next(candidates.Count)];}return list[(i+1)%list.Count];}return list[0];}
 AudioClip BaseTrack(bool combat){if(titleTrack!=null)return titleTrack;if(cityMusic)return settings!=null?settings.cityMusic:null;if(!baseContextKnown||baseCombat!=combat||baseSelected==null){baseCombat=combat;baseContextKnown=true;baseSelected=NextTrack(combat);if(combat)lastCombat=baseSelected;else lastExploration=baseSelected;}return baseSelected;}
 void TickPlaylist(){if(titleTrack!=null||cityMusic||encounterMusic||transitionSilence||jumpMusicOverride||pendingSwitch||fadeActive||musicSource.clip==null)return;var list=Tracks(requestedCombat);musicSource.loop=list.Count<=1;if(!musicSource.loop&&!musicSource.isPlaying&&musicSource.time<=.01f){baseContextKnown=false;var next=BaseTrack(requestedCombat);if(next==musicSource.clip)musicSource.Play();else RequestMusic(next,1,.2f);}}
}
