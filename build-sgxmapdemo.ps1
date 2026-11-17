# SgxMapDemo kiadasi build a Build\SgxMapDemo mappaba (exe + HFsystemMap.ini + fuggosegek).
#
# Hasznalat (a repo gyokereben):
#   .\build-sgxmapdemo.ps1                  # .NET 8 Desktop Runtime kell a celgepen
#   .\build-sgxmapdemo.ps1 -SelfContained   # .NET runtime-mal egyutt, barmely Windows x64 gepen fut
#
# A Build\SgxMapDemo mappa tartalmat a build elott torli, de a meglevo HFsystemMap.ini-t
# megtartja, igy a mar beallitott szervercim nem vesz el.

param(
    [switch]$SelfContained,
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root   = $PSScriptRoot
$outDir = Join-Path $root "Build\SgxMapDemo"
$ini    = Join-Path $outDir "HFsystemMap.ini"
$keepIni = $null

if (Test-Path $ini) {
    $keepIni = Get-Content $ini -Raw -Encoding UTF8
}

if (Test-Path $outDir) {
    Remove-Item $outDir -Recurse -Force
}

$publishArgs = @(
    "publish", (Join-Path $root "SgxMapDemo\SgxMapDemo.csproj"),
    "-c", $Configuration,
    "-o", $outDir,
    "-r", "win-x64",
    "-p:AssemblyOriginatorKeyFile=$(Join-Path $root 'sn.snk')"
)

if ($SelfContained) {
    $publishArgs += @("--self-contained", "true")
} else {
    $publishArgs += @("--self-contained", "false")
}

& dotnet @publishArgs
if ($LASTEXITCODE -ne 0) {
    throw "A build nem sikerult (dotnet publish exit code: $LASTEXITCODE)"
}

if ($keepIni) {
    Set-Content -Path $ini -Value $keepIni -Encoding UTF8 -NoNewline
    Write-Host "A korabbi HFsystemMap.ini megtartva."
}

Write-Host ""
Write-Host "Kesz: $outDir"
Write-Host "  Inditas:      $(Join-Path $outDir 'SgxMapDemo.exe')"
Write-Host "  Beallitasok:  $ini"
