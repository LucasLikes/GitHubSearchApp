using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Interfaces;
using GitHubSearchApp.Infrastructure.Repositories;
using GitHubSearchApp.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura��o de servi�os
builder.Services.AddHttpClient(); // Adiciona suporte a IHttpClientFactory

builder.Services.AddSingleton<IGitHubRepository, GitHubRepository>(); // Registra o reposit�rio GitHub
builder.Services.AddSingleton<IRepositorioService, RepositorioService>(); // Registra o servi�o de neg�cio

builder.Services.AddControllers(); // Habilita uso de controllers
builder.Services.AddEndpointsApiExplorer(); // Para documenta��o minimal APIs
builder.Services.AddSwaggerGen(); // Swagger/OpenAPI

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(); // Habilita CORS
app.UseAuthorization();

app.MapControllers(); // Roteia para os controllers

app.Run();
