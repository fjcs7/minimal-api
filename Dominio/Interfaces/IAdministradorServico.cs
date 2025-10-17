using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace minimal_api.Dominio.Interfaces;
public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}
