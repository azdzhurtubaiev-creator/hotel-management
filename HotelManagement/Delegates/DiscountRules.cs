namespace HotelManagement
{
    public static class DiscountRules
    {
        public const decimal LongStayPercent = 10m;
        public const decimal LoyalGuestPercent = 5m;
        public const int LongStayNights = 3;
        public const int LoyalGuestBookings = 2;

        public static decimal NoDiscount(Booking booking)
        {
            return 0m;
        }

        public static decimal LongStayDiscount(Booking booking)
        {
            return booking.GetNights() >= LongStayNights ? LongStayPercent : 0m;
        }

        public static decimal LoyalGuestDiscount(Booking booking)
        {
            return booking.Guest.Bookings.Count >= LoyalGuestBookings ? LoyalGuestPercent : 0m;
        }
    }
}
