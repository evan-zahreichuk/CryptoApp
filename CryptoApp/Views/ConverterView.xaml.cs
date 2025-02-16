using System.Windows;
using System.Windows.Controls;
namespace CryptoApp.Views
{
    public partial class ConverterView : Page
    {
        private bool _isDarkTheme;
        public ConverterView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ConverterViewModel();
        }
        private void SwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            string themeSource = _isDarkTheme ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";
            var newDict = new ResourceDictionary { Source = new Uri(themeSource, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newDict);
            _isDarkTheme = !_isDarkTheme;
            ThemeSwitchButton.Content = _isDarkTheme ? "☀" : "🌙";
        }
    }
}
