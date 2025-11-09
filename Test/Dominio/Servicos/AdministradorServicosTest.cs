
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Dominio.Servicos;

[TestClass]
public class AdministradorServicosTest
{
    private DbContexto context = default!;

    private void CriarContextoTest()
    {
        if(context != null)
           return;
    
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings-test.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuracao = builder.Build();

       context = new DbContexto(configuracao);
    }

    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        //Arrange
        CriarContextoTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "senha_teste_123";
        adm.Perfil = "Adm";

        
        var administradorServico = new AdministradorServico(context);

        //Act
        administradorServico.Incluir(adm);


        //Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());

    }

    [TestMethod]
    public void TestandoBuscarAdministradorPorId()
    {
        //Arrange
        CriarContextoTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "senha_teste_123";
        adm.Perfil = "Adm";


        var administradorServico = new AdministradorServico(context);

        //Act
        administradorServico.Incluir(adm);
        var administradorDb = administradorServico.BuscaId(1) ?? new Administrador();


        //Assert
        Assert.AreEqual(1, administradorDb.Id);
        Assert.AreEqual(adm.Email, administradorDb.Email);
        Assert.AreEqual(adm.Senha, administradorDb.Senha);
        Assert.AreEqual(adm.Perfil, administradorDb.Perfil);

    }
    
    public void Dispose()
    {
        context.Dispose();
    }
}
