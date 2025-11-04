namespace MinimalApi.Dominio.ModelViews;

public class ErrosDeValidacao
{
    public ErrosDeValidacao()
    {
        Mensagens = new List<string>();
    }
    public List<string> Mensagens { get; set; }
}