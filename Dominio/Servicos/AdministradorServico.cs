using System.Data.Common;
using Microsoft.EntityFrameworkCore.Metadata;
using minimal_api.Dominio.Interfaces;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos;

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
