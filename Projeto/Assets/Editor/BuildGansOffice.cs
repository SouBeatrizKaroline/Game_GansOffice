using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildGansOffice
{
    [MenuItem("GansOffice/Gerar versão Windows")]
    public static void Windows()
    {
        string destino = Environment.GetEnvironmentVariable("GANSOFFICE_BUILD_DIR");
        if (string.IsNullOrWhiteSpace(destino))
            destino = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/Windows"));
        Directory.CreateDirectory(destino);

        const string cena = "Assets/Scenes/SampleScene.unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(cena) == null)
            throw new InvalidOperationException("Cena principal não encontrada: " + cena);
        var cenaAberta = EditorSceneManager.OpenScene(cena);
        PrimeiroEnigma jogo = null;
        foreach (var raiz in cenaAberta.GetRootGameObjects())
        {
            jogo = raiz.GetComponentInChildren<PrimeiroEnigma>();
            if (jogo != null) break;
        }
        if (jogo == null) throw new InvalidOperationException("Enigmas ausentes na cena.");
        var referencias = new SerializedObject(jogo);
        foreach (var campo in new[] { "gelatina", "relogio", "cafeteira", "impressora", "calculadora",
            "ambiente", "cameraJogo", "costas", "bodel", "abertura", "cinematicFinal", "somRelogio",
            "somImpressora", "somFrio", "somBotao", "somAmbiente" })
        {
            if (referencias.FindProperty(campo).objectReferenceValue == null)
                throw new InvalidOperationException("Recurso não importado: " + campo);
        }
        var quadros = referencias.FindProperty("caminhada");
        if (quadros.arraySize != 9) throw new InvalidOperationException("Ciclo de caminhada incompleto.");
        for (int i = 0; i < quadros.arraySize; i++)
            if (quadros.GetArrayElementAtIndex(i).objectReferenceValue == null)
                throw new InvalidOperationException("Quadro de caminhada não importado: " + i);
        if (jogo.GetComponent<Rigidbody2D>() == null || jogo.GetComponent<Collider2D>() == null)
            throw new InvalidOperationException("Física de Gander incompleta.");
        Debug.Log("GANSOFFICE_SCENE_OK: referências importadas e componentes compilados.");

        var plataforma = UnityEditor.Build.NamedBuildTarget.Standalone;
        PlayerSettings.SetScriptingBackend(plataforma, ScriptingImplementation.Mono2x);
        PlayerSettings.companyName = "SouBeatrizKaroline";
        PlayerSettings.productName = "GansOffice";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;

        var relatorio = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { cena },
            locationPathName = Path.Combine(destino, "GansOffice.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CompressWithLz4HC | BuildOptions.StrictMode
        });
        if (relatorio.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException("Build falhou: " + relatorio.summary.result +
                "; erros: " + relatorio.summary.totalErrors);
        File.WriteAllText(Path.Combine(destino, "LEIA-ME.txt"),
            "GansOffice — versão Windows 64 bits\r\n\r\n" +
            "Extraia a pasta inteira e abra GansOffice.exe. Mantenha o executável, " +
            "a pasta GansOffice_Data, MonoBleedingEdge e as DLLs juntos.\r\n\r\n" +
            "WASD/setas: mover. E: interagir. Enter: confirmar ou pular vídeo. " +
            "Esc: pausar/voltar. Alt+F4: fechar.\r\n\r\n" +
            "Arte, roteiro e animações originais da equipe Women Game Jam 2022. " +
            "Desenvolvimento atual: Beatriz Karoline.\r\n" +
            "Fonte: https://github.com/SouBeatrizKaroline/Game_GansOffice\r\n");
        Debug.Log("GANSOFFICE_BUILD_OK: " + destino + "; bytes: " + relatorio.summary.totalSize);
    }
}
