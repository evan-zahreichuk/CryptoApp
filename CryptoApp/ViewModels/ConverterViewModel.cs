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
        private readonly CoinMarketCapApiService _apiService;
        private bool _isUpdating;
        public ObservableCollection<Currency> Currencies { get; set; }

        private Currency _selectedFromCurrency;
        public Currency SelectedFromCurrency
        {
            get => _selectedFromCurrency;
            set
            {
                _selectedFromCurrency = value;
                OnPropertyChanged(nameof(SelectedFromCurrency));
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    CalculateForward();
                    _isUpdating = false;
                }
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
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    CalculateForward();
                    _isUpdating = false;
                }
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
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    CalculateForward();
                    _isUpdating = false;
                }
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
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    CalculateBackward();
                    _isUpdating = false;
                }
            }
        }

        public ICommand NavigateBackCommand { get; set; }

        public ConverterViewModel()
        {
            string apiKey = "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83";
            _apiService = new CoinMarketCapApiService(apiKey);
            Currencies = new ObservableCollection<Currency>();
            NavigateBackCommand = new RelayCommand(o => NavigateBack());
            LoadCurrencies();
        }

        private async void LoadCurrencies()
        {
            var data = await _apiService.GetTopCurrenciesAsync(1, 100);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Currencies.Clear();
                foreach (var c in data)
                    Currencies.Add(c);
            });
        }

        void CalculateForward()
        {
            if (SelectedFromCurrency == null || SelectedToCurrency == null ||
                !decimal.TryParse(Amount, out decimal amt))
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

        void CalculateBackward()
        {
            if (SelectedFromCurrency == null || SelectedToCurrency == null ||
                !decimal.TryParse(ConvertedAmount, out decimal cAmt))
            {
                Amount = "";
                return;
            }

            if (decimal.TryParse(SelectedFromCurrency.PriceUsd, out decimal fromPrice) &&
                decimal.TryParse(SelectedToCurrency.PriceUsd, out decimal toPrice) &&
                fromPrice != 0)
            {
                decimal result = cAmt * toPrice / fromPrice;
                Amount = result.ToString("F4");
            }
            else
                Amount = "";
        }

        private void NavigateBack()
        {
            var frame = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.GoBack();
        }
    }
}
