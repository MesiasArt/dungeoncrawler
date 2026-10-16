#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad] public static class DemoPhase2Boot {
 static double deadline;
 static DemoPhase2Boot(){deadline=EditorApplication.timeSinceStartup+600;EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;}
 static void Tick(){if(EditorApplication.timeSinceStartup>deadline){File.AppendAllText("phase2-results.txt","\nFAIL: editor timeout");EditorApplication.Exit(2);}if(EditorApplication.isPlaying)EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Repaint();}
 static void State(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredEditMode&&SessionState.GetBool("DemoPhase2.Done",false)){bool success=SessionState.GetBool("DemoPhase2.Success",false);if(success&&File.Exists("phase2-build-after-tests"))try{DemoPhase2ReleaseBuild.Run();}catch(System.Exception e){File.AppendAllText("phase2-results.txt","FAIL: "+e+"\n");success=false;}EditorApplication.Exit(success?0:1);}}
 public static void Run(){Application.runInBackground=true;SessionState.SetBool("DemoPhase2.Done",false);File.WriteAllText("phase2-results.txt","");
  #if DEMO_PHASE3_CACHE
  File.WriteAllText("phase3-edit-results.txt","");File.AppendAllText("phase3-edit-results.txt",DemoPhase3EditValidation.Run()+"\n");
  #endif
  #if DEMO_PHASE2_TRANSITION
  Directory.CreateDirectory("Assets/DemoPhase2Fixtures");
  foreach(bool delayed in new[]{false,true}){var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("Prevent default dungeon bootstrap").AddComponent<CodeboundTitleScreen>().enabled=false;new GameObject("Fixture Camera").AddComponent<Camera>();new GameObject("Fixture Light").AddComponent<Light>().type=LightType.Directional;if(delayed)new GameObject("Delayed readiness").AddComponent<DemoPhase2DelayedDungeon>();string path="Assets/DemoPhase2Fixtures/"+(delayed?"Delayed":"MissingReady")+".unity";EditorSceneManager.SaveScene(scene,path);var scenes=new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);scenes.RemoveAll(s=>s.path==path);scenes.Add(new EditorBuildSettingsScene(path,true));EditorBuildSettings.scenes=scenes.ToArray();}
  var pixel=EditorSceneManager.OpenScene("Assets/Scenes/RetroDungeonExperiment.unity");UnityEngine.Object.FindAnyObjectByType<RetroDungeonWallComposer>().enabled=false;string unready="Assets/DemoPhase2Fixtures/PixelUnready.unity";EditorSceneManager.SaveScene(pixel,unready);var fixtureScenes=new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);fixtureScenes.RemoveAll(s=>s.path==unready);fixtureScenes.Add(new EditorBuildSettingsScene(unready,true));EditorBuildSettings.scenes=fixtureScenes.ToArray();
  #endif
  EditorSceneManager.OpenScene("Assets/Scenes/RetroDungeonExperiment.unity");EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Show();EditorApplication.isPlaying=true;}
}
#endif
