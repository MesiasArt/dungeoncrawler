using System;
using UnityEngine;

public sealed partial class DungeonAudio {
 internal static Action CaptureTransitionRecovery(){
  EnsureInstance();var owner=instance.mapAudioOwner;var profile=instance.mapAudio;bool city=instance.cityMusic,combat=instance.requestedCombat,known=instance.baseContextKnown;
  var selected=instance.baseSelected;var exploration=instance.lastExploration;var battle=instance.lastCombat;var clip=instance.musicSource.clip;
  return ()=>{EnsureInstance();var a=instance;a.mapAudioOwner=owner;a.mapAudio=profile;a.cityMusic=city;a.transitionSilence=false;a.requestedCombat=combat;a.baseCombat=combat;a.baseContextKnown=known;a.baseSelected=selected;a.lastExploration=exploration;a.lastCombat=battle;a.RequestMusic(clip,1,.5f);};
 }
}
