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
    
    public void Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        if (pagina != null && pagina != 0)
        {
            int intensPorPagina = 10;
            query = query.Skip(((int)pagina - 1) * intensPorPagina).Take(intensPorPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscaId(int id)
    {
        return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
    }
    
    public void Atualizar(Administrador administrador)
    {
        _contexto.Administradores.Update(administrador);
        _contexto.SaveChanges();
    }
}
