using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace wpfPr4.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        private List<PointData> _last = new List<PointData>();

        public Page3()
        {
            InitializeComponent();

            PlotCanvas.SizeChanged += PlotCanvas_SizeChanged;
        }

        private void PlotCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_last != null && _last.Count > 0)
                DrawPlot(_last);
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            double x0;
            double xk;
            double dx;
            double d;

            if (!TryParseDouble(TbX0.Text, out x0))
            {
                MessageBox.Show("Введите корректное число x0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbX0.Focus();
                return;
            }

            if (!TryParseDouble(TbXk.Text, out xk))
            {
                MessageBox.Show("Введите корректное число xk.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbXk.Focus();
                return;
            }

            if (!TryParseDouble(TbDx.Text, out dx))
            {
                MessageBox.Show("Введите корректное число dx.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbDx.Focus();
                return;
            }

            if (!TryParseDouble(TbD.Text, out d))
            {
                MessageBox.Show("Введите корректное число d.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbD.Focus();
                return;
            }

            List<PointData> data;
            string error;

            if (!AllFormuls.TryBuildTable(x0, xk, dx, d, out data, out error))
            {
                MessageBox.Show(error, "Ошибка вычисления", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("x\t\ty");

            int i;
            for (i = 0; i < data.Count; i++)
            {
                if (data[i].IsValid)
                    sb.AppendLine(Fmt(data[i].X) + "\t\t" + Fmt(data[i].Y));
                else
                    sb.AppendLine(Fmt(data[i].X) + "\t\tundefined");
            }

            TbLog.Text = sb.ToString();
            _last = data;

            DrawPlot(data);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TbX0.Clear();
            TbXk.Clear();
            TbDx.Clear();
            TbD.Clear();
            TbLog.Clear();

            _last.Clear();
            PlotCanvas.Children.Clear();

            TbHint.Text = "Нажмите «Вычислить», чтобы построить график";
            TbHint.Visibility = Visibility.Visible;
        }

        private void DrawPlot(List<PointData> data)
        {
            PlotCanvas.Children.Clear();

            double w = PlotCanvas.ActualWidth;
            double h = PlotCanvas.ActualHeight;

            if (w < 50 || h < 50)
                return;

            List<PointData> validPoints = new List<PointData>();

            int i;
            for (i = 0; i < data.Count; i++)
            {
                if (data[i].IsValid)
                    validPoints.Add(data[i]);
            }

            if (validPoints.Count < 2)
            {
                TbHint.Text = "Недостаточно точек для построения графика.";
                TbHint.Visibility = Visibility.Visible;
                return;
            }

            TbHint.Visibility = Visibility.Collapsed;

            double xmin = validPoints[0].X;
            double xmax = validPoints[0].X;
            double ymin = validPoints[0].Y;
            double ymax = validPoints[0].Y;

            for (i = 1; i < validPoints.Count; i++)
            {
                if (validPoints[i].X < xmin) xmin = validPoints[i].X;
                if (validPoints[i].X > xmax) xmax = validPoints[i].X;
                if (validPoints[i].Y < ymin) ymin = validPoints[i].Y;
                if (validPoints[i].Y > ymax) ymax = validPoints[i].Y;
            }

            if (Math.Abs(xmax - xmin) < 1e-12)
                xmax = xmin + 1;

            if (Math.Abs(ymax - ymin) < 1e-12)
                ymax = ymin + 1;

            double left = 50;
            double right = 20;
            double top = 20;
            double bottom = 40;

            double plotWidth = w - left - right;
            double plotHeight = h - top - bottom;

            double yPad = (ymax - ymin) * 0.1;
            if (yPad <= 0) yPad = 1;

            ymin -= yPad;
            ymax += yPad;

            Rectangle border = new Rectangle();
            border.Width = plotWidth;
            border.Height = plotHeight;
            border.Stroke = Brushes.LightGray;
            border.StrokeThickness = 1;

            Canvas.SetLeft(border, left);
            Canvas.SetTop(border, top);
            PlotCanvas.Children.Add(border);

            if (xmin <= 0 && xmax >= 0)
            {
                Line axisY = new Line();
                axisY.X1 = MapX(0, xmin, xmax, left, plotWidth);
                axisY.X2 = axisY.X1;
                axisY.Y1 = top;
                axisY.Y2 = top + plotHeight;
                axisY.Stroke = Brushes.Gray;
                axisY.StrokeThickness = 1;
                PlotCanvas.Children.Add(axisY);
            }

            if (ymin <= 0 && ymax >= 0)
            {
                Line axisX = new Line();
                axisX.X1 = left;
                axisX.X2 = left + plotWidth;
                axisX.Y1 = MapY(0, ymin, ymax, top, plotHeight);
                axisX.Y2 = axisX.Y1;
                axisX.Stroke = Brushes.Gray;
                axisX.StrokeThickness = 1;
                PlotCanvas.Children.Add(axisX);
            }

            AddLabel(Fmt(xmin), MapX(xmin, xmin, xmax, left, plotWidth), top + plotHeight + 8);
            AddLabel(Fmt(xmax), MapX(xmax, xmin, xmax, left, plotWidth) - 30, top + plotHeight + 8);
            AddLabel(Fmt(ymin), 5, MapY(ymin, ymin, ymax, top, plotHeight) - 10);
            AddLabel(Fmt(ymax), 5, MapY(ymax, ymin, ymax, top, plotHeight) - 10);

            Polyline currentLine = null;

            for (i = 0; i < data.Count; i++)
            {
                if (!data[i].IsValid)
                {
                    currentLine = null;
                    continue;
                }

                Point p = new Point(
                    MapX(data[i].X, xmin, xmax, left, plotWidth),
                    MapY(data[i].Y, ymin, ymax, top, plotHeight)
                );

                if (currentLine == null)
                {
                    currentLine = new Polyline();
                    currentLine.Stroke = Brushes.DodgerBlue;
                    currentLine.StrokeThickness = 2;
                    PlotCanvas.Children.Add(currentLine);
                }

                currentLine.Points.Add(p);
            }
        }

        private void AddLabel(string text, double x, double y)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Foreground = Brushes.Gray;
            tb.FontSize = 12;

            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);

            PlotCanvas.Children.Add(tb);
        }

        private double MapX(double x, double xmin, double xmax, double left, double plotWidth)
        {
            return left + (x - xmin) / (xmax - xmin) * plotWidth;
        }

        private double MapY(double y, double ymin, double ymax, double top, double plotHeight)
        {
            return top + (ymax - y) / (ymax - ymin) * plotHeight;
        }

        private static string Fmt(double v)
        {
            return v.ToString("G10", CultureInfo.InvariantCulture);
        }

        private static bool TryParseDouble(string text, out double value)
        {
            text = (text ?? "").Trim();

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
                return true;

            text = text.Replace(',', '.');
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}