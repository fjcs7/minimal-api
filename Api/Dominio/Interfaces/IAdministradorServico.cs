using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces;
public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    void Incluir(Administrador administrador);

    List<Administrador> Todos(int? pagina);

    Administrador? BuscaId(int id);

    void Atualizar(Administrador administrador);
}
