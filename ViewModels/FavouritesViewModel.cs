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
    /// Datacontext class for FavouritesView. Inherits MainViewModel class.
    /// </summary>
    internal class FavouritesViewModel : MainViewModel
    {
        private static bool _hideLoadingIcon;
        public static bool HideLoadingIcon { get { return _hideLoadingIcon; } set { _hideLoadingIcon = value; OnStaticPropertyChanged(); } }

        private static bool _isFavouritesEmpty;
        public static bool IsFavouritesEmpty { get { return _isFavouritesEmpty; } set { _isFavouritesEmpty = value; OnStaticPropertyChanged(); } }

        public static ObservableCollection<object> FavouritesCollection { get; set; } = new();

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
        /// A method that adds media items to the FavouritesCollection by iterating over a given list of MediaModel objects,
        /// creating a Border object with an image and a click event handler for each item, and adding them to the FavouritesCollection ObservableCollection.
        /// </summary>
        /// <param name="favouritesList">A List of MediaModel objects representing the media items to be added to the FavouritesCollection.</param>
        /// <param name="cancellationToken">A CancellationToken that can be used to cancel the asynchronous operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddFavourites(List<MediaModel> favouritesList, CancellationToken cancellationToken)
        {
            HideLoadingIcon = true;

            if (favouritesList.Count < 1)
            {
                IsFavouritesEmpty = true;
            }
            else
            {
                IsFavouritesEmpty = false;

                for (int i = 0; i < favouritesList.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        FavouritesCollection.Add(SetLoadingBorder());
                    }

                    var netflixId = favouritesList[i].netflix_id.ToString();
                    var imageUrl = favouritesList[i].img;

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
                        await ShowFavouriteInfo(netflixId, imageUrl);
                    };

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        FavouritesCollection.RemoveAt(FavouritesCollection.Count - 1);
                        FavouritesCollection.Add(border);
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
        private static async Task ShowFavouriteInfo(string netflixId, string imageUrl)
        {
            SetViewModel(new TargetDetailViewModel());
            await TargetDetailViewModel.InitTargetDetails(netflixId, imageUrl);
        }
    }
}