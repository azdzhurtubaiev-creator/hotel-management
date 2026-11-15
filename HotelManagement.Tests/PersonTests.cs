using System;
using System.Collections.Generic;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class PersonTests
    {
        private Guest _guest;
        private Employee _employee;

        [TestInitialize]
        public void Setup()
        {
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _employee = new Employee("Олена", "Коваль", "0671234567", Position.Receptionist, 20000m);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _guest = null;
            _employee = null;
        }

        [TestMethod]
        public void Person_IsAbstract()
        {
            // Assert
            Assert.IsTrue(typeof(Person).IsAbstract);
        }

        [TestMethod]
        public void GuestAndEmployee_InheritPerson()
        {
            // Assert
            Assert.IsInstanceOfType<Person>(_guest);
            Assert.IsInstanceOfType<Person>(_employee);
        }

        [TestMethod]
        public void Person_ImplementsSeveralInterfaces()
        {
            // Assert
            Assert.IsInstanceOfType<IPerson>(_guest);
            Assert.IsInstanceOfType<IPrintable>(_guest);
            Assert.IsInstanceOfType<IComparable<Person>>(_guest);
        }

        [TestMethod]
        public void Role_AbstractProperty_DependsOnType()
        {
            // Arrange
            Person guest = _guest;
            Person employee = _employee;

            // Act
            string guestRole = guest.Role;
            string employeeRole = employee.Role;

            // Assert
            Assert.AreEqual("Гість", guestRole);
            Assert.AreEqual("Працівник", employeeRole);
        }

        [TestMethod]
        public void GetGreeting_AbstractMethod_DependsOnType()
        {
            // Arrange
            Person guest = _guest;
            Person employee = _employee;

            // Act
            string guestGreeting = guest.GetGreeting();
            string employeeGreeting = employee.GetGreeting();

            // Assert
            Assert.AreEqual("Ласкаво просимо до готелю, Іван!", guestGreeting);
            Assert.AreEqual("Гарної зміни, Олена!", employeeGreeting);
        }

        [TestMethod]
        public void GetInfo_Guest_ContainsBaseInfoAndBookingsCount()
        {
            // Arrange
            Person person = _guest;

            // Act
            string info = person.GetInfo();

            // Assert
            Assert.AreEqual("Гість: Іван Петренко, тел. 0501234567, бронювань: 0", info);
        }

        [TestMethod]
        public void GetInfo_Employee_ContainsBaseInfoPositionAndSalary()
        {
            // Arrange
            Person person = _employee;

            // Act
            string info = person.GetInfo();

            // Assert
            Assert.AreEqual("Працівник: Олена Коваль, тел. 0671234567, посада: Receptionist, зарплата: 20000 грн", info);
        }

        [TestMethod]
        public void GetInfo_Guest_ChangesAfterBooking()
        {
            // Arrange
            Booking booking = new Booking(_guest, new Room(101, RoomType.Single, 800m), new DateTime(2026, 11, 1), new DateTime(2026, 11, 3));
            _guest.AddBooking(booking);

            // Act
            string info = _guest.GetInfo();

            // Assert
            Assert.IsTrue(info.EndsWith("бронювань: 1"));
        }

        [TestMethod]
        public void ToString_ReturnsGetInfo()
        {
            // Act
            string text = _employee.ToString();

            // Assert
            Assert.AreEqual(_employee.GetInfo(), text);
        }

        [TestMethod]
        [DataRow("Іван", "Петренко", "Олена", "Коваль", 1)]
        [DataRow("Олена", "Коваль", "Іван", "Петренко", -1)]
        [DataRow("Андрій", "Коваль", "Олена", "Коваль", -1)]
        [DataRow("Олена", "Коваль", "Олена", "Коваль", 0)]
        public void CompareTo_ComparesByLastNameThenFirstName(string first1, string last1, string first2, string last2, int expectedSign)
        {
            // Arrange
            Person a = new Guest(first1, last1, "0501234567");
            Person b = new Employee(first2, last2, "0671234567", Position.Cook, 1000m);

            // Act
            int result = a.CompareTo(b);

            // Assert
            Assert.AreEqual(expectedSign, Math.Sign(result));
        }

        [TestMethod]
        public void CompareTo_Null_ReturnsPositive()
        {
            // Act
            int result = _guest.CompareTo(null);

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Sort_ListOfPersons_UsesIComparable()
        {
            // Arrange
            List<Person> people = new List<Person>
            {
                _guest,
                _employee,
                new Guest("Андрій", "Бондар", "0931234567")
            };

            // Act
            people.Sort();

            // Assert
            Assert.AreEqual("Бондар", people[0].LastName);
            Assert.AreEqual("Коваль", people[1].LastName);
            Assert.AreEqual("Петренко", people[2].LastName);
        }
    }
}
