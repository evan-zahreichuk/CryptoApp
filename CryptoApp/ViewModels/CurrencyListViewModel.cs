using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CryptoApp.Models;
using CryptoApp.Services;

namespace CryptoApp.ViewModels
{
    public class CurrencyListViewModel : BaseViewModel
    {
        private CoinMarketCapApiService _apiService;
        public ObservableCollection<Currency> Currencies { get; set; }
        public ObservableCollection<Currency> FilteredCurrencies { get; set; }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterCurrencies();
            }
        }
        private Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
            }
        }
        public ICommand NavigateConverterCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public int CurrentPage { get; set; }

        public CurrencyListViewModel()
        {
            string apiKey = "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83";
            _apiService = new CoinMarketCapApiService(apiKey);
            Currencies = new ObservableCollection<Currency>();
            FilteredCurrencies = new ObservableCollection<Currency>();
            NavigateConverterCommand = new RelayCommand(o => NavigateToConverter());
            NextPageCommand = new RelayCommand(o => {/* Implement next page logic */});
            PreviousPageCommand = new RelayCommand(o => {/* Implement previous page logic */});
            CurrentPage = 1;
            LoadCurrencies();
        }
        private async void LoadCurrencies()
        {
            var data = await _apiService.GetTopCurrenciesAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Currencies.Clear();
                foreach (var c in data)
                    Currencies.Add(c);
                FilterCurrencies();
            });
        }
        private void FilterCurrencies()
        {
            if (Currencies == null) return;
            FilteredCurrencies.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? Currencies
                : Currencies.Where(c => c.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        c.Symbol.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
            foreach (var c in filtered)
                FilteredCurrencies.Add(c);
        }
        private void NavigateToConverter()
        {
            var frame = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.Navigate(new Views.ConverterView());
        }
    }
}
