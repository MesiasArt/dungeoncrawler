#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
public static class DemoPhase2ReleaseBuild {
 public static void Run(){
  var scenes=EditorBuildSettings.scenes.Where(s=>s.enabled&&!s.path.StartsWith("Assets/DemoPhase2Fixtures/")).Select(s=>s.path).ToArray();
  string output=File.Exists("demo-build-output")?File.ReadAllText("demo-build-output").Trim():Path.GetFullPath("Builds/DemoPhase2/Codebound.exe");Directory.CreateDirectory(Path.GetDirectoryName(output));
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=scenes,locationPathName=output,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
  File.WriteAllText("phase2-release-build.json",JsonUtility.ToJson(new Result{result=report.summary.result.ToString(),errors=(int)report.summary.totalErrors,warnings=(int)report.summary.totalWarnings,bytes=(long)report.summary.totalSize,seconds=report.summary.totalTime.TotalSeconds,scenes=scenes},true));
  if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Phase 2 Release build failed");
 }
 [System.Serializable] sealed class Result {public string result;public int errors,warnings;public long bytes;public double seconds;public string[] scenes;}
}
#endif
