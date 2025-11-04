using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;

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

    public ErrosDeValidacao ValidaDTO()
    {
        var validacao = new ErrosDeValidacao();
        if (string.IsNullOrEmpty(this.Nome))
            validacao.Mensagens.Add("O nome não pode ser vazio.");

        if (string.IsNullOrEmpty(this.Marca))
            validacao.Mensagens.Add("A marca não pode ser vazia.");

        if (this.Ano < 1950)
            validacao.Mensagens.Add("Veículo muito antigo, aceito somente veículos com ano superior a 1950.");

        return validacao;
    }
}
