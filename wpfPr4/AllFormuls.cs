using System;
using System.Collections.Generic;

namespace wpfPr4
{
    /// <summary>
    /// Класс с математическими функциями для практической работы.
    /// </summary>
    public static class AllFormuls
    {
        /// <summary>
        /// Вычисляет кубический корень.
        /// Подходит для старых версий .NET, где нет Math.Cbrt().
        /// </summary>
        public static double CubeRoot(double x)
        {
            if (x >= 0)
                return Math.Pow(x, 1.0 / 3.0);

            return -Math.Pow(Math.Abs(x), 1.0 / 3.0);
        }

        /// <summary>
        /// Вычисляет значение функции beta.
        /// Формула:
        /// beta = sqrt(10 * (cuberoot(x) + x^(y+2)) * (asin(z)^2 - |x-y|))
        /// </summary>
        public static bool TryCalculateBeta(double x, double y, double z, out double result, out string error)
        {
            result = 0;
            error = "";

            if (z < -1 || z > 1)
            {
                error = "Для asin(z) нужно, чтобы z находился в диапазоне [-1; 1].";
                return false;
            }

            if (x < 0)
            {
                error = "Для данной формулы рекомендуется вводить x >= 0.";
                return false;
            }

            double cubeRootX = CubeRoot(x);
            double powerPart = Math.Pow(x, y + 2);
            double asinPart = Math.Pow(Math.Asin(z), 2) - Math.Abs(x - y);

            double underRoot = 10.0 * (cubeRootX + powerPart) * asinPart;

            if (underRoot < 0)
            {
                error = "Подкоренное выражение не должно быть отрицательным.";
                return false;
            }

            result = Math.Sqrt(underRoot);
            return true;
        }

        /// <summary>
        /// Вычисляет f(x) для второй страницы.
        /// mode:
        /// 1 = sh(x)
        /// 2 = x^2
        /// 3 = e^x
        /// </summary>
        public static double CalculateFx(double x, int mode)
        {
            switch (mode)
            {
                case 1:
                    return Math.Sinh(x);
                case 2:
                    return x * x;
                case 3:
                    return Math.Exp(x);
                default:
                    throw new ArgumentException("Некорректный режим функции. Допустимы значения 1, 2, 3.");
            }
        }

        /// <summary>
        /// Вычисляет кусочную функцию g.
        /// </summary>
        public static bool TryCalculateG(double x, double b, int mode, out double result, out string error)
        {
            result = 0;
            error = "";

            try
            {
                double fx = CalculateFx(x, mode);
                double xb = x * b;

                if (xb > 0.5 && xb < 10)
                {
                    result = Math.Exp(fx - Math.Abs(b));
                    return true;
                }

                if (xb > 0.1 && xb < 0.5)
                {
                    result = Math.Sqrt(Math.Abs(fx + b));
                    return true;
                }

                result = 2 * fx * fx;
                return true;
            }
            catch (ArgumentException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Вычисляет y = x^2 + tan(5x + d/x).
        /// </summary>
        public static bool TryCalculateY(double x, double d, out double result, out string error)
        {
            result = 0;
            error = "";

            if (Math.Abs(x) < 1e-12)
            {
                error = "x не должен быть равен 0.";
                return false;
            }

            double arg = 5 * x + d / x;

            if (Math.Abs(Math.Cos(arg)) < 1e-12)
            {
                error = "Функция tan не определена для данного значения x.";
                return false;
            }

            result = x * x + Math.Tan(arg);
            return true;
        }

        /// <summary>
        /// Строит таблицу значений функции y на интервале [x0; xk] с шагом dx.
        /// Если в точке функция не определена, точка добавляется как невалидная.
        /// </summary>
        public static bool TryBuildTable(double x0, double xk, double dx, double d, out List<PointData> points, out string error)
        {
            points = new List<PointData>();
            error = "";

            if (Math.Abs(dx) < 1e-12)
            {
                error = "dx не должен быть равен 0.";
                return false;
            }

            if (xk > x0 && dx < 0)
                dx = Math.Abs(dx);

            if (xk < x0 && dx > 0)
                dx = -Math.Abs(dx);

            int guard = 0;
            double x = x0;

            while ((dx > 0 && x <= xk) || (dx < 0 && x >= xk))
            {
                double y;
                string localError;

                if (TryCalculateY(x, d, out y, out localError))
                {
                    points.Add(new PointData
                    {
                        X = x,
                        Y = y,
                        IsValid = true
                    });
                }
                else
                {
                    points.Add(new PointData
                    {
                        X = x,
                        Y = 0,
                        IsValid = false
                    });
                }

                x += dx;
                guard++;

                if (guard > 20000)
                {
                    error = "Слишком много точек. Увеличьте шаг dx.";
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Точка графика.
    /// </summary>
    public class PointData
    {
        /// <summary>
        /// Координата X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Показывает, можно ли использовать точку.
        /// </summary>
        public bool IsValid { get; set; }
    }
}