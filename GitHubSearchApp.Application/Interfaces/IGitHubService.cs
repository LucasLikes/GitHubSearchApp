using GitHubSearchApp.Domain.Entities;

namespace GitHubSearchApp.Application.Interfaces
{
    public interface IGitHubService
    {
        Task<List<Repository>> BuscarRepositorios(string query);
        Task<List<Repository>> ListarRepositoriosDoUsuario(string username);
    }
}
