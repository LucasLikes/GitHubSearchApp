using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
        return Ok(favoritos);
    }

    /// <summary>
    /// Marca um repositório como favorito.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Favoritar([FromBody] Repository repo)
    {
        try
        {
            await _repositorioService.AdicionarFavorito(repo);
            return Ok("Repositório favoritado.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
