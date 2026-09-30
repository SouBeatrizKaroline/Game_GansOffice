using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Usa exclusivamente a arte, os sons e as cinematics originais do projeto.
public sealed class ApresentacaoGansOffice : MonoBehaviour
{
    public Sprite[] caminhada;
    public Sprite costas;
    public Texture2D bodel;
    public VideoClip abertura;
    public VideoClip final;
    public AudioClip somRelogio, somImpressora, somFrio, somBotao, somAmbiente;
    public bool Ativa { get; private set; }
    public bool Pausada { get; private set; }
    public bool FinalExibido { get; private set; }

    private SpriteRenderer personagem;
    private Rigidbody2D corpo;
    private Sprite repouso;
    private VideoPlayer video;
    private AudioSource relogio, frio, efeitos, ambiente, audioVideo;
    private RenderTexture tela;
    private Action terminouVideo;
    private float preparacao, tempoPassos;
    private bool preparando;
    private Coroutine arremesso;
    private Transform item;
    private Vector3 escalaItem;

    private void Awake()
    {
        personagem = GetComponent<SpriteRenderer>();
        corpo = GetComponent<Rigidbody2D>();
        repouso = personagem.sprite;
        relogio = CriarAudio(true);
        frio = CriarAudio(true);
        efeitos = CriarAudio(false);
        ambiente = CriarAudio(true);
        audioVideo = CriarAudio(false);
        video = gameObject.AddComponent<VideoPlayer>();
        video.playOnAwake = false;
        video.isLooping = false;
        video.renderMode = VideoRenderMode.RenderTexture;
        video.audioOutputMode = VideoAudioOutputMode.AudioSource;
        video.SetTargetAudioSource(0, audioVideo);
        video.loopPointReached += AoTerminarVideo;
        video.errorReceived += AoFalharVideo;
        video.prepareCompleted += AoPrepararVideo;
    }

    private AudioSource CriarAudio(bool repetir)
    {
        var fonte = gameObject.AddComponent<AudioSource>();
        fonte.playOnAwake = false;
        fonte.loop = repetir;
        fonte.volume = repetir ? 0.25f : 0.65f;
        fonte.spatialBlend = 0f;
        return fonte;
    }

    public void TocarAbertura(Action aoTerminar) { TocarVideo(abertura, aoTerminar); }
    public void TocarFinal(Action aoTerminar)
    {
        FinalExibido = true;
        TocarVideo(final, aoTerminar);
    }

    private void TocarVideo(VideoClip clip, Action aoTerminar)
    {
        if (clip == null) { aoTerminar?.Invoke(); return; }
        Ativa = true;
        terminouVideo = aoTerminar;
        preparando = true;
        preparacao = 0f;
        if (tela == null)
        {
            tela = new RenderTexture(1280, 720, 0);
            tela.Create();
        }
        video.targetTexture = tela;
        video.clip = clip;
        video.Prepare();
    }

    private void AoPrepararVideo(VideoPlayer fonte)
    {
        if (!Ativa) return;
        preparando = false;
        fonte.Play();
        if (Pausada) fonte.Pause();
    }

    private void AoTerminarVideo(VideoPlayer fonte) { EncerrarVideo(); }
    private void AoFalharVideo(VideoPlayer fonte, string erro)
    {
        Debug.LogWarning("Não foi possível reproduzir a cinematic: " + erro, this);
        EncerrarVideo();
    }

    public void EncerrarVideo()
    {
        if (!Ativa || arremesso != null) return;
        video.Stop();
        preparando = false;
        Ativa = false;
        var callback = terminouVideo;
        terminouVideo = null;
        callback?.Invoke();
    }

    public void Pausar(bool pausa)
    {
        if (Pausada == pausa) return;
        Pausada = pausa;
        foreach (var fonte in new[] { relogio, frio, ambiente, efeitos, audioVideo })
            if (pausa) fonte.Pause(); else fonte.UnPause();
        if (Ativa && arremesso == null && !preparando)
            if (pausa) video.Pause(); else video.Play();
    }

    public void AtualizarEstado(EnigmaRelogio enigma)
    {
        AtualizarLoop(relogio, somRelogio, !enigma.Concluido && !Ativa);
        AtualizarLoop(frio, somFrio, enigma.ImpressoraResolvida && !enigma.Terminou && !Ativa);
        AtualizarLoop(ambiente, somAmbiente, !Ativa && !enigma.Terminou);
        frio.volume = 0.15f + (3 - enigma.Vidas) * 0.12f;
        personagem.color = Color.Lerp(Color.white, new Color(0.45f, 0.75f, 1f),
            enigma.ImpressoraResolvida && !enigma.Escapou ? (3 - enigma.Vidas) / 3f : 0f);
    }

    private void AtualizarLoop(AudioSource fonte, AudioClip clip, bool tocar)
    {
        if (fonte.clip != clip) fonte.clip = clip;
        if (!tocar) { fonte.Stop(); return; }
        if (!Pausada && !fonte.isPlaying && clip != null) fonte.Play();
    }

    public void Imprimir() { if (somImpressora != null) efeitos.PlayOneShot(somImpressora); }
    public void Tecla() { if (somBotao != null) efeitos.PlayOneShot(somBotao); }

    public void Arremessar(Transform gelatina, Vector3 destino, Action aoTerminar)
    {
        Ativa = true;
        item = gelatina;
        escalaItem = item.localScale;
        arremesso = StartCoroutine(AnimarArremesso(destino, aoTerminar));
    }

    private IEnumerator AnimarArremesso(Vector3 destino, Action aoTerminar)
    {
        Vector3 origem = transform.position + Vector3.up * 0.5f;
        item.GetComponent<SpriteRenderer>().enabled = true;
        float tempo = 0f;
        while (tempo < 0.65f)
        {
            if (!Pausada) tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / 0.65f);
            item.position = Vector3.Lerp(origem, destino, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * 1.2f;
            yield return null;
        }
        item.position = destino;
        tempo = 0f;
        while (tempo < 0.3f)
        {
            if (!Pausada) tempo += Time.deltaTime;
            float deformacao = Mathf.Sin(tempo * 35f) * (1f - Mathf.Clamp01(tempo / 0.3f)) * 0.22f;
            item.localScale = Vector3.Scale(escalaItem, new Vector3(1f + deformacao, 1f - deformacao, 1f));
            yield return null;
        }
        item.localScale = escalaItem;
        arremesso = null;
        Ativa = false;
        aoTerminar?.Invoke();
    }

    private void Update()
    {
        if (preparando && !Pausada)
        {
            preparacao += Time.unscaledDeltaTime;
            if (preparacao > 15f) EncerrarVideo();
        }
        Vector2 direcao = corpo.linearVelocity;
        if (Ativa || Pausada || direcao.sqrMagnitude < 0.01f)
        {
            tempoPassos = 0f;
            if (personagem.sprite != costas) personagem.sprite = repouso;
            return;
        }
        tempoPassos += Time.deltaTime;
        if (Mathf.Abs(direcao.x) > 0.1f) personagem.flipX = direcao.x < 0f;
        if (direcao.y > Mathf.Abs(direcao.x) && costas != null) personagem.sprite = costas;
        else if (caminhada != null && caminhada.Length > 0)
            personagem.sprite = caminhada[(int)(tempoPassos * 12f) % caminhada.Length];
        else personagem.sprite = repouso;
    }

    public void Reiniciar()
    {
        terminouVideo = null;
        video.Stop();
        if (arremesso != null) StopCoroutine(arremesso);
        if (item != null) item.localScale = escalaItem;
        arremesso = null;
        preparando = false;
        Ativa = false;
        FinalExibido = false;
        Pausar(false);
        efeitos.Stop();
        personagem.sprite = repouso;
        personagem.flipX = false;
        personagem.color = Color.white;
    }

    public void DesenharVideo()
    {
        if (!Ativa) return;
        if (arremesso != null)
        {
            if (Pausada && GUI.Button(new Rect(Screen.width / 2f - 120, Screen.height / 2f - 25, 240, 50),
                "Continuar arremesso [Esc]")) Pausar(false);
            return;
        }
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.blackTexture);
        if (!preparando && tela != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tela, ScaleMode.ScaleToFit);
        if (GUI.Button(new Rect(Screen.width - 210, Screen.height - 60, 190, 40),
            Pausada ? "Continuar [Esc]" : "Pular cena [Enter]"))
            if (Pausada) Pausar(false); else EncerrarVideo();
    }

    private void OnDestroy()
    {
        video.loopPointReached -= AoTerminarVideo;
        video.errorReceived -= AoFalharVideo;
        video.prepareCompleted -= AoPrepararVideo;
        if (tela != null) { video.targetTexture = null; tela.Release(); Destroy(tela); }
    }
}
