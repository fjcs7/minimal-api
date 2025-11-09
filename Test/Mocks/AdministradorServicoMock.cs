using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Api.Test.Mocks;

public class AdministradorServicoMock : IAdministradorServico
{
    private static List<Administrador> administradores = new()
    {
        new Administrador
        {
            Id = 1,
            Email = "adm@test.com",
            Senha = "mock@123",
            Perfil = "Adm"
        },
        new Administrador
        {
            Id = 2,
            Email = "editor@test.com",
            Senha = "mock@123",
            Perfil = "Editor"
        },
    };
    public void Atualizar(Administrador administrador)
    {
        throw new NotImplementedException();
    }

    public Administrador? BuscaId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }

    public void Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count() + 1;
        administradores.Add(administrador);
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
         return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }

    public List<Administrador> Todos(int? pagina)
    {
        return administradores;
    }
}