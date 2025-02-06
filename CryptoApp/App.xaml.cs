using System;
using System.Linq;
using System.Windows;

namespace CryptoApp
{
    public partial class App : Application
    {
        public void ToggleTheme()
        {
            var darkThemeUri = new Uri("pack://application:,,,/Themes/DarkTheme.xaml", UriKind.Absolute);
            var lightThemeUri = new Uri("pack://application:,,,/Themes/LightTheme.xaml", UriKind.Absolute);
            var dictionaries = Current.Resources.MergedDictionaries;
            var currentTheme = dictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));

            if (currentTheme != null && currentTheme.Source.OriginalString.Contains("DarkTheme.xaml"))
            {
                dictionaries.Remove(currentTheme);
                dictionaries.Add(new ResourceDictionary() { Source = lightThemeUri });
            }
            else
            {
                if (currentTheme != null)
                    dictionaries.Remove(currentTheme);
                dictionaries.Add(new ResourceDictionary() { Source = darkThemeUri });
            }
        }
    }
}
