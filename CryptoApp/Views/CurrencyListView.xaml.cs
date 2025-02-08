using System.Windows.Controls;
using System.Windows.Navigation;
using CryptoApp.Models;
using CryptoApp.ViewModels;
namespace CryptoApp.Views
{
    public partial class CurrencyListView : Page
    {
        public CurrencyListView()
        {
            InitializeComponent();
            DataContext = new CurrencyListViewModel();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem is Currency selected)
                NavigationService.Navigate(new CurrencyDetailView(selected.Id, "1df5b82e-ed64-4b29-b27c-0b5f4abcbe83"));
        }
    }
}
