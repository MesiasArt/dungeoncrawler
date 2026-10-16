#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using UnityEngine;
public static class DemoPhase3MaskComparison {
 [Serializable] class Result {public int pairs=24;public double baselineMs,optimizedMs;public string scope="Alternating old/new mask submissions in the same process; not isolated GPU time";}
 public static IEnumerator Run(DungeonPrototype d){var type=typeof(DungeonPrototype).Assembly.GetType("HelmetCRTLayerMaskBaseline");if(type==null)throw new Exception("Original mask snapshot missing");var old=d.RetroView.gameObject.AddComponent(type);var build=(Func<Camera,int,int,RenderTexture>)Delegate.CreateDelegate(typeof(Func<Camera,int,int,RenderTexture>),old,type.GetMethod("Build"));var current=d.RetroView.GetComponent<HelmetCRTLayerMask>();var report=new Result();try{for(int i=0;i<4;i++){build(d.RetroView,Screen.width,Screen.height);current.Build(d.RetroView,Screen.width,Screen.height);yield return new WaitForEndOfFrame();}for(int i=0;i<report.pairs;i++){yield return new WaitForEndOfFrame();for(int order=0;order<2;order++){bool baseline=(i+order)%2==0;var timer=System.Diagnostics.Stopwatch.StartNew();if(baseline)build(d.RetroView,Screen.width,Screen.height);else current.Build(d.RetroView,Screen.width,Screen.height);timer.Stop();if(baseline)report.baselineMs+=timer.Elapsed.TotalMilliseconds;else report.optimizedMs+=timer.Elapsed.TotalMilliseconds;}}report.baselineMs/=report.pairs;report.optimizedMs/=report.pairs;File.WriteAllText("phase3-mask-paired.json",JsonUtility.ToJson(report,true));}finally{UnityEngine.Object.Destroy(old);}}
}
#endif
