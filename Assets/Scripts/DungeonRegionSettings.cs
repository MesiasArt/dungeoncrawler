using System;
using System.Collections.Generic;
using UnityEngine;
public enum PlacedEnemyCategory { Normal, Elite, Boss }
public enum EncounterTrigger { OnDungeonStart, InteractiveEvent }
[Serializable] public class DungeonRegion {
 public string id=Guid.NewGuid().ToString("N"),displayName="Nueva zona";
 public Color editorColor=Color.cyan;
 public List<Vector2Int> cells=new List<Vector2Int>();
 public EnemyZoneSettings enemies;
 public bool spawnsEnabled=true,safeArea;
 public bool CanSpawn {get{return spawnsEnabled&&!safeArea&&enemies!=null&&enemies.spawnsEnabled;}}
}
[Serializable] public class PlacedEnemyDefinition {
 public string id=Guid.NewGuid().ToString("N"),speciesId="Tapaku";
 public Vector2Int cell;
 [Tooltip("Encuentro manual por interacción dentro de un refugio; no permite población aleatoria.")] public bool allowInSafeArea;
 public bool enabled=true,randomLevel=true,respawn=true,unique,countsTowardLimit=true;
 [Min(1)] public int fixedLevel=1;
 public PlacedEnemyCategory category;
 public EncounterTrigger trigger;
 public bool holdPosition;public InventoryItemRecord[] guaranteedDrops=new InventoryItemRecord[0];
 public bool CanRespawn {get{return respawn&&!unique&&category!=PlacedEnemyCategory.Boss;}}
}
[CreateAssetMenu(menuName="Virtual Dungeon/Regiones y encuentros")]
public sealed class DungeonRegionSettings:ScriptableObject {
 public DungeonRegion[] regions=new DungeonRegion[0];
 public PlacedEnemyDefinition[] placedEnemies=new PlacedEnemyDefinition[0];
 [Tooltip("Conserva las siete colocaciones originales. Desactiva para usar solo colocaciones manuales y población por zona.")] public bool legacyPlacements=true;
 [Min(0)] public int initialRandomPerZone=2;
 public static DungeonRegionSettings Current {get{return Resources.Load<DungeonRegionSettings>("Exploration/Zones/DungeonRegions")??Resources.Load<DungeonRegionSettings>("Exploration/DungeonRegions");}}
 public DungeonRegion At(Vector2Int cell){DungeonRegion found=null;foreach(var region in regions)if(region!=null&&region.cells.Contains(cell)){if(found!=null)return null;found=region;}return found;}
 public List<string> Validate(string[] map) {
  var errors=new List<string>();var ids=new HashSet<string>();var assigned=new Dictionary<Vector2Int,string>();
  foreach(var r in regions){if(r==null)continue;if(string.IsNullOrEmpty(r.id)||!ids.Add(r.id))errors.Add("ID de zona vacío o duplicado");if(r.enemies==null&&!r.safeArea)errors.Add(r.displayName+": falta configuración");
   var local=new HashSet<Vector2Int>();foreach(var cell in r.cells){if(!local.Add(cell))errors.Add(r.displayName+": casilla duplicada "+cell);if(cell.y<0||cell.y>=map.Length||cell.x<0||cell.x>=map[0].Length||map[cell.y][cell.x]!='.')errors.Add(r.displayName+": casilla inválida "+cell);if(assigned.ContainsKey(cell))errors.Add("Solapamiento en "+cell);else assigned[cell]=r.id;}
   if(r.enemies!=null){var speciesIds=new HashSet<string>();foreach(var row in r.enemies.species){if(row==null)continue;if(!speciesIds.Add(row.speciesId))errors.Add(r.displayName+": especie duplicada "+row.speciesId);if(EnemyCharacterCatalog.Current.FindId(row.speciesId)==null)errors.Add(r.displayName+": especie desconocida "+row.speciesId);if(row.weight<0||float.IsNaN(row.weight)||float.IsInfinity(row.weight))errors.Add(r.displayName+": peso inválido");}}
   if(r.CanSpawn&&r.enemies.LevelValues().Length==0)errors.Add(r.displayName+": niveles vacíos");if(r.CanSpawn&&r.enemies.SelectKind(.5)<0)errors.Add(r.displayName+": no hay especies aleatorias válidas");
  }
  for(int y=0;y<map.Length;y++)for(int x=0;x<map[y].Length;x++)if(map[y][x]=='.'&&!assigned.ContainsKey(new Vector2Int(x,y)))errors.Add("Sin zona: "+x+","+y);
  ids.Clear();var locations=new HashSet<Vector2Int>();foreach(var e in placedEnemies){if(e==null)continue;if(string.IsNullOrEmpty(e.id)||!ids.Add(e.id))errors.Add("ID de colocación vacío/duplicado");if(!locations.Add(e.cell)&&e.trigger==EncounterTrigger.OnDungeonStart)errors.Add("Colocaciones en misma casilla "+e.cell);var r=At(e.cell);if(r==null)errors.Add(e.id+": colocación sin zona válida");if(EnemyCharacterCatalog.Current.FindId(e.speciesId)==null)errors.Add(e.id+": especie desconocida");if(e.speciesId=="Mimic"&&e.trigger!=EncounterTrigger.InteractiveEvent)errors.Add(e.id+": Mimic necesita evento interactivo");if(r!=null&&!r.CanSpawn&&e.enabled&&!e.allowInSafeArea)errors.Add(e.id+": colocación en zona segura/desactivada");}
  return errors;
 }
}
