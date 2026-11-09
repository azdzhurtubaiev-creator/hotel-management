using System;
using System.Collections.Generic;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class HotelTests
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
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _employee = new Employee("Олена", "Коваль", "0671234567", Position.Receptionist, 20000m);
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

        private void AddThreeRooms()
        {
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.AddRoom(201, RoomType.Double, 1300m);
        }

        [TestMethod]
        public void Constructor_ValidValues_CreatesEmptyHotel()
        {
            // Assert
            Assert.AreEqual("Харків Палас", _hotel.Name);
            Assert.AreEqual(4, _hotel.Stars);
            Assert.AreEqual(0, _hotel.Rooms.Count);
            Assert.AreEqual(0, _hotel.Employees.Count);
            Assert.AreEqual(0, _hotel.Guests.Count);
            Assert.AreEqual(0, _hotel.Bookings.Count);
        }

        [TestMethod]
        public void Constructor_SetsTrimmedAddress()
        {
            // Act
            Hotel hotel = new Hotel("Готель", 3, "  Київ, вул. Хрещатик, 1  ");

            // Assert
            Assert.AreEqual("Київ, вул. Хрещатик, 1", hotel.Address);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_EmptyName_ThrowsArgumentException(string name)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Hotel(name, 3, "Харків, вул. Сумська, 10"));
        }

        [TestMethod]
        public void Constructor_NullName_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Hotel(null, 3, "Харків, вул. Сумська, 10"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(6)]
        public void Constructor_StarsOutOfRange_ThrowsArgumentOutOfRangeException(int stars)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Hotel("Готель", stars, "Харків, вул. Сумська, 10"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_EmptyAddress_ThrowsArgumentException(string address)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Hotel("Готель", 3, address));
        }

        [TestMethod]
        public void AddRoom_ValidValues_CreatesRoomInsideHotel()
        {
            // Act
            Room room = _hotel.AddRoom(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(1, _hotel.Rooms.Count);
            Assert.AreSame(room, _hotel.Rooms[0]);
            Assert.AreEqual(101, room.Number);
            Assert.AreEqual(RoomType.Single, room.Type);
        }

        [TestMethod]
        public void AddRoom_DuplicateNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            _hotel.AddRoom(101, RoomType.Single, 800m);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.AddRoom(101, RoomType.Double, 1200m));
            Assert.AreEqual(1, _hotel.Rooms.Count);
        }

        [TestMethod]
        public void AddRoom_InvalidPrice_ThrowsAndDoesNotAdd()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _hotel.AddRoom(101, RoomType.Single, 0m));
            Assert.AreEqual(0, _hotel.Rooms.Count);
        }

        [TestMethod]
        public void FindRoom_ExistingNumber_ReturnsRoom()
        {
            // Arrange
            AddThreeRooms();

            // Act
            Room room = _hotel.FindRoom(102);

            // Assert
            Assert.IsNotNull(room);
            Assert.AreEqual(102, room.Number);
        }

        [TestMethod]
        public void FindRoom_MissingNumber_ReturnsNull()
        {
            // Arrange
            AddThreeRooms();

            // Act
            Room room = _hotel.FindRoom(999);

            // Assert
            Assert.IsNull(room);
        }

        [TestMethod]
        public void RemoveRoom_ExistingFreeRoom_ReturnsTrue()
        {
            // Arrange
            AddThreeRooms();

            // Act
            bool removed = _hotel.RemoveRoom(102);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual(2, _hotel.Rooms.Count);
            Assert.IsNull(_hotel.FindRoom(102));
        }

        [TestMethod]
        public void RemoveRoom_MissingRoom_ReturnsFalse()
        {
            // Act
            bool removed = _hotel.RemoveRoom(999);

            // Assert
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void RemoveRoom_WithActiveBooking_ThrowsInvalidOperationException()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.RemoveRoom(101));
            Assert.AreEqual(3, _hotel.Rooms.Count);
        }

        [TestMethod]
        public void HireEmployee_ReadyEmployee_IsAdded()
        {
            // Act
            _hotel.HireEmployee(_employee);

            // Assert
            Assert.AreEqual(1, _hotel.Employees.Count);
            Assert.AreSame(_employee, _hotel.Employees[0]);
        }

        [TestMethod]
        public void HireEmployee_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.HireEmployee(null));
        }

        [TestMethod]
        public void HireEmployee_Twice_ThrowsInvalidOperationException()
        {
            // Arrange
            _hotel.HireEmployee(_employee);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.HireEmployee(_employee));
        }

        [TestMethod]
        public void FireEmployee_HiredEmployee_RemovesButObjectStillExists()
        {
            // Arrange
            _hotel.HireEmployee(_employee);

            // Act
            bool fired = _hotel.FireEmployee(_employee);

            // Assert
            Assert.IsTrue(fired);
            Assert.AreEqual(0, _hotel.Employees.Count);
            Assert.AreEqual("Олена Коваль", _employee.GetFullName());
        }

        [TestMethod]
        public void FireEmployee_NotHired_ReturnsFalse()
        {
            // Act
            bool fired = _hotel.FireEmployee(_employee);

            // Assert
            Assert.IsFalse(fired);
        }

        [TestMethod]
        public void RegisterGuest_ReadyGuest_IsAdded()
        {
            // Act
            _hotel.RegisterGuest(_guest);

            // Assert
            Assert.AreEqual(1, _hotel.Guests.Count);
            Assert.AreSame(_guest, _hotel.Guests[0]);
        }

        [TestMethod]
        public void RegisterGuest_Null_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.RegisterGuest(null));
        }

        [TestMethod]
        public void RegisterGuest_Twice_ThrowsInvalidOperationException()
        {
            // Arrange
            _hotel.RegisterGuest(_guest);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.RegisterGuest(_guest));
        }

        [TestMethod]
        public void CreateBooking_FreeRoom_AddsBookingToHotelAndGuest()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);

            // Act
            Booking booking = _hotel.CreateBooking(_guest, 102, _from, _to);

            // Assert
            Assert.AreEqual(1, _hotel.Bookings.Count);
            Assert.AreSame(booking, _hotel.Bookings[0]);
            Assert.AreSame(booking, _guest.Bookings[0]);
            Assert.AreEqual(102, booking.Room.Number);
            Assert.AreEqual(BookingStatus.Created, booking.Status);
        }

        [TestMethod]
        public void CreateBooking_GuestNotRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            AddThreeRooms();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.CreateBooking(_guest, 101, _from, _to));
        }

        [TestMethod]
        public void CreateBooking_NullGuest_ThrowsArgumentNullException()
        {
            // Arrange
            AddThreeRooms();

            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.CreateBooking(null, 101, _from, _to));
        }

        [TestMethod]
        public void CreateBooking_MissingRoom_ThrowsInvalidOperationException()
        {
            // Arrange
            _hotel.RegisterGuest(_guest);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.CreateBooking(_guest, 999, _from, _to));
        }

        [TestMethod]
        public void CreateBooking_WrongDates_ThrowsArgumentException()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _hotel.CreateBooking(_guest, 101, _to, _from));
            Assert.AreEqual(0, _hotel.Bookings.Count);
        }

        [TestMethod]
        public void CreateBooking_OverlappingDates_ThrowsInvalidOperationException()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => _hotel.CreateBooking(_guest, 101, _from.AddDays(1), _to.AddDays(2)));
            Assert.AreEqual(1, _hotel.Bookings.Count);
        }

        [TestMethod]
        public void CreateBooking_StartsOnCheckOutDay_IsAllowed()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            Booking second = _hotel.CreateBooking(_guest, 101, _to, _to.AddDays(2));

            // Assert
            Assert.AreEqual(2, _hotel.Bookings.Count);
            Assert.AreEqual(_to, second.CheckIn);
        }

        [TestMethod]
        public void CreateBooking_AfterCancel_RoomIsFreeAgain()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            Booking first = _hotel.CreateBooking(_guest, 101, _from, _to);
            _hotel.CancelBooking(first);

            // Act
            Booking second = _hotel.CreateBooking(_guest, 101, _from, _to);

            // Assert
            Assert.AreEqual(BookingStatus.Created, second.Status);
            Assert.AreEqual(2, _hotel.Bookings.Count);
        }

        [TestMethod]
        public void CancelBooking_OwnBooking_ReturnsTrueAndCancels()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            Booking booking = _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            bool cancelled = _hotel.CancelBooking(booking);

            // Assert
            Assert.IsTrue(cancelled);
            Assert.AreEqual(BookingStatus.Cancelled, booking.Status);
        }

        [TestMethod]
        public void CancelBooking_ForeignBooking_ReturnsFalse()
        {
            // Arrange
            Booking foreign = new Booking(_guest, new Room(1, RoomType.Single, 500m), _from, _to);

            // Act
            bool cancelled = _hotel.CancelBooking(foreign);

            // Assert
            Assert.IsFalse(cancelled);
            Assert.AreEqual(BookingStatus.Created, foreign.Status);
        }

        [TestMethod]
        public void IsRoomAvailable_BookedPeriod_ReturnsFalse()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            bool available = _hotel.IsRoomAvailable(101, _from, _to);

            // Assert
            Assert.IsFalse(available);
        }

        [TestMethod]
        public void IsRoomAvailable_OtherPeriod_ReturnsTrue()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            bool available = _hotel.IsRoomAvailable(101, _to.AddDays(5), _to.AddDays(7));

            // Assert
            Assert.IsTrue(available);
        }

        [TestMethod]
        public void IsRoomAvailable_MissingRoom_ThrowsInvalidOperationException()
        {
            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.IsRoomAvailable(999, _from, _to));
        }

        [TestMethod]
        public void FindAvailableRooms_OneRoomBooked_ReturnsOthers()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 102, _from, _to);

            // Act
            List<Room> rooms = _hotel.FindAvailableRooms(_from, _to);

            // Assert
            Assert.AreEqual(2, rooms.Count);
            Assert.IsFalse(rooms.Exists(r => r.Number == 102));
        }

        [TestMethod]
        public void FindAvailableRooms_ByType_ReturnsOnlyFreeRoomsOfType()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 102, _from, _to);

            // Act
            List<Room> rooms = _hotel.FindAvailableRooms(RoomType.Double, _from, _to);

            // Assert
            Assert.AreEqual(1, rooms.Count);
            Assert.AreEqual(201, rooms[0].Number);
        }

        [TestMethod]
        public void FindAvailableRooms_NoRooms_ReturnsEmptyList()
        {
            // Act
            List<Room> rooms = _hotel.FindAvailableRooms(_from, _to);

            // Assert
            Assert.IsNotNull(rooms);
            Assert.AreEqual(0, rooms.Count);
        }

        [TestMethod]
        public void FindAvailableRooms_WrongPeriod_ThrowsArgumentException()
        {
            // Arrange
            AddThreeRooms();

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _hotel.FindAvailableRooms(_to, _from));
        }

        [TestMethod]
        public void CalculateIncome_SumsOnlyPaidBookings()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            Booking paid = _hotel.CreateBooking(_guest, 101, _from, _to);
            _hotel.CreateBooking(_guest, 102, _from, _to);
            paid.Pay(PaymentMethod.Card);

            // Act
            decimal income = _hotel.CalculateIncome();

            // Assert
            Assert.AreEqual(2400m, income);
        }

        [TestMethod]
        public void CalculateIncome_NoPayments_ReturnsZero()
        {
            // Act
            decimal income = _hotel.CalculateIncome();

            // Assert
            Assert.AreEqual(0m, income);
        }

        [TestMethod]
        public void GetSalaryExpenses_SumsSalariesOfEmployees()
        {
            // Arrange
            _hotel.HireEmployee(_employee);
            _hotel.HireEmployee(new Employee("Петро", "Сидоренко", "0931234567", Position.Manager, 35000m));

            // Act
            decimal expenses = _hotel.GetSalaryExpenses();

            // Assert
            Assert.AreEqual(55000m, expenses);
        }

        [TestMethod]
        public void GetOccupancyRate_OneOfThreeRoomsBusy_ReturnsPercent()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            double rate = _hotel.GetOccupancyRate(_from.AddDays(1));

            // Assert
            Assert.AreEqual(33.33, rate, 0.01);
        }

        [TestMethod]
        public void GetOccupancyRate_CheckOutDay_RoomIsFree()
        {
            // Arrange
            AddThreeRooms();
            _hotel.RegisterGuest(_guest);
            _hotel.CreateBooking(_guest, 101, _from, _to);

            // Act
            double rate = _hotel.GetOccupancyRate(_to);

            // Assert
            Assert.AreEqual(0, rate, 0.01);
        }

        [TestMethod]
        public void GetOccupancyRate_NoRooms_ReturnsZero()
        {
            // Act
            double rate = _hotel.GetOccupancyRate(_from);

            // Assert
            Assert.AreEqual(0, rate, 0.01);
        }

        [TestMethod]
        public void Rooms_ReturnsReadOnlyCollection()
        {
            // Arrange
            AddThreeRooms();

            // Act
            IReadOnlyList<Room> rooms = _hotel.Rooms;

            // Assert
            Assert.IsInstanceOfType<System.Collections.ObjectModel.ReadOnlyCollection<Room>>(rooms);
        }
    }
}
