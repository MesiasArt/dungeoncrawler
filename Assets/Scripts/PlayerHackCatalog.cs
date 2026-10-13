using System;
using System.Collections.Generic;
using UnityEngine;
public enum HackUnlockMode { PlayerLevel, FutureEvent }
[Serializable] public class PlayerHackDefinition {
 public string id;
 public string displayName;
 [TextArea] public string description;
 [Min(1)] public int minimumLevel=1;
 [Min(0)] public int bitsCost;
 public bool available=true;
 public HackUnlockMode unlockMode;
 public UnityEngine.Object implementationScript;
 [HideInInspector] public string implementationTypeName;
 public Type ImplementationType {get{if(string.IsNullOrWhiteSpace(implementationTypeName))return null;var type=typeof(CombatHack).Assembly.GetType(implementationTypeName??"");if(type==null)foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){type=assembly.GetType(implementationTypeName??"");if(type!=null)break;}return type!=null&&!type.IsAbstract&&typeof(CombatHack).IsAssignableFrom(type)?type:null;}}
 public bool Unlocked(int level,ICollection<string> eventIds){return available&&ImplementationType!=null&&(unlockMode==HackUnlockMode.PlayerLevel?level>=Math.Max(1,minimumLevel):eventIds!=null&&eventIds.Contains(id));}
}
[CreateAssetMenu(menuName="Virtual Dungeon/Catálogo de HACKS")]
public class PlayerHackCatalog:ScriptableObject {
 public PlayerHackDefinition[] hacks=new PlayerHackDefinition[0];
 public static PlayerHackCatalog Current {get{return Resources.Load<PlayerHackCatalog>("Combat/PlayerHackCatalog");}}
 public PlayerHackDefinition Find(string id){PlayerHackDefinition found=null;foreach(var h in hacks??new PlayerHackDefinition[0])if(h!=null&&h.id==id){if(found!=null)return null;found=h;}return found;}
 public bool ValidEntry(PlayerHackDefinition entry){return entry!=null&&!string.IsNullOrWhiteSpace(entry.id)&&Find(entry.id)==entry;}
 public List<string> ValidateCatalog(){var problems=new List<string>();var ids=new HashSet<string>();foreach(var h in hacks??new PlayerHackDefinition[0]){if(h==null){problems.Add("Entrada vacía.");continue;}if(string.IsNullOrWhiteSpace(h.id))problems.Add("ID vacío: "+h.displayName);else if(!ids.Add(h.id))problems.Add("ID duplicado: "+h.id);if(h.ImplementationType==null)problems.Add("Implementación no válida: "+h.displayName);}return problems;}
 void OnValidate(){
 #if UNITY_EDITOR
 foreach(var h in hacks??new PlayerHackDefinition[0])if(h!=null){var script=h.implementationScript as UnityEditor.MonoScript;var type=script!=null?script.GetClass():null;h.implementationTypeName=type!=null&&!type.IsAbstract&&typeof(CombatHack).IsAssignableFrom(type)?type.FullName:null;h.minimumLevel=Math.Max(1,h.minimumLevel);}
 #endif
 }
}
