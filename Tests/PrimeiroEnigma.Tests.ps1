$ErrorActionPreference = 'Stop'
$assets = Join-Path $PSScriptRoot '../Projeto/Assets'
Add-Type -Path (Join-Path $assets 'EnigmaRelogio.cs')
$puzzle = [EnigmaRelogio]::new()
if ($puzzle.PararRelogio()) { throw 'Nao pode resolver sem coletar a gelatina.' }
if (-not $puzzle.PegarGelatina() -or -not $puzzle.TemGelatina) { throw 'Coleta falhou.' }
if ($puzzle.PegarGelatina()) { throw 'Coleta duplicada permitida.' }
if (-not $puzzle.PararRelogio() -or -not $puzzle.Concluido -or $puzzle.TemGelatina) {
    throw 'Conclusao nao consumiu o item ou nao resolveu o enigma.'
}
if ($puzzle.PararRelogio() -or $puzzle.PegarGelatina()) { throw 'Estado final foi alterado.' }
$puzzle.Reiniciar()
if ($puzzle.Concluido -or $puzzle.TemGelatina) { throw 'Reinicio nao limpou o progresso.' }
if (-not $puzzle.PegarGelatina() -or -not $puzzle.PararRelogio()) { throw 'Segunda partida falhou.' }

$scene = Get-Content (Join-Path $assets 'Scenes/SampleScene.unity') -Raw
$ids = @([regex]::Matches($scene, '(?m)^--- !u!\d+ &(\d+)') | ForEach-Object { $_.Groups[1].Value })
if (($ids | Sort-Object -Unique).Count -ne $ids.Count) { throw 'IDs duplicados na cena.' }
foreach ($id in @('583311865','886158438','536343505','519420031','2100000001','2100000002')) {
    if ($ids -notcontains $id) { throw "Referencia ausente: $id" }
}
if ($scene -notmatch '(?s)--- !u!61 &536343507\r?\n.*?m_Enabled: 0') { throw 'Piso ainda bloqueia o jogador.' }
if ($scene -notmatch 'guid: cb2350a57c4d42ccaa41ca49de2af110') { throw 'Enigma nao conectado a cena.' }
Write-Output 'PASS: regras compiladas, progressao, coleta unica, reinicio e referencias da cena.'
Write-Output 'Pendente: compilar e testar fisica, interface e alcance na Unity 2021.3.17f1.'
