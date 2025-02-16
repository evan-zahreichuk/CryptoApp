using System.Configuration;
using System.Windows;
using CryptoApp.Services;

namespace CryptoApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string apiKey = ConfigurationManager.AppSettings["CMC_API_Key"];
            var coinMarketCapService = new CoinMarketCapApiService(apiKey);
            MainFrame.Navigate(new Views.CurrencyListView());
        }
    }
}
