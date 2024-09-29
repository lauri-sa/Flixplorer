using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using Flixplorer.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Flixplorer.Helpers
{
    /// <summary>
    /// A helper class that provides methods for establishing connections to external API using HttpClient.
    /// </summary>
    internal class ApiConnectionHelper
    {
        /// <summary>
        /// This method fetches a list of movies from an external API using HttpClient.
        /// It retrieves movies that were added in the last month and orders them by date.
        /// </summary>
        /// <returns>A list of MediaModel objects.</returns>
        public static async Task<List<MediaModel>> FetchMoviesFromApi()
        {
            var date = DateTime.Now.AddMonths(-1).Date.ToString("yyyy-MM-dd");

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://unogs-unogs-v1.p.rapidapi.com/search/titles?new_date={date}&order_by=date&type=movie"),
                Headers =
                {
                    { "X-RapidAPI-Key", ConfigurationManager.AppSettings["ApiKey2"] },
                    { "X-RapidAPI-Host", "unogs-unogs-v1.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                
                var body = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    var apiResponseModel = await JsonSerializer.DeserializeAsync<ApiResponseModel>(stream);

                    apiResponseModel.results.Reverse();

                    return apiResponseModel.results;
                }
            };
        }

        /// <summary>
        /// This method fetches a list of series from an external API using HttpClient.
        /// It retrieves series that were added in the last month and orders them by date.
        /// </summary>
        /// <returns>A list of MediaModel objects.</returns>
        public static async Task<List<MediaModel>> FetchSeriesFromApi()
        {
            var date = DateTime.Now.AddMonths(-1).Date.ToString("yyyy-MM-dd");

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://unogs-unogs-v1.p.rapidapi.com/search/titles?new_date={date}&order_by=date&type=series"),
                Headers =
                {
                    { "X-RapidAPI-Key", ConfigurationManager.AppSettings["ApiKey2"] },
                    { "X-RapidAPI-Host", "unogs-unogs-v1.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                
                var body = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    var apiResponseModel = await JsonSerializer.DeserializeAsync<ApiResponseModel>(stream);

                    apiResponseModel.results.Reverse();

                    return apiResponseModel.results;
                }
            };
        }

        /// <summary>
        /// This method fetches a list of search targets from an external API using HttpClient.
        /// The search targets are titles that match a given search string.
        /// </summary>
        /// <param name="searchString">The string to use as the search term for titles.</param>
        /// <returns>A list of MediaModel objects.</returns>
        public async static Task<List<MediaModel>> FetchSearchTargetsFromApi(string searchString)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://unogs-unogs-v1.p.rapidapi.com/search/titles?order_by=title&title={searchString}"),
                Headers =
                {
                    { "X-RapidAPI-Key", ConfigurationManager.AppSettings["ApiKey2"] },
                    { "X-RapidAPI-Host", "unogs-unogs-v1.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    var apiResponseModel = await JsonSerializer.DeserializeAsync<ApiResponseModel>(stream);

                    return apiResponseModel.results;
                }
            }
        }
        /// <summary>
        /// This method fetches the details for a selected target from an external API using HttpClient.
        /// The selected target is identified by a Netflix ID.
        /// The response is deserialized into a TargetDetailModel object and returned.
        /// </summary>
        /// <param name="netflixId">The Netflix ID of the selected target.</param>
        /// <returns>A TargetDetailModel object representing the details of the selected target</returns>
        public async static Task<TargetDetailModel> FetchTargetDetailsFromApi(string netflixId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://unogs-unogs-v1.p.rapidapi.com/aaapi.cgi?t=loadvideo&q={netflixId}"),
                Headers =
                {
                    { "X-RapidAPI-Key", ConfigurationManager.AppSettings["ApiKey1"] },
                    { "X-RapidAPI-Host", "unogs-unogs-v1.p.rapidapi.com" },
                },
            };
            
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    return await JsonDocumentHelper.ParseTargetDetailsAsync(stream);
                }
            };
        }

        /// <summary>
        /// This method fetches the image path for a selected target from an external API using HttpClient.
        /// The search target is identified by a Netflix ID.
        /// The response is deserialized into a string representing the image path and returned.
        /// </summary>
        /// <param name="netflixId">The Netflix ID of the selected target.</param>
        /// <returns>A string representing the image path of the search target.</returns>
        public async static Task<string> FetchTargetImagePathFromApi(string netflixId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://unogs-unogs-v1.p.rapidapi.com/title/images?netflix_id={netflixId}"),
                Headers =
                {
                    { "X-RapidAPI-Key", ConfigurationManager.AppSettings["ApiKey2"] },
                    { "X-RapidAPI-Host", "unogs-unogs-v1.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    return await JsonDocumentHelper.ParseTargetImagePathAsync(stream);
                }
            };
        }
    }
}