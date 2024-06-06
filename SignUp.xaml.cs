using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;

namespace Игра_в_слова
{
    public partial class SignUp : Page
    {
        DBConnection db = new DBConnection();

        public SignUp()
        {
            InitializeComponent();

            // Открываем соединение с сервером
            db.InitConnection();

            // Заполнение массива инпутов
            tbs[0] = loginPut;
            tbs[1] = passwordPut;
            tbs[2] = namePut;
            tbs[3] = emailPut;
            tbs[4] = secretWordPut;

            // Заполнение массива паттернов регулярных выражений
            regexArray[0] = loginPut_regex;
            regexArray[1] = passwordPut_regex;
            regexArray[2] = namePut_regex;
            regexArray[3] = emailPut_regex;
            regexArray[4] = secretWordPut_regex;

            // Метод заполнения стран
            ComboBoxFill();
        }

        #region Фокус и заполнение странами
        // Заполнение стран
        private void ComboBoxFill()
        {
            string path = "../../Resources/countriesList.txt";
            string[] countriesList = File.ReadAllLines(path);
            foreach (string t in countriesList)
            {
                countriesComboBox.Items.Add(t);
            }
        }

        // Фокус на любом инпуте
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox).Text == (sender as TextBox).Tag.ToString())
            {
                (sender as TextBox).Text = "";
            }
        }

        // Расфокус любого инпута
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
            }
        }
        #endregion

        #region Переменные
        // Regex выражения
        string loginPut_regex = @"^[aA-zZ0-9]+$";
        string passwordPut_regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
        string namePut_regex = @"^[аА-яЯ]+$";
        string emailPut_regex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        string secretWordPut_regex = @"^[A-Za-z]+$";

        // Массив паттернов для регулярных выражений
        string[] regexArray = new string[5];

        // Булевы переменные для проверок
        bool isError = false;
        bool isFilled = false;

        // Массив инпутов
        private TextBox[] tbs = new TextBox[5];
        // Выбранный пол
        private string gender = "";
        #endregion

        #region Основные методы

        // Проверка на заполнение всех полей
        private void CheckFillTextBox()
        {
            byte countOfErrors = 0;

            // Проверяем есть ли незаполненные первичные поля
            for (int i = 0; i < tbs.Length; i++)
            {
                if ((tbs[i].Text == tbs[i].Tag.ToString()) || (String.IsNullOrWhiteSpace(tbs[i].Text)))
                {
                    tbs[i].Style = this.Resources["TextError"] as Style;
                    countOfErrors++;
                }
                else tbs[i].Style = (Style)this.FindResource("inputStyle");
            }

            // Если все первичные поля заполнены
            if (countOfErrors != 0)
            {
                MessageBox.Show("Есть незаполненные поля!\nВсе поля обязательны к заполнению!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Проверяем второстепенные поля
            else
            {
                if (gender == "")
                {
                    MessageBox.Show("Укажите пол!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (countriesComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Укажите страну!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (datePut.SelectedDate == null)
                {
                    MessageBox.Show("Укажите дату рождения!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    isFilled = true;
                }
            }
        }

        // Проверка правильности заполнения
        private void CheckCorrectnessTextBox()
        {
            for (int i = 0; i < tbs.Length; i++)
            {
                var regex_name = tbs[i].Name + "_regex";
                Regex rg = new Regex(regexArray[i]);
                do
                {
                    if (rg.IsMatch(tbs[i].Text) == false)
                    {
                        tbs[i].Style = this.Resources["TextError"] as Style;
                        isError = true;
                        MessageBox.Show("Неправильно!");
                        return;
                    }
                    else
                    {
                        tbs[i].Style = this.Resources["TextSuccess"] as Style;
                        isError = false;
                    }
                    break;
                }
                while (isError == true);
            }
            RegistrationDiirect();
        }

        // Получить время
        private DateTime GetTimeNow()
        {
            try
            {
                using (var response =
                  WebRequest.Create("http://www.google.com").GetResponse())
                    return DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);
            }
            catch (WebException)
            {
                return DateTime.Now; //In case something goes wrong. 
            }
        }

        // Метод регистрации
        private void RegistrationDiirect()
        {
            string loginUser = loginPut.Text;
            string passwordUser = passwordPut.Text;
            string user_name = namePut.Text;
            string email = emailPut.Text;
            string country = countriesComboBox.SelectedItem.ToString();
            string secretWord = secretWordPut.Text;

            DateTime age_date = datePut.SelectedDate.Value;

            DateTime reg_date = GetTimeNow();

            // Подключаемся к БД
            bool result = db.Registration_Procedure(loginUser, passwordUser, user_name, email, gender, country, age_date, reg_date, secretWord);

            if (result == true)
            {
                MessageBoxResult resultBox = MessageBox.Show("Вы успешно зарегистрировались!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                if (resultBox == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("SignIn.xaml", UriKind.Relative));
                }
            }
            else if (result == false)
            {
                MessageBox.Show($"Такой пользователь уже существует! Укажите другой логин, email", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Кнопка регистрации
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            CheckFillTextBox();

            if (isFilled == true)
            {
                CheckCorrectnessTextBox();
            }
        }

        // Узнаём выбранный пол
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            gender = (sender as RadioButton).Content.ToString();
        }

        // FAQ
        private void FAQText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Логин: англ. буквы и цифры\n" +
                "Пароль: англ.буквы и цифры (первая буква заглавная, хотя бы один спец.символ)\n" +
                "Имя: на русском (первая буква заглавная)\n" +
                "Секретное слово: английские буквы\n" +
                "ВАЖНО: логин и почта уникальны, учтите это при регистрации!!!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Переход на страницу авторизации
        private void SignInText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("SignIn.xaml", UriKind.Relative));
        }
        #endregion

        // Переход на предыдущую страницу
        private void goBackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}