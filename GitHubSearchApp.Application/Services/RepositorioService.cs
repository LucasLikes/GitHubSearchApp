using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Infrastructure.Logging;

namespace GitHubSearchApp.Application.Services
{
    public class RepositorioService : IRepositorioService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IFavoritosService _favoritosService;
        private readonly IRelevanciaService _relevanciaService;

        public RepositorioService(
            IGitHubService gitHubService,
            IFavoritosService favoritosService,
            IRelevanciaService relevanciaService)
        {
            _gitHubService = gitHubService;
            _favoritosService = favoritosService;
            _relevanciaService = relevanciaService;
        }

        public async Task<List<Repository>> BuscarRepositorios(string query)
        {
            try
            {
                FileLogger.Log($"Iniciando busca por repositorios com query: {query}");
                var repos = await _gitHubService.BuscarRepositorios(query);
                var reposOrdenados = _relevanciaService.OrdenarRepositoriosPorRelevancia(repos);
                return reposOrdenados;
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Erro ao buscar repositorios para a query: {query}");
                throw;
            }
        }

        public async Task<List<Repository>> ListarRepositoriosDoUsuario(string username)
        {
            var repos = await _gitHubService.ListarRepositoriosDoUsuario(username);
            return _relevanciaService.OrdenarRepositoriosPorRelevancia(repos ?? new List<Repository>());
        }

        public Task AdicionarFavorito(Repository repo)
        {
            _favoritosService.AdicionarFavorito(repo);
            return Task.CompletedTask;
        }

        public Task RemoverFavorito(long repoId)
        {
            _favoritosService.RemoverFavorito(repoId);
            return Task.CompletedTask;
        }

        public Task<List<Repository>> ListarFavoritos()
        {
            var favoritos = _favoritosService.ObterFavoritos();
            return Task.FromResult(favoritos);
        }
    }
}