using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    public Administrador? Login(LoginDTO login)
    {
        var administrador = _contexto
                .Administradores.Where(
                    adm => adm.Email.Equals(login.Email)
                        && adm.Senha.Equals(login.Senha));

        return administrador.FirstOrDefault();
                        
    }
}
