// GitHubRepositoryTimeoutTest.cs
using GitHubSearchApp.Application.Services;
using GitHubSearchApp.Domain.Interfaces;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GitHubSearchApp.Tests
{
    public class GitHubRepositoryTimeoutTests
    {
        private readonly Mock<IGitHubRepository> _gitHubRepositoryMock;

        public GitHubRepositoryTimeoutTests()
        {
            _gitHubRepositoryMock = new Mock<IGitHubRepository>();
        }

        [Fact]
        public async Task BuscarRepositorios_ShouldThrowTimeoutException_WhenGitHubApiIsDown()
        {
            // Arrange: Simular falha de rede (timeout)
            _gitHubRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Request Timeout"));

            var service = new GitHubService(_gitHubRepositoryMock.Object);

            // Act & Assert: Verificar se a exceção de timeout é lançada
            await Assert.ThrowsAsync<HttpRequestException>(() => service.BuscarRepositorios("test-repo"));
        }
    }
}
