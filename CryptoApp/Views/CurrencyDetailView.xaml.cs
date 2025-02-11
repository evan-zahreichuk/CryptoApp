using System.Windows.Controls;

namespace CryptoApp.Views
{
    public partial class CurrencyDetailView : Page
    {
        public CurrencyDetailView(string currencyId, string apiKey)
        {
            InitializeComponent();
            DataContext = new ViewModels.CurrencyDetailViewModel(currencyId, apiKey);
        }
    }
}
