using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Execute com -batchmode -executeMethod QAGansOffice.Run (sem -quit).
[InitializeOnLoad]
public static class QAGansOffice
{
    private const string Sessao = "GansOffice.QA";
    private static int quadros;
    private static readonly List<string> resultados = new List<string>();
    private static int falhas;
    private static readonly BindingFlags Campos = BindingFlags.Instance | BindingFlags.NonPublic;

    static QAGansOffice()
    {
        if (SessionState.GetBool(Sessao, false))
            EditorApplication.update += Aguardar;
    }

    public static void Run()
    {
        SessionState.SetBool(Sessao, true);
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        EditorApplication.update -= Aguardar;
        EditorApplication.update += Aguardar;
        EditorApplication.isPlaying = true;
    }

    private static void Aguardar()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        if (++quadros < 30) return;
        EditorApplication.update -= Aguardar;
        try { Verificar(); }
        catch (Exception erro) { falhas++; resultados.Add("FAIL: " + erro); }
        SessionState.SetBool(Sessao, false);
        string destino = Environment.GetEnvironmentVariable("GANSOFFICE_QA_REPORT");
        if (!string.IsNullOrEmpty(destino)) File.WriteAllLines(destino, resultados);
        foreach (string resultado in resultados) Debug.Log(resultado);
        Debug.Log("GANSOFFICE_QA_DONE: falhas=" + falhas);
        EditorApplication.Exit(falhas == 0 ? 0 : 1);
    }

    private static T Campo<T>(object objeto, string nome)
    {
        return (T)objeto.GetType().GetField(nome, Campos).GetValue(objeto);
    }

    private static void Definir(object objeto, string nome, object valor)
    {
        objeto.GetType().GetField(nome, Campos).SetValue(objeto, valor);
    }

    private static object Chamar(object objeto, string nome, params object[] args)
    {
        return objeto.GetType().GetMethod(nome, Campos).Invoke(objeto, args);
    }

    private static void Exigir(bool valor, string motivo)
    {
        if (!valor) throw new InvalidOperationException(motivo);
    }

    private static void Caso(string nome, Action verificar)
    {
        try { verificar(); resultados.Add("PASS: " + nome); }
        catch (Exception erro) { falhas++; resultados.Add("FAIL: " + nome + ": " + erro.GetBaseException().Message); }
    }

    private static void Verificar()
    {
        var jogo = UnityEngine.Object.FindFirstObjectByType<PrimeiroEnigma>();
        Exigir(jogo != null && jogo.enabled, "Cena sem controlador ativo.");
        var apresentacao = jogo.GetComponent<ApresentacaoGansOffice>();
        var movimento = jogo.GetComponent<MovimentoGanzel>();
        var corpo = jogo.GetComponent<Rigidbody2D>();
        var enigma = Campo<EnigmaRelogio>(jogo, "enigma");
        apresentacao.EncerrarVideo();

        Caso("bilhete inicial bloqueia movimento", () => Exigir(!movimento.enabled, "Movimento liberado durante bilhete."));
        Caso("objetos do enigma acessiveis com as colisoes reais", () => Alcance(jogo));
        Caso("perda de foco preserva calculadora e pausa", () =>
        {
            Definir(jogo, "bilhete", false);
            Definir(jogo, "calculadoraAberta", true);
            Chamar(jogo, "AtualizarMovimento");
            Chamar(jogo, "OnApplicationFocus", false);
            Exigir(Campo<bool>(jogo, "pausado") && !movimento.enabled && apresentacao.Pausada, "Perda de foco nao pausou.");
            Chamar(jogo, "OnApplicationFocus", true);
            Exigir(Campo<bool>(jogo, "pausado"), "Retorno de foco retomou sem acao do jogador.");
            Chamar(jogo, "TratarEscape");
            Exigir(!Campo<bool>(jogo, "pausado") && Campo<bool>(jogo, "calculadoraAberta") && !movimento.enabled, "Esc descartou calculadora ao retomar pausa.");
            Chamar(jogo, "TratarEscape");
            Exigir(!Campo<bool>(jogo, "calculadoraAberta") && movimento.enabled, "Esc nao fechou calculadora apos retomar.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("pausa preserva bilhete ate o segundo Esc", () =>
        {
            Chamar(jogo, "OnApplicationFocus", false);
            Chamar(jogo, "TratarEscape");
            Exigir(!Campo<bool>(jogo, "pausado") && Campo<bool>(jogo, "bilhete") && !movimento.enabled, "Pista perdida ao retomar pausa.");
            Chamar(jogo, "TratarEscape");
            Exigir(!Campo<bool>(jogo, "bilhete") && movimento.enabled, "Bilhete nao fechou.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("Continuar preserva pista ao retomar pausa", () =>
        {
            Chamar(jogo, "OnApplicationFocus", false);
            Chamar(jogo, "Continuar");
            Exigir(!Campo<bool>(jogo, "pausado") && Campo<bool>(jogo, "bilhete") && !movimento.enabled, "Continuar descartou pista durante pausa.");
            Chamar(jogo, "Continuar");
            Exigir(!Campo<bool>(jogo, "bilhete") && movimento.enabled, "Continuar nao fechou pista apos retomar.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("derrota nao e substituida por pausa ao perder foco", () =>
        {
            Chamar(jogo, "Reiniciar");
            enigma.PegarGelatina(); enigma.PararRelogio(); enigma.PegarCafe(); enigma.AbastecerImpressora();
            for (int i = 0; i < 3; i++) enigma.ResolverCalculadora("290");
            Chamar(jogo, "OnApplicationFocus", false);
            Exigir(!Campo<bool>(jogo, "pausado"), "A tela final ficou presa na pausa.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("vitoria nao e substituida por pausa ao perder foco", () =>
        {
            Chamar(jogo, "Reiniciar");
            enigma.PegarGelatina(); enigma.PararRelogio(); enigma.PegarCafe(); enigma.AbastecerImpressora();
            enigma.ResolverCalculadora("2+9+0");
            Exigir(enigma.Escapou, "Preparacao de vitoria falhou.");
            Chamar(jogo, "OnApplicationFocus", false);
            Exigir(!Campo<bool>(jogo, "pausado"), "Vitoria ocultada por pausa sem continuacao.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("reinicio cancela arremesso pausado e restaura item", () =>
        {
            Transform gelatina = Campo<Transform>(jogo, "gelatina");
            Vector3 escala = gelatina.localScale;
            apresentacao.Arremessar(gelatina, Vector3.zero, () => { throw new Exception("Callback antigo executado."); });
            apresentacao.Pausar(true);
            Chamar(jogo, "Reiniciar");
            Exigir(!apresentacao.Ativa && !apresentacao.Pausada && Campo<Coroutine>(apresentacao, "arremesso") == null, "Arremesso nao cancelado.");
            Exigir(gelatina.position == Campo<Vector3>(jogo, "posicaoGelatina") && gelatina.localScale == escala && gelatina.GetComponent<SpriteRenderer>().enabled, "Item nao restaurado.");
            Exigir(enigma.Vidas == 3 && !enigma.Terminou && !movimento.enabled, "Estado inicial nao restaurado.");
        });
        Caso("calculadora ignora resposta vazia e envio durante pausa", () =>
        {
            PrepararCalculadora(jogo);
            Definir(jogo, "resposta", "   ");
            Chamar(jogo, "ConfirmarResposta");
            Exigir(enigma.Vidas == 3 && Campo<bool>(jogo, "calculadoraAberta"), "Vazio consumiu vida ou fechou calculadora.");
            Definir(jogo, "resposta", "290");
            Chamar(jogo, "OnApplicationFocus", false);
            Chamar(jogo, "ConfirmarResposta");
            Exigir(enigma.Vidas == 3, "Resposta enviada durante pausa.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("erros da interface levam a derrota e bloqueiam envios duplicados", () =>
        {
            PrepararCalculadora(jogo);
            for (int vidas = 2; vidas >= 0; vidas--)
            {
                Definir(jogo, "bilhete", false);
                Definir(jogo, "calculadoraAberta", true);
                Definir(jogo, "resposta", "290");
                Chamar(jogo, "ConfirmarResposta");
                Exigir(enigma.Vidas == vidas && Campo<bool>(jogo, "bilhete") && !movimento.enabled, "Erro nao atualizou vida, bilhete ou movimento.");
                Chamar(jogo, "ConfirmarResposta");
                Exigir(enigma.Vidas == vidas, "Mesmo envio consumiu mais de uma vida.");
            }
            Exigir(enigma.Terminou && !enigma.Escapou && enigma.Temperatura == 0, "Derrota inconsistente.");
            Chamar(jogo, "Reiniciar");
        });
        Caso("acerto exibe final e reinicio permite nova partida", () =>
        {
            PrepararCalculadora(jogo);
            Definir(jogo, "resposta", "2+9+0");
            Chamar(jogo, "ConfirmarResposta");
            Exigir(enigma.Escapou && enigma.Temperatura == 29 && !movimento.enabled && apresentacao.FinalExibido, "Vitoria nao iniciou final.");
            apresentacao.EncerrarVideo();
            Exigir(Campo<bool>(jogo, "bilhete") && Campo<string>(jogo, "mensagem").Contains("Bodel"), "Final sem mensagem de Bodel.");
            Chamar(jogo, "Reiniciar");
            Exigir(!enigma.Escapou && !apresentacao.FinalExibido && corpo.position == (Vector2)Campo<Vector3>(jogo, "inicio"), "Reinicio incompleto.");
        });
        Caso("desativar controlador interrompe movimento", () =>
        {
            Definir(jogo, "bilhete", false);
            Chamar(jogo, "AtualizarMovimento");
            corpo.linearVelocity = Vector2.one;
            jogo.enabled = false;
            Exigir(!movimento.enabled && corpo.linearVelocity == Vector2.zero, "Controlador desativado liberou personagem.");
            Exigir(apresentacao.Pausada, "Desativacao nao pausou apresentacao.");
            jogo.enabled = true;
            Exigir(movimento.enabled && !apresentacao.Pausada, "Reativacao nao restaurou estado jogavel.");
        });
    }

    private static void PrepararCalculadora(PrimeiroEnigma jogo)
    {
        Chamar(jogo, "Reiniciar");
        var enigma = Campo<EnigmaRelogio>(jogo, "enigma");
        enigma.PegarGelatina(); enigma.PararRelogio(); enigma.PegarCafe(); enigma.AbastecerImpressora();
        Definir(jogo, "bilhete", false);
        Definir(jogo, "calculadoraAberta", true);
        Chamar(jogo, "AtualizarMovimento");
    }

    private static void Alcance(PrimeiroEnigma jogo)
    {
        var corpo = jogo.GetComponent<Rigidbody2D>();
        var pes = jogo.GetComponent<BoxCollider2D>();
        Exigir(pes != null, "Colisor dos pes ausente.");
        var ambiente = Campo<SpriteRenderer>(jogo, "ambiente").bounds;
        Vector2 escala = jogo.transform.lossyScale;
        Vector2 tamanho = Vector2.Scale(pes.size, escala);
        Vector2 deslocamento = Vector2.Scale(pes.offset, escala);
        Vector2 inicio = corpo.position;
        const float passo = 0.12f;
        int largura = Mathf.CeilToInt(ambiente.size.x / passo) + 1;
        int altura = Mathf.CeilToInt(ambiente.size.y / passo) + 1;
        var visitados = new HashSet<Vector2Int>();
        var fila = new Queue<Vector2Int>();
        var alcancados = new HashSet<string>();
        string[] alvos = { "gelatina", "relogio", "cafeteira", "impressora", "calculadora" };
        Vector2Int[] passos = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var consulta = new ContactFilter2D { useTriggers = false };
        var colisoes = new Collider2D[64];
        Physics2D.SyncTransforms();
        // Grade ancorada no spawn real, com colisor deslocado para os pes.
        fila.Enqueue(Vector2Int.zero);
        visitados.Add(Vector2Int.zero);
        while (fila.Count > 0)
        {
            Vector2Int celula = fila.Dequeue();
            Vector2 posicao = inicio + (Vector2)celula * passo;
            foreach (string alvo in alvos)
            {
                var objeto = Campo<Transform>(jogo, alvo);
                float alcance = Campo<float>(jogo, "alcance") + (alvo == "relogio" ? 0.8f : 0f);
                if (Vector2.Distance(posicao, objeto.position) <= alcance) alcancados.Add(alvo);
            }
            foreach (var direcao in passos)
            {
                Vector2Int proxima = celula + direcao;
                if (Mathf.Abs(proxima.x) > largura || Mathf.Abs(proxima.y) > altura || visitados.Contains(proxima)) continue;
                visitados.Add(proxima);
                Vector2 centro = inicio + (Vector2)proxima * passo + deslocamento;
                int quantidade = Physics2D.OverlapBox(centro, tamanho, 0f, consulta, colisoes);
                bool bloqueado = quantidade == colisoes.Length;
                for (int i = 0; i < quantidade; i++)
                    if (colisoes[i] != pes) bloqueado = true;
                if (!bloqueado) fila.Enqueue(proxima);
            }
        }
        foreach (string alvo in alvos) Exigir(alcancados.Contains(alvo), "Objeto inacessivel: " + alvo);
    }
}
