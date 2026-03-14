using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wpfPr4;

namespace UnitTestProject
{
    /// <summary>
    /// Набор модульных тестов для проверки математических функций.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Проверяет корректное вычисление beta при допустимых входных данных.
        /// </summary>
        [TestMethod]
        public void TryCalculateBeta_ValidData_ReturnsTrueAndCorrectResult()
        {
            double x = 1;
            double y = 0;
            double z = 1;

            double result;
            string error;

            bool ok = AllFormuls.TryCalculateBeta(x, y, z, out result, out error);

            double cubeRootX = 1;
            double expected = Math.Sqrt(
                10.0 * (cubeRootX + Math.Pow(1, 2)) *
                (Math.Pow(Math.Asin(1), 2) - Math.Abs(1 - 0))
            );

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.AreEqual(expected, result, 1e-10);
        }

        /// <summary>
        /// Проверяет, что функция beta возвращает false,
        /// если z выходит за область определения asin.
        /// </summary>
        [TestMethod]
        public void TryCalculateBeta_InvalidZ_ReturnsFalse()
        {
            double result;
            string error;

            bool ok = AllFormuls.TryCalculateBeta(1, 2, 2, out result, out error);

            Assert.IsFalse(ok);
            Assert.AreEqual(0.0, result, 0.0);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }

        /// <summary>
        /// Проверяет, что функция beta возвращает false,
        /// если подкоренное выражение отрицательное.
        /// </summary>
        [TestMethod]
        public void TryCalculateBeta_NegativeUnderRoot_ReturnsFalse()
        {
            double result;
            string error;

            bool ok = AllFormuls.TryCalculateBeta(1, 100, 0, out result, out error);

            Assert.IsFalse(ok);
            Assert.AreEqual(0.0, result, 0.0);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }

        /// <summary>
        /// Проверяет первую ветвь функции g: 0.5 &lt; x*b &lt; 10.
        /// </summary>
        [TestMethod]
        public void TryCalculateG_FirstBranch_ReturnsCorrectResult()
        {
            double x = 1;
            double b = 1;
            int mode = 2;

            double result;
            string error;

            bool ok = AllFormuls.TryCalculateG(x, b, mode, out result, out error);

            double fx = x * x;
            double expected = Math.Exp(fx - Math.Abs(b));

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.AreEqual(expected, result, 1e-10);
        }

        /// <summary>
        /// Проверяет вторую ветвь функции g: 0.1 &lt; x*b &lt; 0.5.
        /// </summary>
        [TestMethod]
        public void TryCalculateG_SecondBranch_ReturnsCorrectResult()
        {
            double x = 1;
            double b = 0.2;
            int mode = 2;

            double result;
            string error;

            bool ok = AllFormuls.TryCalculateG(x, b, mode, out result, out error);

            double fx = x * x;
            double expected = Math.Sqrt(Math.Abs(fx + b));

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.AreEqual(expected, result, 1e-10);
        }

        /// <summary>
        /// Проверяет ветвь "иначе" функции g.
        /// </summary>
        [TestMethod]
        public void TryCalculateG_ElseBranch_ReturnsCorrectResult()
        {
            double x = 1;
            double b = 20;
            int mode = 2;

            double result;
            string error;

            bool ok = AllFormuls.TryCalculateG(x, b, mode, out result, out error);

            double fx = x * x;
            double expected = 2 * fx * fx;

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.AreEqual(expected, result, 1e-10);
        }

        /// <summary>
        /// Проверяет ошибку при неверном mode.
        /// </summary>
        [TestMethod]
        public void TryCalculateG_InvalidMode_ReturnsFalse()
        {
            double result;
            string error;

            bool ok = AllFormuls.TryCalculateG(1, 2, 99, out result, out error);

            Assert.IsFalse(ok);
            Assert.AreEqual(0.0, result, 0.0);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }

        /// <summary>
        /// Проверяет корректное вычисление функции y.
        /// </summary>
        [TestMethod]
        public void TryCalculateY_ValidData_ReturnsTrueAndCorrectResult()
        {
            double x = 1;
            double d = 1;

            double result;
            string error;

            bool ok = AllFormuls.TryCalculateY(x, d, out result, out error);

            double expected = x * x + Math.Tan(5 * x + d / x);

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.AreEqual(expected, result, 1e-10);
        }

        /// <summary>
        /// Проверяет, что функция y не вычисляется при x = 0.
        /// </summary>
        [TestMethod]
        public void TryCalculateY_XEqualsZero_ReturnsFalse()
        {
            double result;
            string error;

            bool ok = AllFormuls.TryCalculateY(0, 2, out result, out error);

            Assert.IsFalse(ok);
            Assert.AreEqual(0.0, result, 0.0);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }

        /// <summary>
        /// Проверяет успешное построение таблицы значений.
        /// </summary>
        [TestMethod]
        public void TryBuildTable_ValidRange_ReturnsPoints()
        {
            List<PointData> points;
            string error;

            bool ok = AllFormuls.TryBuildTable(1, 2, 0.5, 1, out points, out error);

            Assert.IsTrue(ok);
            Assert.AreEqual("", error);
            Assert.IsNotNull(points);
            Assert.IsTrue(points.Count > 0);
        }

        /// <summary>
        /// Проверяет, что при dx = 0 построение таблицы невозможно.
        /// </summary>
        [TestMethod]
        public void TryBuildTable_ZeroStep_ReturnsFalse()
        {
            List<PointData> points;
            string error;

            bool ok = AllFormuls.TryBuildTable(1, 2, 0, 1, out points, out error);

            Assert.IsFalse(ok);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }
    }
}