param(
    [string]$UnityEditorRoot = 'C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor'
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$sdkRoot = Join-Path $UnityEditorRoot 'Data\DotNetSdk'
$compiler = Get-ChildItem -LiteralPath (Join-Path $sdkRoot 'sdk') -Directory |
    ForEach-Object { Join-Path $_.FullName 'Roslyn\bincore\csc.dll' } |
    Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
$referenceVersion = Get-ChildItem -LiteralPath (Join-Path $sdkRoot 'packs\Microsoft.NETCore.App.Ref') -Directory |
    Sort-Object { [version]$_.Name } -Descending | Select-Object -First 1
$references = Get-ChildItem -LiteralPath (Join-Path $referenceVersion.FullName 'ref\net8.0') -Filter '*.dll'
$outputRoot = Join-Path $projectRoot 'Temp\IntroTests'
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null
$testAssembly = Join-Path $outputRoot 'IntroSequenceTests.dll'
$arguments = @('/nologo', '/target:exe', ('/out:"{0}"' -f $testAssembly))
$arguments += $references | ForEach-Object { '/r:"{0}"' -f $_.FullName }
$arguments += '"{0}"' -f (Join-Path $projectRoot 'Assets\Scripts\IntroSequence.cs')
$arguments += '"{0}"' -f (Join-Path $PSScriptRoot 'IntroSequenceTests.cs')
$responseFile = Join-Path $outputRoot 'tests.rsp'
[System.IO.File]::WriteAllLines($responseFile, $arguments)
& (Join-Path $sdkRoot 'dotnet.exe') $compiler ('@' + $responseFile)
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
$runtimeConfig = '{"runtimeOptions":{"tfm":"net8.0","framework":{"name":"Microsoft.NETCore.App","version":"8.0.0"}}}'
[System.IO.File]::WriteAllText((Join-Path $outputRoot 'IntroSequenceTests.runtimeconfig.json'), $runtimeConfig)
& (Join-Path $sdkRoot 'dotnet.exe') $testAssembly
if ($LASTEXITCODE -ne 0) { throw 'Intro regression tests failed' }
