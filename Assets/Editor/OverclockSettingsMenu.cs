using UnityEditor;
public static class OverclockSettingsMenu {
 [MenuItem("Editor/OVERCLOCK")]
 static void Open(){Selection.activeObject=OverclockSettings.Current;EditorGUIUtility.PingObject(Selection.activeObject);}
}
