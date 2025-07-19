using GitHubSearchApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubSearchApp.Application.Interfaces
{
    public interface IRepositorioService
    {
        Task<List<Repository>> BuscarRepositorios(string query);
        Task<List<Repository>> ListarRepositoriosDoUsuario(string username);
        Task AdicionarFavorito(Repository repo);
        Task RemoverFavorito(long repoId);
        Task<List<Repository>> ListarFavoritos();
    }
}
