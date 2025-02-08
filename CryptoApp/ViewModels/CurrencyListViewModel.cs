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
        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PageNumberDisplay));
            }
        }

        public string PageNumberDisplay => CurrentPage == 1 ? " " : $" {CurrentPage}";

        public int ItemsPerPage { get; set; } = 10;
        private bool _isPreviousPageVisible;
        public bool IsPreviousPageVisible
        {
            get => _isPreviousPageVisible;
            set
            {
                _isPreviousPageVisible = value;
                OnPropertyChanged(nameof(IsPreviousPageVisible));
            }
        }

        public CurrencyListViewModel()
        {
            string apiKey = "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83";
            _apiService = new CoinMarketCapApiService(apiKey);
            Currencies = new ObservableCollection<Currency>();
            FilteredCurrencies = new ObservableCollection<Currency>();
            NavigateConverterCommand = new RelayCommand(o => NavigateToConverter());
            NextPageCommand = new RelayCommand(o => NextPage());
            PreviousPageCommand = new RelayCommand(o => PreviousPage(), o => CurrentPage > 1);
            CurrentPage = 1;
            IsPreviousPageVisible = false;
            LoadCurrencies();
        }

        private async void LoadCurrencies()
        {
            int start = ((CurrentPage - 1) * ItemsPerPage) + 1;
            var data = await _apiService.GetTopCurrenciesAsync(start, ItemsPerPage);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Currencies.Clear();
                foreach (var c in data)
                    Currencies.Add(c);
                if (string.IsNullOrWhiteSpace(SearchText))
                    FilterLocalCurrencies();
            });
        }

        private async void FilterCurrencies()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilterLocalCurrencies();
            }
            else
            {
                var searchResults = await _apiService.SearchCurrenciesAsync(SearchText);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FilteredCurrencies.Clear();
                    foreach (var c in searchResults)
                        FilteredCurrencies.Add(c);
                });
            }
        }

        private void FilterLocalCurrencies()
        {
            FilteredCurrencies.Clear();
            foreach (var c in Currencies)
                FilteredCurrencies.Add(c);
        }

        private void NavigateToConverter()
        {
            var frame = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.Navigate(new Views.ConverterView());
        }

        private void NextPage()
        {
            CurrentPage++;
            IsPreviousPageVisible = true;
            LoadCurrencies();
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                if (CurrentPage == 1)
                    IsPreviousPageVisible = false;
                LoadCurrencies();
            }
        }
    }
}
