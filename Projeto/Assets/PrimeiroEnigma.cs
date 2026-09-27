using UnityEngine;

[RequireComponent(typeof(MovimentoGanzel), typeof(Rigidbody2D))]
public class PrimeiroEnigma : MonoBehaviour
{
    [SerializeField] private Transform gelatina;
    [SerializeField] private Transform relogio;
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
        if (gelatina == null || relogio == null || ambiente == null || cameraJogo == null)
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
            if (enigma.Concluido) return;
            if (bilhete) bilhete = false;
            else pausado = !pausado;
            AtualizarMovimento();
            return;
        }
        if ((bilhete || pausado) && !enigma.Concluido &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            bilhete = false;
            pausado = false;
            AtualizarMovimento();
            return;
        }
        if (pausado || bilhete || enigma.Concluido) return;
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
        if (!enigma.TemGelatina && !enigma.Concluido && Perto(gelatina))
            return "Pegar gelatina";
        if (Perto(relogio)) return enigma.TemGelatina ? "Usar gelatina no relógio" : "Examinar relógio";
        return null;
    }

    private void Interagir()
    {
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
                MostrarBilhete("A gelatina grudou no relógio. O tempo parou! Primeiro enigma concluído. " +
                    "A próxima etapa do roteiro é acordar a impressora com café — ainda em desenvolvimento.");
            }
            else MostrarBilhete("O relógio anda ao contrário. Só uma coisa estranha pode acabar com algo assim...");
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
        movimento.enabled = !pausado && !bilhete && !enigma.Concluido;
        corpo.velocity = Vector2.zero;
    }

    private void Reiniciar()
    {
        enigma.Reiniciar();
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
        string objetivo = enigma.Concluido ? "Enigma 1 concluído!" :
            enigma.TemGelatina ? "Objetivo: leve a gelatina até o relógio." : "Objetivo: explore o escritório. Encontre a gelatina.";
        GUI.Label(new Rect(38, 57, 1020, 48), objetivo, texto);
        Painel(new Rect(20, 657, 1060, 93));
        GUI.Label(new Rect(38, 670, 710, 30), "WASD / Setas: mover    E: interagir    Esc: pausa", texto);
        GUI.Label(new Rect(38, 706, 710, 30), "Inventário: " + (enigma.TemGelatina ? "Gelatina" : "vazio"), texto);
        if (!bilhete && !pausado && !enigma.Concluido)
        {
            string acao = AcaoDisponivel();
            if (acao != null && GUI.Button(new Rect(750, 674, 310, 60), "[E] " + acao, botao)) Interagir();
        }

        if (bilhete || pausado || enigma.Concluido)
        {
            Painel(new Rect(200, 214, 700, 330));
            GUI.Label(new Rect(228, 234, 644, 42), pausado ? "Pausa para o café" :
                enigma.Concluido ? "Tempo parado!" : "Um bilhete para Gander", titulo);
            GUI.Label(new Rect(228, 288, 644, 172), pausado ? "O escritório pode esperar. Continue quando quiser." : mensagem, texto);
            if (!enigma.Concluido && GUI.Button(new Rect(228, 468, 304, 52), "Continuar [Enter]", botao))
            {
                pausado = false;
                bilhete = false;
                AtualizarMovimento();
            }
            if (GUI.Button(new Rect(554, 468, 318, 52), "Recomeçar enigma", botao)) Reiniciar();
        }
        GUI.matrix = anterior;
    }

    private static void Painel(Rect retangulo)
    {
        Color anterior = GUI.color;
        GUI.color = new Color(0.07f, 0.16f, 0.18f, 1f);
        GUI.DrawTexture(retangulo, Texture2D.whiteTexture);
        GUI.color = anterior;
    }
}
