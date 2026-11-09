using System;

namespace HotelManagement
{
    public class Employee : IPerson
    {
        public Employee(string firstName, string lastName, string phone, Position position, decimal salary)
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

        public Position Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public decimal Salary
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string GetFullName()
        {
            throw new NotImplementedException();
        }

        public void RaiseSalary(decimal percent)
        {
            throw new NotImplementedException();
        }
    }
}
