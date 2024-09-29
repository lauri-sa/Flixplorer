namespace Flixplorer.Models
{
    /// <summary>
    /// A model class representing detailed information about a Netflix target (movie or series).
    /// </summary>
    internal class TargetDetailModel
    {
        public string? NetflixId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? TypeSpecificInfo { get; set; }
        public string? ReleaseYear { get; set; }
        public string? Plot { get; set; }
        public string? Director { get; set; }
        public string? Actors { get; set; }
        public string? Genres { get; set; }
    }
}