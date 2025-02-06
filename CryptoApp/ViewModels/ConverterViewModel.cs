using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CryptoApp.Models;
using CryptoApp.Services;
namespace CryptoApp.ViewModels
{
    public class ConverterViewModel : BaseViewModel
    {
        private CoinMarketCapApiService _apiService;
        public ObservableCollection<Currency> Currencies { get; set; }
        private Currency _selectedFromCurrency;
        public Currency SelectedFromCurrency
        {
            get => _selectedFromCurrency;
            set
            {
                _selectedFromCurrency = value;
                OnPropertyChanged(nameof(SelectedFromCurrency));
                CalculateConversion();
            }
        }
        private Currency _selectedToCurrency;
        public Currency SelectedToCurrency
        {
            get => _selectedToCurrency;
            set
            {
                _selectedToCurrency = value;
                OnPropertyChanged(nameof(SelectedToCurrency));
                CalculateConversion();
            }
        }
        private string _amount;
        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
                CalculateConversion();
            }
        }
        private string _convertedAmount;
        public string ConvertedAmount
        {
            get => _convertedAmount;
            set
            {
                _convertedAmount = value;
                OnPropertyChanged(nameof(ConvertedAmount));
            }
        }
        public ICommand NavigateBackCommand { get; set; }
        public ConverterViewModel()
        {
            string apiKey = "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83"; // Replace with your actual API key
            _apiService = new CoinMarketCapApiService(apiKey);
            Currencies = new ObservableCollection<Currency>();
            NavigateBackCommand = new RelayCommand(o => NavigateBack());
            LoadCurrencies();
        }
        private async void LoadCurrencies()
        {
            var data = await _apiService.GetTopCurrenciesAsync();
            Application.Current.Dispatcher.Invoke(() => {
                Currencies.Clear();
                foreach (var c in data)
                    Currencies.Add(c);
            });
        }
        private void CalculateConversion()
        {
            if (SelectedFromCurrency == null || SelectedToCurrency == null || !decimal.TryParse(Amount, out decimal amt))
            {
                ConvertedAmount = "";
                return;
            }
            if (decimal.TryParse(SelectedFromCurrency.PriceUsd, out decimal fromPrice) &&
                decimal.TryParse(SelectedToCurrency.PriceUsd, out decimal toPrice) &&
                toPrice != 0)
            {
                decimal result = amt * fromPrice / toPrice;
                ConvertedAmount = result.ToString("F4");
            }
            else
                ConvertedAmount = "";
        }
        private void NavigateBack()
        {
            var frame = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.GoBack();
        }
    }
}
