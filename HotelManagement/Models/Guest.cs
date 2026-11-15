using System;
using System.Collections.Generic;

namespace HotelManagement
{
    public class Guest : Person
    {
        private readonly List<Booking> _bookings = new List<Booking>();

        public Guest(string firstName, string lastName, string phone)
            : base(firstName, lastName, phone)
        {
        }

        public override string Role
        {
            get => "Гість";
        }

        public IReadOnlyList<Booking> Bookings
        {
            get => _bookings.AsReadOnly();
        }

        public override string GetGreeting()
        {
            return $"Ласкаво просимо до готелю, {FirstName}!";
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, бронювань: {_bookings.Count}";
        }

        public void AddBooking(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Бронювання не може бути відсутнім!");
            }

            if (booking.Guest != this)
            {
                throw new ArgumentException("Це бронювання належить іншому гостю!", nameof(booking));
            }

            if (_bookings.Contains(booking))
            {
                throw new InvalidOperationException("Це бронювання вже додано!");
            }

            _bookings.Add(booking);
        }
    }
}
