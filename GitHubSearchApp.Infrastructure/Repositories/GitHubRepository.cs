using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubSearchApp.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação da interface IGitHubRepository que faz chamadas HTTP para a API pública do GitHub.
    /// </summary>
    public class GitHubRepository : IGitHubRepository
    {
        private readonly HttpClient _httpClient;

        public GitHubRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("GitHubSearchApp", "1.0"));
            }

            if (!_httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")))
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            }
        }

        /// <summary>
        /// Pesquisa repositórios públicos no GitHub com base em uma query.
        /// </summary>
        public async Task<IReadOnlyList<Repository>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query não pode ser vazia.", nameof(query));

            var requestUri = $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(query)}";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro ao buscar repositórios: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            var searchResult = JsonSerializer.Deserialize<GitHubSearchResult>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var list = new List<Repository>();

            foreach (var item in searchResult?.Items ?? new List<GitHubRepoItem>())
            {
                if (string.IsNullOrWhiteSpace(item.name) || string.IsNullOrWhiteSpace(item.html_url))
                    continue;

                list.Add(new Repository(
                    id: item.id,
                    name: item.name,
                    description: item.description ?? string.Empty,
                    htmlUrl: item.html_url,
                    stars: item.stargazers_count,
                    forks: item.forks_count,
                    watchers: item.watchers_count));
            }

            return list;
        }

        /// <summary>
        /// Retorna os repositórios públicos de um usuário específico do GitHub.
        /// </summary>
        public async Task<IReadOnlyList<Repository>> GetUserRepositoriesAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Nome de usuário não pode ser vazio.", nameof(username));

            var requestUri = $"https://api.github.com/users/{Uri.EscapeDataString(username)}/repos";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro ao buscar repositórios do usuário: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            var repoItems = JsonSerializer.Deserialize<List<GitHubRepoItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var list = new List<Repository>();

            foreach (var item in repoItems ?? new List<GitHubRepoItem>())
            {
                if (string.IsNullOrWhiteSpace(item.name) || string.IsNullOrWhiteSpace(item.html_url))
                    continue;

                list.Add(new Repository(
                    id: item.id,
                    name: item.name,
                    description: item.description ?? string.Empty,
                    htmlUrl: item.html_url,
                    stars: item.stargazers_count,
                    forks: item.forks_count,
                    watchers: item.watchers_count));
            }

            return list;
        }

        // Classes auxiliares para desserialização
        public class GitHubSearchResult
        {
            public int TotalCount { get; set; }
            public bool IncompleteResults { get; set; }
            public List<GitHubRepoItem> Items { get; set; } = new ();
        }

        public class GitHubRepoItem
        {
            public long id { get; set; }
            public string name { get; set; } = string.Empty;
            public string html_url { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public int stargazers_count { get; set; }
            public int forks_count { get; set; }
            public int watchers_count { get; set; }
        }
    }
}

