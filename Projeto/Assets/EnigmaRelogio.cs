// Regras independentes da cena para permitir testes sem o editor.
public sealed class EnigmaRelogio
{
    public bool TemGelatina { get; private set; }
    public bool Concluido { get; private set; }
    public bool TemCafe { get; private set; }
    public bool ImpressoraResolvida { get; private set; }
    public bool Escapou { get; private set; }
    public int Vidas { get; private set; } = 3;
    public bool Terminou { get { return Escapou || Vidas == 0; } }

    public bool PegarCafe()
    {
        if (!Concluido || TemCafe || ImpressoraResolvida || Terminou) return false;
        TemCafe = true;
        return true;
    }

    public bool AbastecerImpressora()
    {
        if (!TemCafe || ImpressoraResolvida || Terminou) return false;
        TemCafe = false;
        ImpressoraResolvida = true;
        return true;
    }

    public bool ResolverCalculadora(string resposta)
    {
        if (!ImpressoraResolvida || Terminou || string.IsNullOrWhiteSpace(resposta)) return false;
        if (resposta.Trim() == "2+9+0")
        {
            Escapou = true;
            return true;
        }
        Vidas--;
        return false;
    }

    public bool PegarGelatina()
    {
        if (TemGelatina || Concluido) return false;
        TemGelatina = true;
        return true;
    }

    public bool PararRelogio()
    {
        if (!TemGelatina || Concluido) return false;
        TemGelatina = false;
        Concluido = true;
        return true;
    }

    public void Reiniciar()
    {
        TemGelatina = false;
        Concluido = false;
        TemCafe = false;
        ImpressoraResolvida = false;
        Escapou = false;
        Vidas = 3;
    }
}
