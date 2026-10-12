using System;
using UnityEngine;
[Serializable] public class WorldDialogueLine {public string speaker;[TextArea(2,5)] public string text;public string giftId;public int giftQuantity;public bool textPanel;public WorldDialogueLine(string s,string t){speaker=s;text=t;}}
public sealed class WorldDialogue {
 public WorldDialogueLine[] lines;public int index;Action finished;Action<WorldDialogueLine> lineFinished;readonly DialogueTypewriter typing=new DialogueTypewriter();
 public bool Active {get{return lines!=null&&index<lines.Length;}}
 public WorldDialogueLine Current {get{return Active?lines[index]:null;}}
 public bool Ready(float speed){return typing.IsComplete(Time.unscaledTime,speed);}
 public string Visible(float speed){return typing.VisibleText(Time.unscaledTime,speed,true);}
 public void Begin(WorldDialogueLine[] value,Action callback=null,Action<WorldDialogueLine> onLineFinished=null){var pages=new System.Collections.Generic.List<WorldDialogueLine>();foreach(var line in value){string remaining=line.text??"";while(remaining.Length>140){int split=remaining.LastIndexOf(' ',140);if(split<40)split=140;pages.Add(new WorldDialogueLine(line.speaker,remaining.Substring(0,split)){textPanel=line.textPanel});remaining=remaining.Substring(split).TrimStart();}pages.Add(new WorldDialogueLine(line.speaker,remaining){giftId=line.giftId,giftQuantity=line.giftQuantity,textPanel=line.textPanel});}lines=pages.ToArray();lineFinished=onLineFinished;index=0;finished=callback;if(Active)typing.SetText(Current.text,Time.unscaledTime,true);}
 public void Advance(float speed){if(!Active)return;if(!Ready(speed)){typing.Complete();return;}DungeonAudio.Play(DungeonAudio.Cue.TextAdvance);lineFinished?.Invoke(Current);index++;if(Active)typing.SetText(Current.text,Time.unscaledTime,true);else{var callback=finished;finished=null;callback?.Invoke();}}
 public void CompleteLetters(){typing.Complete();}
 public void Cancel(){lines=null;finished=null;lineFinished=null;}
}
