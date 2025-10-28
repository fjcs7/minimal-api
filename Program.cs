using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

#region Auxiliares
const string ADMINISTRADOR = "Administradores";
const string HOME = "Home";
const string VEICULOS = "Veículos";
#endregion

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
options.UseMySql(
    builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});
var app = builder.Build();

#endregion

#region Home

app.MapGet("/", () => Results.Json(new Home())).WithTags(HOME);

#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
}).WithTags(ADMINISTRADOR);

app.MapPost("/administradores/", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = administradorDTO.ValidaDTO();
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var novoAdministrador = administradorDTO.toAdministrador();
    administradorServico.Incluir(novoAdministrador);

    var novoAdmin = AdministradorModelView.ToAdministradorModelView(novoAdministrador);

    return Results.Created($"/administradores/{novoAdmin.Id}", novoAdmin);
}).WithTags(ADMINISTRADOR);

app.MapGet("/administradores/", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach(var adm in administradores)
    {
        adms.Add(AdministradorModelView.ToAdministradorModelView(adm));
    }

    return Results.Ok(adms);
}).WithTags(ADMINISTRADOR);

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{

    var administrador = administradorServico.BuscaId(id);

    if (administrador == null)
        return Results.NotFound();

    return Results.Ok(AdministradorModelView.ToAdministradorModelView(administrador));
}).WithTags(ADMINISTRADOR);

#endregion

#region  Veiculos
app.MapPost("/veiculos/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = veiculoDTO.ValidaDTO();
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    
    var novoVeiculo = veiculoDTO.toVeiculo();
    veiculoServico.Incluir(novoVeiculo);

    return Results.Created($"/veiculos/{novoVeiculo.Id}", novoVeiculo);
}).WithTags(VEICULOS);

app.MapGet("/veiculos/", ([FromQuery] int? pagina,  IVeiculoServico veiculoServico) => {

    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags(VEICULOS);

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{

    var veiculo = veiculoServico.BuscaId(id);

    if (veiculo == null)
        return Results.NotFound();

    return Results.Ok(veiculo);
}).WithTags(VEICULOS);

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{

    var veiculo = veiculoServico.BuscaId(id);
    if (veiculo == null)
        return Results.NotFound();

    var validacao = veiculoDTO.ValidaDTO();
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags(VEICULOS);

app.MapDelete("/veiculos/{id}", ([FromRoute] int id,  IVeiculoServico veiculoServico) => {

    var veiculo = veiculoServico.BuscaId(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags(VEICULOS);

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

