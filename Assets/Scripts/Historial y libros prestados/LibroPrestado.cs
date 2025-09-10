[System.Serializable]
public class LibroPrestado
{
    public string cliente;
    public string titulo;

    public LibroPrestado(string cliente, string titulo)
    {
        this.cliente = cliente;
        this.titulo = titulo;
    }
}
