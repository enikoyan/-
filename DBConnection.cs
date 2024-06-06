using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;

namespace Игра_в_слова
{
    public class DBConnection
    {
        #region Переменные
        private static MySqlConnection connection { get; set; }
        private bool isConnected = false;
        private String host { get; set; }
        private String user { get; set; }
        private String password { get; set; }
        private String database { get; set; }
        private String port { get; set; }
        private String SQLConnection { get; set; }
        #endregion

        // Конструктор
        public DBConnection()
        {
            this.host = "cvk-pr.ru";
            this.user = "elik";
            this.password = "tKbr$0202";
            this.database = "elik";
            this.port = "3306";
        }

        // Подключение к серверу
        public void InitConnection()
        {
            if (isConnected == false)
            {
                DBConnection sql = new DBConnection();
                SQLConnection = $"Server={sql.host};port={port};Database={sql.database};Uid={sql.user};Pwd={sql.password};Convert Zero Datetime=True";
                isConnected = true;
            }
        }

        // Сохранить статистику игрока
        public bool SaveState()
        {
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                try
                {

                    connection.Open();

                    string queryString = $"INSERT INTO `State` (`Id`, `user_id`, `usedWords_count`, `games_count`, `win_time`, `hint_count`) " +
                        $"VALUES (NULL, (SELECT id_user FROM User WHERE login = '{Properties.Settings.Default.loginUser}'), @usedWords_count, @games_count, @win_time, @hint_count)" +
                        $"ON DUPLICATE KEY UPDATE " +
                        $"usedWords_count = @usedWords_count," +
                        $"games_count = @games_count," +
                        $"win_time = @win_time," +
                        $"hint_count = @hint_count";

                    MySqlCommand command = new MySqlCommand(queryString, connection);

                    command.Parameters.Clear();

                    command.Parameters.Add(new MySqlParameter("@usedWords_count", Properties.Settings.Default.usedWord_count));
                    command.Parameters.Add(new MySqlParameter("@games_count", Properties.Settings.Default.games_count));
                    command.Parameters.Add(new MySqlParameter("@win_time", Properties.Settings.Default.win_time));
                    command.Parameters.Add(new MySqlParameter("@hint_count", Properties.Settings.Default.hint_count));

                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
        }

        // Загрузить статистику игрока
        public void LoadState(string loginUser)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                try
                {

                    connection.Open();

                    string queryString = $"SELECT User.login, User.user_name, User.gender, State.usedWords_count, State.games_count, " +
                        $"State.win_time, State.hint_count " +
                        $"FROM State JOIN User ON User.id_user = State.user_id AND login = '{loginUser}'";

                    MySqlCommand command = new MySqlCommand(queryString, connection);

                    var dr = command.ExecuteReader();

                    if (dr.HasRows)
                    {
                        dr.Read();

                        // Сохраняем полученные данные
                        Properties.Settings.Default.loginUser = dr.GetString(0);
                        Properties.Settings.Default.nameUser = dr.GetString(1);
                        Properties.Settings.Default.gender = dr.GetString(2);
                        Properties.Settings.Default.usedWord_count = dr.GetInt32(3);
                        Properties.Settings.Default.games_count = dr.GetInt32(4);
                        Properties.Settings.Default.win_time = dr.GetDouble(5);
                        Properties.Settings.Default.hint_count = dr.GetInt32(6);
                        Properties.Settings.Default.Save();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Процедура регистрации
        public bool Registration_Procedure(string loginUser, string passwordUser, string user_name, string email, string gender, string country, DateTime age_date, DateTime reg_date, string secretWord)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = "Registration";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("login", loginUser);
                cmd.Parameters.AddWithValue("password", passwordUser);
                cmd.Parameters.AddWithValue("user_name", user_name);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("gender", gender);
                cmd.Parameters.AddWithValue("country", country);
                cmd.Parameters.AddWithValue("age_date", age_date);
                cmd.Parameters.AddWithValue("reg_date", reg_date);
                cmd.Parameters.AddWithValue("secret_word", secretWord);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else return false;
            }
        }

        // Процедура авторизации
        public bool Authorization_Procedure(string loginUser, string passwordUser)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    DataTable table = new DataTable();
                    MySqlCommand cmd = new MySqlCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Authorization";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("loginUser", loginUser);
                    cmd.Parameters.AddWithValue("passwordUser", passwordUser);

                    adapter.SelectCommand = cmd;
                    adapter.Fill(table);

                    // Получить значения имени, логина и пола игрока
                    var dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        // Сохраняем вход пользователя
                        Properties.Settings.Default.loginUser = loginUser;
                        Properties.Settings.Default.nameUser = dr.GetString(0);
                        Properties.Settings.Default.gender = dr.GetString(1);
                        Properties.Settings.Default.Save();
                    }

                    dr.Close();

                    if (table.Rows.Count == 1)
                    {
                        LoadState(loginUser);
                        MessageBoxResult result = MessageBox.Show($"Добро пожаловать, {Properties.Settings.Default.nameUser}", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"Такого игрока нет. Неправильный логин или пароль. Перепроверьте введённые данные", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
        }

        // Процедура восстановления пароля
        public string RecoverPassword_Procedure(string loginUser, string secretWord)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                DataTable table = new DataTable();
                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = "RecoverPassword";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("loginUser", loginUser);
                cmd.Parameters.AddWithValue("secretWord", secretWord);

                adapter.SelectCommand = cmd;
                adapter.Fill(table);

                if (table.Rows.Count == 1)
                {
                    string passwordUser = cmd.ExecuteScalar().ToString();
                    return passwordUser;
                }
                return "Fail";
            }
        }
    }
}