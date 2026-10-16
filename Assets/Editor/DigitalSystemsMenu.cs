using UnityEngine;using UnityEditor;
public static class DigitalSystemsMenu {
 [MenuItem("Editor/System Messages/Ajustes generales")]static void Settings(){Selection.activeObject=SystemMessageSettings.Current;}
 [MenuItem("Editor/System Messages/Demostración independiente")]static void Demo(){if(EditorApplication.isPlaying){Debug.LogWarning("Sal de Play Mode antes de abrir la demo.");return;}if(!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/DigitalSystemsDemo.unity");}
}
