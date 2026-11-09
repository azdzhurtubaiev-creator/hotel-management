using System;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class BookingTests
    {
        private Guest _guest;
        private Room _room;
        private DateTime _checkIn;
        private DateTime _checkOut;
        private Booking _booking;

        [TestInitialize]
        public void Setup()
        {
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _room = new Room(101, RoomType.Double, 1200m);
            _checkIn = new DateTime(2026, 11, 1);
            _checkOut = new DateTime(2026, 11, 4);
            _booking = new Booking(_guest, _room, _checkIn, _checkOut);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _guest = null;
            _room = null;
            _booking = null;
        }

        [TestMethod]
        public void Constructor_ValidValues_SetsPropertiesAndCreatedStatus()
        {
            // Assert
            Assert.AreSame(_guest, _booking.Guest);
            Assert.AreSame(_room, _booking.Room);
            Assert.AreEqual(_checkIn, _booking.CheckIn);
            Assert.AreEqual(_checkOut, _booking.CheckOut);
            Assert.AreEqual(BookingStatus.Created, _booking.Status);
            Assert.IsFalse(_booking.IsPaid);
            Assert.IsNull(_booking.Payment);
        }

        [TestMethod]
        public void Constructor_NullGuest_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => new Booking(null, _room, _checkIn, _checkOut));
        }

        [TestMethod]
        public void Constructor_NullRoom_ThrowsArgumentNullException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => new Booking(_guest, null, _checkIn, _checkOut));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-2)]
        public void Constructor_CheckOutNotAfterCheckIn_ThrowsArgumentException(int days)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Booking(_guest, _room, _checkIn, _checkIn.AddDays(days)));
        }

        [TestMethod]
        public void Constructor_DatesWithTime_KeepsOnlyDate()
        {
            // Act
            Booking booking = new Booking(_guest, _room, new DateTime(2026, 11, 1, 14, 30, 0), new DateTime(2026, 11, 2, 11, 0, 0));

            // Assert
            Assert.AreEqual(new DateTime(2026, 11, 1), booking.CheckIn);
            Assert.AreEqual(new DateTime(2026, 11, 2), booking.CheckOut);
        }

        [TestMethod]
        public void GetNights_ReturnsDaysBetweenDates()
        {
            // Act
            int nights = _booking.GetNights();

            // Assert
            Assert.AreEqual(3, nights);
        }

        [TestMethod]
        public void CalculateTotal_ReturnsNightsMultipliedByPrice()
        {
            // Act
            decimal total = _booking.CalculateTotal();

            // Assert
            Assert.AreEqual(3600m, total);
        }

        [TestMethod]
        public void Booking_ImplementsIPayableAndIPrintable()
        {
            // Assert
            Assert.IsInstanceOfType<IPayable>(_booking);
            Assert.IsInstanceOfType<IPrintable>(_booking);
        }

        [TestMethod]
        [DataRow(2026, 10, 30, 2026, 11, 2, true)]
        [DataRow(2026, 11, 3, 2026, 11, 6, true)]
        [DataRow(2026, 11, 2, 2026, 11, 3, true)]
        [DataRow(2026, 11, 4, 2026, 11, 6, false)]
        [DataRow(2026, 10, 28, 2026, 11, 1, false)]
        public void Overlaps_ReturnsExpected(int y1, int m1, int d1, int y2, int m2, int d2, bool expected)
        {
            // Act
            bool actual = _booking.Overlaps(new DateTime(y1, m1, d1), new DateTime(y2, m2, d2));

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Overlaps_WrongPeriod_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => _booking.Overlaps(_checkOut, _checkIn));
        }

        [TestMethod]
        public void Confirm_CreatedBooking_SetsConfirmed()
        {
            // Act
            _booking.Confirm();

            // Assert
            Assert.AreEqual(BookingStatus.Confirmed, _booking.Status);
        }

        [TestMethod]
        public void Confirm_Twice_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Confirm();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.Confirm());
        }

        [TestMethod]
        public void CheckInGuest_ConfirmedBooking_SetsCheckedIn()
        {
            // Arrange
            _booking.Confirm();

            // Act
            _booking.CheckInGuest();

            // Assert
            Assert.AreEqual(BookingStatus.CheckedIn, _booking.Status);
        }

        [TestMethod]
        public void CheckInGuest_NotConfirmed_ThrowsInvalidOperationException()
        {
            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.CheckInGuest());
        }

        [TestMethod]
        public void CheckOutGuest_PaidAndCheckedIn_SetsCheckedOut()
        {
            // Arrange
            _booking.Confirm();
            _booking.CheckInGuest();
            _booking.Pay(PaymentMethod.Card);

            // Act
            _booking.CheckOutGuest();

            // Assert
            Assert.AreEqual(BookingStatus.CheckedOut, _booking.Status);
        }

        [TestMethod]
        public void CheckOutGuest_NotPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Confirm();
            _booking.CheckInGuest();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.CheckOutGuest());
        }

        [TestMethod]
        public void CheckOutGuest_NotCheckedIn_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Pay(PaymentMethod.Cash);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.CheckOutGuest());
        }

        [TestMethod]
        public void Cancel_CreatedOrConfirmed_SetsCancelled()
        {
            // Arrange
            Booking confirmed = new Booking(_guest, _room, _checkIn, _checkOut);
            confirmed.Confirm();

            // Act
            _booking.Cancel();
            confirmed.Cancel();

            // Assert
            Assert.AreEqual(BookingStatus.Cancelled, _booking.Status);
            Assert.AreEqual(BookingStatus.Cancelled, confirmed.Status);
        }

        [TestMethod]
        public void Cancel_CheckedIn_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Confirm();
            _booking.CheckInGuest();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.Cancel());
            Assert.AreEqual(BookingStatus.CheckedIn, _booking.Status);
        }

        [TestMethod]
        public void IsActive_DependsOnStatus()
        {
            // Arrange
            Booking cancelled = new Booking(_guest, _room, _checkIn, _checkOut);
            cancelled.Cancel();

            // Assert
            Assert.IsTrue(_booking.IsActive);
            Assert.IsFalse(cancelled.IsActive);
        }

        [TestMethod]
        public void Pay_CreatesPaymentWithTotalAmount()
        {
            // Act
            Payment payment = _booking.Pay(PaymentMethod.Card);

            // Assert
            Assert.IsNotNull(payment);
            Assert.AreSame(payment, _booking.Payment);
            Assert.AreEqual(3600m, payment.Amount);
            Assert.AreEqual(PaymentMethod.Card, payment.Method);
            Assert.IsTrue(_booking.IsPaid);
        }

        [TestMethod]
        public void Pay_Twice_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Pay(PaymentMethod.Cash);

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.Pay(PaymentMethod.Card));
        }

        [TestMethod]
        public void Pay_CancelledBooking_ThrowsInvalidOperationException()
        {
            // Arrange
            _booking.Cancel();

            // Act + Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _booking.Pay(PaymentMethod.Cash));
            Assert.IsFalse(_booking.IsPaid);
        }

        [TestMethod]
        public void GetInfo_ContainsGuestRoomAndStatus()
        {
            // Act
            string info = _booking.GetInfo();

            // Assert
            Assert.IsTrue(info.Contains("Іван Петренко"));
            Assert.IsTrue(info.Contains("101"));
            Assert.IsTrue(info.Contains("01.11.2026"));
            Assert.IsTrue(info.Contains("Created"));
        }
    }
}
