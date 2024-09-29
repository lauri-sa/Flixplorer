using Flixplorer.Commands;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for ErrorView. Inherits MainViewModel class.
    /// </summary>
    internal class ErrorViewModel : MainViewModel
    {
        public static string? NetflixId { private get; set; }

        public static string? ImageUrl { private get; set; }

        private static bool _searchError;
        public static bool SearchError { get { return _searchError; } set { _searchError = value; OnStaticPropertyChanged(); } }

        private static bool _initializeError;
        public static bool InitializeError { get { return _initializeError; } set { _initializeError = value; OnStaticPropertyChanged(); } }

        private static bool _favouritesError;
        public static bool FavouritesError { get { return _favouritesError; } set { _favouritesError = value; OnStaticPropertyChanged(); } }

        private static bool _targetDetailsError;
        public static bool TargetDetailsError { get { return _targetDetailsError; } set { _targetDetailsError = value; OnStaticPropertyChanged(); } }

        /// <summary>
        /// A command that re-initializes data fetching for movies- and tvseriesview by calling the ReInitialize method.
        /// </summary>
        public ICommand ReInitializeCommand => new DelegateCommand(ReInitialize);

        /// <summary>
        /// A command that reloads the search results by calling the ReloadSearch method.
        /// </summary>
        public ICommand ReloadSearchCommand => new DelegateCommand(ReloadSearch);

        /// <summary>
        /// A command that reloads the favourites view by calling the ReloadFavourites method.
        /// </summary>
        public ICommand ReloadFavouritesCommand => new DelegateCommand(ReloadFavourites);

        /// <summary>
        /// A command that reloads the target details view by calling the ReloadTargetDetails method.
        /// </summary>
        public ICommand ReloadTargetDetailsCommand => new DelegateCommand(ReloadTargetDetails);

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
        /// Resets all error flags to false.
        /// </summary>
        public static void ResetErrorFlags()
        {
            SearchError = false;
            InitializeError = false;
            FavouritesError = false;
            TargetDetailsError = false;
        }

        /// <summary>
        /// A method that re-initializes data fetching for movies- and tvseriesview by calling the asynchronous Initialize method of the MainWindowViewModel.
        /// </summary>
        private async void ReInitialize()
        {
            await MainWindowViewModel.Initialize();
        }

        /// <summary>
        /// A method that reloads the search results by calling the asynchronous SearchTargets method of the MainWindowViewModel.
        /// </summary>
        private async void ReloadSearch()
        {
            await MainWindowViewModel.SearchTargets();
        }

        /// <summary>
        /// A method that reloads the favourites view by calling the asynchronous InitializeFavouritesView method of the MainWindowViewModel.
        /// </summary>
        private async void ReloadFavourites()
        {
            await MainWindowViewModel.InitializeFavouritesView();
        }

        /// <summary>
        /// A method that reloads the target details view by calling the asynchronous InitTargetDetails method of the TargetDetailViewModel.
        /// </summary>
        private async void ReloadTargetDetails()
        {
            SetViewModel(new TargetDetailViewModel());
            await TargetDetailViewModel.InitTargetDetails(NetflixId, ImageUrl);
        }
    }
}