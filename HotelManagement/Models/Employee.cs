using System;

namespace HotelManagement
{
    public class Employee : Person
    {
        private Position _position;
        private decimal _salary;

        public Employee(string firstName, string lastName, string phone, Position position, decimal salary)
            : base(firstName, lastName, phone)
        {
            Position = position;
            Salary = salary;
        }

        public override string Role
        {
            get => "Працівник";
        }

        public Position Position
        {
            get => _position;
            set
            {
                if (!Enum.IsDefined(typeof(Position), value))
                {
                    throw new ArgumentException("Обрано неіснуючу посаду!", nameof(Position));
                }

                _position = value;
            }
        }

        public decimal Salary
        {
            get => _salary;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Salary), "Зарплата не може бути від'ємною!");
                }

                _salary = value;
            }
        }

        public override string GetGreeting()
        {
            return $"Гарної зміни, {FirstName}!";
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, посада: {Position}, зарплата: {Salary:0.##} грн";
        }

        public void RaiseSalary(decimal percent)
        {
            if (percent <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(percent), "Відсоток підвищення має бути більше 0!");
            }

            Salary = Math.Round(Salary * (1 + percent / 100), 2);
        }
    }
}
