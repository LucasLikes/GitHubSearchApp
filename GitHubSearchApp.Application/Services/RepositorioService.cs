using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Application.Utils;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;
using GitHubSearchApp.Infrastructure.Logging;
using Microsoft.Extensions.Caching.Memory;


namespace GitHubSearchApp.Application.Services
{
    public class RepositorioService : IRepositorioService
    {
        private readonly IGitHubRepository _gitHubRepository;
        private readonly IMemoryCache _cache;
        private const string FavoritosKey = "favoritos";
        public RepositorioService(IGitHubRepository gitHubRepository, IMemoryCache cache)
        {
            _gitHubRepository = gitHubRepository;
            _cache = cache;
        }

        public async Task<List<Repository>> BuscarRepositorios(string query)
        {
            FileLogger.Log($"Iniciando busca por repositorios com query: {query}");

            try
            {
                var repos = await _gitHubRepository.SearchAsync(query);
                var ordenados = repos.OrderByDescending(r => RelevanceCalculator.Calcular(r)).ToList();

                FileLogger.Log($"Busca finalizada. {ordenados.Count} repositorios encontrados.");
                return ordenados;
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Erro ao buscar repositorios para a query: {query}");
                throw;
            }
        }

        public Task AdicionarFavorito(Repository repo)
        {
            var favoritos = ObterFavoritos();

            if (!favoritos.Any(f => f.Id == repo.Id))
            {
                favoritos.Add(repo);
                _cache.Set(FavoritosKey, favoritos);
                FileLogger.Log($"Repositório favoritado: {repo.Name} (ID: {repo.Id})");
            }
            else
            {
                FileLogger.Log($"Tentativa de favoritar repositório já existente: {repo.Name} (ID: {repo.Id})");
                throw new InvalidOperationException("Repositório já está favoritado.");
            }

            return Task.CompletedTask;
        }

        public Task RemoverFavorito(long repoId)
        {
            var favoritos = ObterFavoritos();

            var repo = favoritos.FirstOrDefault(f => f.Id == repoId);
            if (repo != null)
            {
                favoritos.Remove(repo);
                _cache.Set(FavoritosKey, favoritos);
                FileLogger.Log($"Repositorio removido dos favoritos: {repo.Name} (ID: {repo.Id})");
            }
            else
            {
                FileLogger.Log($"Tentativa de remover favorito que nao existe. ID: {repoId}");
            }

            return Task.CompletedTask;
        }

        public Task<List<Repository>> ListarFavoritos()
        {
            var favoritos = ObterFavoritos();
            FileLogger.Log($"Listando favoritos. Total: {favoritos.Count}");
            return Task.FromResult(favoritos);
        }

        public async Task<List<Repository>> ListarRepositoriosDoUsuario(string username)
        {
            FileLogger.Log($"Buscando repositorios do usuario: {username}");

            try
            {
                var repos = await _gitHubRepository.GetUserRepositoriesAsync(username);
                var ordenados = repos.OrderByDescending(r => r.Stars * 2 + r.Forks + r.Watchers).ToList();

                FileLogger.Log($"Foram encontrados {ordenados.Count} repositorios para o usrio: {username}");
                return ordenados;
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Erro ao buscar repositorios do usuario: {username}");
                throw;
            }
        }

        private List<Repository> ObterFavoritos()
        {
            if (!_cache.TryGetValue(FavoritosKey, out List<Repository>? favoritos))
            {
                favoritos = new List<Repository>();
                _cache.Set(FavoritosKey, favoritos);
            }

            return favoritos;
        }
    }
}