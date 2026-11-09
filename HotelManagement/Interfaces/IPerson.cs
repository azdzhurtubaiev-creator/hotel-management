namespace HotelManagement
{
    public interface IPerson
    {
        string FirstName { get; }

        string LastName { get; }

        string Phone { get; }

        string GetFullName();
    }
}
