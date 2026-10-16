using System;using System.Collections.Generic;using UnityEngine;
public enum DungeonBossAdapterKind { SignalsOnly, MakongLegacy }
[Serializable]public sealed class DungeonBossPhase {public string id=Guid.NewGuid().ToString("N");[Range(0,1)]public float maximumHealthFraction=.5f;public DungeonInteractionSettings onEntered;}
[Serializable]public sealed class DungeonBossEventRecord {public string id,profileId,sourceId;}
[CreateAssetMenu(menuName="Virtual Dungeon/Jefes/Fases y eventos")]
public sealed class DungeonBossProfile:ScriptableObject {
 public string id=Guid.NewGuid().ToString("N");public DungeonBossAdapterKind adapter;
 public DungeonBossPhase[] phases=new DungeonBossPhase[0];public DungeonInteractionSettings onVictory;
 public List<string> Validate(string species){var errors=new List<string>();if(string.IsNullOrEmpty(id))errors.Add("Perfil de jefe sin ID");if(adapter==DungeonBossAdapterKind.MakongLegacy&&species!="Makong")errors.Add("Makong Legacy requiere la especie Makong");var ids=new HashSet<string>();foreach(var phase in phases??new DungeonBossPhase[0]){if(phase==null||string.IsNullOrEmpty(phase.id)||!ids.Add(phase.id)){errors.Add("ID de fase vacío/duplicado");continue;}if(float.IsNaN(phase.maximumHealthFraction)||phase.maximumHealthFraction<0||phase.maximumHealthFraction>1)errors.Add("Umbral HP inválido");}return errors;}
}
// This adapter preserves MakongTactics and its animation/attack controller verbatim.
// Profiles emit event signals; they never replace or schedule combat attacks.
public static class MakongLegacyBossAdapter {
 public static bool Accepts(DungeonBossProfile profile,EnemyCombatStats stats){return profile!=null&&stats!=null&&(profile.adapter!=DungeonBossAdapterKind.MakongLegacy||stats.speciesId=="Makong");}
 public static bool PhaseReached(DungeonBossPhase phase,int hp,int maximumHP){return phase!=null&&maximumHP>0&&hp/(double)maximumHP<=phase.maximumHealthFraction;}
}
