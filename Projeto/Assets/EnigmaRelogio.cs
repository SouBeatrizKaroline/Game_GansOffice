// Regras independentes da cena para permitir testes sem o editor.
public sealed class EnigmaRelogio
{
    public bool TemGelatina { get; private set; }
    public bool Concluido { get; private set; }

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
    }
}
