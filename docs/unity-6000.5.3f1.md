# Migração para Unity 6000.5.3f1

Status: preparação concluída; importação, compilação e execução na Unity ainda não verificadas. Não integrar à main antes dos testes abaixo. O editor não está instalado no ambiente e a usuária confirmou que também não o possui. Testes de regras isoladas não provam compatibilidade do jogo com o editor.

Base recuperável: commit `7a807921` (Unity 2021.3.17f1, com os três enigmas). Abra essa revisão em outra pasta caso precise voltar; não abra uma pasta já convertida com o editor antigo.

## Alterações

- Versão exata 6000.5.3f1, revisão c2eb47b3a2a9, conforme o instalador oficial.
- Escritas de velocidade do Rigidbody2D usam `linearVelocity`, API da Unity 6, mantendo os mesmos vetores e valores.
- Mantidos os eixos Horizontal/Vertical, `activeInputHandler: 0`, renderização Built-in, cenas, GUIDs, arte, áudio, física e regras dos enigmas.
- Removidos do manifesto os pacotes de ferramentas/recursos não usados pelos três scripts ou pela cena: Collaborate, Rider, Visual Studio, VS Code, feature 2D, Test Framework, TextMesh Pro, Timeline, uGUI e Visual Scripting. Isso também libera as dependências transitivas antigas de Burst e ferramentas 2D. Não há UI Canvas, TMP, Timeline, gráficos de Visual Scripting ou testes NUnit no projeto atual. Ferramentas de IDE poderão ser instaladas separadamente pelo Package Manager em versões compatíveis.
- Mantido explicitamente o pacote embutido 2D Sprite. Os módulos do editor foram preservados, exceto o módulo legado Umbra, sem uso na cena 2D.
- Removido o lock da Unity 2021 para que o Package Manager resolva o conjunto na versão alvo. O novo lock precisa ser gerado e versionado pelo editor; não foi fabricado manualmente.

## Validação obrigatória antes do merge

1. Instalar a versão exata pelo Unity Hub, incluindo Windows Build Support. Ativar a licença normalmente no Hub.
2. Seguir a orientação oficial de atualização incremental: validar uma cópia na 2022.3 antes da passagem para Unity 6. A branch contém a preparação do destino, não evidência de conversão incremental executada.
3. Abrir `Projeto` em 6000.5.3f1. Aguardar resolução de pacotes, importação e API Updater. Investigar qualquer pacote indisponível ou erro antes de prosseguir.
4. Versionar o `Packages/packages-lock.json` gerado e as conversões necessárias do editor. Revisar diferenças nas cenas e configurações; confirmar ausência de Missing Scripts e sprites ausentes.
5. Executar `pwsh -File Tests/PrimeiroEnigma.Tests.ps1` e `pwsh -File Tests/UnityMigration.Tests.ps1`.
6. Em Play, testar movimento diagonal, colisões, bordas, pausa, perda de foco, textos, teclado e mouse em 16:9 e 4:3. Manter Built-in e Input Manager para esta migração.
7. Completar gelatina → relógio → café → impressora → calculadora. Testar erro, vitória, três erros e reinício, conforme `docs/primeira-melhoria.md`.
8. Gerar um build Windows pelo Build Profiles com `Assets/Scenes/SampleScene.unity` habilitada e repetir a partida fora do editor. Registrar versão, logs e resultado visual na proposta.

## Fontes oficiais

- [Release e instalador 6000.5.3f1](https://unity.com/releases/editor/whats-new/6000.5.3f1)
- [Procedimento de atualização](https://docs.unity3d.com/6000.0/Documentation/Manual/upgrade-project.html)
- [Mudanças da Unity 6](https://docs.unity3d.com/6000.0/Documentation/Manual/UpgradeGuideUnity6.html)
- [Rigidbody2D.linearVelocity](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D-linearVelocity.html)
