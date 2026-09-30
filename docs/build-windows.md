# Gerar GansOffice para Windows

A versão alpha é distribuída em [Releases](https://github.com/SouBeatrizKaroline/Game_GansOffice/releases/tag/v0.2.0-alpha-windows). Baixe o ZIP, extraia a pasta inteira e abra GansOffice.exe.

## Reproduzir a compilação

1. Instale e ative a Unity 6000.5.3f1 pelo Unity Hub.
2. Abra a pasta Projeto, aguarde a importação e confirme ausência de erros.
3. No menu GansOffice, selecione **Gerar versão Windows**.
4. A saída padrão fica em Projeto/Builds/Windows. Distribua a pasta inteira em ZIP.

O script Assets/Editor/BuildGansOffice.cs usa Windows 64 bits com Mono, janela redimensionável de 1280 × 720 e valida as referências dos objetos, dos nove quadros de caminhada, vídeos e sons antes de gerar o aplicativo.

Também pode ser executado pela linha de comando:

```powershell
& 'CAMINHO/Editor/Unity.exe' -batchmode -quit -nographics -accept-apiupdate -buildTarget Win64 -projectPath 'CAMINHO/Projeto' -executeMethod BuildGansOffice.Windows -logFile 'build-windows.log'
```

A variável de ambiente GANSOFFICE_BUILD_DIR define outra pasta de saída. O log precisa conter GANSOFFICE_SCENE_OK e GANSOFFICE_BUILD_OK. Mantenha o executável, GansOffice_Data, MonoBleedingEdge e as DLLs juntos.

## Correções de compatibilidade

- A dependência antiga com.unity.modules.vr não existe nesta versão e foi removida.
- A ferramenta opcional com.unity.2d.sprite veio com arquivos/classes incompletos no editor instalado. Ela foi removida das dependências; os sprites individuais existentes foram importados e suas referências conferidas pela Unity.
- O VideoPlayer passa a controlar explicitamente a trilha de áudio 0.
- A Unity atualizou a serialização das configurações e dos importadores de vídeo.
- O campo de passcode do PS4 está vazio. Ele não é necessário para Windows.

## Validação desta alpha

Os três testes PowerShell passaram. A compilação Windows terminou com sucesso, sem erros de C#, e o aplicativo permaneceu ativo ao inicializar com Direct3D 11. A abertura carregou sem erro de reprodução; a Unity registrou um aviso de timestamps no vídeo original e ajustou-os durante a reprodução.

A verificação de inicialização não substitui jogar os três enigmas. Movimento, colisões, pausa, arremesso, derrota, reinício e cinematic final ainda precisam de teste manual completo.

ZIP: GansOffice-Windows.zip, 97.223.005 bytes.

SHA-256: `3E2E11BC6887731A88F04AC146180BFB847094688A4DEBAD3FBA8B0FB860AC7E`.
