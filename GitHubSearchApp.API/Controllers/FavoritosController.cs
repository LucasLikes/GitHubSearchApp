using GitHubSearchApp.API.DTOs;
using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using GitHubSearchApp.Infrastructure.Logging;


[ApiController]
[Route("api/[controller]")]
public class FavoritosController : ControllerBase
{
    private readonly IRepositorioService _repositorioService;

    public FavoritosController(IRepositorioService repositorioService)
    {
        _repositorioService = repositorioService;
    }

    /// <summary>
    /// Lista os repositórios favoritos atuais.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFavoritos()
    {
        var favoritos = await _repositorioService.ListarFavoritos();

        var result = favoritos.Select(r => new RepositoryResponseDTO
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

    /// <summary>
    /// Marca um repositório como favorito.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Favoritar([FromBody] RepositoryRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var repo = new Repository(dto.Id, dto.Name, dto.Description, dto.HtmlUrl, dto.Stars, dto.Forks, dto.Watchers);
            await _repositorioService.AdicionarFavorito(repo);
            return Ok(new { sucesso = true, mensagem = "Repositorio favoritado." });
        }
        catch (InvalidOperationException ex)
        {
            FileLogger.LogError( ex, $"Erro ao favoritar repositorio com ID: {dto.Id}\n{ex.Message}\n{ex.StackTrace}");
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            FileLogger.LogError(ex, $"Erro inesperado ao favoritar repositorio com ID: {dto.Id}");
            return StatusCode(500, new { sucesso = false, mensagem = "Erro interno no servidor." });
        }
    }

    /// <summary>
    /// Remove um repositório dos favoritos.
    /// </summary>
    [HttpDelete("{repoId:long}")]
    public async Task<IActionResult> RemoveFav(long repoId)
    {
        try
        {
            await _repositorioService.RemoverFavorito(repoId);
            return Ok(new { sucesso = true, mensagem = "Repositorio removido dos favoritos com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            FileLogger.LogError(ex, $"Erro ao remover repositorio favorito com ID: {repoId}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            FileLogger.LogError(ex, $"inesperado ao remover repositorio favorito com ID: {repoId}");
            return StatusCode(500, new { sucesso = false, mensagem = "Erro interno no servidor." });
        }
    }
}
