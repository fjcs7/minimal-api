using System.Text;
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

        var validacaoPerfil = VerificaRangeDoPerfil();
        if(!string.IsNullOrEmpty(validacaoPerfil))
            validacao.Mensagens.Add(validacaoPerfil);

        return validacao;
    }
    
    private string VerificaRangeDoPerfil()
    {
        if (this.Perfil == null)
        {
            return "A perfil não pode ser vazio.";
        }

        StringBuilder errosDeValidacao = new();
        Enum.TryParse(typeof(Perfil), Perfil.ToString(), out var resultado);

        if (!Enum.IsDefined(typeof(Perfil), resultado ?? -1))
        {
            errosDeValidacao.Append("Valor definido para o perfil fora do range aceito: ");
            
             int maiorValorNumerico = Enum.GetValues(typeof(Perfil))
                                     .Cast<int>()
                                     .Max();
            foreach (int item in Enum.GetValues(typeof(Perfil)))
            {
                string espaco = string.Empty;
                if (item < maiorValorNumerico)
                    espaco = ", ";
                else
                    espaco = ".";

                errosDeValidacao.Append($"'{item} - {(Perfil)(item)}'{espaco}");
            }
        }

        return errosDeValidacao.ToString();
    }
}
