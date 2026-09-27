using UnityEngine;

[RequireComponent(typeof(MovimentoGanzel), typeof(Rigidbody2D))]
public class PrimeiroEnigma : MonoBehaviour
{
    [SerializeField] private Transform gelatina;
    [SerializeField] private Transform relogio;
    [SerializeField] private Transform cafeteira;
    [SerializeField] private Transform impressora;
    [SerializeField] private Transform calculadora;
    [SerializeField] private SpriteRenderer ambiente;
    [SerializeField] private Camera cameraJogo;
    [SerializeField, Min(0.1f)] private float alcance = 1.65f;

    private readonly EnigmaRelogio enigma = new EnigmaRelogio();
    private MovimentoGanzel movimento;
    private Rigidbody2D corpo;
    private SpriteRenderer arteGelatina;
    private Vector3 posicaoGelatina;
    private Vector3 inicio;
    private bool pausado;
    private bool calculadoraAberta;
    private string resposta = string.Empty;
    private bool bilhete = true;
    private string mensagem = "Se gosta de procrastinar, seu tempo irá rebobinar. " +
        "A saída do escritório terás que achar, e da forma normal é que não será!";
    private GUIStyle texto;
    private GUIStyle titulo;
    private GUIStyle botao;

    private void Awake()
    {
        movimento = GetComponent<MovimentoGanzel>();
        corpo = GetComponent<Rigidbody2D>();
        if (gelatina == null || relogio == null || ambiente == null || cameraJogo == null || cafeteira == null || impressora == null || calculadora == null)
        {
            Debug.LogError("PrimeiroEnigma: configure as referências da cena.", this);
            enabled = false;
            return;
        }
        inicio = transform.position;
        posicaoGelatina = gelatina.position;
        arteGelatina = gelatina.GetComponent<SpriteRenderer>();
        CriarLimites();
        AtualizarMovimento();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (enigma.Terminou) return;
            if (calculadoraAberta) { calculadoraAberta = false; AtualizarMovimento(); return; }
            if (bilhete) bilhete = false;
            else pausado = !pausado;
            AtualizarMovimento();
            return;
        }
        if ((bilhete || pausado) && !enigma.Terminou &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            bilhete = false;
            pausado = false;
            AtualizarMovimento();
            return;
        }
        if (pausado || bilhete || enigma.Terminou) return;
        if (calculadoraAberta) return;
        if (Input.GetKeyDown(KeyCode.E)) Interagir();
    }

    private void LateUpdate()
    {
        // Enquadrar todo o escritório, inclusive em janelas estreitas.
        Bounds area = ambiente.bounds;
        cameraJogo.orthographicSize = Mathf.Max(area.extents.y + 1.6f,
            (area.extents.x + 0.4f) / Mathf.Max(cameraJogo.aspect, 0.1f));
        cameraJogo.transform.position = new Vector3(area.center.x, area.center.y, -10f);
    }

    private bool Perto(Transform alvo)
    {
        // O relógio está na parede: a gelatina pode ser arremessada do corredor.
        float distancia = alvo == relogio ? alcance + 0.8f : alcance;
        return Vector2.Distance(corpo.position, alvo.position) <= distancia;
    }

    private void CriarLimites()
    {
        Bounds area = ambiente.bounds;
        float topo = area.max.y - 2.5f;
        CriarParede("Limite esquerdo", new Vector2(area.min.x, area.center.y), new Vector2(0.2f, area.size.y));
        CriarParede("Limite direito", new Vector2(area.max.x, area.center.y), new Vector2(0.2f, area.size.y));
        CriarParede("Limite inferior", new Vector2(area.center.x, area.min.y), new Vector2(area.size.x, 0.2f));
        CriarParede("Parede do fundo", new Vector2(area.center.x, topo), new Vector2(area.size.x, 0.2f));
    }

    private void CriarParede(string nome, Vector2 centro, Vector2 tamanho)
    {
        GameObject parede = new GameObject(nome);
        parede.transform.SetParent(ambiente.transform, true);
        parede.transform.position = centro;
        parede.AddComponent<BoxCollider2D>().size = tamanho;
    }

    private string AcaoDisponivel()
    {
        if (enigma.Concluido)
        {
            if (Perto(impressora)) return enigma.TemCafe ? "Colocar café na impressora" : "Examinar impressora";
            if (Perto(cafeteira)) return "Examinar cafeteira";
            if (Perto(calculadora)) return "Usar calculadora";
        }
        if (!enigma.TemGelatina && !enigma.Concluido && Perto(gelatina))
            return "Pegar gelatina";
        if (Perto(relogio)) return enigma.TemGelatina ? "Usar gelatina no relógio" : "Examinar relógio";
        return null;
    }

    private void Interagir()
    {
        if (enigma.Concluido && Perto(impressora))
        {
            if (enigma.AbastecerImpressora())
                MostrarBilhete("A impressora acordou! Ela imprime: ‘O que o jogo uniu, você precisa separar. O mais é menos e 290 é mais’. O ar-condicionado dispara. Procure a calculadora na mesa inferior direita.");
            else MostrarBilhete(enigma.ImpressoraResolvida ?
                "A pista impressa: ‘O que o jogo uniu, você precisa separar. O mais é menos e 290 é mais’." :
                "A impressora está sem tinta. Um bilhete diz: ‘Só o café faz acordar a vida’. Será que ela também precisa de uma xícara?");
            return;
        }
        if (enigma.Concluido && Perto(cafeteira))
        {
            MostrarBilhete(enigma.PegarCafe() ? "Você pegou uma xícara de café. Talvez a impressora aceite esta tinta incomum." :
                enigma.TemCafe ? "Você já está levando uma xícara de café." : "O café já acordou a impressora.");
            return;
        }
        if (enigma.Concluido && Perto(calculadora))
        {
            if (!enigma.ImpressoraResolvida) MostrarBilhete("A calculadora aguarda uma pista. Investigue a impressora primeiro.");
            else { calculadoraAberta = true; resposta = string.Empty; AtualizarMovimento(); }
            return;
        }
        if (!enigma.TemGelatina && Perto(gelatina) && enigma.PegarGelatina())
        {
            arteGelatina.enabled = false;
            MostrarBilhete("Na embalagem: ‘Eu endureço o tempo’. Talvez ela consiga parar aquele relógio...");
        }
        else if (Perto(relogio))
        {
            if (enigma.PararRelogio())
            {
                gelatina.position = relogio.position + new Vector3(0f, -0.12f, -0.1f);
                arteGelatina.enabled = true;
                MostrarBilhete("A gelatina grudou no relógio. O tempo parou! A impressora está apitando. Vá investigar.");
            }
            else MostrarBilhete(enigma.Concluido ? "O tempo está parado. Investigue a impressora." : "O relógio anda ao contrário. Só uma coisa estranha pode acabar com algo assim...");
        }
    }

    private void MostrarBilhete(string conteudo)
    {
        mensagem = conteudo;
        bilhete = true;
        AtualizarMovimento();
    }

    private void AtualizarMovimento()
    {
        movimento.enabled = !pausado && !bilhete && !calculadoraAberta && !enigma.Terminou;
        corpo.velocity = Vector2.zero;
    }

    private void Reiniciar()
    {
        enigma.Reiniciar();
        calculadoraAberta = false;
        resposta = string.Empty;
        transform.position = inicio;
        corpo.position = inicio;
        gelatina.position = posicaoGelatina;
        arteGelatina.enabled = true;
        pausado = false;
        MostrarBilhete("O relógio está rebobinando o tempo. Explore o escritório e encontre uma forma de pará-lo.");
    }

    private void OnApplicationFocus(bool foco)
    {
        if (!foco && movimento != null)
        {
            pausado = true;
            AtualizarMovimento();
        }
    }

    private void OnDisable()
    {
        if (movimento != null) movimento.enabled = true;
    }

    private void PrepararEstilos()
    {
        if (texto != null) return;
        texto = new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true };
        texto.normal.textColor = Color.white;
        titulo = new GUIStyle(texto) { fontSize = 25, fontStyle = FontStyle.Bold };
        botao = new GUIStyle(GUI.skin.button) { fontSize = 20, wordWrap = true };
    }

    private void OnGUI()
    {
        PrepararEstilos();
        Matrix4x4 anterior = GUI.matrix;
        float escala = Mathf.Min(Screen.width / 1100f, Screen.height / 760f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1100f * escala) / 2f,
            (Screen.height - 760f * escala) / 2f, 0f), Quaternion.identity, Vector3.one * escala);

        Painel(new Rect(20, 10, 1060, 102));
        GUI.Label(new Rect(38, 18, 760, 34), "GANSOFFICE  /  O tempo está ao contrário", titulo);
        string objetivo = enigma.Terminou ? "Use Recomeçar para jogar novamente." :
            enigma.ImpressoraResolvida ? "Objetivo: resolva a calculadora na mesa inferior direita. Vidas: " + enigma.Vidas :
            enigma.TemCafe ? "Objetivo: leve o café até a impressora." :
            enigma.Concluido ? "Objetivo: investigue a impressora e procure café." :
            enigma.TemGelatina ? "Objetivo: leve a gelatina até o relógio." : "Objetivo: explore o escritório. Encontre a gelatina.";
        GUI.Label(new Rect(38, 57, 1020, 48), objetivo, texto);
        Painel(new Rect(20, 657, 1060, 93));
        GUI.Label(new Rect(38, 670, 710, 30), "WASD / Setas: mover    E: interagir    Esc: pausa", texto);
        GUI.Label(new Rect(38, 706, 710, 30), "Inventário: " + (enigma.TemGelatina ? "Gelatina" : enigma.TemCafe ? "Café" : "vazio"), texto);
        if (!bilhete && !pausado && !calculadoraAberta && !enigma.Terminou)
        {
            string acao = AcaoDisponivel();
            if (acao != null && GUI.Button(new Rect(750, 674, 310, 60), "[E] " + acao, botao)) Interagir();
        }

        if (bilhete || pausado || enigma.Terminou)
        {
            Painel(new Rect(200, 214, 700, 330));
            GUI.Label(new Rect(228, 234, 644, 42), pausado ? "Pausa para o café" :
                enigma.Escapou ? "Escritório resolvido!" : enigma.Terminou ? "Frio demais!" : "Um bilhete para Gander", titulo);
            GUI.Label(new Rect(228, 288, 644, 172), pausado ? "O escritório pode esperar. Continue quando quiser." : mensagem, texto);
            if (!enigma.Terminou && GUI.Button(new Rect(228, 468, 304, 52), "Continuar [Enter]", botao))
            {
                pausado = false;
                bilhete = false;
                AtualizarMovimento();
            }
            if (GUI.Button(new Rect(554, 468, 318, 52), "Recomeçar enigma", botao)) Reiniciar();
        }
        if (calculadoraAberta && !pausado && !bilhete && !enigma.Terminou) DesenharCalculadora();
        GUI.matrix = anterior;
    }

    private void DesenharCalculadora()
    {
        Painel(new Rect(200, 154, 700, 450));
        GUI.Label(new Rect(228, 172, 644, 38), "Calculadora  /  Vidas: " + enigma.Vidas, titulo);
        GUI.Label(new Rect(228, 218, 644, 68), "O que o jogo uniu, você precisa separar. O mais é menos e 290 é mais.", texto);
        GUI.SetNextControlName("resposta");
        resposta = GUI.TextField(new Rect(228, 296, 644, 46), resposta, 5, botao);
        string teclas = "1234567890+-*/";
        for (int i = 0; i < teclas.Length; i++)
        {
            if (GUI.Button(new Rect(228 + (i % 7) * 92, 354 + (i / 7) * 50, 84, 42), teclas[i].ToString(), botao) && resposta.Length < 5)
                resposta += teclas[i];
        }
        bool enviar = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
        if (enviar) Event.current.Use();
        if (GUI.Button(new Rect(228, 466, 304, 48), "Confirmar [Enter]", botao) || enviar)
        {
            if (string.IsNullOrWhiteSpace(resposta)) return;
            bool acertou = enigma.ResolverCalculadora(resposta);
            calculadoraAberta = false;
            MostrarBilhete(acertou ? "A temperatura estabilizou e a porta destrancou! Você resolveu os três enigmas. Bodel espera no elevador. A transição para a cena final será integrada em uma próxima versão." :
                enigma.Terminou ? "O gato tem sete vidas e você tem três. O escritório congelou! Recomece e pense fora da caixa." :
                "Ficou mais frio! Vidas restantes: " + enigma.Vidas + ". Volte à calculadora e tente separar os algarismos de 290.");
        }
        if (GUI.Button(new Rect(554, 466, 152, 48), "Limpar", botao)) resposta = string.Empty;
        if (GUI.Button(new Rect(718, 466, 154, 48), "Voltar [Esc]", botao))
        { calculadoraAberta = false; AtualizarMovimento(); }
    }

    private static void Painel(Rect retangulo)
    {
        Color anterior = GUI.color;
        GUI.color = new Color(0.07f, 0.16f, 0.18f, 1f);
        GUI.DrawTexture(retangulo, Texture2D.whiteTexture);
        GUI.color = anterior;
    }
}
