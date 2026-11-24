using System;
using System.Collections.Generic;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class DelegateTests
    {
        private Hotel _hotel;
        private Guest _guest;
        private List<string> _calls;

        [TestInitialize]
        public void Setup()
        {
            _hotel = new Hotel("Харків Палас", 4, "Харків, вул. Сумська, 10");
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _calls = new List<string>();
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.AddRoom(301, RoomType.Family, 2000m);
            _hotel.RegisterGuest(_guest);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _hotel = null;
            _guest = null;
            _calls = null;
        }

        private static bool IsCheap(Room room)
        {
            return room.PricePerNight <= 1000m;
        }

        private static bool IsSuite(Room room)
        {
            return room.Type == RoomType.Suite;
        }

        private static bool IsAny(Booking booking)
        {
            return true;
        }

        private static bool IsPaid(Booking booking)
        {
            return booking.IsPaid;
        }

        private static decimal Nights(Booking booking)
        {
            return booking.GetNights();
        }

        private static decimal Total(Booking booking)
        {
            return booking.CalculateTotal();
        }

        private static decimal Fixed25(Booking booking)
        {
            return 25m;
        }

        private static decimal Hundred(Booking booking)
        {
            return 100m;
        }

        private static decimal Negative(Booking booking)
        {
            return -1m;
        }

        private void RememberRoom(Room room)
        {
            _calls.Add("R" + room.Number);
        }

        private void RememberNumber(Room room)
        {
            _calls.Add("N" + room.Number);
        }

        private Booking AddBooking(int room, int nights)
        {
            DateTime from = new DateTime(2026, 11, 1);
            return _hotel.CreateBooking(_guest, room, from, from.AddDays(nights));
        }

        [TestMethod]
        public void FindRooms_PredicateCheap_ReturnsOnlyCheapRooms()
        {
            // Act
            List<Room> result = _hotel.FindRooms(IsCheap);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(101, result[0].Number);
        }

        [TestMethod]
        public void FindRooms_PredicateWithoutMatches_ReturnsEmptyList()
        {
            // Act
            List<Room> result = _hotel.FindRooms(IsSuite);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindRooms_NullCondition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.FindRooms(null));
        }

        [TestMethod]
        public void FindRooms_DoesNotChangeHotelRooms()
        {
            // Act
            _hotel.FindRooms(IsCheap);

            // Assert
            Assert.AreEqual(3, _hotel.Rooms.Count);
        }

        [TestMethod]
        public void FindBookings_PredicatePaid_ReturnsOnlyPaidBookings()
        {
            // Arrange
            Booking first = AddBooking(101, 2);
            AddBooking(102, 3);
            first.Pay(PaymentMethod.Cash);

            // Act
            List<Booking> result = _hotel.FindBookings(IsPaid);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(first, result[0]);
        }

        [TestMethod]
        public void FindBookings_NullCondition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.FindBookings(null));
        }

        [TestMethod]
        public void SumBookings_FuncNights_SumsNightsOfAllBookings()
        {
            // Arrange
            AddBooking(101, 2);
            AddBooking(102, 3);

            // Act
            decimal result = _hotel.SumBookings(Nights, IsAny);

            // Assert
            Assert.AreEqual(5m, result);
        }

        [TestMethod]
        public void SumBookings_FuncTotalWithPaidFilter_SumsOnlyPaid()
        {
            // Arrange
            Booking first = AddBooking(101, 2);
            AddBooking(102, 3);
            first.Pay(PaymentMethod.Card);

            // Act
            decimal result = _hotel.SumBookings(Total, IsPaid);

            // Assert
            Assert.AreEqual(1600m, result);
        }

        [TestMethod]
        public void SumBookings_NoBookings_ReturnsZero()
        {
            // Act
            decimal result = _hotel.SumBookings(Total, IsAny);

            // Assert
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void SumBookings_NullSelector_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.SumBookings(null, IsAny));
        }

        [TestMethod]
        public void SumBookings_NullCondition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.SumBookings(Total, null));
        }

        [TestMethod]
        public void ForEachRoom_Action_CallsActionForEveryRoom()
        {
            // Act
            _hotel.ForEachRoom(RememberRoom);

            // Assert
            Assert.AreEqual(3, _calls.Count);
            Assert.IsTrue(_calls.Contains("R101"));
            Assert.IsTrue(_calls.Contains("R102"));
            Assert.IsTrue(_calls.Contains("R301"));
        }

        [TestMethod]
        public void ForEachRoom_MulticastAction_CallsAllHandlersInOrder()
        {
            // Arrange
            Action<Room> action = RememberRoom;
            action += RememberNumber;

            // Act
            _hotel.ForEachRoom(action);

            // Assert
            Assert.AreEqual(6, _calls.Count);
            Assert.AreEqual("R101", _calls[0]);
            Assert.AreEqual("N101", _calls[1]);
        }

        [TestMethod]
        public void ForEachRoom_AfterRemovingHandler_CallsOnlyRemaining()
        {
            // Arrange
            Action<Room> action = RememberRoom;
            action += RememberNumber;
            action -= RememberRoom;

            // Act
            _hotel.ForEachRoom(action);

            // Assert
            Assert.AreEqual(3, _calls.Count);
            Assert.AreEqual("N101", _calls[0]);
        }

        [TestMethod]
        public void ForEachRoom_NullAction_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.ForEachRoom(null));
        }

        [TestMethod]
        public void CalculateTotal_NoDiscountRule_EqualsFullPrice()
        {
            // Arrange
            Booking booking = AddBooking(102, 3);

            // Act
            decimal result = booking.CalculateTotal(DiscountRules.NoDiscount);

            // Assert
            Assert.AreEqual(3600m, result);
        }

        [TestMethod]
        public void CalculateTotal_LongStayRuleThreeNights_AppliesTenPercent()
        {
            // Arrange
            Booking booking = AddBooking(102, 3);

            // Act
            decimal result = booking.CalculateTotal(DiscountRules.LongStayDiscount);

            // Assert
            Assert.AreEqual(3240m, result);
        }

        [TestMethod]
        public void CalculateTotal_LongStayRuleTwoNights_NoDiscount()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act
            decimal result = booking.CalculateTotal(DiscountRules.LongStayDiscount);

            // Assert
            Assert.AreEqual(2400m, result);
        }

        [TestMethod]
        public void CalculateTotal_LoyalGuestRuleSecondBooking_AppliesFivePercent()
        {
            // Arrange
            AddBooking(101, 2);
            Booking second = _hotel.CreateBooking(_guest, 102, new DateTime(2026, 12, 1), new DateTime(2026, 12, 3));

            // Act
            decimal result = second.CalculateTotal(DiscountRules.LoyalGuestDiscount);

            // Assert
            Assert.AreEqual(2280m, result);
        }

        [TestMethod]
        public void CalculateTotal_LoyalGuestRuleFirstBooking_NoDiscount()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act
            decimal result = booking.CalculateTotal(DiscountRules.LoyalGuestDiscount);

            // Assert
            Assert.AreEqual(2400m, result);
        }

        [TestMethod]
        public void CalculateTotal_CustomRuleMethod_UsesReturnedPercent()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act
            decimal result = booking.CalculateTotal(Fixed25);

            // Assert
            Assert.AreEqual(1800m, result);
        }

        [TestMethod]
        public void CalculateTotal_NullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => booking.CalculateTotal(null));
        }

        [TestMethod]
        public void CalculateTotal_RuleReturnsHundred_ThrowsInvalidOperationException()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => booking.CalculateTotal(Hundred));
        }

        [TestMethod]
        public void CalculateTotal_RuleReturnsNegative_ThrowsInvalidOperationException()
        {
            // Arrange
            Booking booking = AddBooking(102, 2);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => booking.CalculateTotal(Negative));
        }

        [TestMethod]
        public void Pay_WithDiscountRule_PaymentContainsDiscountedAmount()
        {
            // Arrange
            Booking booking = AddBooking(102, 3);

            // Act
            Payment payment = booking.Pay(PaymentMethod.Card, DiscountRules.LongStayDiscount);

            // Assert
            Assert.AreEqual(3240m, payment.Amount);
            Assert.IsTrue(booking.IsPaid);
        }

        [TestMethod]
        public void Pay_WithDiscountRuleTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            Booking booking = AddBooking(102, 3);
            booking.Pay(PaymentMethod.Cash, DiscountRules.NoDiscount);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => booking.Pay(PaymentMethod.Cash, DiscountRules.NoDiscount));
        }

        [TestMethod]
        public void CalculateExpectedIncome_WithRule_SumsDiscountedActiveBookings()
        {
            // Arrange
            AddBooking(102, 3);
            AddBooking(101, 2);

            // Act
            decimal result = _hotel.CalculateExpectedIncome(DiscountRules.LongStayDiscount);

            // Assert
            Assert.AreEqual(3240m + 1600m, result);
        }

        [TestMethod]
        public void CalculateExpectedIncome_NoDiscountRule_EqualsRegularExpectedIncome()
        {
            // Arrange
            AddBooking(102, 3);
            AddBooking(101, 2);

            // Act
            decimal result = _hotel.CalculateExpectedIncome(DiscountRules.NoDiscount);

            // Assert
            Assert.AreEqual(_hotel.CalculateExpectedIncome(), result);
        }

        [TestMethod]
        public void CalculateExpectedIncome_WithRule_SkipsCancelledBookings()
        {
            // Arrange
            Booking cancelled = AddBooking(102, 3);
            AddBooking(101, 2);
            cancelled.Cancel();

            // Act
            decimal result = _hotel.CalculateExpectedIncome(DiscountRules.NoDiscount);

            // Assert
            Assert.AreEqual(1600m, result);
        }

        [TestMethod]
        public void CalculateExpectedIncome_NullRule_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _hotel.CalculateExpectedIncome((DiscountRule)null));
        }

        [TestMethod]
        public void DiscountRule_MulticastDelegate_ReturnsResultOfLastMethod()
        {
            // Arrange
            Booking booking = AddBooking(102, 3);
            DiscountRule rule = DiscountRules.NoDiscount;
            rule += DiscountRules.LongStayDiscount;

            // Act
            decimal result = rule(booking);

            // Assert
            Assert.AreEqual(DiscountRules.LongStayPercent, result);
        }

        [TestMethod]
        public void DiscountRules_NoDiscount_ReturnsZero()
        {
            // Arrange
            Booking booking = AddBooking(102, 5);

            // Act
            decimal result = DiscountRules.NoDiscount(booking);

            // Assert
            Assert.AreEqual(0m, result);
        }
    }
}
