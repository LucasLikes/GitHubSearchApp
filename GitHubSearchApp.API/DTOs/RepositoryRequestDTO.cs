using System.ComponentModel.DataAnnotations;

namespace GitHubSearchApp.API.DTOs
{
    public class RepositoryRequestDTO
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string HtmlUrl { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stars { get; set; }

        [Range(0, int.MaxValue)]
        public int Forks { get; set; }

        [Range(0, int.MaxValue)]
        public int Watchers { get; set; }
    }
}
