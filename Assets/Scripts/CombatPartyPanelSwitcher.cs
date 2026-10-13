using System;
using System.Collections.Generic;
using UnityEngine;
// Read-only HUD values supplied by actual character controllers; no persistent stats here.
public struct CombatPartyHudView {public string name;public Texture2D portrait;public int level,hp,maximumHP,bits,maximumBits;public long experience,requiredExperience;}
public sealed class CombatPartyPanelSwitcher {
 public sealed class Member {public string id;public Func<CombatPartyHudView> read;public Func<bool> canAct;public Action selected;public Action<int> command;}
 readonly List<Member> members=new List<Member>();int current,pending;float elapsed;bool incoming;
 public bool Animating {get;private set;}public float Offset {get;private set;}
 public int Count {get{return members.Count;}}public Member Active {get{return members.Count==0?null:members[current];}}
 public bool Register(Member member){if(member==null||string.IsNullOrEmpty(member.id)||member.read==null||members.Count>=4||members.Exists(m=>m.id==member.id))return false;members.Add(member);return true;}
 public bool Switch(int direction){if(Animating||members.Count<2||direction==0)return false;for(int step=1;step<members.Count;step++){int index=(current+(direction>0?step:-step)+members.Count)%members.Count;var next=members[index];if(next.canAct!=null&&!next.canAct())continue;pending=index;elapsed=0;incoming=false;Animating=true;return true;}return false;}
 public void Tick(float dt,float halfDuration,float distance){if(!Animating)return;elapsed+=Mathf.Max(0,dt);halfDuration=Mathf.Max(.05f,halfDuration);float t=Mathf.Clamp01(elapsed/halfDuration);Offset=Mathf.Lerp(incoming?-distance:0,incoming?0:-distance,Mathf.SmoothStep(0,1,t));if(t<1)return;if(!incoming){current=pending;Active.selected?.Invoke();incoming=true;elapsed=0;Offset=-distance;}else{Offset=0;Animating=false;}}
 public void Reset(){Animating=false;Offset=0;elapsed=0;incoming=false;current=0;pending=0;}
}
