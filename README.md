# GansOffice

Jogo 2D criado durante a Women Game Jam 2022.

## Baixar e jogar no Windows

[**Baixar GansOffice para Windows 64 bits — alpha (93 MB)**](https://github.com/SouBeatrizKaroline/Game_GansOffice/releases/download/v0.2.0-alpha-windows/GansOffice-Windows.zip)

1. Baixe o ZIP e extraia a pasta inteira.
2. Abra **GansOffice.exe**. Mantenha as pastas e DLLs junto do executável.
3. Use **WASD/setas** para mover, **E** para interagir, **Enter** para confirmar/pular vídeos e **Esc** para pausar. **Alt+F4** fecha o jogo.

Não é necessário instalar a Unity para jogar. Veja as [notas da versão](https://github.com/SouBeatrizKaroline/Game_GansOffice/releases/tag/v0.2.0-alpha-windows) e [como gerar o executável](docs/build-windows.md).

Esta versão de teste teve compilação e inicialização com Direct3D 11 verificadas; a partida completa ainda precisa de teste manual.

## Imagens do jogo

### O escritório

![Arte final do escritório de GansOffice, com seis mesas, computadores, impressora, relógio e cafeteira.](Projeto/Assets/Cen%C3%A1rio/Background/Final%20Art/Office%20Background%20with%20sprites_Final%20Art.png)

*Arte final do cenário do jogo.*

<details>
<summary>Veja também o desenho de construção do cenário</summary>

![Desenho em linhas coloridas do escritório, mostrando a disposição das mesas e dos objetos dos enigmas.](Projeto/Assets/Cen%C3%A1rio/Background/Concept/Office%20Background_with%20sprites_Concept.png)

*Estudo do cenário antes da aplicação de cores e texturas.*

</details>

As imagens são artes originais do projeto; não são capturas da versão atual em execução.

> GansOffice |
Plataforma de conexão entre voluntários e pessoas com perda total ou parcial da visão, para acesso a ambientes sem acessibilidade, com apoio em descrição de imagens e uso de plataformas.

## Equipe de Criação (Women Game Jam 2022)

> Alana Freitas - Game Designer 

> Ana Carolina - Designer de Narrativas

> Ana Marcello - Desenvolvedora

> Beatriz Karoline - Designer de Narrativas 

> Diana Imaizumi - Artista e animadora 2D

> Gabrielle Bocal - Artista e animadora 2D

## Em desenvolvimento atualmente por

- Beatriz Karoline

> Projeto sendo desenvolvido em lives no YouTube

- [🔗 Assista Aqui](https://youtube.com/playlist?list=PL1ldPEBU1lB_gMV2i_xQnkE9h1vJhogAa)

- [🔗 Página do projeto no itch.io](https://soubeatrizkaroline.itch.io/gansoffice)

## 🛠 Tecnologias

- Unity 6000.5.3f1 (compilação Windows validada)

- C#

- Git e GitHub

## Abrir o projeto na Unity

1. Instale a Unity `6000.5.3f1` pelo Unity Hub.
2. No Unity Hub, selecione **Abrir** e escolha a pasta `Projeto`.
3. Aguarde a Unity importar os recursos e recriar a pasta `Library`.
4. Abra a cena `Assets/Scenes/SampleScene.unity`.
5. Pressione **Play**.

Use as teclas `WASD` ou as setas para movimentar Gander pelo escritório.

## Três enigmas em desenvolvimento

Explore o escritório, recolha a gelatina e use-a no relógio. Pressione `E`
perto de um objeto para interagir, `Enter` para fechar bilhetes e `Esc`
para pausar. O botão **Recomeçar enigma** restaura a primeira etapa.

Depois do relógio, investigue a impressora e use o café da cafeteira.
A pista impressa leva à calculadora da mesa inferior direita. Digite a
expressão ou use os botões; cada resposta errada consome uma das três vidas.
Resolver a charada conclui o protótipo; errar três vezes permite recomeçar.
A abertura e a cinematic final estão conectadas, com retrato e fala de Bodel. Gander usa os quadros originais de caminhada; a gelatina tem arremesso animado, e erros na calculadora ativam efeitos de frio e temperatura.
As regras têm testes automatizados. O projeto compilou na Unity e iniciou com Direct3D 11; a partida completa e a integração visual e física ainda precisam de teste manual. Veja [referências dos enigmas](docs/primeira-melhoria.md) e [animações, limitações e validação](docs/animacoes-e-final.md).

## Estrutura do repositório

- `Projeto/Assets`: cena, scripts, arte e áudio do jogo.
- `Projeto/Packages`: dependências da Unity.
- `Projeto/ProjectSettings`: configurações do projeto.

Pastas geradas pela Unity, como `Library`, `Temp`, `Logs`, `obj` e
`UserSettings`, não devem ser versionadas. Elas são recriadas ao abrir o
projeto e podem variar entre computadores.

## Migração para Unity 6

O projeto foi importado e compilado para Windows na Unity 6000.5.3f1. A inicialização com Direct3D 11 foi conferida; a validação completa da partida permanece pendente. Consulte [o procedimento e as limitações](docs/unity-6000.5.3f1.md).
