namespace Flixplorer.Models
{
    /// <summary>
    /// A model class for a media item such as movie or series.
    /// </summary>
    public class MediaModel
    {
        public int ID { get; set; }

        public int netflix_id { get; set; }

        public string? title { get; set; }

        public string? img { get; set; }

        public bool hideFromList { get; set; }
    }
}