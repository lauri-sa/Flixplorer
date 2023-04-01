using System;
using System.IO;
using System.Windows;
using System.Net.Http;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Base class for all view models in the application.
    /// Implements the INotifyPropertyChanged interface, allowing the view to be notified of changes to properties in the view model.
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        public static Cursor HandCursor { get; private set; } = new Cursor(Application.GetResourceStream(new Uri("Resources/HandCursor.cur", UriKind.Relative)).Stream);

        public static Cursor BeamCursor { get; private set; } = new Cursor(Application.GetResourceStream(new Uri("Resources/BeamCursor.cur", UriKind.Relative)).Stream);

        public static Cursor ArrowCursor { get; private set; } = new Cursor(Application.GetResourceStream(new Uri("Resources/ArrowCursor.cur", UriKind.Relative)).Stream);

        public static ObservableCollection<MainViewModel> ViewModelCollection { get; private set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Method that raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Opens LogInView.
        /// </summary>
        protected static void OpenLogInView()
        {
            SetViewModel(new LogInViewModel());
        }

        /// <summary>
        /// Opens MoviesView and sets current category infotext to movies.
        /// </summary>
        protected static void OpenMoviesView()
        {
            SetInitErrorFlag();
            SetViewModel(new MoviesViewModel());
            MainWindowViewModel.CategoryText = "Current category\nMovies";
        }

        /// <summary>
        /// Resets all errorflags and sets initialize error flag to true in the ErrorViewModel.
        /// </summary>
        protected static void SetInitErrorFlag()
        {
            ErrorViewModel.ResetErrorFlags();
            ErrorViewModel.InitializeError = true;
        }

        /// <summary>
        /// Clears the existing ViewModelCollection and adds the specified ViewModel to it.
        /// </summary>
        /// <param name="viewModel">The ViewModel to add to the ViewModelCollection.</param>
        protected static void SetViewModel(MainViewModel viewModel)
        {
            ViewModelCollection.Clear();
            ViewModelCollection.Add(viewModel);
        }

        /// <summary>
        /// Resets remember me values and saves changes to the settings file.
        /// </summary>
        protected static void ResetRememberMeValues()
        {
            Properties.Settings.Default["RememberMe"] = false;
            Properties.Settings.Default["UserName"] = string.Empty;
            Properties.Settings.Default["Password"] = string.Empty;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Asynchronously retrieves a bitmap image from the specified URL.
        /// </summary>
        /// <param name="imgsrc">The URL of the image to retrieve.</param>
        /// <returns>A BitmapImage object representing the retrieved image.</returns>
        protected static async Task<BitmapImage> GetImageAsync(string imgsrc)
        {
            BitmapImage bitmap = null;
            var httpClient = new HttpClient();
            using (var response = await httpClient.GetAsync(imgsrc))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = new MemoryStream())
                    {
                        await response.Content.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Creates a border element with a red circular loading animation inside it.
        /// </summary>
        /// <returns>The created border element with loading animation.</returns>
        protected static Border SetLoadingBorder()
        {
            var border = new Border
            {
                Width = 200,
                Height = 300,
                Cursor = HandCursor,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(14, 10, 14, 10),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Red
            };

            Ellipse ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                StrokeThickness = 2,
                Stroke = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 1),
                    EndPoint = new Point(0, 0),
                    GradientStops =
                    {
                        new GradientStop(Colors.Red, 0.5),
                        new GradientStop(Colors.Black, 1)
                    }
                },
                RenderTransform = new RotateTransform(0, 15, 15)
            };

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };

            ellipse.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, animation);

            border.Child = ellipse;

            return border;
        }
    }
}