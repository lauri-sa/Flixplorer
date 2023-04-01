using System.Linq;
using System.Windows;
using System.Threading;
using Flixplorer.Models;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for SearchView. Inherits MainViewModel class.
    /// </summary>
    internal class SearchViewModel : MainViewModel
    {
        private static bool _hideLoadingIcon;
        public static bool HideLoadingIcon { get { return _hideLoadingIcon; } set { _hideLoadingIcon = value; OnStaticPropertyChanged(); } }

        private static bool _noSearchResults;
        public static bool NoSearchResults { get { return _noSearchResults; } set { _noSearchResults = value; OnStaticPropertyChanged(); } }

        public static ObservableCollection<object> SearchCollection { get; private set; } = new();

        public static event PropertyChangedEventHandler? StaticPropertyChanged;

        /// <summary>
        /// Method that raises the PropertyChanged event for the specified static property.
        /// </summary>
        /// <param name="name"></param>
        public static void OnStaticPropertyChanged([CallerMemberName] string name = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// A method that adds media items to the SearchCollection by iterating over a given list of MediaModel objects,
        /// creating a Border object with an image and a click event handler for each item, and adding them to the SearchCollection ObservableCollection.
        /// </summary>
        /// <param name="searchTargetsList">A List of MediaModel objects representing the media items to be added to the SearchCollection.</param>
        /// <param name="trimmedSearchString">The trimmed search string used to trim the searchTargetsList</param>
        /// <param name="cancellationToken">A CancellationToken that can be used to cancel the asynchronous operation.</param>
        /// <returns></returns>
        public static async Task AddSearchTargets(List<MediaModel> searchTargetsList, string trimmedSearchString, CancellationToken cancellationToken)
        {
            HideLoadingIcon = true;

            searchTargetsList = TrimSearchTargetsList(searchTargetsList, trimmedSearchString);

            if (cancellationToken.IsCancellationRequested)
            {
                SearchCollection.Clear();
                return;
            }

            if (searchTargetsList == null || searchTargetsList.Count < 1)
            {
                NoSearchResults = true;
            }
            else
            {
                for (int i = 0; i < searchTargetsList.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        SearchCollection.Clear();
                        return;
                    }
                    else
                    {
                        SearchCollection.Add(SetLoadingBorder());
                    }

                    var netflixId = searchTargetsList[i].netflix_id.ToString();
                    var imageUrl = searchTargetsList[i].img;

                    var border = new Border
                    {
                        Width = 200,
                        Height = 300,
                        Cursor = HandCursor,
                        CornerRadius = new CornerRadius(10),
                        Margin = new Thickness(14, 10, 14, 10),
                        BorderThickness = new Thickness(2),
                        BorderBrush = Brushes.Red,
                        Background = new ImageBrush()
                        {
                            ImageSource = await GetImageAsync(imageUrl)
                        },
                    };

                    border.MouseLeftButtonDown += async (s, e) =>
                    {
                        await ShowSearchTargetInfo(netflixId, imageUrl);
                    };

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        SearchCollection.RemoveAt(SearchCollection.Count - 1);
                        SearchCollection.Add(border);
                    }
                }
            }
        }

        /// <summary>
        /// Opens TargetDetailView and initializes it with data for a given Netflix ID and image URL.
        /// </summary>
        /// <param name="netflixId">A string representing the Netflix ID of the media item.</param>
        /// <param name="imageUrl">A string representing the URL of the image for the media item.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task ShowSearchTargetInfo(string netflixId, string imageUrl)
        {
            SetViewModel(new TargetDetailViewModel());
            await TargetDetailViewModel.InitTargetDetails(netflixId, imageUrl);
        }

        /// <summary>
        /// Trims the provided search targets list to only include MediaModels whose titles contain the provided trimmed search string.
        /// The resulting list is then sorted so that items that start with the search string come first,
        /// followed by items that contain the search string elsewhere in the title.
        /// </summary>
        /// <param name="searchTargetsList">The list of MediaModels to trim and sort.</param>
        /// <param name="trimmedSearchString">The search string to use for filtering and sorting the MediaModels.</param>
        /// <returns>The trimmed and sorted list of MediaModels.</returns>
        private static List<MediaModel> TrimSearchTargetsList(List<MediaModel> searchTargetsList, string trimmedSearchString)
        {
            if (searchTargetsList == null)
            {
                return searchTargetsList;
            }

            searchTargetsList.RemoveAll(target => !target.title.ToLower().Contains(trimmedSearchString));

            searchTargetsList = searchTargetsList
                                .OrderByDescending(target => target.title.ToLower().StartsWith(trimmedSearchString))
                                .ThenBy(target => target.title.Split(' ')[0])
                                .ToList();

            return searchTargetsList;
        }
    }
}