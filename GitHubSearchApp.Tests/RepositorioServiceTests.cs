using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Application.Services;
using GitHubSearchApp.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace GitHubSearchApp.Tests
{
    public class RepositorioServiceTests
    {
        private readonly Mock<IGitHubService> _gitHubServiceMock;
        private readonly Mock<IFavoritosService> _favoritosServiceMock;
        private readonly Mock<IRelevanciaService> _relevanciaServiceMock;
        private readonly RepositorioService _service;

        private readonly List<Repository> _fakeFavoritosList;

        public RepositorioServiceTests()
        {
            _gitHubServiceMock = new Mock<IGitHubService>();
            _favoritosServiceMock = new Mock<IFavoritosService>();
            _relevanciaServiceMock = new Mock<IRelevanciaService>();
            _fakeFavoritosList = new List<Repository>();

            // Mock do serviço de favoritos
            _favoritosServiceMock.Setup(f => f.ObterFavoritos())
                .Returns(() => _fakeFavoritosList);

            _favoritosServiceMock.Setup(f => f.AdicionarFavorito(It.IsAny<Repository>()))
                .Callback<Repository>((repo) =>
                {
                    if (_fakeFavoritosList.Any(f => f.Id == repo.Id))
                        throw new InvalidOperationException("Repositorio ja esta favoritado.");
                    _fakeFavoritosList.Add(repo);
                });

            _favoritosServiceMock.Setup(f => f.RemoverFavorito(It.IsAny<long>()))
                .Callback<long>((id) =>
                {
                    var repo = _fakeFavoritosList.FirstOrDefault(f => f.Id == id);
                    if (repo != null) _fakeFavoritosList.Remove(repo);
                });

            _favoritosServiceMock.Setup(f => f.ListarFavoritos())
                .ReturnsAsync(() => _fakeFavoritosList.ToList());

            // Mock do serviço de relevância - configuração padrão
            _relevanciaServiceMock.Setup(r => r.OrdenarRepositoriosPorRelevancia(It.IsAny<List<Repository>>()))
                .Returns<List<Repository>>(repos => repos ?? new List<Repository>());

            _service = new RepositorioService(
                _gitHubServiceMock.Object,
                _favoritosServiceMock.Object,
                _relevanciaServiceMock.Object
            );
        }

        [Fact]
        public async Task AdicionarFavorito_DeveAdicionarRepositorios()
        {
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", 10, 2, 5);

            await _service.AdicionarFavorito(repo);
            var favoritos = await _service.ListarFavoritos();

            Assert.Single(favoritos);
            Assert.Contains(favoritos, r => r.Id == 1);
        }

        [Fact]
        public async Task AdicionarFavorito_Duplicado_DeveLancarExcecao()
        {
            var repo = new Repository(1, "Repo1", "Desc1", "http://url1", 10, 2, 5);
            _fakeFavoritosList.Add(repo);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarFavorito(repo));

            Assert.Equal("Repositorio ja esta favoritado.", ex.Message);
        }

        [Fact]
        public async Task RemoverFavorito_IdInexistente_NaoLancaErro()
        {
            var exception = await Record.ExceptionAsync(() => _service.RemoverFavorito(999));
            Assert.Null(exception);
        }

        [Fact]
        public async Task BuscarRepositorios_ErroNaApi_DeveLancarExcecao()
        {
            _gitHubServiceMock.Setup(s => s.BuscarRepositorios(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Erro na API"));

            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.BuscarRepositorios("qualquer"));

            Assert.Contains("Erro na API", ex.Message);
        }

        [Fact]
        public async Task ListarRepositoriosDoUsuario_SemRepositorios_DeveRetornarListaVazia()
        {
            // Configura o GitHub Service para retornar uma lista vazia
            _gitHubServiceMock.Setup(s => s.ListarRepositoriosDoUsuario("usuario-invalido"))
                .ReturnsAsync(new List<Repository>());

            // Configura o serviço de relevância para retornar a mesma lista vazia
            _relevanciaServiceMock.Setup(r => r.OrdenarRepositoriosPorRelevancia(It.IsAny<List<Repository>>()))
                .Returns<List<Repository>>(repos => repos ?? new List<Repository>());

            var result = await _service.ListarRepositoriosDoUsuario("usuario-invalido");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListarRepositoriosDoUsuario_ComNull_DeveRetornarListaVazia()
        {
            // Testa o cenário onde o GitHub Service retorna null
            _gitHubServiceMock.Setup(s => s.ListarRepositoriosDoUsuario("usuario-invalido"))
                .ReturnsAsync((List<Repository>)null);

            // Configura o serviço de relevância para lidar com lista vazia
            _relevanciaServiceMock.Setup(r => r.OrdenarRepositoriosPorRelevancia(It.IsAny<List<Repository>>()))
                .Returns<List<Repository>>(repos => repos ?? new List<Repository>());

            var result = await _service.ListarRepositoriosDoUsuario("usuario-invalido");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListarRepositoriosDoUsuario_DeveOrdenarPorFormulaCustomizada()
        {
            var repos = new List<Repository>
            {
                new Repository(1, "A", "D", "url", 1, 2, 3), // total = 6 (stars + forks + watchers)
                new Repository(2, "B", "D", "url", 5, 0, 0), // total = 5
                new Repository(3, "C", "D", "url", 0, 5, 5)  // total = 10
            };

            _gitHubServiceMock.Setup(s => s.ListarRepositoriosDoUsuario(It.IsAny<string>()))
                .ReturnsAsync(repos);

            // Mock do serviço de relevância para ordenar por stars + forks + watchers (decrescente)
            _relevanciaServiceMock.Setup(r => r.OrdenarRepositoriosPorRelevancia(It.IsAny<List<Repository>>()))
                .Returns<List<Repository>>(reposList =>
                    reposList?.OrderByDescending(r => r.Stars + r.Forks + r.Watchers).ToList() ?? new List<Repository>());

            var result = await _service.ListarRepositoriosDoUsuario("user");

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].Id); // primeiro com 10 pontos (0+5+5)
            Assert.Equal(1, result[1].Id); // segundo com 6 pontos (1+2+3)
            Assert.Equal(2, result[2].Id); // terceiro com 5 pontos (5+0+0)
        }

        [Fact]
        public async Task BuscarRepositorios_DeveUsarServicoRelevancia()
        {
            var repos = new List<Repository>
            {
                new Repository(1, "Repo1", "Desc1", "url1", 10, 2, 5),
                new Repository(2, "Repo2", "Desc2", "url2", 5, 3, 2)
            };

            var reposOrdenados = new List<Repository>
            {
                new Repository(2, "Repo2", "Desc2", "url2", 5, 3, 2),
                new Repository(1, "Repo1", "Desc1", "url1", 10, 2, 5)
            };

            _gitHubServiceMock.Setup(s => s.BuscarRepositorios("test"))
                .ReturnsAsync(repos);

            _relevanciaServiceMock.Setup(r => r.OrdenarRepositoriosPorRelevancia(repos))
                .Returns(reposOrdenados);

            var result = await _service.BuscarRepositorios("test");

            Assert.Equal(reposOrdenados, result);
            _relevanciaServiceMock.Verify(r => r.OrdenarRepositoriosPorRelevancia(repos), Times.Once);
        }
    }
}