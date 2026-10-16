param([string]$UnityEditorRoot='C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor',[switch]$BuildRelease,[switch]$Phase3,[switch]$GameplaySmoke,[string]$BenchmarkLabel='',[string]$TestRootPath='',[switch]$RenderElision)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$testRoot=if($TestRootPath){[IO.Path]::GetFullPath($TestRootPath)}else{Join-Path $projectRoot 'Temp/DemoPhase2Regression'}
if($testRoot.TrimEnd('\','/')-eq$projectRoot.TrimEnd('\','/')){throw 'Test project must be separate from production'}
New-Item -ItemType Directory -Force $testRoot | Out-Null
# Copy, never mirror/delete. Unity imports and all fixtures stay in this disposable project.
foreach($folder in @('Assets','Packages','ProjectSettings','Library/PackageCache')){
 $from=Join-Path $projectRoot $folder;$to=Join-Path $testRoot $folder
 New-Item -ItemType Directory -Force $to | Out-Null
 & robocopy $from $to /E /R:0 /W:0 /NFL /NDL /NJH /NJS /NP | Out-Null
 if($LASTEXITCODE -ge 8){throw "Copy failed: $folder"}
}
foreach($name in @('DemoPhase2Boot','DemoPhase2ReleaseBuild')){Copy-Item "$PSScriptRoot/$name.cs" "$testRoot/Assets/Editor/$name.cs"}
foreach($name in @('DemoPhase2Validation','DemoPhase2SaveValidation','DemoPhase2TransitionValidation','DemoPhase2DelayedDungeon')){Copy-Item "$PSScriptRoot/$name.cs" "$testRoot/Assets/Scripts/$name.cs"}
if($Phase3){
 foreach($name in @('DemoPhase3CacheTests','DemoPhase3AudioTests','DemoPhase3Benchmark','DemoPhase3GameplaySmoke','DemoPhase3MaskComparison','DemoPhase3EditValidation','DemoPhase3RenderElisionTests','DemoPhase3BatchTrial')){Copy-Item "$PSScriptRoot/$name.cs" "$testRoot/Assets/Scripts/$name.cs"}
 Copy-Item "$PSScriptRoot/DemoPhase3ResourceInventory.cs" "$testRoot/Assets/Editor/DemoPhase3ResourceInventory.cs"
 $baseline=[IO.File]::ReadAllText("$projectRoot/Docs/DemoPhase3Data/HelmetCRTLayerMask.Before.cs.txt").Replace('HelmetCRTLayerMask','HelmetCRTLayerMaskBaseline');[IO.File]::WriteAllText("$testRoot/Assets/Scripts/HelmetCRTLayerMaskBaseline.cs","#if UNITY_EDITOR`n"+$baseline+"`n#endif")
 foreach($path in @("$testRoot/Assets/Editor/DemoPhase2Boot.cs","$testRoot/Assets/Scripts/DemoPhase3EditValidation.cs","$testRoot/Assets/Scripts/DemoPhase2Validation.cs","$testRoot/Assets/Scripts/DemoPhase3Benchmark.cs","$testRoot/Assets/Scripts/DemoPhase3CacheTests.cs","$testRoot/Assets/Scripts/DemoPhase3AudioTests.cs")){
  [IO.File]::WriteAllText($path,"#define DEMO_PHASE3_CACHE`n#define DEMO_PHASE3_AUDIO`n#define DEMO_PHASE3_BENCHMARK`n#define DEMO_PHASE3_SMOKE`n"+[IO.File]::ReadAllText($path))
 }
 if($RenderElision){foreach($path in @("$testRoot/Assets/Scripts/DemoPhase3Benchmark.cs","$testRoot/Assets/Scripts/DemoPhase3RenderElisionTests.cs","$testRoot/Assets/Scripts/DemoPhase3BatchTrial.cs")){[IO.File]::WriteAllText($path,"#define DEMO_PHASE3_RENDER_ELISION`n"+[IO.File]::ReadAllText($path))}}
 $lab="$testRoot/Assets/Scripts/CombatLaboratory.cs";[IO.File]::WriteAllText($lab,[IO.File]::ReadAllText($lab).Replace('System.IO.Path.Combine(Application.persistentDataPath,"Codebound-laboratory.json")','System.IO.Path.GetFullPath("Temp/Phase3LabFixture/slot.json")'))
}
foreach($path in @("$testRoot/Assets/Editor/DemoPhase2Boot.cs","$testRoot/Assets/Scripts/DemoPhase2Validation.cs")){
 $source=[IO.File]::ReadAllText($path);[IO.File]::WriteAllText($path,"#define DEMO_PHASE2_SAVE`n#define DEMO_PHASE2_TRANSITION`n"+$source)
}
$store="$testRoot/Assets/Scripts/DungeonSaveStore.cs"
$source=[IO.File]::ReadAllText($store)
$slot='return Path.Combine(Application.persistentDataPath,"Codebound-slot-"+(slot+1)+"-v1.json");'
$legacy='static string LegacyPath {get{return Path.Combine(Application.persistentDataPath,"Codebound-save-v1.json");}}'
if(!$source.Contains($slot)-or!$source.Contains($legacy)){throw 'Isolation anchors changed; refusing to run with unguarded save paths'}
$guard="`n#if UNITY_EDITOR`nthrow new InvalidOperationException(`"Isolated fixture path required`" );`n#else`n"+$slot+"`n#endif`n"
$legacyGuard="static string LegacyPath {get{`n#if UNITY_EDITOR`nthrow new InvalidOperationException(`"Personal legacy path forbidden in tests`" );`n#else`nreturn Path.Combine(Application.persistentDataPath,`"Codebound-save-v1.json`" );`n#endif`n}}"
$source=$source.Replace($slot,$guard).Replace($legacy,$legacyGuard)
if($Phase3){$source=$source.Replace('static DungeonSaveData ReadFile(string path){',"static DungeonSaveData ReadFile(string path){`n#if UNITY_EDITOR`nDemoPhase3Counters.reads++;`n#endif`n")}
[IO.File]::WriteAllText($store,$source)
foreach($marker in @('phase3-label','phase3-smoke','phase2-build-after-tests','phase3-micro-only','phase2-ui-only')){if(Test-Path "$testRoot/$marker"){Remove-Item -LiteralPath "$testRoot/$marker"}}
if($GameplaySmoke){if(!$Phase3){throw 'GameplaySmoke requires Phase3'};[IO.File]::WriteAllText("$testRoot/phase3-smoke",'enabled')}
if($BenchmarkLabel){if(!$Phase3){throw 'BenchmarkLabel requires Phase3'};[IO.File]::WriteAllText("$testRoot/phase3-label",$BenchmarkLabel)}
if($BuildRelease){[IO.File]::WriteAllText("$testRoot/phase2-build-after-tests",'enabled')}
$log=Join-Path $testRoot 'phase2-unity.log'
$arguments='-projectPath "{0}" -executeMethod DemoPhase2Boot.Run -logFile "{1}"' -f $testRoot,$log
$process=Start-Process -FilePath (Join-Path $UnityEditorRoot 'Unity.exe') -ArgumentList $arguments -WindowStyle Hidden -PassThru
$process.WaitForExit()
$result=Get-Content "$testRoot/phase2-results.txt" -Raw
if($process.ExitCode -ne 0-or$result.Contains('FAIL:')-or!$result.Contains('PASS:')){throw "Isolated regression failed. $log`n$result"}
Write-Output $result
if($BuildRelease){Get-Content "$testRoot/phase2-release-build.json"}
