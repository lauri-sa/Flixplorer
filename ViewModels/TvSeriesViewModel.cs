using System.Windows;
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
    /// Datacontext class for TvSeriesView. Inherits MainViewModel class.
    /// </summary>
    internal class TvSeriesViewModel : MainViewModel
    {
        private static bool _hideLoadingIcon;
        public static bool HideLoadingIcon { get { return _hideLoadingIcon; } set { _hideLoadingIcon = value; OnStaticPropertyChanged(); } }

        public static ObservableCollection<object> SeriesCollection { get; private set; } = new();

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
        /// A method that adds media items to the SeriesCollection by iterating over a given list of MediaModel objects,
        /// creating a Border object with an image and a click event handler for each item, and adding them to the SeriesCollection ObservableCollection.
        /// </summary>
        /// <param name="seriesList">A List of MediaModel objects representing the media items to be added to the SeriesCollection.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task AddSeries(List<MediaModel> seriesList)
        {
            HideLoadingIcon = true;

            for (int i = 0; i < seriesList.Count; i++)
            {
                SeriesCollection.Add(SetLoadingBorder());

                var netflixId = seriesList[i].netflix_id.ToString();
                var imageUrl = seriesList[i].img;

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
                    await ShowSeriesInfo(netflixId, imageUrl);
                };

                SeriesCollection.RemoveAt(SeriesCollection.Count - 1);
                SeriesCollection.Add(border);
            }
        }

        /// <summary>
        /// Opens TargetDetailView and initializes it with data for a given Netflix ID and image URL.
        /// </summary>
        /// <param name="netflixId">A string representing the Netflix ID of the media item.</param>
        /// <param name="imageUrl">A string representing the URL of the image for the media item.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task ShowSeriesInfo(string netflixId, string imageUrl)
        {
            SetViewModel(new TargetDetailViewModel());
            await TargetDetailViewModel.InitTargetDetails(netflixId, imageUrl);
        }
    }
}