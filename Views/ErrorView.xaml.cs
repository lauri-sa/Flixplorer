using Flixplorer.ViewModels;
using System.Windows.Controls;

namespace Flixplorer.Views
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public ErrorView()
        {
            InitializeComponent();
            this.DataContext = new ErrorViewModel();
        }
    }
}