using System;
using System.Windows;
using System.Threading;
using Flixplorer.Models;
using Flixplorer.Helpers;
using Flixplorer.Commands;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for MainWindowView. Inherits MainViewModel class.
    /// </summary>
    class MainWindowViewModel : MainViewModel
    {
        private static bool canSearch = true;

        private static CancellationTokenSource searchTokenSource;
        private static CancellationTokenSource favouritesTokenSource;

        public static double ScaleX { get; private set; }
        public static double ScaleY { get; private set; }

        private static string _searchString = string.Empty;
        public static string SearchString { get { return _searchString; } set { _searchString = value; OnStaticPropertyChanged(); } }

        private static bool _isEnabled;
        public static bool IsEnabled { get { return _isEnabled; } set { _isEnabled = value; OnStaticPropertyChanged(); } }

        private static string _categoryText;
        public static string CategoryText { get { return _categoryText; } set { _categoryText = value; OnStaticPropertyChanged(); } }

        private static string _loggedInInfoText;
        public static string LoggedInInfoText
        {
            get
            {
                return string.IsNullOrEmpty(_loggedInInfoText) ? "\n\nNot logged in" : _loggedInInfoText;
            }

            set
            {
                _loggedInInfoText = value;
                OnStaticPropertyChanged();
            }
        }

        private static bool _isUpdatingInfoVisible;
        public static bool IsUpdatingInfoVisible { get { return _isUpdatingInfoVisible; } set { _isUpdatingInfoVisible = value; OnStaticPropertyChanged(); } }

        private static string _updatingInfoText;
        public static string UpdatingInfoText { get { return _updatingInfoText; } set { _updatingInfoText = value; OnStaticPropertyChanged(); } }

        /// <summary>
        /// Command that is responsible for closing the application.
        /// </summary>
        public ICommand QuitCommand => new DelegateCommand(Quit);

        /// <summary>
        /// Command that is responsible for logging the user out.
        /// </summary>
        public ICommand LogOutCommand => new DelegateCommand(LogOut);

        /// <summary>
        /// Command that is responsible for opening the searchview.
        /// </summary>
        public ICommand SearchCommand => new DelegateCommand(OpenSearchView);

        /// <summary>
        /// Command that is responsible for opening the moviesview.
        /// </summary>
        public ICommand MoviesCommand => new DelegateCommand(OpenMoviesView);

        /// <summary>
        /// Command that is responsible for opening the tvseriesview.
        /// </summary>
        public ICommand TvSeriesCommand => new DelegateCommand(OpenTvSeriesView);

        /// <summary>
        /// Command that is responsible for opening the favouritesview.
        /// </summary>
        public ICommand FavouritesCommand => new DelegateCommand(OpenFavouritesView);

        public static event PropertyChangedEventHandler? StaticPropertyChanged;

        static MainWindowViewModel()
        {
            SetScale();

            OpenLogInView();

            App.Current.MainWindow.Loaded += MainWindowLoaded;
        }

        /// <summary>
        /// Handles the loaded event of the MainWindow and calls the Initialize method asynchronously.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private static async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            await Initialize();
        }

        /// <summary>
        /// Initializes the application by updating the database and fetching content.
        /// Shows a loading icon and updates the status message while the database is being updated and content is being fetched.
        /// If an error occurs, logs the error, hides the loading icons, and sets error view models.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Initialize()
        {
            try
            {
                IsUpdatingInfoVisible = true;
                ClearContentCollections();
                MoviesViewModel.HideLoadingIcon = false;
                TvSeriesViewModel.HideLoadingIcon = false;
                UpdatingInfoText = "Updating Database";
                await UpdateDatabase();
                UpdatingInfoText = "Fetching Content";
                await FetchFromDatabase();
                IsUpdatingInfoVisible = false;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                IsUpdatingInfoVisible = false;
                MoviesViewModel.HideLoadingIcon = true;
                TvSeriesViewModel.HideLoadingIcon = true;
                SetErrorViewModels();
            }
        }

        /// <summary>
        /// Updates the database with movies and TV series fetched from an API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation./returns>
        private static async Task UpdateDatabase()
        {
            var fetchMoviesTask = ApiConnectionHelper.FetchMoviesFromApi();
            var fetchSeriesTask = ApiConnectionHelper.FetchSeriesFromApi();
            await Task.WhenAll(fetchMoviesTask, fetchSeriesTask);
            
            var moviesList = fetchMoviesTask.Result;
            var seriesList = fetchSeriesTask.Result;

            var addMoviesTask = SqlConnectionHelper.AddMoviesToDatabase(moviesList);
            var addSeriesTask = SqlConnectionHelper.AddSeriesToDatabase(seriesList);
            await Task.WhenAll(addMoviesTask, addSeriesTask);
        }

        /// <summary>
        /// Fetches movies and series data from the database and adds them to the collections in the corresponding view models.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task FetchFromDatabase()
        {
            var fetchMoviesTask = SqlConnectionHelper.GetMoviesFromDatabase();
            var fetchSeriesTask = SqlConnectionHelper.GetSeriesFromDatabase();
            await Task.WhenAll(fetchMoviesTask, fetchSeriesTask);
            
            var moviesList = fetchMoviesTask.Result;
            var seriesList = fetchSeriesTask.Result;
            
            var addMoviesToCollectionTask = MoviesViewModel.AddMovies(moviesList);
            var addSeriesToCollectionTask = TvSeriesViewModel.AddSeries(seriesList);
            await Task.WhenAll(addMoviesToCollectionTask, addSeriesToCollectionTask);
        }

        /// <summary>
        /// Searches for targets using the current search query and displays the results in the SearchViewModel.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SearchTargets()
        {
            try
            {
                canSearch = false;
                SetViewModel(new SearchViewModel());
                CategoryText = "Current category\nSearch";
                SearchViewModel.NoSearchResults = false;
                SearchViewModel.HideLoadingIcon = false;
                await PopulateSearchCollection();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                canSearch = true;
                SearchViewModel.HideLoadingIcon = true;
                SearchViewModel.SearchCollection.Clear();
                SearchViewModel.SearchCollection.Add(new ErrorViewModel());
            }
        }

        /// <summary>
        /// Initializes the Favourites View by setting up the view model, setting the loading icon visibility,
        /// setting the category text, and populating the favourites collection asynchronously.
        /// If an exception occurs, it logs the error, sets the loading icon visibility to true, and adds an error view model to the favourites collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task InitializeFavouritesView()
        {
            try
            {
                SetViewModel(new FavouritesViewModel());
                FavouritesViewModel.HideLoadingIcon = false;
                CategoryText = "Current category\nFavourites";
                await PopulateFavouritesCollection();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                FavouritesViewModel.HideLoadingIcon = true;
                FavouritesViewModel.FavouritesCollection.Clear();
                FavouritesViewModel.FavouritesCollection.Add(new ErrorViewModel());
            }
        }

        /// <summary>
        /// Populates the search collection by fetching search targets from the API based on the current search string.
        /// If a search is already in progress, it cancels the previous one and starts a new one.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task PopulateSearchCollection()
        {
            searchTokenSource?.Cancel();
            searchTokenSource = new CancellationTokenSource();
            SearchViewModel.SearchCollection.Clear();
            var trimmedSearchString = SearchString.ToLower().Trim();
            var searchTargetsList = await ApiConnectionHelper.FetchSearchTargetsFromApi(trimmedSearchString);
            canSearch = true;
            await SearchViewModel.AddSearchTargets(searchTargetsList, trimmedSearchString, searchTokenSource.Token);
        }

        /// <summary>
        /// Populates the favourites collection with the user's favourite items from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task PopulateFavouritesCollection()
        {
            favouritesTokenSource?.Cancel();
            favouritesTokenSource = new CancellationTokenSource();
            FavouritesViewModel.FavouritesCollection.Clear();
            var favouritesList = await SqlConnectionHelper.GetFavouritesFromDatabase(UserModel.ID);
            await FavouritesViewModel.AddFavourites(favouritesList, favouritesTokenSource.Token);
        }

        /// <summary>
        /// Method that raises the PropertyChanged event for the specified static property.
        /// </summary>
        /// <param name="name"></param>
        public static void OnStaticPropertyChanged([CallerMemberName] string name = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Sets the ScaleX and ScaleY properties based on the primary screen's width and height.
        /// ScaleX is set to the primary screen width divided by 1920,
        /// and ScaleY is set to the primary screen height divided by 1080.
        /// </summary>
        private static void SetScale()
        {
            ScaleX = SystemParameters.PrimaryScreenWidth / 1920;
            ScaleY = SystemParameters.PrimaryScreenHeight / 1080;
        }

        /// <summary>
        /// Resets the values related to a logged-in user, including the user name,
        /// logged-in info text, category text, search string, and disables buttons.
        /// </summary>
        private void ResetLoggedInValues()
        {
            UserModel.UserName = string.Empty;
            LoggedInInfoText = string.Empty;
            CategoryText = string.Empty;
            SearchString = string.Empty;
            IsEnabled = false;
        }

        /// <summary>
        /// Clears the content collections and adds an ErrorViewModel to the movies and TV series collections.
        /// </summary>
        private static void SetErrorViewModels()
        {
            ClearContentCollections();
            MoviesViewModel.MoviesCollection.Add(new ErrorViewModel());
            TvSeriesViewModel.SeriesCollection.Add(new ErrorViewModel());
        }

        /// <summary>
        /// Clears the movies and TV series collections in the corresponding view models.
        /// </summary>
        private static void ClearContentCollections()
        {
            MoviesViewModel.MoviesCollection.Clear();
            TvSeriesViewModel.SeriesCollection.Clear();
        }

        /// <summary>
        /// Triggers a search if the search string is not empty and searching is allowed.
        /// </summary>
        private async void OpenSearchView()
        {
            if(SearchString.Trim().Length > 0 && canSearch)
            {
                ErrorViewModel.ResetErrorFlags();
                ErrorViewModel.SearchError = true;
                await SearchTargets();
            }
        }

        /// <summary>
        /// Opens the TV-Series view and updates the category text.
        /// </summary>
        private void OpenTvSeriesView()
        {
            SetInitErrorFlag();
            SetViewModel(new TvSeriesViewModel());
            CategoryText = "Current category\nTV-Series";
        }

        /// <summary>
        /// Opens the Favourites view and initializes it with the user's favourite movies and TV series.
        /// </summary>
        private async void OpenFavouritesView()
        {
            ErrorViewModel.ResetErrorFlags();
            ErrorViewModel.FavouritesError = true;
            await InitializeFavouritesView();
        }

        /// <summary>
        /// Clears the logged-in user's data and opens the login view.
        /// </summary>
        private void LogOut()
        {
            ResetLoggedInValues();
            ResetRememberMeValues();
            OpenLogInView();
        }

        /// <summary>
        /// Shuts down the application by calling the Shutdown method of the current App instance.
        /// </summary>
        private void Quit()
        {
            App.Current.Shutdown();
        }
    }
}