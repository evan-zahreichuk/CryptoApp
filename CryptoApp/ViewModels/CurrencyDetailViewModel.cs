using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using CryptoApp.Models;
using CryptoApp.Services;

namespace CryptoApp.ViewModels
{
    public class CurrencyDetailViewModel : BaseViewModel
    {
        private readonly string _currencyId;
        private readonly string _apiKey;
        private readonly CoinMarketCapApiService _apiService;
        private string _currentTimeframe = "1d"; // default timeframe: last 24 hours

        public CurrencyDetailViewModel(string currencyId, string apiKey)
        {
            _currencyId = currencyId;
            _apiKey = apiKey;
            _apiService = new CoinMarketCapApiService(apiKey);

            // Initialize commands first
            LoadChartCommand = new RelayCommand(async param =>
            {
                _currentTimeframe = param?.ToString() ?? "1d";
                await LoadChartData(_currentTimeframe);
            });
            NavigateBackCommand = new RelayCommand(o => NavigateBack());

            // Initialize available chart currencies and default selection
            ChartCurrencies = new ObservableCollection<string> { "USD", "EUR", "UAH" };
            // Set default after commands are initialized
            SelectedChartCurrency = "USD";

            // Load currency details and initial chart data (24h)
            LoadCurrencyDetail();
            LoadChartCommand.Execute("1d");
        }

        // Display properties
        private string _displayTitle;
        public string DisplayTitle
        {
            get => _displayTitle;
            set { _displayTitle = value; OnPropertyChanged(nameof(DisplayTitle)); }
        }

        private string _subTitle;
        public string SubTitle
        {
            get => _subTitle;
            set { _subTitle = value; OnPropertyChanged(nameof(SubTitle)); }
        }

        private SeriesCollection _chartSeries;
        public SeriesCollection ChartSeries
        {
            get => _chartSeries;
            set { _chartSeries = value; OnPropertyChanged(nameof(ChartSeries)); }
        }

        private List<string> _chartLabels;
        public List<string> ChartLabels
        {
            get => _chartLabels;
            set { _chartLabels = value; OnPropertyChanged(nameof(ChartLabels)); }
        }

        private Func<double, string> _yFormatter;
        public Func<double, string> YFormatter
        {
            get => _yFormatter;
            set { _yFormatter = value; OnPropertyChanged(nameof(YFormatter)); }
        }

        private string _lastUpdatedText;
        public string LastUpdatedText
        {
            get => _lastUpdatedText;
            set { _lastUpdatedText = value; OnPropertyChanged(nameof(LastUpdatedText)); }
        }

        // Chart currency selection
        public ObservableCollection<string> ChartCurrencies { get; set; }
        private string _selectedChartCurrency;
        public string SelectedChartCurrency
        {
            get => _selectedChartCurrency;
            set
            {
                _selectedChartCurrency = value;
                OnPropertyChanged(nameof(SelectedChartCurrency));
                // Check if LoadChartCommand is initialized before executing
                if (LoadChartCommand != null)
                    LoadChartCommand.Execute(_currentTimeframe);
            }
        }

        // Commands
        public ICommand LoadChartCommand { get; set; }
        public ICommand NavigateBackCommand { get; set; }

        // Load currency details from API
        private async void LoadCurrencyDetail()
        {
            try
            {
                Currency detail = await _apiService.GetCurrencyDetailAsync(_currencyId);
                DisplayTitle = $"{detail.Name} ({detail.Symbol})";
                SubTitle = $"{detail.Symbol} to USD: 1 {detail.Symbol} = ${detail.PriceUsd}";
                LastUpdatedText = $"Last updated: {DateTime.Now:g}";
            }
            catch (Exception ex)
            {
                DisplayTitle = "Error loading data";
                SubTitle = ex.Message;
            }
        }

        // Load chart data based on timeframe and selected chart currency.
        private async Task LoadChartData(string timeframe)
        {
            await Task.Delay(500); // Simulate API call delay

            int pointsCount = 24; // default for "1d"
            string labelFormat = "HH:mm";
            switch (timeframe)
            {
                case "1d":
                    pointsCount = 24;
                    labelFormat = "HH:mm";
                    break;
                case "7d":
                    pointsCount = 7;
                    labelFormat = "dd MMM";
                    break;
                case "1m":
                    pointsCount = 30;
                    labelFormat = "dd MMM";
                    break;
                case "3m":
                    pointsCount = 90;
                    labelFormat = "dd MMM";
                    break;
                case "1y":
                    pointsCount = 12;
                    labelFormat = "MMM yyyy";
                    break;
                default:
                    pointsCount = 24;
                    labelFormat = "HH:mm";
                    break;
            }

            // Determine conversion factor based on the selected chart currency.
            double conversionFactor = 1.0;
            if (SelectedChartCurrency == "EUR")
                conversionFactor = 0.9;
            else if (SelectedChartCurrency == "UAH")
                conversionFactor = 36;

            var random = new Random();
            var values = new ChartValues<double>();
            var labels = new List<string>();
            DateTime now = DateTime.Now;

            for (int i = 0; i < pointsCount; i++)
            {
                double basePrice = random.Next(30000, 60000);
                double convertedPrice = basePrice * conversionFactor;
                values.Add(convertedPrice);

                DateTime labelTime;
                if (timeframe == "1d")
                    labelTime = now.AddHours(-pointsCount + i + 1);
                else if (timeframe == "7d" || timeframe == "1m" || timeframe == "3m")
                    labelTime = now.AddDays(-pointsCount + i + 1);
                else // "1y"
                    labelTime = now.AddMonths(-pointsCount + i + 1);

                labels.Add(labelTime.ToString(labelFormat));
            }

            ChartSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Price",
                    Values = values,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8
                }
            };
            ChartLabels = labels;
            YFormatter = value =>
            {
                string symbol = SelectedChartCurrency == "USD" ? "$" :
                                SelectedChartCurrency == "EUR" ? "€" :
                                SelectedChartCurrency == "UAH" ? "₴" : "$";
                return symbol + value.ToString("N0");
            };
            LastUpdatedText = $"Last updated: {DateTime.Now:g}";
        }

        // Navigate back using the MainWindow frame.
        private void NavigateBack()
        {
            var frame = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.GoBack();
        }
    }
}
