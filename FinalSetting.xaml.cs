using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Игра_в_слова
{
    public partial class FinalSetting : Page
    {
        public FinalSetting()
        {
            InitializeComponent();

            // Открыть личный кабинет
            if (Properties.Settings.Default.loginUser != "")
            {
                persAccount.Visibility = Visibility.Visible;
            }
        }

        private readonly Brush chooseColor = new SolidColorBrush(Color.FromRgb(15, 111, 255));
        private readonly Brush unchooseColor = new SolidColorBrush(Color.FromRgb(30, 35, 85));

        // Игра с компьютером
        private void PlayToComputerRBtn_Checked(object sender, RoutedEventArgs e)
        {
            playToComputerBorder.Background = chooseColor;
            playToPersonRBtn.IsChecked = false;
            playToPersonBorder.Background = unchooseColor;

            // Режим игры
            App.Current.Resources["gameMode"] = "gameToComputer";
        }

        private void PlayToComputerBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            playToComputerRBtn.IsChecked = true;
        }

        // Игра с пользователем
        private void PlayToPersonRBtn_Checked(object sender, RoutedEventArgs e)
        {
            playToPersonBorder.Background = chooseColor;
            playToComputerRBtn.IsChecked = false;
            playToComputerBorder.Background = unchooseColor;

            // Режим игры
            App.Current.Resources["gameMode"] = "gameToPerson";
        }

        private void PlayToPersonBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            playToPersonRBtn.IsChecked = true;
        }

        // Кнопка НАЗАД
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if(persAccount.Visibility == Visibility.Hidden)
            {
                NavigationService.Navigate(new Uri("VariantOfGame.xaml", UriKind.Relative));
            }
            else NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));

        }

        // Кнопка вперёд
        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playToComputerRBtn.IsChecked == false && playToPersonRBtn.IsChecked == false)
            {
                MessageBox.Show("Выберите режим игры!","Предупреждение",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (playToComputerRBtn.IsChecked == true)
                {
                    // Не авторизованный пользователь выбрал игру с компьютером
                    if (Properties.Settings.Default.loginUser == "")
                    {
                        putNameGrid.Visibility = Visibility.Visible;
                        playGrid.IsEnabled = false;
                        playGrid.Opacity = 0.1;
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri("Game.xaml", UriKind.Relative));
                    }
                }

                else
                {
                    // Не авторизованный пользователь выбрал игру с другом
                    if (Properties.Settings.Default.loginUser == "")
                    {
                        putNamesGrid.Visibility = Visibility.Visible;
                        playGrid.IsEnabled = false;
                        playGrid.Opacity = 0.1;
                    }
                    else
                    {
                        tb1.IsEnabled = false;
                        tb1.Opacity = 0.4;
                        tb1.Text = Properties.Settings.Default.nameUser;
                        putNamesGrid.Visibility = Visibility.Visible;
                        playGrid.IsEnabled = false;
                        playGrid.Opacity = 0.1;
                    }
                }
            }
        }

        // Обработка инпутов (placeholders)
        private void NamePut_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if((sender as TextBox).Text == (sender as TextBox).Tag.ToString())
            {
                (sender as TextBox).Text = "";
            }
        }

        private void NamePut_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
            }
        }

        // Играть после ввода имени при выборе игры с компьютером
        private void PlayToComputerBtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(namePut.Text) || namePut.Text == namePut.Tag.ToString())
            {
                MessageBox.Show("Введите имя!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Properties.Settings.Default.nameUser = namePut.Text;
                NavigationService.Navigate(new Uri("Game.xaml", UriKind.Relative));
            }
        }

        // Отмена ввода имени для игры с компьютером
        private void PlayToComputerBtnBack_Click(object sender, RoutedEventArgs e)
        {
            putNameGrid.Visibility = Visibility.Hidden;
            playGrid.IsEnabled = true;
            playGrid.Opacity = 1;
        }

        // Играть после ввода имени при выборе игры с другом
        private void PlayToPersonBtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tb1.Text) || String.IsNullOrWhiteSpace(tb2.Text) || tb1.Text == tb1.Tag.ToString() || tb2.Text == tb2.Tag.ToString())
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Properties.Settings.Default.nameUser = tb1.Text;
                Properties.Settings.Default.secondUserName = tb2.Text;

                NavigationService.Navigate(new Uri("Game.xaml", UriKind.Relative));
            }
        }

        // Отмена ввода имени для игры с другом
        private void PlayToPersonBtnBack_Click(object sender, RoutedEventArgs e)
        {
            putNamesGrid.Visibility = Visibility.Hidden;
            playGrid.IsEnabled = true;
            playGrid.Opacity = 1;
        }
    }
}