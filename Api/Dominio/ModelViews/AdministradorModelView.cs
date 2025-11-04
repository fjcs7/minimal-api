using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;

namespace MinimalApi.Dominio.ModelViews;

public record AdministradorModelView
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;

    public static AdministradorModelView ToAdministradorModelView(Administrador adm)
    {
        return new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        };
    }
}