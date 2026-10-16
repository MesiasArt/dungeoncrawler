using System;
using UnityEngine;
[Serializable] public class WorldDialogueLine {
 public string presentationCue;
 public string speaker;[TextArea(2,5)]public string text;public string giftId;public int giftQuantity;public bool textPanel;
 [Header("Optional text presentation")]
 public DialogueAudiovisualCue audiovisualCue;
 public DialogueCorruptionLevel corruption;
 [Min(0)]public float writingSpeed,pauseBefore,recoverySeconds;
 [Range(-1,.4f)]public float characterChance=-1;
 [Range(-1,.3f)]public float flickerStrength=-1;
 [Tooltip("0 keeps all characters; a positive value ends the displayed line abruptly without changing its source.")] [Min(0)]public int interruptAfterCharacters;
 public WorldDialogueLine(string s,string t){speaker=s;text=t;}
 public WorldDialogueLine Page(string value,bool last){return new WorldDialogueLine(speaker,value){presentationCue=presentationCue,giftId=last?giftId:null,giftQuantity=last?giftQuantity:0,textPanel=textPanel,audiovisualCue=audiovisualCue,corruption=corruption,writingSpeed=writingSpeed,pauseBefore=pauseBefore,recoverySeconds=recoverySeconds,characterChance=characterChance,flickerStrength=flickerStrength,interruptAfterCharacters=interruptAfterCharacters};}
}
public sealed class WorldDialogue {
 public WorldDialogueLine[] lines;public int index;Action finished;Action<WorldDialogueLine> lineFinished;readonly DialogueTypewriter typing=new DialogueTypewriter();
 public event Action<WorldDialogueLine> LineStarted;
 float lineStarted;bool forced;
 public bool Active {get{return lines!=null&&index<lines.Length;}}
 public WorldDialogueLine Current {get{return Active?lines[index]:null;}}
 float Speed(float fallback){return Current!=null&&Current.writingSpeed>0?Current.writingSpeed:fallback;}
 float Age {get{return Mathf.Max(0,Time.unscaledTime-lineStarted);}}
 double TypingNow {get{float elapsed=Mathf.Max(0,Age-(Current!=null?Current.pauseBefore:0));var settings=DialogueCorruptionSettings.Current;if(settings!=null&&settings.enabledEffects&&settings.allowTypingPauses&&Current!=null&&Current.corruption>=DialogueCorruptionLevel.Moderate){float pause=settings.typingPauseSeconds*settings.intensity,cycle=.55f+pause;elapsed=Mathf.Floor(elapsed/cycle)*.55f+Mathf.Min(.55f,elapsed%cycle);}return lineStarted+elapsed;}}
 string Source {get{var value=Current!=null?Current.text??"":"";return Current!=null&&Current.interruptAfterCharacters>0?value.Substring(0,Mathf.Min(value.Length,Current.interruptAfterCharacters)):value;}}
 void StartLine(){forced=false;lineStarted=Time.unscaledTime;typing.SetText(Source,lineStarted,true);LineStarted?.Invoke(Current);}
 public bool Ready(float speed){return forced||Current!=null&&Age>=Current.pauseBefore&&typing.IsComplete(TypingNow,Speed(speed));}
 public string Visible(float speed){var value=typing.VisibleText(TypingNow,Speed(speed),true);return forced?Source:DialogueCorruptionEffects.Display(value,Current,Age,DialogueCorruptionSettings.Current);}
 public float FramePulse {get{var s=DialogueCorruptionSettings.Current;var line=Current;if(line==null||line.corruption==DialogueCorruptionLevel.Normal||forced||s==null||!s.enabledEffects||!s.allowFlicker)return 0;float phase=Age%1.7f;if(phase>.055f)return 0;return (line.flickerStrength>=0?line.flickerStrength:s.flickerStrength)*s.intensity*(int)line.corruption/3f;}}
 public void Begin(WorldDialogueLine[] value,Action callback=null,Action<WorldDialogueLine> onLineFinished=null){var pages=new System.Collections.Generic.List<WorldDialogueLine>();foreach(var line in value??new WorldDialogueLine[0]){if(line==null)continue;string remaining=line.text??"";while(remaining.Length>140){int split=remaining.LastIndexOf(' ',140);if(split<40)split=140;pages.Add(line.Page(remaining.Substring(0,split),false));remaining=remaining.Substring(split).TrimStart();}pages.Add(line.Page(remaining,true));}lines=pages.ToArray();lineFinished=onLineFinished;index=0;finished=callback;if(Active)StartLine();else{finished=null;callback?.Invoke();}}
 public void Advance(float speed){if(!Active)return;if(!Ready(speed)){CompleteLetters();return;}DungeonAudio.Play(DungeonAudio.Cue.TextAdvance);lineFinished?.Invoke(Current);index++;if(Active)StartLine();else{var callback=finished;finished=null;callback?.Invoke();}}
 public void CompleteLetters(){forced=true;typing.Complete();}
 public void Cancel(){lines=null;finished=null;lineFinished=null;forced=false;}
}
