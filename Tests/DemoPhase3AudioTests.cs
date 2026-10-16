#if UNITY_EDITOR && DEMO_PHASE3_AUDIO
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public static class DemoPhase3AudioTests {
 public static void Run(Action<bool,string> check,DungeonAudio audio){
  var f=BindingFlags.Instance|BindingFlags.NonPublic;string[] fields={"mapAudioOwner","mapAudio","dungeonTrack","combatTrack","lastExploration","lastCombat","playlistRandom"};var saved=new object[fields.Length];for(int i=0;i<fields.Length;i++)saved[i]=typeof(DungeonAudio).GetField(fields[i],f).GetValue(audio);
  var original=audio.settings;var music=(AudioSource)typeof(DungeonAudio).GetField("musicSource",f).GetValue(audio);var playing=music.clip;float volume=music.volume;
  var profile=ScriptableObject.CreateInstance<DungeonAudioSettings>();var a=AudioClip.Create("Phase3 A",32,1,8000,false);var b=AudioClip.Create("Phase3 B",32,1,8000,false);var c=AudioClip.Create("Phase3 C",32,1,8000,false);var owner=new GameObject("Phase3 audio fixture owner");
  var tracks=(Func<bool,List<AudioClip>>)Delegate.CreateDelegate(typeof(Func<bool,List<AudioClip>>),audio,typeof(DungeonAudio).GetMethod("Tracks",f));var next=(Func<bool,AudioClip>)Delegate.CreateDelegate(typeof(Func<bool,AudioClip>),audio,typeof(DungeonAudio).GetMethod("NextTrack",f));
  Action<string,object> set=(name,value)=>typeof(DungeonAudio).GetField(name,f).SetValue(audio,value);
  try{
   audio.settings=profile;set("mapAudioOwner",owner);set("mapAudio",profile);set("dungeonTrack",a);set("combatTrack",b);profile.explorationPlaylist=new[]{a,null,b,a};profile.normalCombatPlaylist=new[]{c,b};
   var first=tracks(false);check(first.SequenceEqual(new[]{a,b}),"Audio: null filtering and duplicate removal retain their original order");check(ReferenceEquals(first,tracks(false)),"Audio: unchanged playlist reuses its list");check(tracks(true).SequenceEqual(new[]{c,b}),"Audio: combat has an independent cached playlist");
   profile.explorationPlaylist[0]=c;check(tracks(false).SequenceEqual(new[]{c,b,a}),"Audio: live in-place Inspector edits invalidate the cache");profile.explorationPlaylist=new AudioClip[0];check(tracks(false).SequenceEqual(new[]{a}),"Audio: empty playlist retains legacy fallback track");
   profile.explorationPlaylist=new[]{a,b,c};profile.shufflePlaylist=false;set("lastExploration",a);check(next(false)==b,"Audio: sequential selection remains unchanged");
   profile.shufflePlaylist=true;set("playlistRandom",new System.Random(1234));var expectedRandom=new System.Random(1234);AudioClip previous=a;bool same=true;
   for(int i=0;i<100;i++){set("lastExploration",previous);var candidates=new[]{a,b,c}.Where(x=>x!=previous).ToArray();var expected=candidates[expectedRandom.Next(candidates.Length)];var actual=next(false);same&=actual==expected&&actual!=previous;previous=actual;}
   check(same,"Audio: 100 shuffled selections match the previous candidate-list algorithm exactly");
   check(music.clip==playing&&Mathf.Approximately(music.volume,volume),"Audio: cache and selection do not replace the playing clip or volume");
  }finally{audio.settings=original;for(int i=0;i<fields.Length;i++)typeof(DungeonAudio).GetField(fields[i],f).SetValue(audio,saved[i]);UnityEngine.Object.Destroy(profile);UnityEngine.Object.Destroy(a);UnityEngine.Object.Destroy(b);UnityEngine.Object.Destroy(c);UnityEngine.Object.Destroy(owner);}
 }
}
#endif
