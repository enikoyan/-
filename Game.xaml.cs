using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Игра_в_слова
{
    public partial class Game : Page
    {
        #region Доп.функции

        // Автопрокрутка в окне описания процесса игры
        private void richText1_TextChanged(object sender, TextChangedEventArgs e)
        {
            richText1.ScrollToEnd();
        }

        // Функция рандомного подбора нового слова компьютером
        private void NewWord()
        {
            computer.Text = DataBase[new Random().Next(dataBase.Length)];
        }

        // Функция смены слова
        private void ChangeWord(object sender, RoutedEventArgs e)
        {
            // Добавляем слово компьютера в коллекцию использованных
            DataBaseCopy.Add(computer.Text);

            // Смена слова
            NewWord();

            // Проверка есть ли заменённое слово в использованных
            while (true)
            {
                if (DataBaseCopy.Contains(computer.Text))
                {
                    // Смена слова
                    NewWord();
                }
                break;
            }

            // Уменьшение кол-ва попыток
            Attempts--;
            Properties.Settings.Default.hint_count += Attempts;
            // Определение кол-ва попыток
            switch (Attempts)
            {
                case 2:
                    richText1.AppendText($"\r★★✰ Слово успешно поменялось, у вас осталось {Attempts} попытки");
                    break;
                case 1:
                    richText1.AppendText($"\r★✰✰ Слово успешно поменялось, у вас осталась {Attempts} попытка");
                    break;
                case 0:
                    richText1.AppendText($"\r✰✰✰ Слово успешно поменялось, у вас не осталось попыток");
                    changeWord.IsEnabled = false;
                    changeWord.Opacity = 0.2;
                    break;
            }

            player.Focus();
            LastLetterOfComputer();
        }

        // Скрыть поле игры
        private void HideGameField()
        {
            gameField.IsEnabled = false;
            gameField.Opacity = 0.4;
        }

        // Показать поле игры
        private void ShowGameField()
        {
            gameField.IsEnabled = true;
            gameField.Opacity = 1;
        }

        /* Кнопка остановить игру */
        private void StopGame(object sender, RoutedEventArgs e)
        {
            // Остановить таймер
            disTime.Stop();

            // Скрыть поле игры
            HideGameField();

            borderExit.IsEnabled = true;
            borderExit.Visibility = Visibility.Visible;
        }

        // Отменить выход из игры (из меню остановки)
        private void BackToGame(object sender, RoutedEventArgs e)
        {
            // Продолжить работу таймера
            disTime.Start();

            // Показать поле игры
            ShowGameField();

            // Скрыть поле exit
            borderExit.Visibility = Visibility.Hidden;
            borderExit.IsEnabled = false;
            player.Focus();
        }

        /* Кнопка правила */

        // Показать правила
        private void ShowRules(object sender, RoutedEventArgs e)
        {
            // Выключаем на время поле игры и останавливаем таймер
            disTime.Stop();
            gameField.Visibility = Visibility.Hidden;
            gameField.IsEnabled = false;

            // Показываем правила
            rulesField.Visibility = Visibility.Visible;
        }

        // Скрыть правила
        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            // Включаем таймер и показываем поле игры
            disTime.Start();
            gameField.Visibility = Visibility.Visible;
            gameField.IsEnabled = true;

            // Скрываем блок правил
            rulesField.Visibility = Visibility.Hidden;
            player.Focus();
        }

        /* Таймер */
        DispatcherTimer disTime = new DispatcherTimer();

        public void Timer()
        {
            disTime.Tick += new EventHandler(disTime_Tick);
            disTime.Interval = new TimeSpan(0, 0, 1);
            disTime.Start();
        }

        public void TimerRestart()
        {
            timerBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#006637");
            SecondsCount = 31;
            disTime.Start();
        }

        private void disTime_Tick(object sender, EventArgs e)
        {
            while (SecondsCount != 0)
            {
                SecondsCount--;
                textSecond.Text = Convert.ToString(SecondsCount);
                if (SecondsCount < 15)
                {
                    timerBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("red");
                }
                break;
            }

            // ПРОИГРЫШ игрока
            if (SecondsCount == 0)
            {
                // Определяем время в конце игры
                timeEnd = DateTime.Now;
                TimeSpan span = timeEnd - timeStart;
                pastTime = span.TotalSeconds;

                // Скрыть поле игры
                HideGameField();

                borderLose2.Visibility = Visibility.Visible;
                disTime.Stop();
            }
        }

        #endregion

        #region Переменные

        // Копия страницы с настройками, чтобы вернуться к ним
        private FinalSetting finalSettingsPage = new FinalSetting();
        public FinalSetting FinalSettingsPage { get => finalSettingsPage; set => finalSettingsPage = value; }

        // БД
        DBConnection db = new DBConnection();

        // Тема игры
        private string themeName = (string)App.Current.Resources["theme"];
        public string ThemeName { get => themeName; set => themeName = value; }

        // Секунды таймера
        private byte secondsCount = 30;
        public byte SecondsCount { get => secondsCount; set => secondsCount = value; }

        // Секунды для таймера (время игры пользователя)
        private double pastTime = 0;
        private DateTime timeStart;
        private DateTime timeEnd;

        // Нашёл компьютер слово или нет
        private bool findWord;
        public bool FindWord { get => findWord; set => findWord = value; }

        /* База данных */

        // Основная БД
        private string[] dataBase;
        public string[] DataBase { get => dataBase; set => dataBase = value; }

        // Путь к файлам БД
        private string path;
        public string Path { get => path; set => path = value; }

        // Копия для временных (использованных) слов
        private List<string> dataBaseCopy = new List<string>();
        public List<string> DataBaseCopy { get => dataBaseCopy; set => dataBaseCopy = value; }
        public byte Attempts { get; set; } = 3;

        // Символы определения последних и первых букв текстбоксов
        private char lastLetterOfComputer;
        private char firstLetterOfPlayer;
        private char lastLetterOfPlayer;
        public char LastLetterOfComputer1 { get => lastLetterOfComputer; set => lastLetterOfComputer = value; }
        public char FirstLetterOfPlayer1 { get => firstLetterOfPlayer; set => firstLetterOfPlayer = value; }
        public char LastLetterOfPlayer1 { get => lastLetterOfPlayer; set => lastLetterOfPlayer = value; }

        #endregion

        #region Первоначальный вход в игру

        public Game()
        {
            InitializeComponent();

            // Открываем соединение с сервером
            db.InitConnection();

            // Устанавливаем имя игрока
            playerName.Text = Properties.Settings.Default.nameUser;

            // Устанавливаем имя 2-го игрока, если выбран режим игры с другом
            if (App.Current.Resources["gameMode"].ToString() == "gameToPerson")
            {
                secondPlayerName.Text = Properties.Settings.Default.secondUserName;
            }
        }

        // Кнопка отмены (возвращение на прошлую страницу)
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(FinalSettingsPage);
        }

        // Кнопка начала игры
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Определяем время вначале игры
            timeStart = DateTime.Now;

            /* Работа с текущим меню */
            borderStart.IsEnabled = false; // выключаем меню
            borderStart.Visibility = Visibility.Hidden; // скрываем меню

            /* Работа с задним фоном (самой игрой) */
            ShowGameField(); // показываем поле игры
            player.Focus(); // фокусируемся на поле ввода пользователя

            StartGame(); // функция начать игру

            LastLetterOfComputer(); // функция определения последней буквы слова ПК
        }

        // Начало игры
        private void StartGame()
        {

            // Путь к файлам БД
            Path = $"../../Resources/Words/{themeName}.txt";

            // База данных (загрузка в массив)
            DataBase = File.ReadAllLines(Path, Encoding.Default);

            // Первое слово (рандомно из БД)
            computer.Text = DataBase[new Random().Next(dataBase.Length)];
            // Оповещаем пользователя
            richText1.AppendText("• Компьютер сказал слово, Ваша очередь!");
            // Запускаем  таймер
            Timer();
        }

        #endregion

        #region Сбор статистики

        // Играть заново
        private void PlayAgain(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.loginUser != "")
            {
                // Сохраняем данные
                SaveInfo();
            }

            NavigationService.GoBack();
        }

        // Выход из приложения через меню остановки игры
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.loginUser != "")
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("Вы уверены, что хотите покинуть игру? Несохраненные данные будут потеряны", "Предупреждение!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.OK)
                {
                    App.Current.Shutdown();
                }
            }
            else App.Current.Shutdown();
        }

        // Выход из приложения после проигрыша / выигрыша
        private void ExitGame(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.loginUser != "")
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("Сохранить результаты игры?", "Предупреждение!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                {
                    bool resultOfSave = SaveInfo();

                    if (resultOfSave == true)
                    {
                        System.Windows.MessageBox.Show("Данные успешно сохранены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        App.Current.Shutdown();
                    }
                }
                else if (res == MessageBoxResult.No) App.Current.Shutdown();
            }
            else
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("Вы уверены, что хотите покинуть игру?", "Предупреждение!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.OK)
                {
                    App.Current.Shutdown();
                }
            }
        }

        // Сохранение результатов из локальных данных в БД
        private bool SaveInfo()
        {
            try
            {
                StateCollect();

                // Заносим данные в БД
                bool result = db.SaveState();

                if(result == true)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                System.Windows.MessageBox.Show("Произошла ошибка, попробуйте снова! Проверьте подключение к интернету", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Занесение данных в локальное хранилище приложения на устройстве пользователя
        private void StateCollect()
        {
            // Сохраняем кол-во использованных слов
            Properties.Settings.Default.usedWord_count += dataBaseCopy.Count;

            // Сохраняем кол-во пройденных игр
            Properties.Settings.Default.games_count++;

            // Сохраняем время прохождения (лучшее)
            if (Properties.Settings.Default.win_time != 0)
            {
                if (pastTime < Properties.Settings.Default.win_time)
                {
                    Properties.Settings.Default.win_time = pastTime;
                }
            }
            else Properties.Settings.Default.win_time = pastTime;

            Properties.Settings.Default.Save();

        }

        #endregion

        #region Основная логика игры

        // ПЕРВАЯ БУКВА СЛОВА ИГРОКА
        private void FirstLetterOfPlayer()
        {
            FirstLetterOfPlayer1 = player.Text[0];
        }

        // ПОСЛЕДНЯЯ БУКВА СЛОВА КОМПЬЮТЕРА
        private void LastLetterOfComputer()
        {
            LastLetterOfComputer1 = computer.Text[computer.Text.Length - 1];
            // Проверяем на исключение последней буквы
            if ((LastLetterOfComputer1 == 'ь') || (LastLetterOfComputer1 == 'ъ') || (LastLetterOfComputer1 == 'ы') || (LastLetterOfComputer1 == 'й'))
            {
                LastLetterOfComputer1 = computer.Text[computer.Text.Length - 2];
            }
            else
            {
                LastLetterOfComputer1 = computer.Text[computer.Text.Length - 1];
            }
        }

        // ПОСЛЕДНЯЯ БУКВА СЛОВА ИГРОКА
        private void LastLetterOfPlayer()
        {
            LastLetterOfPlayer1 = player.Text[player.Text.Length - 1];
            // Проверяем на исключение последней буквы
            if ((LastLetterOfPlayer1 == 'ь') || (LastLetterOfPlayer1 == 'ъ') || (LastLetterOfPlayer1 == 'ы') || (LastLetterOfPlayer1 == 'й'))
            {
                LastLetterOfPlayer1 = player.Text[player.Text.Length - 2];
            }
            else
            {
                LastLetterOfPlayer1 = player.Text[player.Text.Length - 1];
            }
        }

        // Нажатие на кнопку (стрелочка) -- предложить слово
        private void SelectWord(object sender, RoutedEventArgs e)
        {
            // Проверяем не пустое ли поле ввода игрока
            if (String.IsNullOrWhiteSpace(player.Text))
            {
                System.Windows.MessageBox.Show("Поле не может быть пустым!", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
                player.Focus();
            }

            else
            {
                // Определяем первую букву слова игрока
                FirstLetterOfPlayer();

                // Последняя буква компьютера и первая буква игрока равны и слово не использовано
                if ((LastLetterOfComputer1 == FirstLetterOfPlayer1) && (!DataBaseCopy.Contains(player.Text)) && (computer.Text != player.Text))
                {
                    // Проверка на наличие слова в БД
                    if (DataBase.Contains(player.Text))
                    {
                        // Добавляем оба слова в коллекцию использованных
                        DataBaseCopy.Add(computer.Text);
                        DataBaseCopy.Add(player.Text);

                        // Определяем последнюю букву слова игрока
                        LastLetterOfPlayer();
                        player.Clear(); // очистить поле игрока

                        // Функция поиска слова со стороны компьютера
                        SearchWordOfComputer();
                    }
                    else
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Компьютер не знает такого слова!\rХотите добавить слово в базу?", "Предупреждение!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            // Добавляем в БД
                            File.AppendAllText(Path, Environment.NewLine + player.Text);
                        }
                        player.Clear();
                    }
                }

                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Вы нарушили правила игры!", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    player.Clear();
                    player.Focus();
                }

            }
        }

        // Логика поиска слова со стороны компьютера
        private void SearchWordOfComputer()
        {
            // Ищем слово в БД (если условие совпадает и слово не использовано)
            foreach (string word in DataBase)
            {
                if ((word[0] == LastLetterOfPlayer1) && (!DataBaseCopy.Contains(word)))
                {
                    richText1.AppendText("\r∎ Молодец, теперь ходит компьютер!\r• Компьютер сказал слово, Ваша очередь!");
                    TimerRestart();
                    computer.Text = word;
                    LastLetterOfComputer();
                    FindWord = true;
                    break;
                }
                else FindWord = false;
            }

            // ПРОИГРЫШ компьютера
            if (FindWord == false)
            {
                // Определяем время в конце игры
                timeEnd = DateTime.Now;
                TimeSpan span = timeEnd - timeStart;
                pastTime = span.TotalSeconds;

                borderLose.Visibility = Visibility.Visible; HideGameField(); disTime.Stop();
            }
        }
        #endregion
    }
}