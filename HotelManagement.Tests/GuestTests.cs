using System;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class GuestTests
    {
        [TestMethod]
        public void Constructor_ValidValues_SetsProperties()
        {
            // Act
            Guest guest = new Guest("Іван", "Петренко", "+380501234567");

            // Assert
            Assert.AreEqual("Іван", guest.FirstName);
            Assert.AreEqual("Петренко", guest.LastName);
            Assert.AreEqual("+380501234567", guest.Phone);
            Assert.AreEqual(0, guest.Bookings.Count);
        }

        [TestMethod]
        [DataRow("", "Петренко")]
        [DataRow("Іван", "  ")]
        [DataRow(null, "Петренко")]
        public void Constructor_EmptyName_ThrowsArgumentException(string firstName, string lastName)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Guest(firstName, lastName, "0501234567"));
        }

        [TestMethod]
        [DataRow("0501234567")]
        [DataRow("+380501234567")]
        public void Constructor_ValidPhone_IsAccepted(string phone)
        {
            // Act
            Guest guest = new Guest("Іван", "Петренко", phone);

            // Assert
            Assert.AreEqual(phone, guest.Phone);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("12345")]
        [DataRow("050-123-45-67")]
        [DataRow("phone12345")]
        public void Constructor_InvalidPhone_ThrowsArgumentException(string phone)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Guest("Іван", "Петренко", phone));
        }

        [TestMethod]
        public void GetFullName_ReturnsFirstAndLastName()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");

            // Act
            string actual = guest.GetFullName();

            // Assert
            Assert.AreEqual("Іван Петренко", actual);
        }

        [TestMethod]
        public void Guest_ImplementsIPerson()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");

            // Assert
            Assert.IsInstanceOfType<IPerson>(guest);
        }

        [TestMethod]
        public void AddBooking_OwnBooking_IsAdded()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");
            Booking booking = new Booking(guest, new Room(101, RoomType.Single, 800m), new DateTime(2026, 11, 1), new DateTime(2026, 11, 3));

            // Act
            guest.AddBooking(booking);

            // Assert
            Assert.AreEqual(1, guest.Bookings.Count);
            Assert.AreSame(booking, guest.Bookings[0]);
        }

        [TestMethod]
        public void AddBooking_Null_ThrowsArgumentNullException()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");

            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => guest.AddBooking(null));
        }

        [TestMethod]
        public void AddBooking_SameBookingTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");
            Booking booking = new Booking(guest, new Room(101, RoomType.Single, 800m), new DateTime(2026, 11, 1), new DateTime(2026, 11, 3));
            guest.AddBooking(booking);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => guest.AddBooking(booking));
        }

        [TestMethod]
        public void AddBooking_BookingOfOtherGuest_ThrowsArgumentException()
        {
            // Arrange
            Guest guest = new Guest("Іван", "Петренко", "0501234567");
            Guest other = new Guest("Олена", "Коваль", "0671234567");
            Booking booking = new Booking(other, new Room(101, RoomType.Single, 800m), new DateTime(2026, 11, 1), new DateTime(2026, 11, 3));

            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => guest.AddBooking(booking));
        }
    }
}
