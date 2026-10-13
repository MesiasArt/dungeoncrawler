using UnityEditor;
using UnityEngine;
public static class HelmetCRTEditor {
 [MenuItem("Editor/Efectos del casco CRT")]
 public static void Open(){Selection.activeObject=HelmetCRTSettings.Current;EditorGUIUtility.PingObject(Selection.activeObject);}
}
