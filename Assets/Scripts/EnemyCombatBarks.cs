using System;
using System.Collections.Generic;
using UnityEngine;
// Independent of combat coroutines, input, animation and the rhythm clock.
public class EnemyCombatBarks : MonoBehaviour
{
 public EnemyBarkSettings settings;
 readonly Dictionary<string,List<string>> pools=new Dictionary<string,List<string>>();
 readonly Dictionary<string,Queue<string>> recent=new Dictionary<string,Queue<string>>();
 readonly List<string> candidates=new List<string>();
 readonly DialogueTypewriter typing=new DialogueTypewriter();
 string phrase;float expires,lastNormal=-100;int priority;
 public bool DefeatVisible { get { return priority==4&&Visible; } }
 public bool Visible { get { return !string.IsNullOrEmpty(phrase)&&Time.unscaledTime<expires; } }
 void Awake() { if(settings==null)settings=Resources.Load<EnemyBarkSettings>("Combat/Barks/EnemyBarkSettings"); }
 public bool TrySpeak(string enemyId,EnemyBarkCategory category) {
  if(settings==null||settings.enemies==null)return false;
  EnemyBarkRule rule=null;
  foreach(var profile in settings.enemies) if(profile!=null&&string.Equals(profile.enemyId,enemyId,StringComparison.OrdinalIgnoreCase)&&profile.rules!=null) { foreach(var r in profile.rules)if(r!=null&&r.category==category){rule=r;break;}break; }
  if(rule==null&&string.Equals(enemyId,"BIG TAPAKU",StringComparison.OrdinalIgnoreCase))return TrySpeak("TAPAKU",category);
  if(rule==null||rule.phrases==null||UnityEngine.Random.value>=rule.probability)return false;
  int nextPriority=category==EnemyBarkCategory.Defeat?4:category==EnemyBarkCategory.Special||category==EnemyBarkCategory.ExtremeCorruption||category==EnemyBarkCategory.LowHealth?3:category==EnemyBarkCategory.CombatStart?2:1;
  if(Visible&&nextPriority<priority)return false;
  if(nextPriority==1&&Time.unscaledTime-lastNormal<settings.normalCooldown)return false;
  string key=enemyId+"/"+category;List<string> lines;
  if(!pools.TryGetValue(key,out lines)) {
   lines=new List<string>();foreach(var raw in rule.phrases.text.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)){var line=raw.Trim().TrimStart('\uFEFF');if(line.Length>0&&!line.StartsWith("#")&&!lines.Contains(line))lines.Add(line);}pools[key]=lines;
  }
  Queue<string> history;if(!recent.TryGetValue(key,out history))recent[key]=history=new Queue<string>();
  candidates.Clear();foreach(var line in lines)if(!history.Contains(line))candidates.Add(line);
  // An undersized bank stays quiet rather than repeating a recent phrase.
  if(candidates.Count==0)return false;
  phrase=candidates[UnityEngine.Random.Range(0,candidates.Count)];priority=nextPriority;typing.SetText(phrase,Time.unscaledTime,true);expires=Time.unscaledTime+Mathf.Max(.5f,settings.displaySeconds)+(float)typing.RevealSeconds(settings.lettersPerSecond);
  history.Enqueue(phrase);while(history.Count>Mathf.Max(1,settings.recentPhraseCount))history.Dequeue();if(nextPriority==1)lastNormal=Time.unscaledTime;return true;
 }
 public void CompleteText() { if(Visible)typing.Complete(); }
 public void ClearDisplay() { phrase=null; }
 public void Draw(float width,float height,Vector2 enemyAnchor,bool aboveEnemy=false,float laneLeft=8,float laneRight=-1) {
  if(!Visible||settings==null)return;
  if(laneRight<0)laneRight=width-8;
  float available=Mathf.Max(40,laneRight-laneLeft);
  float boxWidth=Mathf.Min(available,Mathf.Min(settings.displayRect.width,320));
  int font=Mathf.Clamp(Mathf.RoundToInt(settings.textFontSize*1.2f),14,20);
  var style=new GUIStyle(GUI.skin.label){fontSize=font,wordWrap=true};
  float textWidth=boxWidth*.77f;
  float textHeight=style.CalcHeight(new GUIContent(phrase),textWidth);
  while(font>14 && textHeight>90){style.fontSize=--font;textHeight=style.CalcHeight(new GUIContent(phrase),textWidth);}
  float boxHeight=Mathf.Max(60,(textHeight+8)/.65f);
  Rect r=new Rect(Mathf.Clamp(enemyAnchor.x-boxWidth*.5f,laneLeft,Mathf.Max(laneLeft,laneRight-boxWidth)),enemyAnchor.y-boxHeight-6,boxWidth,boxHeight);
  r.y=Mathf.Max(4,r.y);
  int savedFont=NeonDungeonUI.FontSize;NeonDungeonUI.FontSize=font;
  NeonDungeonUI.CompactEnemySpeech(r,typing.VisibleText(Time.unscaledTime,settings.lettersPerSecond,true));
  NeonDungeonUI.FontSize=savedFont;
 }
}
