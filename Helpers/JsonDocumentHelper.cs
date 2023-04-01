using System.IO;
using System.Net;
using System.Linq;
using System.Text.Json;
using Flixplorer.Models;
using System.Threading.Tasks;

namespace Flixplorer.Helpers
{
    /// <summary>
    /// A helper class that provides methods to parse JSON documents and extract specific information from them.
    /// </summary>
    internal class JsonDocumentHelper
    {
        /// <summary>
        /// Parses a JSON stream and returns the URL of the target image path.
        /// </summary>
        /// <param name="stream">The stream containing the JSON to be parsed.</param>
        /// <returns>The URL of the target image path, or null if not found.</returns>
        public static async Task<string> ParseTargetImagePathAsync(Stream stream)
        {
            using (var document = await JsonDocument.ParseAsync(stream))
            {
                var root = document.RootElement;

                var results = root.GetProperty("results");

                for (int i = 0; i < results.GetArrayLength(); i++)
                {
                    if (results[i].GetProperty("image_type").GetString().Equals("bo1280x448"))
                    {
                        return results[i].GetProperty("url").GetString();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Parses the target details JSON document from the provided stream and returns a TargetDetailModel object that contains the details of the target.
        /// </summary>
        /// <param name="stream">The stream that contains the JSON document.</param>
        /// <returns>TargetDetailModel object.</returns>
        public static async Task<TargetDetailModel> ParseTargetDetailsAsync(Stream stream)
        {
            using (var document = await JsonDocument.ParseAsync(stream))
            {
                var root = document.RootElement;

                var nfinfo = root.GetProperty("RESULT").GetProperty("nfinfo");

                var people = root.GetProperty("RESULT").GetProperty("people");

                var targetDetailModel = new TargetDetailModel
                {
                    Type = nfinfo.GetProperty("type").GetString(),

                    Title = WebUtility.HtmlDecode(nfinfo.GetProperty("title").GetString()),

                    ReleaseYear = $"Released: {nfinfo.GetProperty("released").GetString()}",

                    Plot = WebUtility.HtmlDecode(nfinfo.GetProperty("synopsis").GetString()),

                    TypeSpecificInfo = nfinfo.GetProperty("type").GetString().Equals("movie")
                        ? $"Runtime: {nfinfo.GetProperty("runtime").GetString().Replace("h", "h ")}"
                        : $"Seasons: {root.GetProperty("RESULT").GetProperty("country")[0].GetProperty("seasons").GetString().Split(" ")[0]}",

                    Director = people.GetArrayLength() > 1 && people[2].GetProperty("director").GetArrayLength() > 0
                        ? $"Director: {WebUtility.HtmlDecode(people[2].GetProperty("director")[0].GetString())}"
                        : "Director: Unknown",

                    Actors = people.GetArrayLength() > 0
                        ? $"Actors: {string.Join(", ", people[0].GetProperty("actor").EnumerateArray().Select(actor => WebUtility.HtmlDecode(actor.ToString())))}"
                        : "Actors: Unknown",

                    Genres = $"Genres: {string.Join(", ", root.GetProperty("RESULT").GetProperty("mgname").EnumerateArray().Select(genre => WebUtility.HtmlDecode(genre.ToString())))}"
                };

                return targetDetailModel;
            }
        }
    }
}