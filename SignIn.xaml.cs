using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Игра_в_слова
{
    public partial class SignIn : Page
    {
        // Шрифты
        FontFamily mainFont = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Visby CF Medium");
        FontFamily passwordFont = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#password");

        DBConnection db = new DBConnection();

        public SignIn()
        {
            InitializeComponent();

            // Открываем соединение с сервером
            db.InitConnection();
        }

        #region Фокус на инпутах

        // Скрыть placeholder при фокусе на инпуте
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox).Text == (sender as TextBox).Tag.ToString())
            {
                (sender as TextBox).Text = "";
            }
        }

        // Показать placeholder при расфокусе инпута и отсутствии текста внутри
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
            }
        }

        #endregion

        #region Основные методы

        // Переход на страницу регистрации
        private void RegText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("SignUp.xaml", UriKind.Relative));
        }

        /* СКРЫТЬ / ПОКАЗАТЬ ПАРОЛЬ */
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            passwordPut.FontFamily = mainFont;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordPut.FontFamily = passwordFont;
        }

        /*-----------------*/

        // Выбор чекбокса по нажатию на текст
        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkBoxPassword.IsChecked == false)
            {
                checkBoxPassword.IsChecked = true;
            }
            else checkBoxPassword.IsChecked = false;
        }


        #endregion

        // Авторизация
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginPut.Text == loginPut.Tag.ToString() || String.IsNullOrWhiteSpace(loginPut.Text) || String.IsNullOrWhiteSpace(passwordPut.Text) || passwordPut.Text == passwordPut.Tag.ToString())
            {
                MessageBox.Show("Введите логин и пароль!", "Заполните все поля", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string loginUser = loginPut.Text;
                string passwordUser = passwordPut.Text;

                var result = db.Authorization_Procedure(loginUser, passwordUser);

                // Авторизоваться если ответ пришёл успешный
                if (result == true)
                {
                    NavigationService.Navigate(new Uri("Settings.xaml", UriKind.Relative));
                }
            }
        }

        #region Восстановление пароля
        // Открыть окно восстановления пароля
        private void TextBlock_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(loginPut.Text) == true || loginPut.Text == loginPut.Tag.ToString())
            {
                MessageBox.Show("Для восстановления пароля укажите логин!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                signBg.Opacity = 0.1;
                signBg.IsEnabled = false;
                recoverPasswordBg.Visibility = Visibility.Visible;
            }
        }

        // Закрыть окно восстановления пароля
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            signBg.Opacity = 1;
            signBg.IsEnabled = true;
            recoverPasswordBg.Visibility = Visibility.Hidden;
        }

        // Кнопка восстановить пароль
        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string userPassword = db.RecoverPassword_Procedure(loginPut.Text, secretWord.Text);

            if (userPassword == "Fail")
            {
                MessageBox.Show($"Секретное слово неверное или такого аккаунта не существует. Перепроверьте введённые данные", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                MessageBoxResult result = MessageBox.Show($"Ваш пароль: {userPassword} скопирован в буфер обмена!", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Information);
                // В буфер обмена
                Clipboard.SetText(userPassword);
                if (result == MessageBoxResult.OK)
                {
                    signBg.Opacity = 1;
                    signBg.IsEnabled = true;
                    recoverPasswordBg.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        // Переход на предыдущую страницу
        private void goBackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}