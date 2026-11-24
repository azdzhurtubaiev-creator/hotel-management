namespace HotelManagement
{
    public delegate decimal DiscountRule(Booking booking);

    public delegate void BookingStatusHandler(Booking booking, BookingStatus oldStatus, BookingStatus newStatus);

    public delegate void PriceChangedHandler(Room room, decimal oldPrice, decimal newPrice);

    public delegate void HotelNotifier(string message);
}
