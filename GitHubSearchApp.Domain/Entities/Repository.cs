using System;

namespace GitHubSearchApp.Domain.Entities
{
    /// <summary>
    /// Representa um repositório público do GitHub.
    /// Entidade central do domínio.
    /// </summary>
    public class Repository : IEquatable<Repository>
    {
        /// <summary>
        /// Identificador único do repositório no GitHub.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Nome do repositório.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Descrição do repositório.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// URL do repositório no GitHub.
        /// </summary>
        public string HtmlUrl { get; private set; } = string.Empty;

        /// <summary>
        /// Quantidade de estrelas (stargazers).
        /// </summary>
        public int Stars { get; private set; }

        /// <summary>
        /// Quantidade de forks.
        /// </summary>
        public int Forks { get; private set; }

        /// <summary>
        /// Quantidade de watchers.
        /// </summary>
        public int Watchers { get; private set; }

        /// <summary>
        /// Cria uma nova instância da entidade Repository.
        /// </summary>
        /// <param name="id">Id único do repositório</param>
        /// <param name="name">Nome do repositório</param>
        /// <param name="description">Descrição do repositório</param>
        /// <param name="htmlUrl">URL do repositório</param>
        /// <param name="stars">Número de estrelas</param>
        /// <param name="forks">Número de forks</param>
        /// <param name="watchers">Número de watchers</param>
        /// <exception cref="ArgumentException">Quando nome ou URL forem inválidos</exception>
        public Repository(long id, string name, string description, string htmlUrl, int stars, int forks, int watchers)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do repositório não pode ser vazio.", nameof(name));
            if (string.IsNullOrWhiteSpace(htmlUrl))
                throw new ArgumentException("URL do repositório não pode ser vazia.", nameof(htmlUrl));

            Id = id;
            Name = name;
            Description = description;
            HtmlUrl = htmlUrl;
            Stars = stars;
            Forks = forks;
            Watchers = watchers;
        }

        /// <summary>
        /// Atualiza os dados do repositório.
        /// </summary>
        /// <param name="description">Nova descrição</param>
        /// <param name="stars">Nova quantidade de estrelas</param>
        /// <param name="forks">Nova quantidade de forks</param>
        /// <param name="watchers">Nova quantidade de watchers</param>
        public void Update(string description, int stars, int forks, int watchers)
        {
            Description = description ?? string.Empty;
            Stars = stars;
            Forks = forks;
            Watchers = watchers;
        }

        /// <summary>
        /// Representação textual para facilitar debug.
        /// </summary>
        public override string ToString()
        {
            return $"Repository: {Name} (Id: {Id}), Stars: {Stars}, Forks: {Forks}, Watchers: {Watchers}";
        }

        /// <summary>
        /// Verifica se outra instância é igual a esta.
        /// </summary>
        public bool Equals(Repository? other)
        {
            if (other is null) return false;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => Equals(obj as Repository);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
