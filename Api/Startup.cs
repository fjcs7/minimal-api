using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

public class Startup
{
    #region Constantes
    const string ADMINISTRADOR = "Administradores";
    const string HOME = "Home";
    const string VEICULOS = "Veículos";
    #endregion
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        jwtKey = Configuration.GetSection("Jwt").ToString() ?? "123456";
    }

    private string jwtKey;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
                                    {
                                        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                    }
                                  )
                .AddJwtBearer(option =>
                                        {
                                            option.TokenValidationParameters = new TokenValidationParameters
                                            {
                                                ValidateLifetime = true,
                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                                                ValidateIssuer = false,
                                                ValidateAudience = false
                                            };
                                        }
                            );

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
                                        {
                                            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                            {
                                                Name = "Authorization",
                                                Type = SecuritySchemeType.Http,
                                                Scheme = "bearer",
                                                BearerFormat = "JWT",
                                                In = ParameterLocation.Header,
                                                Description = "Insira o token JWT aqui:"
                                            });

                                            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                            {
                                                {
                                                    new OpenApiSecurityScheme
                                                    {
                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.SecurityScheme,
                                                            Id = "Bearer"
                                                        }
                                                    },
                                                    new string[] {}
                                                }
                                            });
                                        }
                                );

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                                Configuration.GetConnectionString("MySql"),
                                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
                            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            #region Home

            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags(HOME);

            #endregion

            #region Administradores
            string GerarJwtToken(Administrador administrador)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
            {
                Administrador adm = administradorServico.Login(loginDTO) ?? new Administrador();
                if (adm.Id != 0)
                {
                    string token = GerarJwtToken(adm);
                    return Results.Ok(new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags(ADMINISTRADOR);

            endpoints.MapPost("/administradores/", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
            {
                var validacao = administradorDTO.ValidaDTO();
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var novoAdministrador = administradorDTO.toAdministrador();
                administradorServico.Incluir(novoAdministrador);

                var novoAdmin = AdministradorModelView.ToAdministradorModelView(novoAdministrador);

                return Results.Created($"/administradores/{novoAdmin.Id}", novoAdmin);
            }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags(ADMINISTRADOR);

            endpoints.MapGet("/administradores/", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
            {
                var adms = new List<AdministradorModelView>();
                var administradores = administradorServico.Todos(pagina);

                foreach (var adm in administradores)
                {
                    adms.Add(AdministradorModelView.ToAdministradorModelView(adm));
                }

                return Results.Ok(adms);
            }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags(ADMINISTRADOR);

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {

                var administrador = administradorServico.BuscaId(id);

                if (administrador == null)
                    return Results.NotFound();

                return Results.Ok(AdministradorModelView.ToAdministradorModelView(administrador));
            }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags(ADMINISTRADOR);

            endpoints.MapPut("/administradores/{id}", ([FromRoute] int id, AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscaId(id);
                if (administrador == null)
                    return Results.NotFound();

                var validacao = administradorDTO.ValidaDTO();
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                administrador.Email = administradorDTO.Email;
                administrador.Senha = administradorDTO.Senha;
                administrador.Perfil = administradorDTO.Perfil.ToString() ?? string.Empty;

                administradorServico.Atualizar(administrador);

                return Results.Ok(administrador);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags(ADMINISTRADOR);

            #endregion

            #region  Veiculos
            endpoints.MapPost("/veiculos/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {
                var validacao = veiculoDTO.ValidaDTO();
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var novoVeiculo = veiculoDTO.toVeiculo();
                veiculoServico.Incluir(novoVeiculo);

                return Results.Created($"/veiculos/{novoVeiculo.Id}", novoVeiculo);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags(VEICULOS);

            endpoints.MapGet("/veiculos/", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
            {

                var veiculos = veiculoServico.Todos(pagina);

                return Results.Ok(veiculos);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags(VEICULOS);

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {

                var veiculo = veiculoServico.BuscaId(id);

                if (veiculo == null)
                    return Results.NotFound();

                return Results.Ok(veiculo);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags(VEICULOS);

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
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
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags(VEICULOS);

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {

                var veiculo = veiculoServico.BuscaId(id);
                if (veiculo == null)
                    return Results.NotFound();

                veiculoServico.Apagar(veiculo);

                return Results.NoContent();
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags(VEICULOS);

            #endregion
        });
    }
    


}