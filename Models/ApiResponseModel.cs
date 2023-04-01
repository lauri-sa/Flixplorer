using System.Collections.Generic;

namespace Flixplorer.Models
{
    /// <summary>
    /// A model class for a response from an API that returns a list of media items.
    /// </summary>
    internal class ApiResponseModel
    {
        public List<MediaModel> results { get; set; }
    }
}