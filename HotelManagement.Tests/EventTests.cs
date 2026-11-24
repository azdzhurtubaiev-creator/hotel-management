using System;
using System.Collections.Generic;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class EventTests
    {
        private Hotel _hotel;
        private Guest _guest;
        private List<string> _messages;
        private List<string> _secondMessages;
        private int _statusEvents;
        private Booking _lastBooking;
        private BookingStatus _lastOld;
        private BookingStatus _lastNew;
        private int _priceEvents;
        private Room _lastRoom;
        private decimal _lastOldPrice;
        private decimal _lastNewPrice;

        [TestInitialize]
        public void Setup()
        {
            _hotel = new Hotel("Харків Палас", 4, "Харків, вул. Сумська, 10");
            _guest = new Guest("Іван", "Петренко", "0501234567");
            _messages = new List<string>();
            _secondMessages = new List<string>();
            _statusEvents = 0;
            _priceEvents = 0;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _hotel = null;
            _guest = null;
            _messages = null;
            _secondMessages = null;
            _lastBooking = null;
            _lastRoom = null;
        }

        private void OnNotify(string message)
        {
            _messages.Add(message);
        }

        private void OnNotifySecond(string message)
        {
            _secondMessages.Add(message);
        }

        private void OnStatusChanged(Booking booking, BookingStatus oldStatus, BookingStatus newStatus)
        {
            _statusEvents++;
            _lastBooking = booking;
            _lastOld = oldStatus;
            _lastNew = newStatus;
        }

        private void OnPriceChanged(Room room, decimal oldPrice, decimal newPrice)
        {
            _priceEvents++;
            _lastRoom = room;
            _lastOldPrice = oldPrice;
            _lastNewPrice = newPrice;
        }

        private Booking PrepareBooking()
        {
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.RegisterGuest(_guest);
            return _hotel.CreateBooking(_guest, 102, new DateTime(2026, 11, 20), new DateTime(2026, 11, 23));
        }

        [TestMethod]
        public void Notify_AddRoom_RaisesEventWithRoomInfo()
        {
            // Arrange
            _hotel.Notify += OnNotify;

            // Act
            _hotel.AddRoom(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("101"));
            Assert.IsTrue(_messages[0].Contains("Single"));
        }

        [TestMethod]
        public void Notify_AddExistingRoom_DoesNotRaiseEvent()
        {
            // Arrange
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.Notify += OnNotify;

            // Act
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.AddRoom(101, RoomType.Single, 800m));

            // Assert
            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void Notify_RemoveRoom_RaisesEvent()
        {
            // Arrange
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.Notify += OnNotify;

            // Act
            _hotel.RemoveRoom(101);

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("101"));
        }

        [TestMethod]
        public void Notify_RemoveMissingRoom_DoesNotRaiseEvent()
        {
            // Arrange
            _hotel.Notify += OnNotify;

            // Act
            _hotel.RemoveRoom(999);

            // Assert
            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void Notify_RegisterGuest_RaisesEventWithGuestName()
        {
            // Arrange
            _hotel.Notify += OnNotify;

            // Act
            _hotel.RegisterGuest(_guest);

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("Іван Петренко"));
        }

        [TestMethod]
        public void Notify_CreateBooking_RaisesEvent()
        {
            // Arrange
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.RegisterGuest(_guest);
            _hotel.Notify += OnNotify;

            // Act
            _hotel.CreateBooking(_guest, 102, new DateTime(2026, 11, 20), new DateTime(2026, 11, 23));

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("бронювання"));
        }

        [TestMethod]
        public void Notify_FailedCreateBooking_DoesNotRaiseEvent()
        {
            // Arrange
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.RegisterGuest(_guest);
            _hotel.Notify += OnNotify;

            // Act
            Assert.ThrowsExactly<InvalidOperationException>(() => _hotel.CreateBooking(_guest, 555, new DateTime(2026, 11, 20), new DateTime(2026, 11, 23)));

            // Assert
            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void Notify_NoSubscribers_OperationsWorkWithoutErrors()
        {
            // Act
            Booking booking = PrepareBooking();
            booking.Confirm();

            // Assert
            Assert.AreEqual(BookingStatus.Confirmed, booking.Status);
        }

        [TestMethod]
        public void Notify_TwoSubscribers_BothReceiveSameMessage()
        {
            // Arrange
            _hotel.Notify += OnNotify;
            _hotel.Notify += OnNotifySecond;

            // Act
            _hotel.AddRoom(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.AreEqual(1, _secondMessages.Count);
            Assert.AreEqual(_messages[0], _secondMessages[0]);
        }

        [TestMethod]
        public void Notify_AfterUnsubscribe_HandlerIsNotCalled()
        {
            // Arrange
            _hotel.Notify += OnNotify;
            _hotel.Notify += OnNotifySecond;
            _hotel.Notify -= OnNotify;

            // Act
            _hotel.AddRoom(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(0, _messages.Count);
            Assert.AreEqual(1, _secondMessages.Count);
        }

        [TestMethod]
        public void Notify_SubscribedTwice_HandlerCalledTwice()
        {
            // Arrange
            _hotel.Notify += OnNotify;
            _hotel.Notify += OnNotify;

            // Act
            _hotel.AddRoom(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(2, _messages.Count);
        }

        [TestMethod]
        public void Notify_BookingConfirm_HotelReceivesStatusMessage()
        {
            // Arrange
            Booking booking = PrepareBooking();
            _hotel.Notify += OnNotify;

            // Act
            booking.Confirm();

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("Created -> Confirmed"));
        }

        [TestMethod]
        public void Notify_RoomPriceChange_HotelReceivesPriceMessage()
        {
            // Arrange
            Room room = _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.Notify += OnNotify;

            // Act
            room.PricePerNight = 900m;

            // Assert
            Assert.AreEqual(1, _messages.Count);
            Assert.IsTrue(_messages[0].Contains("800 -> 900"));
        }

        [TestMethod]
        public void Notify_PriceChangeOfRemovedRoom_HotelDoesNotReceiveMessage()
        {
            // Arrange
            Room room = _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.RemoveRoom(101);
            _hotel.Notify += OnNotify;

            // Act
            room.PricePerNight = 900m;

            // Assert
            Assert.AreEqual(0, _messages.Count);
        }

        [TestMethod]
        public void Notify_ForEachRoomRaisePrice_RaisesEventForEveryRoom()
        {
            // Arrange
            _hotel.AddRoom(101, RoomType.Single, 800m);
            _hotel.AddRoom(102, RoomType.Double, 1200m);
            _hotel.Notify += OnNotify;

            // Act
            _hotel.ForEachRoom(RaisePriceByHundred);

            // Assert
            Assert.AreEqual(2, _messages.Count);
        }

        private static void RaisePriceByHundred(Room room)
        {
            room.PricePerNight += 100m;
        }

        [TestMethod]
        public void StatusChanged_Confirm_PassesBookingAndStatuses()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;

            // Act
            booking.Confirm();

            // Assert
            Assert.AreEqual(1, _statusEvents);
            Assert.AreSame(booking, _lastBooking);
            Assert.AreEqual(BookingStatus.Created, _lastOld);
            Assert.AreEqual(BookingStatus.Confirmed, _lastNew);
        }

        [TestMethod]
        public void StatusChanged_FullCycle_RaisesEventOnEveryTransition()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;

            // Act
            booking.Confirm();
            booking.CheckInGuest();
            booking.Pay(PaymentMethod.Cash);
            booking.CheckOutGuest();

            // Assert
            Assert.AreEqual(3, _statusEvents);
            Assert.AreEqual(BookingStatus.CheckedIn, _lastOld);
            Assert.AreEqual(BookingStatus.CheckedOut, _lastNew);
        }

        [TestMethod]
        public void StatusChanged_Cancel_RaisesEvent()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;

            // Act
            booking.Cancel();

            // Assert
            Assert.AreEqual(1, _statusEvents);
            Assert.AreEqual(BookingStatus.Cancelled, _lastNew);
        }

        [TestMethod]
        public void StatusChanged_InvalidTransition_DoesNotRaiseEvent()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;

            // Act
            Assert.ThrowsExactly<InvalidOperationException>(() => booking.CheckInGuest());

            // Assert
            Assert.AreEqual(0, _statusEvents);
            Assert.AreEqual(BookingStatus.Created, booking.Status);
        }

        [TestMethod]
        public void StatusChanged_AfterUnsubscribe_HandlerIsNotCalled()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;
            booking.StatusChanged -= OnStatusChanged;

            // Act
            booking.Confirm();

            // Assert
            Assert.AreEqual(0, _statusEvents);
        }

        [TestMethod]
        public void StatusChanged_PayDoesNotChangeStatus_NoEvent()
        {
            // Arrange
            Booking booking = PrepareBooking();
            booking.StatusChanged += OnStatusChanged;

            // Act
            booking.Pay(PaymentMethod.Card);

            // Assert
            Assert.AreEqual(0, _statusEvents);
        }

        [TestMethod]
        public void PriceChanged_NewPrice_PassesRoomAndPrices()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);
            room.PriceChanged += OnPriceChanged;

            // Act
            room.PricePerNight = 950m;

            // Assert
            Assert.AreEqual(1, _priceEvents);
            Assert.AreSame(room, _lastRoom);
            Assert.AreEqual(800m, _lastOldPrice);
            Assert.AreEqual(950m, _lastNewPrice);
        }

        [TestMethod]
        public void PriceChanged_SamePrice_DoesNotRaiseEvent()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);
            room.PriceChanged += OnPriceChanged;

            // Act
            room.PricePerNight = 800m;

            // Assert
            Assert.AreEqual(0, _priceEvents);
        }

        [TestMethod]
        public void PriceChanged_InvalidPrice_DoesNotRaiseEventAndKeepsPrice()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);
            room.PriceChanged += OnPriceChanged;

            // Act
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => room.PricePerNight = -5m);

            // Assert
            Assert.AreEqual(0, _priceEvents);
            Assert.AreEqual(800m, room.PricePerNight);
        }

        [TestMethod]
        public void PriceChanged_Constructor_DoesNotRaiseEvent()
        {
            // Act
            Room room = new Room(101, RoomType.Single, 800m);

            // Assert
            Assert.AreEqual(800m, room.PricePerNight);
            Assert.AreEqual(0, _priceEvents);
        }

        [TestMethod]
        public void PriceChanged_AfterUnsubscribe_HandlerIsNotCalled()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);
            room.PriceChanged += OnPriceChanged;
            room.PriceChanged -= OnPriceChanged;

            // Act
            room.PricePerNight = 900m;

            // Assert
            Assert.AreEqual(0, _priceEvents);
        }

        [TestMethod]
        public void PriceChanged_AfterBooking_BookingKeepsPriceFromCreation()
        {
            // Arrange
            Booking booking = PrepareBooking();
            Room room = booking.Room;

            // Act
            room.PricePerNight = 2000m;

            // Assert
            Assert.AreEqual(1200m, booking.NightPrice);
            Assert.AreEqual(3600m, booking.CalculateTotal());
        }

        [TestMethod]
        public void PriceChanged_NewBookingAfterChange_UsesNewPrice()
        {
            // Arrange
            Booking first = PrepareBooking();
            first.Room.PricePerNight = 2000m;

            // Act
            Booking second = _hotel.CreateBooking(_guest, 102, new DateTime(2026, 12, 1), new DateTime(2026, 12, 3));

            // Assert
            Assert.AreEqual(2000m, second.NightPrice);
            Assert.AreEqual(4000m, second.CalculateTotal());
        }

        [TestMethod]
        public void PriceChanged_ChangedTwice_RaisedTwiceWithLatestValues()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);
            room.PriceChanged += OnPriceChanged;

            // Act
            room.PricePerNight = 900m;
            room.PricePerNight = 1000m;

            // Assert
            Assert.AreEqual(2, _priceEvents);
            Assert.AreEqual(900m, _lastOldPrice);
            Assert.AreEqual(1000m, _lastNewPrice);
        }
    }
}
