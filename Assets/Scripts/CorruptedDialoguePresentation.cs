using UnityEngine;
// Opt-in adapter: observes the existing dialogue, audio player and helmet CRT.
public sealed class CorruptedDialoguePresentation:MonoBehaviour {
 WorldDialogue dialogue;Object owner;HelmetCRTController crt;CorruptedEncounterPresentationSettings settings;WorldDialogueLine pending;float due,lastEffect=-100,phaseScale=1;bool oldDialogueContext;
 public int EffectsPlayed {get;private set;}
 public void Bind(WorldDialogue value,Object audioOwner,Camera camera,CorruptedEncounterPresentationSettings profile,bool defeat=false){Unbind();dialogue=value;owner=audioOwner;settings=profile;phaseScale=defeat?.45f:1;crt=camera!=null?camera.GetComponent<HelmetCRTController>():null;if(crt!=null){oldDialogueContext=crt.DialogueContext;crt.SetDialogueContext(true);}if(dialogue!=null)dialogue.LineStarted+=OnLine;}
 public void Unbind(){if(dialogue!=null)dialogue.LineStarted-=OnLine;dialogue=null;pending=null;if(crt!=null)crt.SetDialogueContext(oldDialogueContext);crt=null;}
 void OnLine(WorldDialogueLine line){
  pending=null;if(settings==null||line==null)return;float gain=line.audiovisualCue==DialogueAudiovisualCue.Observed?.25f:line.audiovisualCue==DialogueAudiovisualCue.Quiet?.12f:line.audiovisualCue==DialogueAudiovisualCue.DramaticCut?0:1;DungeonAudio.SetEncounterLineGain(owner,gain);
  var level=line.audiovisualCue==DialogueAudiovisualCue.Distress?DialogueCorruptionLevel.Moderate:line.corruption;
  if(line.audiovisualCue==DialogueAudiovisualCue.Quiet||level==DialogueCorruptionLevel.Normal)return;
  pending=line;due=Time.unscaledTime+Mathf.Max(0,line.pauseBefore)+settings.synchronizationDelay;
 }
 void Update(){if(dialogue==null||settings==null||pending==null||Time.unscaledTime<due)return;var line=pending;pending=null;if(dialogue.Current!=line||Time.unscaledTime-lastEffect<settings.effectCooldown)return;var level=line.audiovisualCue==DialogueAudiovisualCue.Distress?DialogueCorruptionLevel.Moderate:line.corruption;Emit(level,phaseScale);}
 public void Pulse(DialogueCorruptionLevel level,float scale=1){Emit(level,scale);}
 public void Disappearance(){if(settings!=null)Emit(DialogueCorruptionLevel.Light,.45f);}
 void Emit(DialogueCorruptionLevel level,float scale){if(settings==null||!settings.audioEffects&&!settings.screenEffects)return;if(Time.unscaledTime-lastEffect<settings.effectCooldown)return;var effect=settings.Level(level);if(effect==null)return;lastEffect=Time.unscaledTime;EffectsPlayed++;if(settings.audioEffects)DungeonAudio.PlayClip(effect.clip,effect.volume*scale);if(settings.screenEffects&&crt!=null)crt.TriggerGlitch(effect.intensity*scale,effect.seconds);}
 void OnDisable(){Unbind();}void OnDestroy(){Unbind();}
}
