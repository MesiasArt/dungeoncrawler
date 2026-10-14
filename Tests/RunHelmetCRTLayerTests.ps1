param([string]$UnityEditorRoot='C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor')
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$testRoot=Join-Path $projectRoot 'Temp/CRTSeparatedTest'
New-Item -ItemType Directory -Force "$testRoot/Assets/Editor","$testRoot/Assets/Runtime","$testRoot/Assets/Resources/HelmetCRT","$testRoot/Packages","$testRoot/ProjectSettings" | Out-Null
Copy-Item "$PSScriptRoot/HelmetCRTSeparatedBoot.cs" "$testRoot/Assets/Editor"
Copy-Item "$PSScriptRoot/HelmetCRTSeparatedValidation.cs" "$testRoot/Assets/Runtime"
foreach($script in @('HelmetCRTController','HelmetCRTSettings','HelmetCRTWholeFrame','HelmetCRTLayerMask')){Copy-Item "$projectRoot/Assets/Scripts/$script.cs" "$testRoot/Assets/Runtime"}
foreach($shader in @('LayerMask','IsolatedComposite','Unpremultiply')){Copy-Item "$projectRoot/Assets/Resources/HelmetCRT/$shader.shader" "$testRoot/Assets/Resources/HelmetCRT"}
foreach($shader in @('NTSC.shader','MultiPipeline.hlsl','HDRPChecker.hlsl')){Copy-Item "$projectRoot/Assets/CRTPostprocess/Resources/Shaders/$shader" "$testRoot/Assets/Resources"}
@"
using UnityEngine;
public class EnemyCombatStats:MonoBehaviour {}
namespace UnityEngine.Rendering {public static class CommandBufferPool {public static CommandBuffer Get(string name){return new CommandBuffer{name=name};}public static void Release(CommandBuffer cmd){cmd.Release();}}}
"@ | Set-Content "$testRoot/Assets/Runtime/TestDependencies.cs"
'{"dependencies":{"com.unity.modules.imgui":"1.0.0","com.unity.modules.screencapture":"1.0.0","com.unity.modules.imageconversion":"1.0.0","com.unity.modules.physics":"1.0.0"}}' | Set-Content "$testRoot/Packages/manifest.json"
'm_EditorVersion: 6000.5.4f1' | Set-Content "$testRoot/ProjectSettings/ProjectVersion.txt"
$log=Join-Path $testRoot 'test.log'
$arguments='-projectPath "{0}" -executeMethod HelmetCRTSeparatedBoot.Run -logFile "{1}"' -f $testRoot,$log
$process=Start-Process -FilePath (Join-Path $UnityEditorRoot 'Unity.exe') -ArgumentList $arguments -WindowStyle Hidden -PassThru -Wait
if($process.ExitCode -ne 0){if(Test-Path "$testRoot/separated-results.txt"){Get-Content "$testRoot/separated-results.txt"};throw "CRT Play Mode tests failed; see $log"}
$result=Get-Content "$testRoot/separated-results.txt" -Raw
if(!$result.StartsWith('PASS:')){throw $result}
Write-Output $result
