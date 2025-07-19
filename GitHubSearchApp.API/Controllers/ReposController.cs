using Microsoft.AspNetCore.Mvc;
using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;

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

        var repos = await _repositorioService.BuscarRepositorios(nome);

        return Ok(repos);
    }
}
