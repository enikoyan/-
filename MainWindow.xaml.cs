using System;
using System.Windows;

namespace Игра_в_слова
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainPage.Content = new MainPage();
        }
    }
}
