using System;
using System.Collections.Generic;

namespace HotelManagement
{
    public class Guest : IPerson
    {
        private List<Booking> _bookings;

        public Guest(string firstName, string lastName, string phone)
        {
            throw new NotImplementedException();
        }

        public string FirstName
        {
            get => throw new NotImplementedException();
        }

        public string LastName
        {
            get => throw new NotImplementedException();
        }

        public string Phone
        {
            get => throw new NotImplementedException();
        }

        public IReadOnlyList<Booking> Bookings
        {
            get => throw new NotImplementedException();
        }

        public string GetFullName()
        {
            throw new NotImplementedException();
        }

        public void AddBooking(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
