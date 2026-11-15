using System;
using System.Collections.Generic;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class PolymorphismTests
    {
        private Hotel _hotel;
        private Guest _guest;
        private Employee _employee;
        private DateTime _from;
        private DateTime _to;

        [TestInitialize]
        public void Setup()
        {
            _hotel = new Hotel("Харків Палас", 4, "Харків, вул. Сумська, 10");
            _hotel.AddRoom(201, RoomType.Double, 1300m);
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _employee = new Employee("Олена", "Коваль", "0671234567", Position.Manager, 30000m);
            _hotel.RegisterGuest(_guest);
            _hotel.HireEmployee(_employee);
            _from = new DateTime(2026, 11, 1);
            _to = new DateTime(2026, 11, 4);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _hotel = null;
            _guest = null;
            _employee = null;
        }

        [TestMethod]
        public void VirtualGetInfo_CalledThroughBaseType_UsesDerivedImplementation()
        {
            // Arrange
            List<Person> people = _hotel.GetAllPeople();

            // Act
            List<string> infos = new List<string>();
            foreach (Person person in people)
            {
                infos.Add(person.GetInfo());
            }

            // Assert
            Assert.AreEqual(2, infos.Count);
            Assert.IsTrue(infos[0].Contains("посада: Manager"));
            Assert.IsTrue(infos[1].Contains("бронювань: 0"));
        }

        [TestMethod]
        public void InterfaceVariable_IPrintable_CallsImplementationOfEachClass()
        {
            // Arrange
            _hotel.CreateBooking(_guest, 101, _from, _to);
            List<IPrintable> items = _hotel.GetPrintableItems();

            // Act
            List<string> infos = new List<string>();
            foreach (IPrintable item in items)
            {
                infos.Add(item.GetInfo());
            }

            // Assert
            Assert.AreEqual(6, infos.Count);
            Assert.IsTrue(infos[0].StartsWith("Номер 201"));
            Assert.IsTrue(infos[3].StartsWith("Гість:"));
            Assert.IsTrue(infos[4].StartsWith("Працівник:"));
            Assert.IsTrue(infos[5].Contains("Created"));
        }

        [TestMethod]
        public void InterfaceVariable_IPerson_WorksForGuestAndEmployee()
        {
            // Arrange
            List<IPerson> people = new List<IPerson> { _guest, _employee };

            // Act
            string names = string.Empty;
            foreach (IPerson person in people)
            {
                names += person.GetFullName() + ";";
            }

            // Assert
            Assert.AreEqual("Іван Петренко;Олена Коваль;", names);
        }

        [TestMethod]
        public void InterfaceVariable_IPayable_CalculatesTotal()
        {
            // Arrange
            IPayable payable = _hotel.CreateBooking(_guest, 102, _from, _to);

            // Act
            decimal total = payable.CalculateTotal();

            // Assert
            Assert.AreEqual(3600m, total);
        }

        [TestMethod]
        public void Hotel_ImplementsIEnumerable_ForeachOverRooms()
        {
            // Arrange
            List<int> numbers = new List<int>();

            // Act
            foreach (Room room in _hotel)
            {
                numbers.Add(room.Number);
            }

            // Assert
            Assert.IsInstanceOfType<IEnumerable<Room>>(_hotel);
            Assert.AreEqual(3, numbers.Count);
            Assert.AreEqual(201, numbers[0]);
        }

        [TestMethod]
        public void Room_ImplementsIComparable_SortsByNumber()
        {
            // Act
            List<Room> rooms = _hotel.GetSortedRooms();

            // Assert
            Assert.IsInstanceOfType<IComparable<Room>>(rooms[0]);
            Assert.AreEqual(101, rooms[0].Number);
            Assert.AreEqual(102, rooms[1].Number);
            Assert.AreEqual(201, rooms[2].Number);
        }

        [TestMethod]
        public void GetSortedRooms_DoesNotChangeOriginalOrder()
        {
            // Act
            _hotel.GetSortedRooms();

            // Assert
            Assert.AreEqual(201, _hotel.Rooms[0].Number);
        }

        [TestMethod]
        [DataRow(101, 102, -1)]
        [DataRow(201, 102, 1)]
        [DataRow(101, 101, 0)]
        public void Room_CompareTo_ReturnsExpectedSign(int first, int second, int expectedSign)
        {
            // Arrange
            Room a = new Room(first, RoomType.Single, 500m);
            Room b = new Room(second, RoomType.Double, 900m);

            // Act
            int result = a.CompareTo(b);

            // Assert
            Assert.AreEqual(expectedSign, Math.Sign(result));
        }

        [TestMethod]
        public void Room_CompareTo_Null_ReturnsPositive()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 500m);

            // Act
            int result = room.CompareTo(null);

            // Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void GetAllPeople_ReturnsGuestsAndEmployeesSorted()
        {
            // Arrange
            _hotel.RegisterGuest(new Guest("Андрій", "Бондар", "0931234567"));

            // Act
            List<Person> people = _hotel.GetAllPeople();

            // Assert
            Assert.AreEqual(3, people.Count);
            Assert.AreEqual("Бондар", people[0].LastName);
            Assert.IsInstanceOfType<Employee>(people[1]);
            Assert.IsInstanceOfType<Guest>(people[2]);
        }

        [TestMethod]
        public void FindGuestByPhone_RegisteredGuest_ReturnsGuest()
        {
            // Act
            Guest found = _hotel.FindGuestByPhone(" 0501234567 ");

            // Assert
            Assert.AreSame(_guest, found);
        }

        [TestMethod]
        public void FindGuestByPhone_UnknownPhone_ReturnsNull()
        {
            // Act
            Guest found = _hotel.FindGuestByPhone("0999999999");

            // Assert
            Assert.IsNull(found);
        }

        [TestMethod]
        public void CalculateExpectedIncome_SumsActiveBookingsOnly()
        {
            // Arrange
            _hotel.CreateBooking(_guest, 101, _from, _to);
            Booking cancelled = _hotel.CreateBooking(_guest, 102, _from, _to);
            _hotel.CancelBooking(cancelled);

            // Act
            decimal expected = _hotel.CalculateExpectedIncome();

            // Assert
            Assert.AreEqual(2400m, expected);
        }

        [TestMethod]
        public void Payment_GetInfo_ContainsAmountAndMethod()
        {
            // Arrange
            IPrintable payment = new Payment(2400m, PaymentMethod.Card);

            // Act
            string info = payment.GetInfo();

            // Assert
            Assert.IsTrue(info.StartsWith("Оплата 2400 грн (Card)"));
        }
    }
}
