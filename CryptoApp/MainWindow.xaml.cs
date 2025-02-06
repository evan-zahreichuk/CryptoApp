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
            //MainFrame.Navigate(new Views.CurrencyDetailView("bitcoin", apiKey = "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83"));

        }
    }
}
