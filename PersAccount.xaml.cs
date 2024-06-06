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
    /// Логика взаимодействия для PersAccount.xaml
    /// </summary>
    public partial class PersAccount : Page
    {
        public PersAccount()
        {
            InitializeComponent();
        }

        // Выход из аккаунта
        private void exitAccount_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Вы уверены что хотите выйти из аккаунта?", "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (res == MessageBoxResult.OK)
            {
                Properties.Settings.Default.loginUser = "";
                Properties.Settings.Default.nameUser = "";
                Properties.Settings.Default.gender = "";
                Properties.Settings.Default.usedWord_count = 0;
                Properties.Settings.Default.win_time = 0;
                Properties.Settings.Default.games_count = 0;
                Properties.Settings.Default.hint_count = 0;
                Properties.Settings.Default.Save();

                MessageBox.Show("Вы вышли из аккаунта!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new Uri("VariantOfGame.xaml", UriKind.Relative));
            }
        }

        // Кнопка вернуться в меню
        private void ReturnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Обработка кнопок
        private void PersData_Click(object sender, RoutedEventArgs e)
        {

        }

        // Кнопка изменить данные
        private void ChangeDataBtn_Click(object sender, RoutedEventArgs e)
        {
            // РАЗБЛОКИРОВАТЬ ВСЕ ТЕКСТБОКСЫ
        }
    }
}
