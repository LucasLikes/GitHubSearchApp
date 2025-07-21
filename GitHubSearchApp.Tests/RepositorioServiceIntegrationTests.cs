//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using System.Net.Http.Json;
//using Microsoft.AspNetCore.Mvc.Testing;
//using GitHubSearchApp.API.DTOs;

//namespace GitHubSearchApp.Tests
//{
//    public class RepositorioServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
//    {
//        private readonly HttpClient _client;
//        private readonly WebApplicationFactory<Program> _factory;

//        public RepositorioServiceIntegrationTests(WebApplicationFactory<Program> factory)
//        {
//            _factory = factory;
//            _client = factory.CreateClient();
//        }

//        [Fact]
//        public async Task BuscarRepositorios_ShouldReturnRepos()
//        {
//            // Arrange
//            var query = "creditcard-payment-api";

//            // Act
//            var response = await _client.GetAsync($"/api/repos/search?q={query}");

//            // Assert
//            response.EnsureSuccessStatusCode();
//            var repos = await response.Content.ReadAsStringAsync();

//            // Verifica se a resposta contém um nome de repositório
//            Assert.Contains("name", repos);
//            Assert.Contains("html_url", repos);
//            Assert.Contains("stars", repos);
//        }

//        //[Fact]
//        //public async Task AdicionarFavorito_ShouldAddRepoToFavorites()
//        //{
//        //    // Arrange
//        //    var repo = new RepositoryRequestDTO
//        //    {
//        //        Id = 1,
//        //        Name = "Test Repo",
//        //        HtmlUrl = "https://github.com/test/repo",
//        //        Stars = 100,
//        //        Forks = 50,
//        //        Watchers = 20
//        //    };

//        //    // Act
//        //    var response = await _client.PostAsJsonAsync("/api/favoritos", repo);

//        //    // Assert
//        //    response.EnsureSuccessStatusCode();
//        //    var result = await response.Content.ReadAsStringAsync();
//        //    Assert.Contains("Repositorio favoritado", result); // Verifica a resposta
//        //}
//    }
//}
