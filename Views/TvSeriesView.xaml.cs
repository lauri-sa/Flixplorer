using Flixplorer.ViewModels;
using System.Windows.Controls;

namespace Flixplorer.Views
{
    /// <summary>
    /// Interaction logic for TvSeriesView.xaml
    /// </summary>
    public partial class TvSeriesView : UserControl
    {
        public TvSeriesView()
        {
            InitializeComponent();
            this.DataContext = new TvSeriesViewModel();
        }
    }
}