using System;using System.Collections.Generic;using System.IO;using UnityEngine;
public static class DrpgPhase4DiagnosticTests {
 public static string Run(){var results=new List<string>();var temp=new List<UnityEngine.Object>();
 Action<bool,string> check=(ok,text)=>results.Add((ok?"PASS ":"FAIL ")+text);
 var map=UnityEngine.Object.Instantiate(Resources.Load<DungeonMapSettings>("DrpgPhase4Tests/FloorA"));temp.Add(map);map.entities=UnityEngine.Object.Instantiate(map.entities);temp.Add(map.entities);
 var p=ScriptableObject.CreateInstance<DungeonInteractionSettings>();p.name="Diagnostic profile";temp.Add(p);map.entities.objects.Find(e=>e.id=="phase4.switch").interaction=p;
 Func<string,bool> has=text=>DungeonInteractionValidation.Validate(map).Exists(e=>e.Contains(text));
 try{
  check(DungeonInteractionValidation.Validate(map).Count==0,"Valid fixture accepted");
  p.actions.Add(new DungeonEventAction{kind=DungeonEventActionKind.OpenDoor,targetId="missing"});check(has("puerta inexistente"),"Missing door detected");p.actions[0].kind=DungeonEventActionKind.UnlockConnection;check(has("conexión inexistente"),"Missing connection detected");p.actions[0].kind=DungeonEventActionKind.SetSwitch;check(has("interruptor inexistente"),"Missing switch action detected");p.actions[0].kind=DungeonEventActionKind.Reward;check(has("recompensa inválida"),"Invalid reward detected");p.actions[0].kind=DungeonEventActionKind.CompleteEvent;check(has("evento sin declarar"),"Undeclared narrative target detected");p.actions[0].kind=DungeonEventActionKind.RunEvent;check(has("falta perfil"),"Missing event implementation detected");p.actions[0].eventProfile=p;check(has("ciclo de eventos"),"Event cycle detected statically");p.actions.Clear();
  p.groups.Add(new DungeonConditionGroup{join=DungeonConditionJoin.Any});check(has("OR vacío"),"Impossible empty OR detected");p.groups[0].join=DungeonConditionJoin.All;
  foreach(var kind in new[]{DungeonConditionKind.Item,DungeonConditionKind.SwitchActive,DungeonConditionKind.EnemyDefeated,DungeonConditionKind.EventCompleted,DungeonConditionKind.HackUnlocked}){p.groups[0].conditions.Clear();p.groups[0].conditions.Add(new DungeonCondition{kind=kind,targetId="missing"});check(DungeonInteractionValidation.Validate(map).Count>0,"Invalid condition reference: "+kind);}
  p.groups[0].conditions.Clear();p.groups[0].conditions.Add(new DungeonCondition{kind=DungeonConditionKind.SwitchActive,targetId="phase4.switch"});p.groups[0].conditions.Add(new DungeonCondition{kind=DungeonConditionKind.SwitchActive,targetId="phase4.switch",negate=true});check(has("AND contradictorio"),"Contradictory AND detected");p.groups[0].conditions.RemoveAt(1);check(has("su propia activación"),"Unreachable self-dependent switch detected");p.groups.Clear();
  p.actions.Add(new DungeonEventAction{id="same",kind=DungeonEventActionKind.Message});p.actions.Add(new DungeonEventAction{id="same",kind=DungeonEventActionKind.Message});check(has("ID de acción"),"Duplicate action IDs detected");p.actions.Clear();
  var other=UnityEngine.Object.Instantiate(p);temp.Add(other);map.entities.objects.Find(e=>e.id=="phase4.unlock").interaction=other;check(has("ID de perfil"),"Duplicate profile IDs detected");map.entities.objects.Find(e=>e.id=="phase4.unlock").interaction=null;
  p.groups.Add(new DungeonConditionGroup{conditions=new List<DungeonCondition>{new DungeonCondition{kind=DungeonConditionKind.Item,targetId="key.red",quantity=3},new DungeonCondition{kind=DungeonConditionKind.Item,targetId="key.red",quantity=2,negate=true}}});check(has("AND contradictorio"),"Impossible item quantity thresholds detected");p.groups.Clear();p.groups.Add(null);check(has("grupo nulo"),"Malformed group reports diagnostic instead of throwing");
  check(DungeonMapDiagnostics.Analyze(DungeonMapSettings.Current).errors.Count==0,"Phase 1 main map diagnostics unchanged");
 }finally{foreach(var obj in temp)UnityEngine.Object.DestroyImmediate(obj);Directory.CreateDirectory("Tests");File.WriteAllLines("Tests/DrpgPhase4Diagnostics.txt",results);}
 return string.Join("\n",results.ToArray());
 }
}
