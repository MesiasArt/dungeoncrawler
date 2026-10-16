param([string]$UnityEditorRoot='C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor',[switch]$BuildRelease)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$testRoot=Join-Path $projectRoot 'Temp/DemoPhase2Regression'
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
foreach($path in @("$testRoot/Assets/Editor/DemoPhase2Boot.cs","$testRoot/Assets/Scripts/DemoPhase2Validation.cs")){
 $source=[IO.File]::ReadAllText($path);[IO.File]::WriteAllText($path,"#define DEMO_PHASE2_SAVE`n#define DEMO_PHASE2_TRANSITION`n"+$source)
}
$store="$testRoot/Assets/Scripts/DungeonSaveStore.cs"
$source=[IO.File]::ReadAllText($store)
$slot='return Path.Combine(Application.persistentDataPath,"Codebound-slot-"+(slot+1)+"-v1.json");'
$legacy='static string LegacyPath {get{return Path.Combine(Application.persistentDataPath,"Codebound-save-v1.json");}}'
if(!$source.Contains($slot)-or!$source.Contains($legacy)){throw 'Isolation anchors changed; refusing to run with unguarded save paths'}
$source=$source.Replace($slot,'throw new InvalidOperationException("Isolated fixture path required");').Replace($legacy,'static string LegacyPath {get{throw new InvalidOperationException("Personal legacy path forbidden in tests");}}')
[IO.File]::WriteAllText($store,$source)
if($BuildRelease){[IO.File]::WriteAllText("$testRoot/phase2-build-after-tests",'enabled')}
$log=Join-Path $testRoot 'phase2-unity.log'
$arguments='-projectPath "{0}" -executeMethod DemoPhase2Boot.Run -logFile "{1}"' -f $testRoot,$log
$process=Start-Process -FilePath (Join-Path $UnityEditorRoot 'Unity.exe') -ArgumentList $arguments -WindowStyle Hidden -PassThru
$process.WaitForExit()
$result=Get-Content "$testRoot/phase2-results.txt" -Raw
if($process.ExitCode -ne 0-or$result.Contains('FAIL:')-or!$result.Contains('PASS: Transition:')){throw "Phase 2 regression failed. $log`n$result"}
Write-Output $result
if($BuildRelease){Get-Content "$testRoot/phase2-release-build.json"}
