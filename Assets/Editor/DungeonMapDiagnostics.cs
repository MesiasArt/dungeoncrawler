using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor only: topology ignores locks, keys and actors; it never changes gameplay.
public static class DungeonMapDiagnostics {
 public sealed class Report { public List<string> errors=new List<string>(); public int floors,reachable; public string summary; }
 public static HashSet<Vector2Int> Reachable(DungeonMapSettings map,Vector2Int start) {
  var visited=new HashSet<Vector2Int>();var queue=new Queue<Vector2Int>();
  if(!map.IsFloor(start))return visited;visited.Add(start);queue.Enqueue(start);
  while(queue.Count>0){var cell=queue.Dequeue();foreach(var d in Directions){var next=cell+d;if(map.IsFloor(next)&&visited.Add(next))queue.Enqueue(next);}}
  return visited;
 }
 static readonly Vector2Int[] Directions={Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right};
 public static Report Analyze(DungeonMapSettings map) {
  var result=new Report();if(map==null||!map.Valid){result.errors.Add("Plano ausente o geometría inválida");return result;}
  if(!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(map))){if(string.IsNullOrEmpty(map.mapId))result.errors.Add("Mapa sin mapId estable");else foreach(var guid in AssetDatabase.FindAssets("t:DungeonMapSettings")){var other=AssetDatabase.LoadAssetAtPath<DungeonMapSettings>(AssetDatabase.GUIDToAssetPath(guid));if(other!=map&&other.mapId==map.mapId)result.errors.Add("mapId duplicado: "+map.mapId);}}
  result.errors.AddRange(DungeonInteractionValidation.Validate(map));
  result.errors.AddRange(DrpgPhase6Diagnostics.Validate(map));
  var regions=map.regions!=null?map.regions:map.includeLegacyObjects?DungeonRegionSettings.Current:null;
  var editor=Editor.CreateEditor(map) as DungeonMapSettingsEditor;
  try {if(editor!=null)result.errors.AddRange(editor.ValidateObjects());}finally{if(editor!=null)UnityEngine.Object.DestroyImmediate(editor);}
  if(regions!=null)result.errors.AddRange(regions.Validate(map.rows));
  var start=map.includeLegacyObjects&&RefugeSettings.Current!=null?RefugeSettings.Current.startCell:map.startCell;
  var visited=Reachable(map,start);result.reachable=visited.Count;
  if(!map.IsFloor(start))result.errors.Add("Inicio fuera del suelo: "+start);
  for(int y=0;y<map.rows.Length;y++)for(int x=0;x<map.rows[y].Length;x++)if(map.rows[y][x]=='.'){result.floors++;var c=new Vector2Int(x,y);if(!visited.Contains(c))result.errors.Add("Suelo desconectado del inicio: "+c);}
  if(map.entities!=null)foreach(var e in map.entities.objects){if(e==null)continue;
   bool accessible=e.kind==DungeonEntityKind.Teleport?visited.Contains(e.cell):false;
   if(e.kind!=DungeonEntityKind.Teleport)foreach(var d in Directions)if(visited.Contains(e.cell+d))accessible=true;
   if(!accessible)result.errors.Add(e.id+": sin acceso geométrico para interactuar");
   if(!string.IsNullOrEmpty(e.guardPlacementId)&&(regions==null||!Array.Exists(regions.placedEnemies,p=>p!=null&&p.enabled&&p.id==e.guardPlacementId)))result.errors.Add(e.id+": guardián inexistente o desactivado "+e.guardPlacementId);
   if(e.kind==DungeonEntityKind.Door&&!string.IsNullOrEmpty(e.destinationRegionId)){
    var target=e.destinationMap!=null?e.destinationMap:map;var targetRegions=target.regions!=null?target.regions:target.includeLegacyObjects?DungeonRegionSettings.Current:null;
    if(targetRegions==null||!Array.Exists(targetRegions.regions,r=>r!=null&&r.id==e.destinationRegionId))result.errors.Add(e.id+": zona de destino inexistente "+e.destinationRegionId);
   }
  }
  result.errors=new List<string>(new HashSet<string>(result.errors));result.errors.Sort(StringComparer.Ordinal);
  result.summary="Suelo accesible: "+result.reachable+" / "+result.floors+". Incidencias: "+result.errors.Count+". BFS geométrico: no simula llaves, puertas cerradas ni actores.";return result;
 }
}
public sealed class DungeonMapDiagnosticsWindow:EditorWindow {
 DungeonMapSettings map;DungeonMapDiagnostics.Report report;Vector2 scroll;
 [MenuItem("Editor/Diagnóstico DRPG")]
 public static void Open(){GetWindow<DungeonMapDiagnosticsWindow>("Diagnóstico DRPG").Show();}
 void OnGUI(){map=(DungeonMapSettings)EditorGUILayout.ObjectField("Plano",map!=null?map:DungeonMapSettings.Current,typeof(DungeonMapSettings),false);
  if(GUILayout.Button("Analizar sin modificar datos"))report=DungeonMapDiagnostics.Analyze(map);
  if(report==null)return;EditorGUILayout.HelpBox(report.summary,MessageType.Info);scroll=EditorGUILayout.BeginScrollView(scroll);foreach(var error in report.errors)EditorGUILayout.HelpBox(error,MessageType.Warning);EditorGUILayout.EndScrollView();
 }
}
