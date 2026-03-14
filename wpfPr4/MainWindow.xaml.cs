using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace wpfPr4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.Page1());
        }
        private void GoPage1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.Page1());
        }
        private void GoPage2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.Page2());
        }
        private void GoPage3_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.Page3());
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var res = MessageBox.Show("Точно выйти из приложения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res != MessageBoxResult.Yes)
                e.Cancel = true;
        }

    }
}
