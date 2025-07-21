using GitHubSearchApp.Application.Services;
using GitHubSearchApp.Domain.Interfaces;
using Moq;

namespace GitHubSearchApp.Tests
{
    public class GitHubRepositoryErrorTests
    {
        private readonly Mock<IGitHubRepository> _gitHubRepositoryMock;

        public GitHubRepositoryErrorTests()
        {
            _gitHubRepositoryMock = new Mock<IGitHubRepository>();
        }

        [Fact]
        public async Task BuscarRepositorios_ShouldThrowHttpRequestException_WhenGitHubReturnsError()
        {
            // Arrange: Simular resposta de erro da API (500 Internal Server Error)
            _gitHubRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Internal Server Error"));

            var service = new GitHubService(_gitHubRepositoryMock.Object);

            // Act & Assert: Verificar se o erro é tratado corretamente
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => service.BuscarRepositorios("test-repo"));
            Assert.Equal("Internal Server Error", ex.Message);
        }
    }
}
