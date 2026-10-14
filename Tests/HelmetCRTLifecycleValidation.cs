using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
public static class HelmetCRTLifecycleValidation {
 public static void RunChecks(){
  const BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
  var root=new GameObject("CRT lifecycle regression");root.AddComponent<Camera>();
  try {
   var controller=root.AddComponent<HelmetCRTController>();var type=typeof(HelmetCRTController);
   var random=type.GetField("random",flags);random.SetValue(controller,null);
   var between=type.GetMethod("Between",flags);
   float value=(float)between.Invoke(controller,new object[]{.1f,.3f});
   if(value<.1f||value>.3f||random.GetValue(controller)==null)throw new Exception("Random state was not rebuilt after reload");
   random.SetValue(controller,null);type.GetMethod("Update",flags).Invoke(controller,null);
   if(random.GetValue(controller)!=null||controller.EffectEnabled)throw new Exception("CRT ran outside Play Mode");
   type.GetField("remaining",flags).SetValue(controller,1f);type.GetMethod("OnDisable",flags).Invoke(controller,null);
   if(controller.GlitchStrength!=0)throw new Exception("Glitch state survived disabling");
   var frame=root.GetComponent<HelmetCRTWholeFrame>()??root.AddComponent<HelmetCRTWholeFrame>();var frameType=typeof(HelmetCRTWholeFrame);
   frameType.GetMethod("EnsureBuffers",flags).Invoke(frame,new object[]{32,32,240});
   frameType.GetMethod("OnDisable",flags).Invoke(frame,null);frameType.GetMethod("OnDisable",flags).Invoke(frame,null);
   foreach(string name in new[]{"source","a","b","output","layeredOutput"})if(frameType.GetField(name,flags).GetValue(frame)!=null)throw new Exception("Leaked buffer: "+name);
   var loop=(IEnumerator)frameType.GetMethod("RenderLoop",flags).Invoke(frame,null);
   if(loop.MoveNext())throw new Exception("Frame coroutine continued outside Play Mode");
  }finally {UnityEngine.Object.DestroyImmediate(root);}
 }
}
