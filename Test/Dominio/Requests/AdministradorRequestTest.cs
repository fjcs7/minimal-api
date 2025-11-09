
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Dominio.Requests;

[TestClass]
public class AdministradorRequestTest
{

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.CassInit(testContext);
    }


    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestandoSalvarAdministrador()
    {
        //Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@test.com",
            Senha = "mock@123"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        //Act
        var response = await Setup.client.PostAsync("/administradores/login", content);


        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email);
        Assert.IsNotNull(admLogado?.Token);
        Assert.IsNotNull(admLogado?.Perfil);

    }


}
