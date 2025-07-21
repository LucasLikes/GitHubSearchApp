namespace GitHubSearchApp.API.DTOs
{
    public class RepositoryResponseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HtmlUrl { get; set; } = string.Empty;
        public int Stars { get; set; }
        public int Forks { get; set; }
        public int Watchers { get; set; }
    }
}
