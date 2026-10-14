using UnityEditor;using UnityEngine;
// Asset GUID is copied once, never recomputed when a published map is moved or renamed.
public sealed class DungeonMapIdEditor:AssetPostprocessor {
 [MenuItem("Assets/Codebound/Dar ID nuevo a copia de mapa",true)] static bool CanAssignCopyId(){var map=Selection.activeObject as DungeonMapSettings;if(map==null||EditorApplication.isPlaying||map.mapId=="map-"+AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(map)))return false;foreach(var guid in AssetDatabase.FindAssets("t:DungeonMapSettings")){var other=AssetDatabase.LoadAssetAtPath<DungeonMapSettings>(AssetDatabase.GUIDToAssetPath(guid));if(other!=map&&other.mapId==map.mapId)return true;}return false;}
 [MenuItem("Assets/Codebound/Dar ID nuevo a copia de mapa")] static void AssignCopyId(){var map=Selection.activeObject as DungeonMapSettings;if(!CanAssignCopyId())return;Undo.RecordObject(map,"Identificar copia independiente del mapa");map.mapId="map-"+AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(map));EditorUtility.SetDirty(map);AssetDatabase.SaveAssetIfDirty(map);}
 static void OnPostprocessAllAssets(string[] imported,string[] deleted,string[] moved,string[] previous){
  foreach(var path in imported){if(!path.EndsWith(".asset"))continue;var map=AssetDatabase.LoadAssetAtPath<DungeonMapSettings>(path);if(map!=null&&string.IsNullOrEmpty(map.mapId)){map.mapId="map-"+AssetDatabase.AssetPathToGUID(path);EditorUtility.SetDirty(map);EditorApplication.delayCall+=()=>{if(map!=null&&!EditorApplication.isPlaying)AssetDatabase.SaveAssetIfDirty(map);};}}
 }
}
