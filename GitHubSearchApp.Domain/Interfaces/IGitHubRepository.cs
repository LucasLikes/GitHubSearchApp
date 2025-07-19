using GitHubSearchApp.Domain.Entities;

namespace GitHubSearchApp.Domain.Interfaces
{
    public interface IGitHubRepository
    {
        Task<IReadOnlyList<Repository>> SearchAsync(string query);
        Task<IReadOnlyList<Repository>> GetUserRepositoriesAsync(string username);
    }
}
