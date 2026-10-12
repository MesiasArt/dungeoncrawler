using System.Globalization;
// Presentation only: time is supplied by the caller, never advances a gameplay action.
public sealed class DialogueTypewriter
{
 string source="",shown="";int[] elements=new int[0];double started;bool completed;int cachedCount=-1,lastSoundCount;
 public string Source { get { return source; } }
 public void SetText(string value,double now,bool restart=false) { value=value??"";if(!restart&&value==source)return;source=value;elements=StringInfo.ParseCombiningCharacters(source);started=now;completed=false;cachedCount=-1;lastSoundCount=0;shown=""; }
 int Count(double now,float speed) { if(completed||speed<=0)return elements.Length;return System.Math.Min(elements.Length,System.Math.Max(0,(int)((now-started)*speed))); }
 public bool IsComplete(double now,float speed) { return Count(now,speed)>=elements.Length; }
 public void Complete() { completed=true;lastSoundCount=elements.Length; }
 public string VisibleText(double now,float speed,bool sound=false) { int count=Count(now,speed);if(count!=cachedCount){shown=count>=elements.Length?source:source.Substring(0,elements[count]);cachedCount=count;}if(sound&&!completed&&speed>0&&count>lastSoundCount){lastSoundCount=count;if(count>0&&char.IsLetterOrDigit(source[elements[count-1]]))DungeonAudio.PlayTextBlip();}return shown; }
 public double RevealSeconds(float speed) { return speed<=0?0:elements.Length/(double)speed; }
}
