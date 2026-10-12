using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public class SpeciesSpawnWeight { public string speciesId; [Min(0)] public float weight=1; }
[CreateAssetMenu(menuName="Virtual Dungeon/Zona de aparicion")]
public sealed class EnemyZoneSettings:ScriptableObject {
 public bool spawnsEnabled=true;
 public bool useIndividualLevels;public int[] individualLevels=new int[0];
 public string zoneId="zone-1";
 [Min(1)] public int minimumLevel=1,maximumLevel=5;
 [Range(0,1)] public float rarityFactor=.65f;
 public SpeciesSpawnWeight[] species=new SpeciesSpawnWeight[0];
 [Min(1)] public int maximumEnemies=9;
 [Min(0)] public float respawnSeconds=30;
 public static EnemyZoneSettings Current {get{return Resources.Load<EnemyZoneSettings>("Exploration/Zones/01-Zona1/Enemigos")??Resources.Load<EnemyZoneSettings>("Exploration/Zone1");}}
 public int[] LevelValues(){if(useIndividualLevels){var set=new SortedSet<int>();foreach(int lv in individualLevels??new int[0])if(lv>=1&&lv<=10000)set.Add(lv);var values=new int[set.Count];set.CopyTo(values);return values;}int low=Mathf.Clamp(minimumLevel,1,10000),high=Mathf.Clamp(maximumLevel,low,10000);var result=new int[high-low+1];for(int i=0;i<result.Length;i++)result[i]=low+i;return result;}
 public double[] LevelProbabilities(){var values=LevelValues();var result=new double[values.Length];if(values.Length==0)return result;double total=0,factor=Math.Max(0,Math.Min(1,rarityFactor));for(int i=0;i<values.Length;i++){result[i]=Math.Pow(factor,values[i]-values[0]);total+=result[i];}for(int i=0;i<result.Length;i++)result[i]/=total;return result;}
 public int SelectLevel(double roll){var values=LevelValues();int index=Select(LevelProbabilities(),roll);return index<0?-1:values[index];}
 public static int Select(double[] weights,double roll){double total=0;foreach(double w in weights)total+=w; if(total<=0)return -1;
  double value=Math.Max(0,Math.Min(.999999999999,roll))*total;for(int i=0;i<weights.Length;i++){value-=weights[i];if(value<0)return i;}return weights.Length-1;}
 public bool AllowsKind(int kind){if(kind<0||kind>=EnemyRoster.Mimic)return false;foreach(var e in species)if(e!=null&&e.speciesId==EnemyRoster.ResourceIds[kind]&&e.weight>0)return true;return false;}
 public int SelectKind(double roll,Predicate<int> allowed=null) {
  if(!spawnsEnabled)return -1;
  var kinds=new List<int>();var weights=new List<double>();
  foreach(var entry in species??new SpeciesSpawnWeight[0]) {
   if(entry==null||EnemyCharacterCatalog.Current!=null&&EnemyCharacterCatalog.Current.FindId(entry.speciesId)!=null&&EnemyCharacterCatalog.Current.FindId(entry.speciesId).reservedForEvents||entry.weight<=0||float.IsNaN(entry.weight)||float.IsInfinity(entry.weight))continue;
   for(int k=0;k<EnemyRoster.Mimic;k++)if(EnemyRoster.ResourceIds[k]==entry.speciesId&&(allowed==null||allowed(k))){kinds.Add(k);weights.Add(entry.weight);break;}
  }
  int index=Select(weights.ToArray(),roll);return index<0?-1:kinds[index];
 }
 void OnValidate(){minimumLevel=Mathf.Clamp(minimumLevel,1,10000);maximumLevel=Mathf.Clamp(maximumLevel,minimumLevel,10000);maximumEnemies=Mathf.Max(1,maximumEnemies);}
}
