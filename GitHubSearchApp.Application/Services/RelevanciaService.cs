using GitHubSearchApp.Application.Interfaces;
using GitHubSearchApp.Domain.Entities;
using GitHubSearchApp.Infrastructure.Logging;

namespace GitHubSearchApp.Application.Services
{
    /// <summary>
    /// Serviço responsável por calcular a relevância de repositórios
    /// e ordená-los com base nesse critério.
    /// </summary>
    public class RelevanciaService : IRelevanciaService
    {
        /// <summary>
        /// Calcula a relevância de um repositório com base em suas estrelas, forks e watchers.
        /// Fórmula: (Stars * 2) + Forks + Watchers
        /// </summary>
        /// <param name="repo">Repositório para cálculo da relevância.</param>
        /// <returns>Valor inteiro representando a relevância do repositório.</returns>
        public int CalcularRelevancia(Repository repo)
        {
            return repo.Stars * 2 + repo.Forks + repo.Watchers;
        }

        /// <summary>
        /// Ordena uma lista de repositórios por relevância, da maior para a menor.
        /// A ordenação é baseada no resultado de <see cref="CalcularRelevancia"/>.
        /// </summary>
        /// <param name="repos">Lista de repositórios a serem ordenados.</param>
        /// <returns>Lista de repositórios ordenada por relevância.</returns>
        public List<Repository> OrdenarRepositoriosPorRelevancia(List<Repository> repos)
        {
            FileLogger.Log("Iniciando ordenação por relevância...");
            var reposOrdenados = repos.OrderByDescending(r => CalcularRelevancia(r)).ToList();
            FileLogger.Log("Ordenação por relevância finalizada.");
            return reposOrdenados;
        }
    }
}
