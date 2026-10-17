using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
public static class ShutdownDiagnosticBuild {
 public static void Run(){
  PlayerSettings.companyName="CodeboundPhase3HomeIsolated";PlayerSettings.productName="CodeboundPhase3Home";
  string output=@"E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Builds/Probe2/Codebound.exe";
  var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/CodeboundTitle.unity","Assets/Scenes/SampleScene.unity","Assets/Scenes/DigitalKingdom.unity","Assets/Scenes/RetroDungeonExperiment.unity"},locationPathName=output,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
  File.WriteAllText(@"E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Probe2Build.txt",result.summary.result+" errors="+result.summary.totalErrors+" warnings="+result.summary.totalWarnings);
  if(result.summary.result!=BuildResult.Succeeded){EditorApplication.Exit(1);return;}
  var scene=UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,UnityEditor.SceneManagement.NewSceneMode.Single);
  new UnityEngine.GameObject("Neutral Camera").AddComponent<UnityEngine.Camera>();
  new UnityEngine.GameObject("Prevent dungeon bootstrap").AddComponent<CodeboundTitleScreen>().enabled=false;
  UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Assets/ShutdownNeutral.unity");
  var neutral=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/ShutdownNeutral.unity"},locationPathName=@"E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/Builds/Neutral/Codebound.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
  File.WriteAllText(@"E:/UNITY PRUEBA/DCG/Codebound-Shutdown-Lab-20261017/NeutralBuild.txt",neutral.summary.result+" errors="+neutral.summary.totalErrors+" warnings="+neutral.summary.totalWarnings);
  EditorApplication.Exit(neutral.summary.result==BuildResult.Succeeded?0:1);
 }
}
