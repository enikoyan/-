using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Игра_в_слова
{
    public partial class VariantOfGame : Page
    {
        public VariantOfGame()
        {
            InitializeComponent();
        }

        // Продолжить играть без сохранения
        private void PlayWithoutSignIn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Settings.xaml", UriKind.Relative));
        }

        // Перейти к этапу входа | регистрации
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("SignIn.xaml", UriKind.Relative));
        }
    }
}
