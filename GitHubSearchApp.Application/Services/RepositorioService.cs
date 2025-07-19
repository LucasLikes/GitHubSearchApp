using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;


namespace GitHubSearchApp.Application.Services
{
    public class RepositorioService : IRepositorioService
    {
        private readonly IGitHubRepository _gitHubRepository;
        private readonly List<Repository> _favoritos = new();

        public RepositorioService(IGitHubRepository gitHubRepository)
        {
            _gitHubRepository = gitHubRepository;
        }

        public async Task<List<Repository>> BuscarRepositorios(string query)
        {
            var repos = await _gitHubRepository.SearchAsync(query);

            // L�gica de relev�ncia: estrelas * 2 + forks + watchers
            return repos.OrderByDescending(r => r.Stars * 2 + r.Forks + r.Watchers).ToList();
        }

        // Outros m�todos usando a lista _favoritos...

        public Task AdicionarFavorito(Repository repo)
        {
            if (!_favoritos.Any(f => f.Id == repo.Id))
                _favoritos.Add(repo);
            else
                throw new InvalidOperationException("Reposit�rio j� est� favoritado.");

            return Task.CompletedTask;
        }

        public Task RemoverFavorito(long repoId)
        {
            var repo = _favoritos.FirstOrDefault(f => f.Id == repoId);
            if (repo != null)
                _favoritos.Remove(repo);

            return Task.CompletedTask;
        }

        public Task<List<Repository>> ListarFavoritos()
        {
            return Task.FromResult(_favoritos.ToList());
        }

        public async Task<List<Repository>> ListarRepositoriosDoUsuario(string username)
        {
            var repos = await _gitHubRepository.GetUserRepositoriesAsync(username);

            // Ordenar tamb�m por relev�ncia, se quiser reutilizar l�gica:
            return repos.OrderByDescending(r => r.Stars * 2 + r.Forks + r.Watchers).ToList();
        }
    }
}