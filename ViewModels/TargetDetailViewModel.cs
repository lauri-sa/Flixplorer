using System;
using System.Linq;
using Flixplorer.Models;
using Flixplorer.Helpers;
using System.Diagnostics;
using Flixplorer.Commands;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for TargetDetailView. Inherits MainViewModel class.
    /// </summary>
    internal class TargetDetailViewModel : MainViewModel
    {
        private bool canClick = true;

        private static TargetDetailModel _targetDetailModel;
        public static TargetDetailModel TargetDetailModel { get { return _targetDetailModel; } set { _targetDetailModel = value; OnStaticPropertyChanged(); } }

        private static BitmapImage _targetImage;
        public static BitmapImage TargetImage { get { return _targetImage; } set { _targetImage = value; OnStaticPropertyChanged(); } }

        private static bool _buttonsEnabled;
        public static bool ButtonsEnabled { get { return _buttonsEnabled; } set { _buttonsEnabled = value; OnStaticPropertyChanged(); } }

        private static bool _hideLoadingIcon;
        public static bool HideLoadingIcon { get { return _hideLoadingIcon; } set { _hideLoadingIcon = value; OnStaticPropertyChanged(); } }

        private static bool _noImageText;
        public static bool NoImageText { get { return _noImageText; } set { _noImageText = value; OnStaticPropertyChanged(); } }

        private string _errorText;
        public string ErrorText { get { return _errorText; } set { _errorText = value; OnPropertyChanged(); } }

        private static string _favouritesButtonText;
        public static string FavouritesButtonText { get { return _favouritesButtonText; } set { _favouritesButtonText = value; OnStaticPropertyChanged(); } }

        private static bool _isFavourite;
        public static bool IsFavourite
        {
            get
            {
                return _isFavourite;
            }
            set
            {
                _isFavourite = value;
                FavouritesButtonText = IsFavourite ? "Remove from Favourites" : "Add to Favourites";
                OnStaticPropertyChanged();
            }
        }

        /// <summary>
        /// A command for favourites button.
        /// </summary>
        public ICommand FavouritesButtonCommand => new DelegateCommand(FavouritesButton);

        /// <summary>
        /// A command for open netflix button.
        /// </summary>
        public ICommand OpenNetflixCommand => new DelegateCommand(OpenNetflix);

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
        /// Asynchronously sets the background image of the target page using the provided Netflix ID.
        /// If a valid image path is retrieved from the API, it is downloaded and set as the TargetImage,
        /// while the loading icon is hidden. If the download fails or invalid image path is retrieved, ImageNotFound() method is called.
        /// </summary>
        /// <param name="netflixId">The Netflix ID of the target media.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task SetBackgroundImage(string netflixId)
        {
            var path = await ApiConnectionHelper.FetchTargetImagePathFromApi(netflixId);

            if (path != null)
            {
                TargetImage = new BitmapImage(new Uri(path, UriKind.Absolute));

                HideLoadingIcon = TargetImage.IsDownloading ? HideLoadingIcon : true;

                TargetImage.DownloadCompleted += (s, e) =>
                {
                    HideLoadingIcon = true;
                };

                TargetImage.DownloadFailed += (s, e) =>
                {
                    ImageNotFound();
                };
            }
            else
            {
                ImageNotFound();
            }
        }

        /// <summary>
        /// Fetch and sets the details for a Netflix target using its ID.
        /// </summary>
        /// <param name="netflixId">The ID of the Netflix target to fetch details for.</param>
        /// <param name="imageUrl">The URL of the image associated with the Netflix target.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task SetTargetDetails(string netflixId, string imageUrl)
        {
            TargetDetailModel = await ApiConnectionHelper.FetchTargetDetailsFromApi(netflixId);

            TargetDetailModel.NetflixId = netflixId;

            TargetDetailModel.ImageUrl = imageUrl;

            ButtonsEnabled = true;
        }

        /// <summary>
        /// Initializes target details by setting the background image, fetching the target details from the API,
        /// and checking if the target is already a favorite for the current user.
        /// </summary>
        /// <param name="netflixId">The Netflix ID of the target media</param>
        /// <param name="imageUrl">The URL of the target media image</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task InitTargetDetails(string netflixId, string imageUrl)
        {
            try
            {
                ButtonsEnabled = false;
                NoImageText = false;
                HideLoadingIcon = false;
                ResetTargetDetails();
                IsFavourite = await SqlConnectionHelper.SearchFavouriteFromDatabase(UserModel.ID, netflixId);
                await SetBackgroundImage(netflixId);
                await SetTargetDetails(netflixId, imageUrl);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                ErrorViewModel.ResetErrorFlags();
                ErrorViewModel.NetflixId = netflixId;
                ErrorViewModel.ImageUrl = imageUrl;
                ErrorViewModel.TargetDetailsError = true;
                SetViewModel(new ErrorViewModel());
            }
        }

        /// <summary>
        /// Resets the target details by creating a new instance of the TargetDetailModel.
        /// </summary>
        public static void ResetTargetDetails()
        {
            TargetDetailModel = new();
        }

        /// <summary>
        /// Displays "No Image Found" text and hides the loading icon.
        /// </summary>
        private static void ImageNotFound()
        {
            HideLoadingIcon = true;
            NoImageText = true;
        }

        /// <summary>
        /// First checks if the user is allowed to click the button.
        /// If the target is not already in the user's favourites list, the method calls the AddToFavourites method.
        /// If it is already in the list, the method calls the RemoveFromFavourites method.
        /// If an exception occurs, the method logs it using the LogHelper and sets the ErrorText property to "An unexpected error occurred".
        /// </summary>
        private async void FavouritesButton()
        {
            if (canClick)
            {
                try
                {
                    canClick = false;
                    this.ErrorText = string.Empty;

                    if (!IsFavourite)
                    {
                        await AddToFavourites();
                    }
                    else if (IsFavourite)
                    {
                        await RemoveFromFavourites();
                    }

                    canClick = true;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                    canClick = true;
                    this.ErrorText = "An unexpected error occurred";
                }
            }
        }

        /// <summary>
        /// Adds the currently selected media item to the user's favourites list, and updates the database accordingly.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task AddToFavourites()
        {
            if (TargetDetailModel.Type == "movie")
            {
                var moviesList = await SqlConnectionHelper.GetMoviesFromDatabase();

                if (!moviesList.Any(movie => movie.netflix_id.ToString().Equals(TargetDetailModel.NetflixId)))
                {
                    moviesList.Clear();

                    moviesList.Add(new MediaModel
                    {
                        netflix_id = int.Parse(TargetDetailModel.NetflixId),
                        img = TargetDetailModel.ImageUrl,
                        hideFromList = true
                    });

                    await SqlConnectionHelper.AddMoviesToDatabase(moviesList);
                }

                await SqlConnectionHelper.AddFavouriteMovieToDatabase(UserModel.ID, TargetDetailModel.NetflixId);
            }
            else
            {
                var seriesList = await SqlConnectionHelper.GetSeriesFromDatabase();

                if (!seriesList.Any(serie => serie.netflix_id.ToString().Equals(TargetDetailModel.NetflixId)))
                {
                    seriesList.Clear();

                    seriesList.Add(new MediaModel
                    {
                        netflix_id = int.Parse(TargetDetailModel.NetflixId),
                        img = TargetDetailModel.ImageUrl,
                        hideFromList = true
                    });

                    await SqlConnectionHelper.AddSeriesToDatabase(seriesList);
                }

                await SqlConnectionHelper.AddFavouriteSeriesToDatabase(UserModel.ID, TargetDetailModel.NetflixId);
            }

            IsFavourite = true;
        }

        /// <summary>
        /// Removes the target media item from the user's favourites list in database.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task RemoveFromFavourites()
        {
            if (TargetDetailModel.Type == "movie")
            {
                await SqlConnectionHelper.RemoveFavouriteMovieFromDatabase(UserModel.ID, TargetDetailModel.NetflixId);
            }
            else
            {
                await SqlConnectionHelper.RemoveFavouriteSeriesFromDatabase(UserModel.ID, TargetDetailModel.NetflixId);
            }
            
            IsFavourite = false;
        }

        /// <summary>
        /// Opens the target page in netflix website.
        /// </summary>
        private void OpenNetflix()
        {
            Process.Start(new ProcessStartInfo($"https://www.netflix.com/watch/{TargetDetailModel.NetflixId}") { UseShellExecute = true });
        }
    }
}