using System.Windows;
using System.Windows.Input;
using CryptoApp.Services;
namespace CryptoApp.ViewModels
{
    public class CurrencyDetailViewModel : BaseViewModel
    {
        private CoinMarketCapApiService _apiService;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string PriceUsd { get; set; }
        public string VolumeUsd { get; set; }
        public string ChangePercent24Hr { get; set; }
        public ICommand NavigateBackCommand { get; set; }
        public CurrencyDetailViewModel(string currencyId, string apiKey)
        {
            Id = currencyId;
            _apiService = new CoinMarketCapApiService(apiKey);
            NavigateBackCommand = new RelayCommand(o => NavigateBack());
            LoadDetails();
        }
        private async void LoadDetails()
        {
            var detail = await _apiService.GetCurrencyDetailAsync(Id);
            if (detail != null)
            {
                Name = detail.Name;
                Symbol = detail.Symbol;
                PriceUsd = detail.PriceUsd;
                VolumeUsd = detail.VolumeUsd;
                ChangePercent24Hr = detail.ChangePercent24Hr;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Symbol));
                OnPropertyChanged(nameof(PriceUsd));
                OnPropertyChanged(nameof(VolumeUsd));
                OnPropertyChanged(nameof(ChangePercent24Hr));
            }
        }
        private void NavigateBack()
        {
            var frame = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x is MainWindow) as MainWindow;
            frame?.MainFrame.GoBack();
        }
    }
}
