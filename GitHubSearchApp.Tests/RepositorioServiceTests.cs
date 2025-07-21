using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubSearchApp.Application.Services;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace GitHubSearchApp.Tests
{
    public class RepositorioServiceTests
    {
        private readonly Mock<IGitHubRepository> _gitHubRepoMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly RepositorioService _service;

        private List<Repository> _fakeCacheStorage;

        public RepositorioServiceTests()
        {
            _gitHubRepoMock = new Mock<IGitHubRepository>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _fakeCacheStorage = new List<Repository>();

            // Mock para o TryGetValue - simula o comportamento de cache.
            object? dummy;
            _memoryCacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy!))
                .Returns((object key, out object? value) =>
                {
                    value = _fakeCacheStorage;  // Retorna o valor armazenado no "cache".
                    return _fakeCacheStorage != null;
                });

            // Simula a adição ao "cache" sem mockar Set() diretamente.
            _memoryCacheMock.Setup(c => c.CreateEntry(It.IsAny<object>()))
                .Returns(new Mock<ICacheEntry>().Object);

            _service = new RepositorioService(_gitHubRepoMock.Object, _memoryCacheMock.Object);
        }

        [Fact]
        public async Task BuscarRepositorios_DeveOrdenarPorRelevancia()
        {
            // Arrange
            var repos = new List<Repository>
            {
                new Repository(1, "Repo1", "Desc1", "http://url1", 10, 2, 5), // 27
                new Repository(2, "Repo2", "Desc2", "http://url2", 5, 10, 10), // 30
                new Repository(3, "Repo3", "Desc3", "http://url3", 20, 1, 1) // 42
            };
            _gitHubRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>())).ReturnsAsync(repos);

            // Act
            var result = await _service.BuscarRepositorios("query");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].Id);  // Repo3 com maior score vem primeiro
            Assert.Equal(2, result[1].Id);  // Repo2 segundo
            Assert.Equal(1, result[2].Id);  // Repo1 último
        }

        [Fact]
        public async Task AdicionarFavorito_DeveAdicionarRepositorios()
        {
            // Arrange
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", 10, 2, 5);

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
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", 10, 2, 5);
            await _service.AdicionarFavorito(repo);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarFavorito(repo));
            Assert.Equal("Repositório já está favoritado.", ex.Message);
        }

        [Fact]
        public async Task RemoverFavorito_IdInexistente_NaoLancaErro()
        {
            // Act
            var exception = await Record.ExceptionAsync(() => _service.RemoverFavorito(999));

            // Assert
            Assert.Null(exception); // Método deve falhar silenciosamente
        }

        [Fact]
        public async Task BuscarRepositorios_ErroNaApi_DeveLancarExcecao()
        {
            // Arrange
            _gitHubRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Erro na API"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.BuscarRepositorios("qualquer"));
            Assert.Contains("Erro na API", ex.Message);
        }

        [Fact]
        public async Task ListarRepositoriosDoUsuario_SemRepositorios_DeveRetornarListaVazia()
        {
            // Arrange
            _gitHubRepoMock.Setup(r => r.GetUserRepositoriesAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Repository>());

            // Act
            var result = await _service.ListarRepositoriosDoUsuario("usuario-invalido");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListarRepositoriosDoUsuario_DeveOrdenarPorFormulaCustomizada()
        {
            // Arrange
            var repos = new List<Repository>
            {
                new Repository(1, "A", "D", "url", 1, 2, 3), // 7
                new Repository(2, "B", "D", "url", 5, 0, 0), // 10
                new Repository(3, "C", "D", "url", 0, 5, 5)  // 10
            };
            _gitHubRepoMock.Setup(r => r.GetUserRepositoriesAsync(It.IsAny<string>()))
                .ReturnsAsync(repos);

            // Act
            var result = await _service.ListarRepositoriosDoUsuario("user");

            // Assert
            Assert.Equal(2, result[0].Id); // 10 pontos, primeiro por ordem
            Assert.Equal(3, result[1].Id); // 10 pontos, depois
            Assert.Equal(1, result[2].Id); // 7 pontos, último
        }
    }
}
