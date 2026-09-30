$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../Projeto'
$version = Get-Content "$project/ProjectSettings/ProjectVersion.txt" -Raw
if ($version -notmatch 'm_EditorVersion: 6000\.5\.3f1\s' -or $version -notmatch 'c2eb47b3a2a9') { throw 'Versao ou revisao incorreta.' }
$settings = Get-Content "$project/ProjectSettings/ProjectSettings.asset" -Raw
if ($settings -notmatch 'activeInputHandler: 0') { throw 'Input Manager legado necessario aos controles.' }
$graphics = Get-Content "$project/ProjectSettings/GraphicsSettings.asset" -Raw
if ($graphics -notmatch 'm_CustomRenderPipeline: \{fileID: 0\}') { throw 'Renderizacao Built-in alterada.' }
$manifest = Get-Content "$project/Packages/manifest.json" -Raw | ConvertFrom-Json -AsHashtable
foreach ($name in @('com.unity.modules.imgui','com.unity.modules.physics2d','com.unity.modules.audio','com.unity.modules.video')) {
    if (-not $manifest.dependencies.ContainsKey($name)) { throw "Modulo necessario ausente: $name" }
}
# Verify every serialized script reference belongs to a local script, not a removed package.
$guids = @(Get-ChildItem "$project/Assets" -Filter '*.cs.meta' -Recurse | ForEach-Object {
    [regex]::Match((Get-Content $_.FullName -Raw), 'guid: (\w+)').Groups[1].Value
})
Get-ChildItem "$project/Assets" -Include *.unity,*.prefab,*.asset -Recurse | ForEach-Object {
    foreach ($match in [regex]::Matches((Get-Content $_.FullName -Raw), 'm_Script: \{fileID: 11500000, guid: (\w+)')) {
        if ($guids -notcontains $match.Groups[1].Value) { throw "Script externo ou ausente: $($_.FullName)" }
    }
}
foreach ($name in @('MovimentoGanzel','PrimeiroEnigma')) {
    $code = Get-Content "$project/Assets/$name.cs" -Raw
    if ($code -match '\.velocity\b' -or $code -notmatch '\.linearVelocity\b') { throw "API fisica nao migrada: $name" }
}
Write-Output 'PASS: versao, entrada, renderizacao, modulos, referencias de scripts e API fisica.'
Write-Output 'Esta verificacao estatica nao substitui importacao, compilacao, Play Mode e build na Unity.'
