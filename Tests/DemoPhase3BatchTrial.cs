#if UNITY_EDITOR && DEMO_PHASE3_RENDER_ELISION
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Experimental only: never installed in production by normal gameplay.
public class DemoPhase3BatchTrial:MonoBehaviour {
 readonly HashSet<Mesh> owned=new HashSet<Mesh>();
 [Serializable] class Result {public int candidates,batched,pixels,changedPixels,maximumChannelDifference,createdMeshes;public double meanChannelDifference,combineMilliseconds;public long meshBytes;}
 static Texture2D Read(RenderTexture rt){var old=RenderTexture.active;try{RenderTexture.active=rt;var t=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false,true);t.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);t.Apply();return t;}finally{RenderTexture.active=old;}}
 public static void Run(DungeonPrototype dungeon){
  var objects=new List<GameObject>();foreach(Transform child in dungeon.transform){var r=child.GetComponent<MeshRenderer>();if(r!=null&&!r.isPartOfStaticBatch&&(child.name=="Wall"||child.name=="Floor"||child.name=="Ceiling"||child.name=="Void ceiling")&&r.sharedMaterial!=null&&r.sharedMaterial.shader.name=="Dungeon/Metal Toon")objects.Add(child.gameObject);}
  var result=new Result{candidates=objects.Count};var owner=dungeon.gameObject.AddComponent<DemoPhase3BatchTrial>();var originals=new HashSet<Mesh>(objects.Select(o=>o.GetComponent<MeshFilter>().sharedMesh));
  var a=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);var b=new RenderTexture(a.descriptor);a.Create();b.Create();Texture2D before=null,after=null;
  try{var request=new UniversalRenderPipeline.SingleCameraRequest{destination=a};RenderPipeline.SubmitRenderRequest(dungeon.RetroView,request);before=Read(a);
   var watch=System.Diagnostics.Stopwatch.StartNew();StaticBatchingUtility.Combine(objects.ToArray(),dungeon.gameObject);watch.Stop();result.combineMilliseconds=watch.Elapsed.TotalMilliseconds;
   foreach(var item in objects){if(item.GetComponent<MeshRenderer>().isPartOfStaticBatch)result.batched++;var mesh=item.GetComponent<MeshFilter>().sharedMesh;if(!originals.Contains(mesh))owner.owned.Add(mesh);}
   result.createdMeshes=owner.owned.Count;result.meshBytes=owner.owned.Sum(m=>UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(m));request.destination=b;RenderPipeline.SubmitRenderRequest(dungeon.RetroView,request);after=Read(b);var left=before.GetPixels32();var right=after.GetPixels32();long total=0;result.pixels=left.Length;
   for(int i=0;i<left.Length;i++){if(!left[i].Equals(right[i]))result.changedPixels++;int dr=Math.Abs(left[i].r-right[i].r),dg=Math.Abs(left[i].g-right[i].g),db=Math.Abs(left[i].b-right[i].b);result.maximumChannelDifference=Math.Max(result.maximumChannelDifference,Math.Max(dr,Math.Max(dg,db)));total+=dr+dg+db;}result.meanChannelDifference=total/(double)(left.Length*3);
   File.WriteAllText("phase3-batch-trial.json",JsonUtility.ToJson(result,true));File.WriteAllBytes("phase3-batch-before.png",before.EncodeToPNG());File.WriteAllBytes("phase3-batch-after.png",after.EncodeToPNG());
  }finally{if(before!=null)Destroy(before);if(after!=null)Destroy(after);a.Release();b.Release();Destroy(a);Destroy(b);}
 }
 void OnDestroy(){foreach(var mesh in owned)if(mesh!=null)Destroy(mesh);}
}
#endif
