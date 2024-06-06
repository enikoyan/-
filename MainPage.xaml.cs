using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Игра_в_слова
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            // Запретить нажимать Backspace (чтобы не переходить на предыдущую страницу)
            foreach (InputGesture gesture in NavigationCommands.BrowseBack.InputGestures)
            {
                if (gesture is KeyGesture && ((KeyGesture)gesture).DisplayString.Equals("Backspace"))
                {
                    NavigationCommands.BrowseBack.InputGestures.Remove(gesture);
                    break;
                }
            }
        } 

        // Дальше
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.loginUser == "")
            {
                NavigationService.Navigate(new Uri("VariantOfGame.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("Settings.xaml", UriKind.Relative));
            }
        }

        // Правила
        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Rules.xaml", UriKind.Relative));
        }
    }
}
