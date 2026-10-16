using System.Collections.Generic;
using UnityEngine;
public sealed partial class DungeonAudio {
 AudioClip titleTrack;
 public static void SetTitleTrack(AudioClip track){if(track==null&&instance==null)return;EnsureInstance();instance.titleTrack=track;instance.baseContextKnown=false;instance.SelectBaseMusic(false);}
 DungeonAudioSettings mapAudio;Object mapAudioOwner;
 public static void SetMapPlaylist(Object owner,DungeonAudioSettings profile){EnsureInstance();var a=instance;a.mapAudioOwner=owner;a.mapAudio=profile;a.InvalidatePlaylists();a.baseContextKnown=false;if(!a.cityMusic&&!a.transitionSilence)a.SelectBaseMusic(a.requestedCombat);}
 public static void ClearMapPlaylist(Object owner){if(instance==null||instance.mapAudioOwner!=owner)return;instance.mapAudio=null;instance.mapAudioOwner=null;instance.InvalidatePlaylists();instance.baseContextKnown=false;}
 bool cityMusic,transitionSilence,baseContextKnown,baseCombat;AudioClip baseSelected,lastExploration,lastCombat;
 readonly System.Random playlistRandom=new System.Random();
 public static void FadeForCityTransition(float seconds){EnsureInstance();instance.transitionSilence=true;instance.RequestMusic(null,0,seconds);}
 public static void SetCityMusic(bool active){EnsureInstance();var a=instance;bool changed=a.cityMusic!=active||a.transitionSilence;a.cityMusic=active;a.transitionSilence=false;if(changed||active){a.baseContextKnown=false;a.RequestMusic(a.BaseTrack(false),1,a.settings!=null?a.settings.cityMusicFadeSeconds:1);}}
 sealed class TrackCache {
  public readonly List<AudioClip> tracks=new List<AudioClip>();public DungeonAudioSettings profile;public AudioClip[] source,entries=new AudioClip[0];public bool[] alive=new bool[0];public AudioClip fallback;public bool fallbackAlive,valid;public int count;
  public void Invalidate(){valid=false;profile=null;source=null;fallback=null;tracks.Clear();System.Array.Clear(entries,0,entries.Length);}
 }
 readonly TrackCache explorationTracks=new TrackCache(),combatTracks=new TrackCache();
 void InvalidatePlaylists(){explorationTracks.Invalidate();combatTracks.Invalidate();}
 List<AudioClip> Tracks(bool combat){
  var cache=combat?combatTracks:explorationTracks;var profile=mapAudioOwner!=null&&mapAudio!=null?mapAudio:settings;var list=profile!=null?(combat?profile.normalCombatPlaylist:profile.explorationPlaylist):null;var fallback=combat?combatTrack:dungeonTrack;int count=list!=null?list.Length:0;
  bool changed=!cache.valid||!ReferenceEquals(profile,cache.profile)||!ReferenceEquals(list,cache.source)||!ReferenceEquals(fallback,cache.fallback)||(fallback!=null)!=cache.fallbackAlive||cache.count!=count;
  if(!changed)for(int i=0;i<count;i++)if(!ReferenceEquals(list[i],cache.entries[i])||(list[i]!=null)!=cache.alive[i]){changed=true;break;}
  if(changed){
   if(cache.entries.Length<count){cache.entries=new AudioClip[count];cache.alive=new bool[count];}else if(cache.count>count)System.Array.Clear(cache.entries,count,cache.count-count);
   cache.tracks.Clear();for(int i=0;i<count;i++){var clip=list[i];cache.entries[i]=clip;cache.alive[i]=clip!=null;if(clip!=null&&!cache.tracks.Contains(clip))cache.tracks.Add(clip);}
   if(cache.tracks.Count==0&&fallback!=null)cache.tracks.Add(fallback);
   cache.profile=profile;cache.source=list;cache.fallback=fallback;cache.fallbackAlive=fallback!=null;cache.count=count;cache.valid=true;
  }
  return cache.tracks;
 }
 AudioClip NextTrack(bool combat){var list=Tracks(combat);if(list.Count==0)return null;var previous=combat?lastCombat:lastExploration;int i=list.IndexOf(previous);if(list.Count>1){if(settings!=null&&settings.shufflePlaylist){int pick=playlistRandom.Next(list.Count-(i>=0?1:0));if(i>=0&&pick>=i)pick++;return list[pick];}return list[(i+1)%list.Count];}return list[0];}
 AudioClip BaseTrack(bool combat){if(titleTrack!=null)return titleTrack;if(cityMusic)return settings!=null?settings.cityMusic:null;if(!baseContextKnown||baseCombat!=combat||baseSelected==null){baseCombat=combat;baseContextKnown=true;baseSelected=NextTrack(combat);if(combat)lastCombat=baseSelected;else lastExploration=baseSelected;}return baseSelected;}
 void TickPlaylist(){if(titleTrack!=null||cityMusic||encounterMusic||transitionSilence||jumpMusicOverride||pendingSwitch||fadeActive||musicSource.clip==null)return;var list=Tracks(requestedCombat);musicSource.loop=list.Count<=1;if(!musicSource.loop&&!musicSource.isPlaying&&musicSource.time<=.01f){baseContextKnown=false;var next=BaseTrack(requestedCombat);if(next==musicSource.clip)musicSource.Play();else RequestMusic(next,1,.2f);}}
}
