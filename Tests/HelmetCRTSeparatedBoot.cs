using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
[InitializeOnLoad] public static class HelmetCRTSeparatedBoot {
 static double deadline;
 static HelmetCRTSeparatedBoot(){deadline=EditorApplication.timeSinceStartup+100;EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;}
 static void Tick(){if(EditorApplication.timeSinceStartup>deadline)EditorApplication.Exit(2);if(EditorApplication.isPlaying)EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Repaint();}
 static void State(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode||!SessionState.GetBool("CRTSeparated.Done",false))return;var camera=GameObject.Find("Separated camera").GetComponent<Camera>();bool ok=SessionState.GetBool("CRTSeparated.Success",false)&&camera.targetTexture==null;File.AppendAllText(Path.Combine(Application.dataPath,"../separated-results.txt"),"; Play/Stop camera restored="+(camera.targetTexture==null));EditorApplication.Exit(ok?0:1);}
 public static void Run(){
  Application.runInBackground=true;SessionState.SetBool("CRTSeparated.Done",false);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var camera=new GameObject("Separated camera").AddComponent<Camera>();camera.backgroundColor=new Color(.2f,.35f,.5f);camera.clearFlags=CameraClearFlags.SolidColor;
  var settings=ScriptableObject.CreateInstance<HelmetCRTSettings>();settings.separateLayerIntensity=true;settings.crtEnabled=true;settings.affectUI=true;settings.backgroundIntensity=1;settings.enemyIntensity=.35f;settings.uiIntensity=0;settings.automaticGlitches=false;settings.scanlines=1;settings.artifacts=1;settings.chromaticFringe=1;settings.curvature=false;settings.cornerMask=false;settings.brightness=1;settings.bufferHeight=340;
  AssetDatabase.CreateAsset(settings,AssetDatabase.GenerateUniqueAssetPath("Assets/TestSettings.asset"));AssetDatabase.SaveAssets();
  var controller=camera.gameObject.AddComponent<HelmetCRTController>();controller.settings=settings;
  var enemy=GameObject.CreatePrimitive(PrimitiveType.Cube);enemy.name="Off-center enemy";enemy.transform.position=new Vector3(1,-.6f,4);enemy.transform.localScale=Vector3.one*.8f;enemy.AddComponent<EnemyCombatStats>();var enemyMaterial=new Material(Shader.Find("Unlit/Color"));enemyMaterial.color=new Color(1,.2f,.05f);enemy.GetComponent<Renderer>().sharedMaterial=enemyMaterial;
  new GameObject("Native GUI controls").AddComponent<HelmetCRTSeparatedValidation>().controller=controller;
  EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Show();EditorApplication.isPlaying=true;
 }
}
