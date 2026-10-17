using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public sealed class ShutdownDiagnosticProbe:MonoBehaviour {
 static string Arg(string key,string fallback){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,key);return i>=0&&i+1<args.Length?args[i+1]:fallback;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Before(){if(Arg("-sd-mode","baseline")=="accessibility-off")UnityEngine.Accessibility.AssistiveSupport.screenReaderStatusOverride=UnityEngine.Accessibility.AssistiveSupport.ScreenReaderStatusOverride.ForceDisabled;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Init(){if(FindAnyObjectByType<ShutdownDiagnosticProbe>()!=null)return;var g=new GameObject("Shutdown diagnostic probe — lab only");DontDestroyOnLoad(g);g.AddComponent<ShutdownDiagnosticProbe>();}
 IEnumerator Start(){Application.runInBackground=true;string mode=Arg("-sd-mode","baseline"),route=Arg("-sd-route","title");float delay=float.Parse(Arg("-sd-delay","12"),System.Globalization.CultureInfo.InvariantCulture);Debug.Log("SD START mode="+mode+" route="+route+" isolatedPath="+Application.persistentDataPath);
  if(route=="load"){yield return new WaitForSecondsRealtime(2);var title=FindAnyObjectByType<CodeboundTitleScreen>();if(title!=null)title.ContinueSlot(0);}
  yield return new WaitForSecondsRealtime(3);
  Debug.Log("SD OBJECTS "+string.Join(" | ",FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Select(x=>x.GetType().Name+":"+x.enabled)));
  if(mode=="no-crt")foreach(var x in FindObjectsByType<HelmetCRTWholeFrame>(FindObjectsSortMode.None))x.enabled=false;
  if(mode=="no-reflections")foreach(var x in FindObjectsByType<DungeonFloorReflection>(FindObjectsSortMode.None))x.enabled=false;
  yield return new WaitForSecondsRealtime(delay);Debug.Log("SD APPLICATION.QUIT");Application.Quit(0);
 }
 void OnApplicationQuit(){Debug.Log("SD OnApplicationQuit");if(Arg("-sd-mode","baseline")=="accessibility-off-at-quit")UnityEngine.Accessibility.AssistiveSupport.screenReaderStatusOverride=UnityEngine.Accessibility.AssistiveSupport.ScreenReaderStatusOverride.ForceDisabled;}
 void OnDisable(){Debug.Log("SD OnDisable");}
 void OnDestroy(){Debug.Log("SD OnDestroy");}
}
