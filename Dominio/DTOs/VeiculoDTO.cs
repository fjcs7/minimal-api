using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.DTOs;
public record VeiculoDTO
{
    public string Nome { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public int Ano { get; set; } = default;

    public Veiculo toVeiculo()
    {
        return new Veiculo()
        {
            Nome = this.Nome,
            Marca = this.Marca,
            Ano = this.Ano
        };
    }
}
