using System;
using HotelManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelManagement.Tests
{
    [TestClass]
    public class RoomTests
    {
        [TestMethod]
        public void Constructor_ValidValues_SetsProperties()
        {
            // Act
            Room room = new Room(101, RoomType.Double, 1200m);

            // Assert
            Assert.AreEqual(101, room.Number);
            Assert.AreEqual(RoomType.Double, room.Type);
            Assert.AreEqual(1200m, room.PricePerNight);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Constructor_NumberNotPositive_ThrowsArgumentOutOfRangeException(int number)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Room(number, RoomType.Single, 800m));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-100)]
        public void Constructor_PriceNotPositive_ThrowsArgumentOutOfRangeException(int price)
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Room(101, RoomType.Single, price));
        }

        [TestMethod]
        public void Constructor_UndefinedType_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => new Room(101, (RoomType)99, 800m));
        }

        [TestMethod]
        public void PricePerNight_ValidValue_IsChanged()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);

            // Act
            room.PricePerNight = 950m;

            // Assert
            Assert.AreEqual(950m, room.PricePerNight);
        }

        [TestMethod]
        public void PricePerNight_NegativeValue_ThrowsAndKeepsOldPrice()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);

            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => room.PricePerNight = -1m);
            Assert.AreEqual(800m, room.PricePerNight);
        }

        [TestMethod]
        [DataRow(RoomType.Single, 1)]
        [DataRow(RoomType.Double, 2)]
        [DataRow(RoomType.Suite, 2)]
        [DataRow(RoomType.Family, 4)]
        public void Capacity_DependsOnType(RoomType type, int expected)
        {
            // Arrange
            Room room = new Room(101, type, 1000m);

            // Act
            int actual = room.Capacity;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetInfo_ContainsNumberTypeAndPrice()
        {
            // Arrange
            Room room = new Room(205, RoomType.Suite, 3000m);

            // Act
            string info = room.GetInfo();

            // Assert
            Assert.IsTrue(info.Contains("205"));
            Assert.IsTrue(info.Contains("Suite"));
            Assert.IsTrue(info.Contains("3000"));
        }

        [TestMethod]
        public void Room_ImplementsIPrintable()
        {
            // Arrange
            Room room = new Room(101, RoomType.Single, 800m);

            // Assert
            Assert.IsInstanceOfType<IPrintable>(room);
        }
    }
}
