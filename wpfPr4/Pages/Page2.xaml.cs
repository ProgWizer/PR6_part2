using System.Globalization;
using System.Windows;
using System.Windows.Controls;


namespace wpfPr4.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }

        private static bool TryParse(string s, out double v)
        {
            s = (s ?? "").Trim().Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
        }

        private int GetSelectedMode()
        {
            if (RbSinh.IsChecked == true)
                return 1;

            if (RbX2.IsChecked == true)
                return 2;

            return 3;
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            TbResult.Text = "";

            double x;
            double b;

            if (!TryParse(TbX.Text, out x) || !TryParse(TbB.Text, out b))
            {
                MessageBox.Show("Введите корректные числа x и b.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double g;
            string error;

            if (!AllFormuls.TryCalculateG(x, b, GetSelectedMode(), out g, out error))
            {
                MessageBox.Show(error, "Ошибка вычисления",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TbResult.Text = g.ToString("G17", CultureInfo.InvariantCulture);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TbX.Clear();
            TbB.Clear();
            TbResult.Clear();
            RbSinh.IsChecked = true;
        }
    }
}