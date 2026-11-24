# SgxMapDemo kiadasi build a Build\SgxMapDemo mappaba (exe + HFsystemMap.ini + fuggosegek).
#
# Hasznalat (a repo gyokereben):
#   .\build-sgxmapdemo.ps1                      # alapertelmezes: self-contained, a .NET runtime is a mappaba
#                                               # kerul, barmely Windows 10/11 x64 gepen fut telepites nelkul
#                                               # (internet nelkuli gepekre is)
#   .\build-sgxmapdemo.ps1 -FrameworkDependent  # kisebb, de a celgepen kell a .NET 8 Desktop Runtime
#                                               # (Microsoft.WindowsDesktop.App 8.x)
#
# A Build\SgxMapDemo mappa tartalmat a build elott torli, de a meglevo HFsystemMap.ini-t
# megtartja, igy a mar beallitott szervercim nem vesz el.

#   .\build-sgxmapdemo.ps1 -OutName SgxMapDemo11182  # masik mappanev a Build alatt

param(
    [switch]$FrameworkDependent,
    [string]$Configuration = "Release",
    [string]$OutName = "SgxMapDemo"
)

$ErrorActionPreference = "Stop"
$root   = $PSScriptRoot
$outDir = Join-Path $root "Build\$OutName"
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

if ($FrameworkDependent) {
    $publishArgs += @("--self-contained", "false")
} else {
    $publishArgs += @("--self-contained", "true")
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
