using System;
using UnityEngine;

public partial class DungeonPrototype {
 bool sceneInitializationCompleted;
 internal bool SceneTransitionReady {get{var composer=GetComponent<RetroDungeonWallComposer>();return sceneInitializationCompleted&&view!=null&&view.isActiveAndEnabled&&(composer==null||composer.PresentationReady);}}
 internal Action CaptureSceneTransitionRecovery(){
  bool paused=pauseOpen,enteringCity=cityActive&&!cityDataOnly;var camera=view;
  int mask=camera!=null?camera.cullingMask:0;var clear=camera!=null?camera.clearFlags:CameraClearFlags.SolidColor;var background=camera!=null?camera.backgroundColor:Color.black;
  return ()=>{
   if(this==null)return;
   // Entering the illustration marks cityActive before the scene is actually loaded.
   if(enteringCity){cityActive=false;cityProgress.inCity=false;worldDialogue.Cancel();}
   if(camera!=null){camera.cullingMask=mask;camera.clearFlags=clear;camera.backgroundColor=background;}
   SetPause(paused&&!enteringCity);Time.timeScale=pauseOpen?0:(previousTimeScale>0?previousTimeScale:1);AudioListener.pause=cityDataOnly?false:pauseOpen;
   ClearMovementInput();
  };
 }
}
