using System.Windows.Controls;
namespace CryptoApp.Views
{
    public partial class ConverterView : Page
    {
        public ConverterView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ConverterViewModel();
        }
    }
}
