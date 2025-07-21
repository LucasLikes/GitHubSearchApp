using GitHubSearchApp.Domain.Entities;

namespace GitHubSearchApp.Application.Interfaces
{
    public interface IRelevanciaService
    {
        int CalcularRelevancia(Repository repo);
        List<Repository> OrdenarRepositoriosPorRelevancia(List<Repository> repos);
    }
}
