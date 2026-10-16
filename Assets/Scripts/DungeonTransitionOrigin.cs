using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Owns only references to the still-loaded origin. No disk save or personal slot is used.
internal sealed class DungeonTransitionOrigin {
 readonly Scene scene;
 readonly Dictionary<GameObject,bool> roots=new Dictionary<GameObject,bool>();
 readonly List<Action> controls=new List<Action>();
 readonly Action audio;
 readonly bool fog;readonly FogMode fogMode;readonly Color fogColor,ambient;
 readonly float fogDensity,fogStart,fogEnd;
 readonly float[] retro;
 readonly Vector4 retroDistances,retroBattle;readonly Color retroColor;
 readonly string[] retroKeys={"_RetroFogEnabled","_RetroFogResponse","_RetroFogQuantized","_RetroFogLevels","_RetroFogSoftness"};
 public DungeonTransitionOrigin(){
  scene=SceneManager.GetActiveScene();foreach(var root in scene.GetRootGameObjects()){roots[root]=root.activeSelf;foreach(var player in root.GetComponentsInChildren<DungeonPrototype>(true))controls.Add(player.CaptureSceneTransitionRecovery());}
  audio=DungeonAudio.CaptureTransitionRecovery();fog=RenderSettings.fog;fogMode=RenderSettings.fogMode;fogColor=RenderSettings.fogColor;fogDensity=RenderSettings.fogDensity;fogStart=RenderSettings.fogStartDistance;fogEnd=RenderSettings.fogEndDistance;ambient=RenderSettings.ambientLight;
  retro=new float[retroKeys.Length];for(int i=0;i<retro.Length;i++)retro[i]=Shader.GetGlobalFloat(retroKeys[i]);retroDistances=Shader.GetGlobalVector("_RetroFogDistances");retroBattle=Shader.GetGlobalVector("_RetroBattleFloorRegion");retroColor=Shader.GetGlobalColor("_RetroFogColor");
 }
 public void Suspend(){foreach(var root in roots.Keys)if(root!=null)root.SetActive(false);}
 public void DestinationLoaded(Scene destination){
  // The existing sceneLoaded launcher can create the default dungeon in the active origin.
  if(scene.IsValid()&&scene.isLoaded)foreach(var root in scene.GetRootGameObjects())if(!roots.ContainsKey(root))SceneManager.MoveGameObjectToScene(root,destination);
  SceneManager.SetActiveScene(destination);
 }
 public void Resume(){if(!scene.IsValid()||!scene.isLoaded)throw new InvalidOperationException("La escena de origen ya no está disponible");SceneManager.SetActiveScene(scene);foreach(var root in roots)if(root.Key!=null)root.Key.SetActive(root.Value);foreach(var restore in controls)restore();RestoreEnvironment();audio();}
 public void RestoreEnvironment(){if(!scene.IsValid()||!scene.isLoaded)return;RenderSettings.fog=fog;RenderSettings.fogMode=fogMode;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;RenderSettings.fogStartDistance=fogStart;RenderSettings.fogEndDistance=fogEnd;RenderSettings.ambientLight=ambient;for(int i=0;i<retro.Length;i++)Shader.SetGlobalFloat(retroKeys[i],retro[i]);Shader.SetGlobalVector("_RetroFogDistances",retroDistances);Shader.SetGlobalVector("_RetroBattleFloorRegion",retroBattle);Shader.SetGlobalColor("_RetroFogColor",retroColor);}
 public AsyncOperation Commit(){
  #if UNITY_EDITOR
  var destination=UnityEngine.Object.FindAnyObjectByType<RetroDungeonWallComposer>();
  if(destination!=null)foreach(var root in roots.Keys)if(root!=null)foreach(var source in root.GetComponentsInChildren<RetroDungeonWallComposer>(true))source.HandOffEditorSavePath(destination);
  #endif
  return scene.IsValid()&&scene.isLoaded?SceneManager.UnloadSceneAsync(scene):null;
 }
 public void HideDestination(Scene destination){
  if(!destination.IsValid()||!destination.isLoaded)return;foreach(var root in destination.GetRootGameObjects())root.SetActive(false);
  if(scene.IsValid()&&scene.isLoaded)SceneManager.SetActiveScene(scene);
 }
 public AsyncOperation Discard(Scene destination){
  if(!destination.IsValid()||!destination.isLoaded)return null;HideDestination(destination);
  var unload=SceneManager.UnloadSceneAsync(destination);if(unload!=null)unload.completed+=ignored=>RestoreEnvironment();return unload;
 }
}
