$ErrorActionPreference = 'Stop'
$assets = Join-Path $PSScriptRoot '../Projeto/Assets'
Add-Type -Path (Join-Path $assets 'EnigmaRelogio.cs')
$jogo = [EnigmaRelogio]::new()
if ($jogo.Temperatura -ne 29) { throw 'Temperatura inicial incorreta.' }
$null = $jogo.PegarGelatina()
$null = $jogo.PararRelogio()
$null = $jogo.PegarCafe()
$null = $jogo.AbastecerImpressora()
if ($jogo.Temperatura -ne 15) { throw 'Ar condicionado nao ativou.' }
foreach ($temperatura in @(10, 5, 0)) {
    $null = $jogo.ResolverCalculadora('290')
    if ($jogo.Temperatura -ne $temperatura) { throw 'Erro nao reduziu temperatura.' }
}
$jogo.Reiniciar()
if ($jogo.Temperatura -ne 29) { throw 'Reinicio nao restaurou temperatura.' }
$null = $jogo.PegarGelatina()
$null = $jogo.PararRelogio()
$null = $jogo.PegarCafe()
$null = $jogo.AbastecerImpressora()
$null = $jogo.ResolverCalculadora('290')
$null = $jogo.ResolverCalculadora('2+9+0')
if ($jogo.Temperatura -ne 29 -or -not $jogo.Escapou) { throw 'Vitoria nao estabilizou temperatura.' }

# Confere os recursos realmente serializados, inclusive os nove quadros originais.
$scene = Get-Content (Join-Path $assets 'Scenes/SampleScene.unity') -Raw
$metas = @{}
Get-ChildItem $assets -Recurse -Filter '*.meta' | ForEach-Object {
    $guid = [regex]::Match((Get-Content $_.FullName -Raw), '(?m)^guid: (\w+)').Groups[1].Value
    if ($guid) { $metas[$guid] = $_.FullName.Substring(0, $_.FullName.Length - 5) }
}
$campos = @('costas','bodel','abertura','cinematicFinal','somRelogio','somImpressora','somFrio','somBotao','somAmbiente')
foreach ($campo in $campos) {
    $guid = [regex]::Match($scene, "${campo}: \{fileID: \d+, guid: (\w+)").Groups[1].Value
    if (-not $guid -or -not $metas.ContainsKey($guid) -or -not (Test-Path -LiteralPath $metas[$guid])) {
        throw "Recurso ausente: $campo"
    }
}
$frames = [regex]::Match($scene, '(?s)caminhada:\s*((?:\s*- \{[^}]+\}\s*)+)').Groups[1].Value
$guids = [regex]::Matches($frames, 'guid: (\w+)')
if ($guids.Count -ne 9) { throw 'Ciclo incompleto.' }
foreach ($match in $guids) {
    $guid = $match.Groups[1].Value
    if (-not $metas.ContainsKey($guid) -or -not (Test-Path -LiteralPath $metas[$guid])) { throw 'Quadro ausente.' }
}
Write-Output 'PASS: frio, derrota, reinicio, estabilizacao e referencias dos sprites, videos e sons.'
Write-Output 'Pendente: compilacao dos componentes, callbacks de video e verificacao visual no editor Unity.'
