using UnityEditor;
using UnityEngine;
public static class EncounterDirectorMenu {
 [MenuItem("Editor/Director de encuentros",false,2012)]static void Open(){Selection.activeObject=Resources.Load<EnemyEncounterSettings>("Exploration/EnemyEncounterSettings");EditorGUIUtility.PingObject(Selection.activeObject);}
}
