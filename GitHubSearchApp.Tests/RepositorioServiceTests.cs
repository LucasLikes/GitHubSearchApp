using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubSearchApp.Application.Services;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;
using Moq;
using Xunit;

namespace GitHubSearchApp.Tests
{
    public class RepositorioServiceTests
    {
        private readonly Mock<IGitHubRepository> _gitHubRepoMock;
        private readonly RepositorioService _service;

        public RepositorioServiceTests()
        {
            _gitHubRepoMock = new Mock<IGitHubRepository>();
            _service = new RepositorioService(_gitHubRepoMock.Object);
        }

        [Fact]
        public async Task BuscarRepositorios_DeveOrdenarPorRelevancia()
        {
            // Arrange
            var repos = new List<Repository>
            {
                new Repository(1, "Repo1", "Desc1", "http://url1", stars: 10, forks: 2, watchers: 5), // score = 10*2+2+5 = 27
                new Repository(2, "Repo2", "Desc2", "http://url2", stars: 5, forks: 10, watchers: 10), // score = 5*2+10+10=30
                new Repository(3, "Repo3", "Desc3", "http://url3", stars: 20, forks: 1, watchers: 1) // score = 20*2+1+1=42
            };
            _gitHubRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>())).ReturnsAsync(repos);

            // Act
            var result = await _service.BuscarRepositorios("query");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].Id); // Repo3 com maior score vem primeiro
            Assert.Equal(2, result[1].Id); // Repo2 segundo
            Assert.Equal(1, result[2].Id); // Repo1 último
        }

        [Fact]
        public async Task AdicionarFavorito_DeveAdicionarRepositorios()
        {
            // Arrange
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", stars: 10, forks: 2, watchers: 5);

            // Act
            await _service.AdicionarFavorito(repo);
            var favoritos = await _service.ListarFavoritos();

            // Assert
            Assert.Single(favoritos);
            Assert.Contains(favoritos, r => r.Id == 1);
        }

        [Fact]
        public async Task AdicionarFavorito_Duplicado_DeveLancarExcecao()
        {
            // Arrange
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", stars: 10, forks: 2, watchers: 5);
            await _service.AdicionarFavorito(repo);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.AdicionarFavorito(repo);
            });
            Assert.Equal("Repositório já está favoritado.", ex.Message);
        }
    }
}
