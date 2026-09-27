# Primeiro enigma do GansOffice

Esta entrega implementa a gelatina e o relógio na cena existente. É uma primeira etapa, não o jogo completo. A impressora, o café, a calculadora e a saída continuam pendentes.

## Referências aproveitadas

- As duas imagens enviadas mostram o escritório em lineart e em arte final. Os recursos correspondentes já estão em `Projeto/Assets/Cenário`; foram mantidos.
- Os três documentos em `Assets/Roteiro` descrevem os personagens, a gelatina que para o relógio, a impressora que precisa de café e a calculadora. Foram usados como referência narrativa, não como instruções operacionais.
- [Gans Office — Cena Final](https://www.youtube.com/watch?v=-Y2Ik9uPGho) e [Gans Office — Trailer](https://www.youtube.com/watch?v=xAMgQwtjTVs): páginas e imagens de apresentação conferidas no navegador. Não houve transcrição ou análise integral do áudio dos vídeos. Os vídeos locais existentes foram preservados.
- `drive-download-20230531T003225Z-001.zip` contém somente três PNGs: `RutherFox_Lab Concept Art1`, `RutherFox_Storage Concept Art1` e `RutherFox_Nuclear Lab Concept Art1`. São conceitos de laboratório e depósito, identificados como RutherFox. Não foram incorporados ao escritório.

## O que mudou

- Bilhete inicial, objetivo, inventário de gelatina e interação por proximidade com E ou botão.
- A gelatina aparece no relógio após resolver o enigma. A tela informa o fim desta primeira etapa.
- Pausa com Esc, confirmação com Enter ou Espaço e reinício sem recarregar a cena. Perder o foco pausa a partida.
- Personagem com colisão nos pés, escala menor, interpolação e detecção contínua. O collider do piso foi desativado; limites ao redor do piso impedem sair do escritório.
- Câmera enquadra todo o cenário de acordo com a proporção da janela.

## Validação

Execute `pwsh -File Tests/PrimeiroEnigma.Tests.ps1` para compilar e testar as regras isoladas e verificar referências da cena. O teste não substitui a compilação do projeto Unity.

O editor Unity não foi encontrado no ambiente de desenvolvimento desta entrega. A compilação dos componentes Unity, o resultado visual e a física precisam ser verificados na Unity 2021.3.17f1 antes de integrar a alteração.

### Verificação no editor

1. Abra `Projeto`, carregue `SampleScene` e entre em Play; confirme ausência de erros no Console.
2. Feche o bilhete com Enter. Confira câmera, legibilidade e movimento por WASD/setas.
3. Tente atravessar mesas e sair pelas bordas: o personagem deve parar. Confirme que consegue circular entre as mesas.
4. Visite o relógio sem gelatina: deve aparecer uma dica sem resolver o enigma.
5. Aproxime-se da gelatina da mesa superior esquerda; pressione E. Confirme inventário e desaparecimento do item.
6. Volte ao relógio pelo corredor. Pressione E e confirme gelatina no relógio e conclusão.
7. Reinicie e repita. Pause enquanto caminha; ao continuar, não deve haver movimento residual sem tecla pressionada.
8. Teste janela 16:9 e 4:3, perda de foco e botões com mouse.

O protótipo ainda não tem animação dos ponteiros, arremesso animado, sons integrados, salvamento nem suporte a leitor de tela. Não representa uma versão final acessível.
