using Flixplorer.ViewModels;
using System.Windows.Controls;

namespace Flixplorer.Views
{
    /// <summary>
    /// Interaction logic for MoviesView.xaml
    /// </summary>
    public partial class MoviesView : UserControl
    {
        public MoviesView()
        {
            InitializeComponent();
            this.DataContext = new MoviesViewModel();
        }
    }
}