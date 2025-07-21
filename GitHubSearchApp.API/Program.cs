using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Interfaces;
using GitHubSearchApp.Infrastructure.Repositories;
using GitHubSearchApp.Application.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var swaggerConfig = builder.Configuration.GetSection("Swagger");

// Configuração de serviços
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGitHubRepository, GitHubRepository>();
builder.Services.AddSingleton<IGitHubService, GitHubService>();
builder.Services.AddSingleton<IFavoritosService, FavoritosService>();
builder.Services.AddSingleton<IRelevanciaService, RelevanciaService>();
builder.Services.AddSingleton<IRepositorioService, RepositorioService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(swaggerConfig["Version"] ?? "v1", new OpenApiInfo
    {
        Title = swaggerConfig["Title"] ?? "GitHub Search API",
        Version = swaggerConfig["Version"] ?? "v1"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Pipeline de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{swaggerConfig["Version"] ?? "v1"}/swagger.json", swaggerConfig["Title"] ?? "GitHub Search API");
    });
}

app.UseHttpsRedirection();
app.UseCors(); // Habilita CORS
app.UseAuthorization();
try
{
    app.MapControllers(); // Roteia para os controllers
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine(loaderException.Message);
    }
    throw; // Rethrow the exception after logging
}

app.Run();
