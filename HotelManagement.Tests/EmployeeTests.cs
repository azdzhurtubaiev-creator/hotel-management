using System;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class EmployeeTests
    {
        private Employee _employee;

        [TestInitialize]
        public void Setup()
        {
            _employee = new Employee("Олена", "Коваль", "0671234567", Position.Receptionist, 20000m);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _employee = null;
        }

        [TestMethod]
        public void Constructor_ValidValues_SetsProperties()
        {
            // Assert
            Assert.AreEqual("Олена", _employee.FirstName);
            Assert.AreEqual("Коваль", _employee.LastName);
            Assert.AreEqual("0671234567", _employee.Phone);
            Assert.AreEqual(Position.Receptionist, _employee.Position);
            Assert.AreEqual(20000m, _employee.Salary);
        }

        [TestMethod]
        public void Constructor_NegativeSalary_ThrowsArgumentOutOfRangeException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new Employee("Олена", "Коваль", "0671234567", Position.Cook, -1m));
        }

        [TestMethod]
        public void Constructor_UndefinedPosition_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => new Employee("Олена", "Коваль", "0671234567", (Position)99, 1000m));
        }

        [TestMethod]
        public void Constructor_InvalidPhone_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => new Employee("Олена", "Коваль", "123", Position.Cook, 1000m));
        }

        [TestMethod]
        public void Salary_ZeroValue_IsAllowed()
        {
            // Act
            _employee.Salary = 0m;

            // Assert
            Assert.AreEqual(0m, _employee.Salary);
        }

        [TestMethod]
        public void Position_NewValue_IsChanged()
        {
            // Act
            _employee.Position = Position.Manager;

            // Assert
            Assert.AreEqual(Position.Manager, _employee.Position);
        }

        [TestMethod]
        [DataRow(10.0, 22000)]
        [DataRow(50.0, 30000)]
        [DataRow(2.5, 20500)]
        public void RaiseSalary_ValidPercent_IncreasesSalary(double percent, int expected)
        {
            // Act
            _employee.RaiseSalary((decimal)percent);

            // Assert
            Assert.AreEqual((decimal)expected, _employee.Salary);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-10)]
        public void RaiseSalary_PercentNotPositive_ThrowsAndKeepsSalary(int percent)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _employee.RaiseSalary(percent));
            Assert.AreEqual(20000m, _employee.Salary);
        }

        [TestMethod]
        public void GetFullName_ReturnsFirstAndLastName()
        {
            // Act
            string actual = _employee.GetFullName();

            // Assert
            Assert.AreEqual("Олена Коваль", actual);
        }

        [TestMethod]
        public void Employee_ImplementsIPerson()
        {
            // Assert
            Assert.IsInstanceOfType<IPerson>(_employee);
        }
    }
}
