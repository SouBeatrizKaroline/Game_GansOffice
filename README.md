# GansOffice

Jogo 2D criado durante a Women Game Jam 2022.

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

- [🔗 Baixar Versão Atual](https://soubeatrizkaroline.itch.io/gansoffice)

## 🛠 Tecnologias

- Unity 6000.5.3f1 (migração em validação)

- C#

- Git e GitHub

## Como executar

1. Instale a Unity `6000.5.3f1` pelo Unity Hub.
2. No Unity Hub, selecione **Abrir** e escolha a pasta `Projeto`.
3. Aguarde a Unity importar os recursos e recriar a pasta `Library`.
4. Abra a cena `Assets/Scenes/SampleScene.unity`.
5. Pressione **Play**.

Use as teclas `WASD` ou as setas para movimentar Gansel pelo escritório.

## Três enigmas em desenvolvimento

Explore o escritório, recolha a gelatina e use-a no relógio. Pressione `E`
perto de um objeto para interagir, `Enter` para fechar bilhetes e `Esc`
para pausar. O botão **Recomeçar enigma** restaura a primeira etapa.

Depois do relógio, investigue a impressora e use o café da cafeteira.
A pista impressa leva à calculadora da mesa inferior direita. Digite a
expressão ou use os botões; cada resposta errada consome uma das três vidas.
Resolver a charada conclui o protótipo; errar três vezes permite recomeçar.
A abertura e a cinematic final estão conectadas, com retrato e fala de Bodel. Gander usa os quadros originais de caminhada; a gelatina tem arremesso animado, e erros na calculadora ativam efeitos de frio e temperatura.
As regras têm testes automatizados; a integração visual e física ainda
precisa de teste no editor Unity. Veja [referências dos enigmas](docs/primeira-melhoria.md) e [animações, limitações e validação](docs/animacoes-e-final.md).

## Estrutura do repositório

- `Projeto/Assets`: cena, scripts, arte e áudio do jogo.
- `Projeto/Packages`: dependências da Unity.
- `Projeto/ProjectSettings`: configurações do projeto.

Pastas geradas pela Unity, como `Library`, `Temp`, `Logs`, `obj` e
`UserSettings`, não devem ser versionadas. Elas são recriadas ao abrir o
projeto e podem variar entre computadores.

## Migração para Unity 6

A branch de migração aponta para 6000.5.3f1, mas ainda requer importação, compilação e teste no editor antes de integrar à main. Consulte [o procedimento e as limitações](docs/unity-6000.5.3f1.md).
