using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab_4_OOP; 
using System;

namespace Lab5_Tests
{
    [TestClass]
    public class DroneTests
    {
        private Drone _testDrone;

        // підготовка чистого об'єкта перед кожним тестом
        [TestInitialize]
        public void Setup()
        {
            _testDrone = new Drone(1, "DJI Mavic 3", 5000, 70);
        }

        // очищення після кожного тесту
        [TestCleanup]
        public void Teardown()
        {
            _testDrone = null;
        }

        // ТЕСТИ ДЛЯ СЕКЦІЇ SET ВЛАСТИВОСТЕЙ ТА ВАЛІДАЦІЇ

        [TestMethod]
        public void Model_SetCorrectValue_ShouldChangeModelName()
        {
            string expectedModel = "Autel Evo";
            _testDrone.Model = expectedModel;
            Assert.AreEqual(expectedModel, _testDrone.Model);
        }

        [TestMethod]
        public void Model_SetEmptyValue_ShouldThrowArgumentException()
        {
            // перевірка винятку через try-catch для старих версій mstest
            try
            {
                _testDrone.Model = "";
                Assert.Fail("помилка: клас мав викинути виняток, але не зробив цього");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-15)]
        public void MaxSpeed_SetZeroOrNegativeValue_ShouldThrowArgumentException(double invalidSpeed)
        {
            // перевірка швидкості через try-catch для сумісності версій
            try
            {
                _testDrone.MaxSpeed = invalidSpeed;
                Assert.Fail("помилка: клас мав заблокувати неправильну швидкість");
            }
            catch (ArgumentException)
            {
            }
        }
        // ТЕСТИ ДЛЯ НЕСТАТИЧНИХ МЕТОДІВ (ПОВЕДІНКА)

        [TestMethod]
        public void StartFlight_CorrectDistance_ShouldReduceBatteryAndReturnSuccess()
        {
            double distance = 10;
            string result = _testDrone.StartFlight(distance);

            Assert.AreEqual(75.0, _testDrone.CurrentBatteryLevel);
            Assert.AreEqual("Політ успішний на 10 км.", result);
        }

        [TestMethod]
        public void StartFlight_LowBatteryLevel_ShouldThrowInvalidOperationException()
        {
            // перевірка захисту від зльоту з низьким зарядом через try-catch
            try
            {
                _testDrone.CurrentBatteryLevel = 10;
                _testDrone.StartFlight(5);
                Assert.Fail("помилка: дрон не мав злетіти з низьким зарядом");
            }
            catch (InvalidOperationException)
            {
            }
        }

        // ТЕСТИ ДЛЯ СТАТИЧНИХ МЕТОДІВ ТА ВЛАСТИВОСТЕЙ КЛАСУ

        [TestMethod]
        public void GlobalManufacturerCountry_ChangeValue_ShouldChangeForWholeClass()
        {
            string newCountry = "США";
            Drone.GlobalManufacturerCountry = newCountry;
            Assert.AreEqual("США", Drone.GlobalManufacturerCountry);
        }

        [TestMethod]
        [DataRow(240, "Мікро-дрон")]
        [DataRow(1500, "Легкий споживчий дрон")]
        [DataRow(4000, "Важкий промисловий або військовий БПЛА")]
        public void GetDroneCategoryByWeight_DifferentWeights_ShouldReturnCorrectString(double weight, string expectedCategory)
        {
            string actualCategory = Drone.GetDroneCategoryByWeight(weight);
            Assert.AreEqual(expectedCategory, actualCategory);
        }

        // ТЕСТИ ДЛЯ ПАРСИНГУ ТА TOSTRING

        [TestMethod]
        public void Parse_CorrectStringFormat_ShouldCreateValidObject()
        {
            string parseInput = "Autel Evo;4000;65";
            Drone resultDrone = Drone.Parse(parseInput);

            Assert.IsNotNull(resultDrone);
            Assert.AreEqual("Autel Evo", resultDrone.Model);
            Assert.AreEqual(4000, resultDrone.BatteryCapacity);
            Assert.AreEqual(65, resultDrone.MaxSpeed);
        }

        [TestMethod]
        [DataRow("ЗламанийТекстБезРоздільників")]
        [DataRow("Модель;БуквиЗамістьЧисла;70")]
        public void Parse_InvalidStringFormat_ShouldThrowFormatException(string invalidInput)
        {
            // перевірка помилки парсингу через класичний try-catch
            try
            {
                Drone.Parse(invalidInput);
                Assert.Fail("помилка: метод мав відхилити цей неправильний рядок");
            }
            catch (FormatException)
            {
            }
        }

        [TestMethod]
        public void TryParse_CorrectString_ShouldReturnTrueAndOutputObject()
        {
            string correctInput = "DJI Mini 2;2250;50";
            bool isSuccess = Drone.TryParse(correctInput, out Drone parsedDrone);

            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(parsedDrone);
            Assert.AreEqual("DJI Mini 2", parsedDrone.Model);
        }

        [TestMethod]
        public void TryParse_InvalidString_ShouldReturnFalseAndOutputNull()
        {
            string corruptedInput = "Помилка;;";
            bool isSuccess = Drone.TryParse(corruptedInput, out Drone parsedDrone);

            Assert.IsFalse(isSuccess);
            Assert.IsNull(parsedDrone);
        }

        [TestMethod]
        public void ToString_CallMethod_ShouldReturnStringInCorrectFormat()
        {
            _testDrone.Model = "Matrix 300";
            _testDrone.BatteryCapacity = 15000;
            _testDrone.MaxSpeed = 82;

            string expectedFormat = "Matrix 300;15000;82";
            string actualFormat = _testDrone.ToString();

            Assert.AreEqual(expectedFormat, actualFormat);
        }
    }
}
