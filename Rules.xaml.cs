using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Игра_в_слова
{
    /// <summary>
    /// Логика взаимодействия для Rules.xaml
    /// </summary>
    public partial class Rules : Page
    {
        public Rules()
        {
            InitializeComponent();

            // Открыть личный кабинет
            if (Properties.Settings.Default.loginUser != "")
            {
                persAccount.Visibility = Visibility.Visible;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.loginUser == "")
            {
                NavigationService.Navigate(new Uri("VariantOfGame.xaml", UriKind.Relative));
            }
            else NavigationService.Navigate(new Uri("Settings.xaml", UriKind.Relative));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
