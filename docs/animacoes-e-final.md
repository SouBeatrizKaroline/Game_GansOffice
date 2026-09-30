# Animações e encerramento

Os roteiros originais em `Projeto/Assets/Roteiro` orientam esta implementação:
Gander joga gelatina no relógio, abastece a impressora com café, resolve a
calculadora, estabiliza o escritório em 29 °C e encontra Bodel no elevador.
A fala de encerramento segue `Roteiro Gansoffice Gameplay.docx`.

## Integrações

- Abertura e final usam os MP4 originais em `Assets/Animações`, sem alterar os arquivos.
- Gander usa os nove PNGs originais de caminhada, espelhamento horizontal e
  a arte de costas ao caminhar para cima. Parado, mantém a direção e retorna
  à pose original; a vista de costas permanece quando aplicável.
- A gelatina percorre um arco e vibra ao grudar no relógio. Movimento e
  interações ficam bloqueados durante o arremesso.
- Sons originais do relógio, impressora, ar condicionado, botões e ambiente.
- A impressora ativa o frio em 15 °C. Erros reduzem a temperatura para 10, 5
  e 0 °C, acompanhados de cor azul e vento mais alto. Acertar restaura 29 °C.
- A cinematic final precede a fala de Bodel com seu retrato original.
- Enter/Espaço ou botão pulam vídeos; Esc pausa e retoma. Perda de foco
  pausa vídeo, áudio e arremesso. Falha de vídeo ou preparação acima de
  15 segundos permite continuar com o bilhete, sem bloquear a partida.
- Recomeçar limpa animações, áudio pontual, cor, temperatura e progresso.

Não há nova arte gerada. Capim e Arcade possuem arte no repositório, mas não
entram como NPCs neste escritório, que o roteiro descreve como vazio após
Gander acordar. Dashi, Yuri, Bats e Pantera permanecem personagens narrativos;
suas artes e interações para os spin-offs ainda precisam ser produzidas.
Os ponteiros do relógio ainda não têm animação independente.

## Validação

Execute cada teste em um processo separado:

```powershell
pwsh -NoProfile -File Tests/PrimeiroEnigma.Tests.ps1
pwsh -NoProfile -File Tests/UnityMigration.Tests.ps1
pwsh -NoProfile -File Tests/Apresentacao.Tests.ps1
```

Estes testes compilam as regras e verificam os recursos serializados; não
compilam os componentes Unity. O editor não está instalado neste ambiente.
A publicação é uma proposta de código, não um novo build jogável no itch.io.

Antes de integrar, abra `Projeto` na Unity 6000.5.3f1 e verifique:

1. Importe sem erros; reproduza a abertura, pause, retome e pule.
2. Ande em todas as direções; confira escala, pivôs, pose parada e colisões.
3. Arremesse a gelatina; tire o foco no meio e retome. Confira o arco,
   vibração, posição final e bloqueio das interações.
4. Resolva a impressora; confira sons e 15 °C. Erre até perder três vidas;
   confira o frio, a derrota e o reinício completo.
5. Acerte depois de um erro; confira o final com áudio e retrato de Bodel.
   Pause e pule o final, inclusive depois de perder foco.
6. Remova temporariamente um VideoClip no Inspector; confira o bilhete
   alternativo. Teste também erro de codec e vídeo em janela 4:3/16:9.

Não houve análise integral das lives no YouTube: a playlist não carregou
neste ambiente. A implementação se apoia nos roteiros, arte e vídeos locais.
