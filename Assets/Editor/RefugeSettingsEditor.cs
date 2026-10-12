using UnityEngine;
using UnityEditor;
public static class RefugeSettingsMenu {
 [MenuItem("Editor/Zona 0 — Refugio",false,2006)] static void Open(){Selection.activeObject=RefugeSettings.Current;EditorGUIUtility.PingObject(Selection.activeObject);}
}
