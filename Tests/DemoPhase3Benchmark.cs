#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using Unity.Profiling;
using UnityEngine.Rendering.Universal;

public sealed class DemoPhase3Benchmark:MonoBehaviour {
 static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
 [Serializable] public class Metric {public string name,unit;public double mean,p50,p95,allocatedBytes=-1,allocations;public long operations,reads,retainedNativeBytes;public bool available=true;}
 [Serializable] public class Report {public string label,unity,gpu,cpu,scope="Isolated Unity Editor; not Release FPS",allocationMethod="GC.Alloc Recorder sampleBlockCount filtered to current thread; frame bytes from GC Allocated In Frame";public int width,height,renderers,enemies;public long fixtureBytes,workingSet;public List<Metric> metrics=new List<Metric>();}
 readonly Report report=new Report();Action<bool,string> check;Action restoreResolution;
 Action<float,float> draw;bool guiMeasure;int guiCount;double guiMilliseconds;long guiAllocated,guiReads;
 public static IEnumerator Run(Action<bool,string> check){var root=new GameObject("Phase 3 controlled measurements");DontDestroyOnLoad(root);var probe=root.AddComponent<DemoPhase3Benchmark>();probe.check=check;int sync=QualitySettings.vSyncCount,cap=Application.targetFrameRate;try{yield return probe.Work();}finally{if(probe.restoreResolution!=null)probe.restoreResolution();QualitySettings.vSyncCount=sync;Application.targetFrameRate=cap;Destroy(root);}}
 static long Reads {get{return (long)typeof(DemoPhase3Counters).GetField("reads").GetValue(null);}}
 Metric Measure(string name,int count,Action action,int warm=20){for(int i=0;i<warm;i++)action();var clock=new System.Diagnostics.Stopwatch();long read=Reads;var alloc=Recorder.Get("GC.Alloc");alloc.FilterToCurrentThread();alloc.enabled=false;alloc.enabled=true;clock.Start();for(int i=0;i<count;i++)action();clock.Stop();alloc.enabled=false;var value=new Metric{name=name,unit="ms/operation",mean=clock.Elapsed.TotalMilliseconds/count,allocations=alloc.sampleBlockCount/(double)count,operations=count,reads=Reads-read};report.metrics.Add(value);return value;}
 void OnGUI(){if(!guiMeasure||Event.current.type!=EventType.Repaint)return;var matrix=GUI.matrix;var alloc=Recorder.Get("GC.Alloc");try{GUI.matrix=Matrix4x4.Scale(new Vector3(Screen.width/1280f,Screen.height/720f,1));var clock=new System.Diagnostics.Stopwatch();long reads=Reads;alloc.FilterToCurrentThread();alloc.enabled=false;alloc.enabled=true;clock.Start();for(int i=0;i<5;i++)draw(1280,720);clock.Stop();alloc.enabled=false;guiMilliseconds+=clock.Elapsed.TotalMilliseconds;guiAllocated+=alloc.sampleBlockCount;guiReads+=Reads-reads;guiCount+=5;if(guiCount>=50)guiMeasure=false;}catch(Exception e){File.AppendAllText("phase2-results.txt","FAIL: slot profiling "+e+"\n");guiMeasure=false;guiCount=-1;}finally{alloc.enabled=false;GUI.matrix=matrix;}}
 IEnumerator Work(){
  report.label=File.ReadAllText("phase3-label").Trim();report.unity=Application.unityVersion;report.gpu=SystemInfo.graphicsDeviceName;report.cpu=SystemInfo.processorType;
  restoreResolution=SetResolution();QualitySettings.vSyncCount=0;Application.targetFrameRate=-1;Application.runInBackground=true;
  UnityEngine.Random.InitState(314159);yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Assets/Scenes/SampleScene.unity");yield return null;yield return null;
  var dungeon=FindAnyObjectByType<DungeonPrototype>();float deadline=Time.realtimeSinceStartup+30;while(!dungeon.LaboratoryReady&&Time.realtimeSinceStartup<deadline)yield return null;check(dungeon.LaboratoryReady,"Phase 3: normal dungeon ready");
  // Recheck phase-2 persistence before accepting the phase-3 baseline.
  DemoPhase2SaveValidation.Run(check);yield return null;
  var control=Measure("allocation positive control",1,()=>{var bytes=new byte[1024*1024];GC.KeepAlive(bytes);},1);check(control.allocations>=1,"Phase 3: GC.Alloc recorder detects the positive control");
  typeof(DungeonPrototype).GetField("worldDialogue",F).GetValue(dungeon).GetType().GetMethod("Cancel").Invoke(typeof(DungeonPrototype).GetField("worldDialogue",F).GetValue(dungeon),null);
  var saved=dungeon.CaptureDungeonSave();saved.introductionCompleted=true;dungeon.RestoreDungeonSave(saved);report.renderers=dungeon.GetComponentsInChildren<Renderer>(true).Length;report.enemies=saved.enemies.Count(e=>!e.defeated);
  DungeonSaveStore.editorTestPath=Path.GetFullPath("Temp/Phase3SlotFixtures/"+report.label+"/slot.json");
  for(int i=0;i<6;i++)DungeonSaveStore.Write(DungeonMapPersistence.Copy(saved),i,"Performance fixture "+i);
  report.fixtureBytes=new FileInfo(DungeonSaveStore.SlotPath(0)).Length;
  var open=typeof(DungeonPrototype).GetMethod("OpenSaveSlots",F);open.Invoke(dungeon,new object[]{true});dungeon.enabled=false;
  draw=(Action<float,float>)Delegate.CreateDelegate(typeof(Action<float,float>),dungeon,typeof(DungeonPrototype).GetMethod("DrawSaveSlots",F));
  // Warm styles and resources through actual Repaint events, then measure synchronous draw work.
  guiMeasure=true;guiCount=0;while(guiMeasure)yield return null;check(guiCount>0,"Phase 3: native IMGUI slot warmup completed");
  guiMilliseconds=0;guiAllocated=guiReads=0;guiCount=0;guiMeasure=true;while(guiMeasure)yield return null;check(guiCount==50,"Phase 3: 50 native IMGUI slot draws measured");
  report.metrics.Add(new Metric{name="slot panel warm draw",unit="ms/draw",mean=guiMilliseconds/guiCount,allocations=guiAllocated/(double)guiCount,operations=guiCount,reads=guiReads});
  #if DEMO_PHASE3_CACHE
  DemoPhase3CacheTests.Run(check);yield return null;
  #endif
  var audio=FindAnyObjectByType<DungeonAudio>();var tracks=(Func<bool,List<AudioClip>>)Delegate.CreateDelegate(typeof(Func<bool,List<AudioClip>>),audio,typeof(DungeonAudio).GetMethod("Tracks",F));
  Measure("playlist lookup",10000,()=>GC.KeepAlive(tracks(false)),100);
  #if DEMO_PHASE3_AUDIO
  DemoPhase3AudioTests.Run(check,audio);yield return null;
  #endif
  var camera=dungeon.RetroView;var mask=camera.GetComponent<HelmetCRTLayerMask>()??camera.gameObject.AddComponent<HelmetCRTLayerMask>();
  for(int i=0;i<3;i++)yield return null;
  Measure("CRT mask build",12,()=>mask.Build(camera,1920,1080),2);
  var oldType=Type.GetType("HelmetCRTLayerMaskBaseline");if(oldType!=null){var enemy=GameObject.CreatePrimitive(PrimitiveType.Cube);enemy.name="CRT comparison fixture";enemy.transform.position=camera.transform.position+camera.transform.forward*.7f+camera.transform.right*.15f;enemy.transform.localScale=Vector3.one*.18f;enemy.GetComponent<Collider>().enabled=false;enemy.AddComponent<EnemyCombatStats>();var old=(Component)camera.gameObject.AddComponent(oldType);oldType.GetMethod("Refresh",F).Invoke(old,null);typeof(HelmetCRTLayerMask).GetMethod("Refresh",F).Invoke(mask,null);var build=(Func<Camera,int,int,RenderTexture>)Delegate.CreateDelegate(typeof(Func<Camera,int,int,RenderTexture>),old,oldType.GetMethod("Build"));var expected=build(camera,1920,1080);var expectedPixels=Pixels(expected);var actualPixels=Pixels(mask.Build(camera,1920,1080));check(expectedPixels.Any(p=>p.r>128),"Phase 3: mask pixel comparison contains a visible enemy fixture");int changed=0;for(int i=0;i<expectedPixels.Length;i++)if(!expectedPixels[i].Equals(actualPixels[i]))changed++;check(changed==0,"Phase 3: optimized CRT visibility mask matches baseline pixel for pixel");report.metrics.Add(new Metric{name="CRT mask changed pixels",unit="pixels",mean=changed,operations=expectedPixels.Length});Destroy(old);Destroy(enemy);yield return null;}
  #if DEMO_PHASE3_RENDER_ELISION
  yield return DemoPhase3RenderElisionTests.Run(dungeon,check);
  #endif
  if(File.Exists("phase3-micro-only")){report.width=Screen.width;report.height=Screen.height;File.WriteAllText("phase3-"+report.label+".json",JsonUtility.ToJson(report,true));check(true,"Phase 3: focused group measurements written");yield break;}
  for(int i=0;i<6;i++)DungeonSaveStore.Write(DungeonMapPersistence.Copy(saved),i,"Performance fixture "+i);open.Invoke(dungeon,new object[]{true});dungeon.enabled=true;typeof(DungeonPrototype).GetMethod("SetPause",F).Invoke(dungeon,new object[]{true});
  yield return FrameCase("slots panel",90);
  typeof(DungeonPrototype).GetMethod("CloseSaveSlots",F).Invoke(dungeon,null);typeof(DungeonPrototype).GetMethod("SetPause",F).Invoke(dungeon,new object[]{false});
  yield return FrameCase("reference world",120);
  if(File.Exists("phase3-office-followup"))yield return FrameCase("moving view with both reflections",120,true);
  var pipeline=UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
  if(pipeline!=null){bool prior=pipeline.useSRPBatcher;pipeline.useSRPBatcher=!prior;yield return FrameCase("URP SRP Batcher alternate",120);pipeline.useSRPBatcher=prior;yield return FrameCase("URP restored reference",120);check(pipeline.useSRPBatcher==prior,"Phase 3: URP test restores the reference setting");}
  #if DEMO_PHASE3_RENDER_ELISION
  if(File.Exists("phase3-batch-trial")){DemoPhase3BatchTrial.Run(dungeon);yield return FrameCase("experimental static batching",120);yield return FrameCase("experimental batching moving view",120,true);}
  #endif
  // Ownership is measured directly; Editor resource totals alone cannot prove a leak.
  var retired=new List<Material>();string[] fields={"wall","floor","ceiling","pink","blue"};
  for(int cycle=0;cycle<20;cycle++){dungeon=FindAnyObjectByType<DungeonPrototype>();foreach(string field in fields){var m=(Material)typeof(DungeonPrototype).GetField(field,F).GetValue(dungeon);if(m!=null)retired.Add(m);}typeof(DungeonPrototype).GetMethod("RestartNewGame",F).Invoke(dungeon,null);yield return null;yield return null;yield return new WaitForEndOfFrame();}
  report.metrics.Add(new Metric{name="retired owned materials alive",unit="objects after 20 restarts",mean=retired.Count(m=>m!=null),operations=20,retainedNativeBytes=retired.Where(m=>m!=null).Sum(m=>Profiler.GetRuntimeMemorySizeLong(m))});
  report.width=Screen.width;report.height=Screen.height;report.workingSet=System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;if(report.workingSet<=0)report.workingSet=-1;
  File.WriteAllText("phase3-"+report.label+".json",JsonUtility.ToJson(report,true));
  check(true,"Phase 3: benchmark report written for "+report.label);
 }
 IEnumerator FrameCase(string label,int frames,bool movingView=false){
  var view=FindAnyObjectByType<DungeonPrototype>().RetroView;var rotation=view.transform.rotation;
  for(int i=0;i<30;i++){if(movingView)view.transform.rotation=rotation*Quaternion.Euler(0,Mathf.Sin(i*.08f)*8,0);yield return null;}
  var cpu=Recorder.Get("PlayerLoop");cpu.enabled=true;var gc=ProfilerRecorder.StartNew(ProfilerCategory.Memory,"GC Allocated In Frame");var timings=new FrameTiming[1];var cpuMs=new List<double>();var gpuMs=new List<double>();var elapsed=new List<double>();var gcBytes=new List<double>();
  for(int i=0;i<frames;i++){if(movingView)view.transform.rotation=rotation*Quaternion.Euler(0,Mathf.Sin((i+30)*.08f)*8,0);yield return new WaitForEndOfFrame();if(cpu.elapsedNanoseconds>0)cpuMs.Add(cpu.elapsedNanoseconds/1000000d);elapsed.Add(Time.unscaledDeltaTime*1000d);if(gc.Valid)gcBytes.Add(gc.LastValue);FrameTimingManager.CaptureFrameTimings();if(FrameTimingManager.GetLatestTimings(1,timings)>0&&timings[0].gpuFrameTime>0)gpuMs.Add(timings[0].gpuFrameTime);}
  if(movingView)view.transform.rotation=rotation;
  AddSamples(label+" CPU PlayerLoop",cpuMs,"ms",gcBytes.Count>0?gcBytes.Average():-1);AddSamples(label+" GPU",gpuMs,"ms",-1);AddSamples(label+" Editor frame interval",elapsed,"ms",-1);gc.Dispose();
 }
 void AddSamples(string name,List<double> values,string unit,double allocated){values.Sort();report.metrics.Add(new Metric{name=name,unit=unit,available=values.Count>0,mean=values.Count>0?values.Average():-1,p50=values.Count>0?values[(values.Count-1)/2]:-1,p95=values.Count>0?values[Mathf.Clamp(Mathf.CeilToInt(values.Count*.95f)-1,0,values.Count-1)]:-1,allocatedBytes=allocated,operations=values.Count});}
 static Color32[] Pixels(RenderTexture source){var previous=RenderTexture.active;RenderTexture.active=source;var image=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false,true);image.ReadPixels(new Rect(0,0,source.width,source.height),0,0);image.Apply();var result=image.GetPixels32();Destroy(image);RenderTexture.active=previous;return result;}
 static Action SetResolution(){var f=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;var asm=typeof(EditorWindow).Assembly;var type=asm.GetType("UnityEditor.GameView");var view=EditorWindow.GetWindow(type);var selected=type.GetProperty("selectedSizeIndex",f);int old=(int)selected.GetValue(view,null);var sizes=asm.GetType("UnityEditor.GameViewSizes");var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizes).GetProperty("instance",BindingFlags.Static|BindingFlags.Public).GetValue(null,null);var group=sizes.GetProperty("currentGroup",f).GetValue(singleton,null);var groupType=group.GetType();int built=(int)groupType.GetMethod("GetBuiltinCount").Invoke(group,null),custom=(int)groupType.GetMethod("GetCustomCount").Invoke(group,null);var size=asm.GetType("UnityEditor.GameViewSize");var kind=Enum.Parse(asm.GetType("UnityEditor.GameViewSizeType"),"FixedResolution");object entry=Activator.CreateInstance(size,f,null,new object[]{kind,1920,1080,"Phase 3 1080p"},null);groupType.GetMethod("AddCustomSize").Invoke(group,new[]{entry});selected.SetValue(view,built+custom,null);return ()=>{selected.SetValue(view,old,null);groupType.GetMethod("RemoveCustomSize").Invoke(group,new object[]{built+custom});};}
}
public static class DemoPhase3Counters {public static long reads;}
#endif
