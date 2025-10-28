using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Dominio.DTOs;
public record AdministradorDTO
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public Perfil? Perfil { get; set; }

    public Administrador toAdministrador()
    {

        return new Administrador()
        {
            Email = this.Email,
            Senha = this.Senha,
            Perfil = (this.Perfil ?? Enuns.Perfil.Editor).ToString()
        };
    }

    public ErrosDeValidacao ValidaDTO()
    {
        var validacao = new ErrosDeValidacao();
        if (string.IsNullOrEmpty(this.Email))
            validacao.Mensagens.Add("O email não pode ser vazio.");

        if (string.IsNullOrEmpty(this.Senha))
            validacao.Mensagens.Add("A senha não pode ser vazia.");

        if (this.Perfil ==  null)
            validacao.Mensagens.Add("A perfil não pode ser vazio.");

        return validacao;
    }
}
