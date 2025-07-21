using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Infrastructure.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace GitHubSearchApp.Application.Services
{
    public class FavoritosService : IFavoritosService
    {
        private readonly IMemoryCache _cache;
        private const string FavoritosKey = "favoritos";

        public FavoritosService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<List<Repository>> ListarFavoritos()
        {
            var favoritos = ObterFavoritos();
            FileLogger.Log($"Listando favoritos. Total: {favoritos.Count}");
            return Task.FromResult(favoritos);
        }

        public List<Repository> ObterFavoritos()
        {
            if (!_cache.TryGetValue(FavoritosKey, out List<Repository>? favoritos))
            {
                favoritos = new List<Repository>();
                _cache.Set(FavoritosKey, favoritos);
            }

            return favoritos;
        }

        public Task AdicionarFavorito(Repository repo)
        {
            var favoritos = ObterFavoritos();

            if (!favoritos.Any(f => f.Id == repo.Id))
            {
                favoritos.Add(repo);
                _cache.Set(FavoritosKey, favoritos);
                FileLogger.Log($"Repositorio favoritado: {repo.Name} (ID: {repo.Id})");
            }
            else
            {
                FileLogger.Log($"Tentativa de favoritar repositorio ja existente: {repo.Name} (ID: {repo.Id})");
                throw new InvalidOperationException("Repositorio ja esta favoritado.");
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
    }
}
