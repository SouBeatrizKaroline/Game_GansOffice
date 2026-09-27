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
$puzzle.Reiniciar()
if ($puzzle.PegarCafe() -or $puzzle.AbastecerImpressora() -or $puzzle.ResolverCalculadora('2+9+0')) { throw 'Etapas puladas.' }
if ($puzzle.Vidas -ne 3) { throw 'Perdeu vida antes de liberar calculadora.' }
$null = $puzzle.PegarGelatina()
$null = $puzzle.PararRelogio()
if (-not $puzzle.PegarCafe() -or $puzzle.PegarCafe()) { throw 'Coleta de cafe incorreta.' }
if (-not $puzzle.AbastecerImpressora() -or $puzzle.TemCafe -or $puzzle.AbastecerImpressora()) { throw 'Impressora incorreta.' }
if ($puzzle.ResolverCalculadora('') -or $puzzle.Vidas -ne 3) { throw 'Entrada vazia consumiu vida.' }
if ($puzzle.ResolverCalculadora('290') -or $puzzle.Vidas -ne 2) { throw 'Erro nao consumiu uma vida.' }
if (-not $puzzle.ResolverCalculadora('2+9+0') -or -not $puzzle.Escapou -or -not $puzzle.Terminou) { throw 'Vitoria falhou.' }
if ($puzzle.ResolverCalculadora('errado') -or $puzzle.Vidas -ne 2) { throw 'Partida concluida alterada.' }
$puzzle.Reiniciar()
if ($puzzle.TemCafe -or $puzzle.ImpressoraResolvida -or $puzzle.Escapou -or $puzzle.Terminou -or $puzzle.Vidas -ne 3) { throw 'Reinicio incompleto.' }
$null = $puzzle.PegarGelatina()
$null = $puzzle.PararRelogio()
$null = $puzzle.PegarCafe()
$null = $puzzle.AbastecerImpressora()
1..4 | ForEach-Object { $null = $puzzle.ResolverCalculadora('290') }
if ($puzzle.Vidas -ne 0 -or -not $puzzle.Terminou -or $puzzle.Escapou -or $puzzle.ResolverCalculadora('2+9+0')) { throw 'Derrota nao bloqueia partida.' }
foreach ($reference in @('cafeteira: {fileID: 1614457158}','impressora: {fileID: 670318142}','calculadora: {fileID: 1022993350}')) {
    if (-not $scene.Contains($reference)) { throw "Referencia de interacao ausente: $reference" }
}
Write-Output 'PASS: sequencia completa, cafe, impressora, resposta vazia, erros, vitoria, derrota e reinicio.'
Write-Output 'Pendente: compilar e testar fisica, interface e alcance na Unity 6000.5.3f1.'
