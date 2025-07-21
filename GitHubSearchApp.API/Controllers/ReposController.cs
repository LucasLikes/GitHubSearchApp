using Microsoft.AspNetCore.Mvc;
using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.API.DTOs;
using GitHubSearchApp.Infrastructure.Logging;

namespace GitHubSearchApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReposController : ControllerBase
{
    private readonly IRepositorioService _repositorioService;

    public ReposController(IRepositorioService repositorioService)
    {
        _repositorioService = repositorioService;
    }

    /// <summary>
    /// Busca repositórios públicos do GitHub pelo nome.
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do repositório</param>
    /// <returns>Lista de repositórios ordenados por relevância</returns>
    [HttpGet("me")]
    public async Task<IActionResult> BuscarRepos([FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest("Nome é obrigatório.");

        try
        {
            var repos = await _repositorioService.BuscarRepositorios(nome);
            var result = repos.Select(r => new RepositoryResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                HtmlUrl = r.HtmlUrl,
                Stars = r.Stars,
                Forks = r.Forks,
                Watchers = r.Watchers
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            FileLogger.LogError(ex, $"Erro ao buscar repositórios com o nome: {nome}");
            return StatusCode(500, "Erro interno no servidor.");
        }
    }
}
