using System;

namespace GitHubSearchApp.Domain.Entities
{
    /// <summary>
    /// Representa um reposit�rio p�blico do GitHub.
    /// Entidade central do dom�nio.
    /// </summary>
    public class Repository : IEquatable<Repository>
    {
        /// <summary>
        /// Identificador �nico do reposit�rio no GitHub.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Nome do reposit�rio.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Descri��o do reposit�rio.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// URL do reposit�rio no GitHub.
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
        /// Cria uma nova inst�ncia da entidade Repository.
        /// </summary>
        /// <param name="id">Id �nico do reposit�rio</param>
        /// <param name="name">Nome do reposit�rio</param>
        /// <param name="description">Descri��o do reposit�rio</param>
        /// <param name="htmlUrl">URL do reposit�rio</param>
        /// <param name="stars">N�mero de estrelas</param>
        /// <param name="forks">N�mero de forks</param>
        /// <param name="watchers">N�mero de watchers</param>
        /// <exception cref="ArgumentException">Quando nome ou URL forem inv�lidos</exception>
        public Repository(long id, string name, string description, string htmlUrl, int stars, int forks, int watchers)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do reposit�rio n�o pode ser vazio.", nameof(name));
            if (string.IsNullOrWhiteSpace(htmlUrl))
                throw new ArgumentException("URL do reposit�rio n�o pode ser vazia.", nameof(htmlUrl));

            Id = id;
            Name = name;
            Description = description;
            HtmlUrl = htmlUrl;
            Stars = stars;
            Forks = forks;
            Watchers = watchers;
        }

        /// <summary>
        /// Atualiza os dados do reposit�rio.
        /// </summary>
        /// <param name="description">Nova descri��o</param>
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
        /// Representa��o textual para facilitar debug.
        /// </summary>
        public override string ToString()
        {
            return $"Repository: {Name} (Id: {Id}), Stars: {Stars}, Forks: {Forks}, Watchers: {Watchers}";
        }

        /// <summary>
        /// Verifica se outra inst�ncia � igual a esta.
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
