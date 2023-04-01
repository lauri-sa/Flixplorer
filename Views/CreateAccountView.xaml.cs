using Flixplorer.ViewModels;
using System.Windows.Controls;

namespace Flixplorer.Views
{
    /// <summary>
    /// Interaction logic for CreateAccountView.xaml
    /// </summary>
    public partial class CreateAccountView : UserControl
    {
        public CreateAccountView()
        {
            InitializeComponent();
            
            this.DataContext = new CreateAccountViewModel();
        }
    }
}