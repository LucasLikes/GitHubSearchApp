using GitHubSearchApp.Domain.Entities;

namespace GitHubSearchApp.Application.Interfaces
{
    public interface IFavoritosService
    {
        List<Repository> ObterFavoritos();
        Task<List<Repository>> ListarFavoritos();
        Task AdicionarFavorito(Repository repo);
        Task RemoverFavorito(long repoId);
    }
}
