using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace wpfPr4.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private static bool TryParse(string s, out double v)
        {
            s = (s ?? "").Trim().Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            TbResult.Text = "";

            double x;
            double y;
            double z;

            if (!TryParse(TbX.Text, out x) ||
                !TryParse(TbY.Text, out y) ||
                !TryParse(TbZ.Text, out z))
            {
                MessageBox.Show("Введите корректные числа x, y, z.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double beta;
            string error;

            if (!AllFormuls.TryCalculateBeta(x, y, z, out beta, out error))
            {
                MessageBox.Show(error, "Ошибка вычисления",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TbResult.Text = beta.ToString("G17", CultureInfo.InvariantCulture);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TbX.Clear();
            TbY.Clear();
            TbZ.Clear();
            TbResult.Clear();
        }
    }
}