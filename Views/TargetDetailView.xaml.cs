using Flixplorer.ViewModels;
using System.Windows.Controls;

namespace Flixplorer.Views
{
    /// <summary>
    /// Interaction logic for TargetDetailView.xaml
    /// </summary>
    public partial class TargetDetailView : UserControl
    {
        public TargetDetailView()
        {
            InitializeComponent();
            this.DataContext = new TargetDetailViewModel();
        }
    }
}