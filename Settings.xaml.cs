using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Игра_в_слова
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private string theme;
        private string path;
        public string Theme { get => theme; set => theme = value; }
        public string Path { get => path; set => path = value; }

        public Settings()
        {
            InitializeComponent();
            this.Width = 950;
            this.Height = 600;

            // Открыть личный кабинет
            if (Properties.Settings.Default.loginUser != "")
            {
                persAccount.Visibility = Visibility.Visible;
            }
        }

        // Вернуться назад
        private void BackButton(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Выбор темы и загрузка определенной БД
        private void ChooseThemeButton(object sender, RoutedEventArgs e)
        {
            // Проверка выбрана тема или нет
            if (radioButtons.Children.OfType<RadioButton>().All(c => c.IsChecked == false))
            {
                MessageBoxResult result = MessageBox.Show("Выберите тему!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Поиск нужной темы по названию radioButton
                foreach (RadioButton r in radioButtons.Children)
                {
                    if (r.IsChecked == true)
                    {
                        App.Current.Resources["theme"] = r.Name;
                    }
                }

                // Перемещение на следующую страницу
                NavigationService.Navigate(new Uri("FinalSetting.xaml", UriKind.Relative));
            }
        }

        // Выбор темы и загрузка определенного изображения
        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton.IsChecked == true)
            {
                Theme = radioButton.Name;
                // Путь к изображениям
                image.Source = new BitmapImage(new Uri($"Resources/Images/themeImg/{Theme}_img.png", UriKind.Relative));
            }
        }
    }
}