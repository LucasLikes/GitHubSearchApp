// GitHubSearchApp.Application.Services.GitHubService.cs
using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;


namespace GitHubSearchApp.Application.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly IGitHubRepository _gitHubRepository;

        public GitHubService(IGitHubRepository gitHubRepository)
        {
            _gitHubRepository = gitHubRepository;
        }

        public async Task<List<Repository>> BuscarRepositorios(string query)
        {
            var repos = await _gitHubRepository.SearchAsync(query);
            return new List<Repository>(repos);
        }

        public async Task<List<Repository>> ListarRepositoriosDoUsuario(string username)
        {
            var repos = await _gitHubRepository.GetUserRepositoriesAsync(username);
            return new List<Repository>(repos);
        }
    }
}
