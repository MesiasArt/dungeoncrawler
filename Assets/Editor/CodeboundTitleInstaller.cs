using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine.SceneManagement;using System.Collections.Generic;
public static class CodeboundTitleInstaller {
 [MenuItem("Editor/Codebound/Preparar escena de inicio")]
 public static string Install(){const string path="Assets/Scenes/CodeboundTitle.unity";if(!System.IO.File.Exists(path)){var old=SceneManager.GetActiveScene();var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);var camera=new GameObject("Title camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.cullingMask=0;camera.gameObject.AddComponent<AudioListener>();var screen=new GameObject("Codebound title screen").AddComponent<CodeboundTitleScreen>();screen.background=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UI/Codebound Title Screen.png");screen.logo=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UI/Codebound Title Screen Logo.png");EditorSceneManager.SaveScene(scene,path);EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(old);}
  var scenes=new List<EditorBuildSettingsScene>{new EditorBuildSettingsScene(path,true)};foreach(var s in EditorBuildSettings.scenes)if(s.path!=path)scenes.Add(s);EditorBuildSettings.scenes=scenes.ToArray();AssetDatabase.SaveAssets();return path;
 }
}
