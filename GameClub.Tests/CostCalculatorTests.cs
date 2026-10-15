using System;
using GameClub.Logic;
using Xunit;

namespace GameClub.Tests
{
    public class CostCalculatorTests
    {
        [Fact]
        public void Calculate_60Minutes_At150PerHour_Returns150()
        {
            // Arrange
            var start = new DateTime(2025, 11, 1, 14, 0, 0);
            var end = new DateTime(2025, 11, 1, 15, 0, 0);
            decimal price = 150m;

            // Act
            decimal result = CostCalculator.Calculate(start, end, price);

            // Assert
            Assert.Equal(150.00m, result);
        }

        [Fact]
        public void Calculate_90Minutes_At150PerHour_Returns225()
        {
            // Arrange
            var start = new DateTime(2025, 11, 1, 14, 0, 0);
            var end = new DateTime(2025, 11, 1, 15, 30, 0);
            decimal price = 150m;

            // Act
            decimal result = CostCalculator.Calculate(start, end, price);

            // Assert
            Assert.Equal(225.00m, result);
        }

        [Fact]
        public void Calculate_45Minutes_At250PerHour_Returns187_50()
        {
            // Arrange
            var start = new DateTime(2025, 11, 1, 14, 0, 0);
            var end = new DateTime(2025, 11, 1, 14, 45, 0);
            decimal price = 250m;

            // Act
            decimal result = CostCalculator.Calculate(start, end, price);

            // Assert
            Assert.Equal(187.50m, result);
        }

        [Fact]
        public void Calculate_ZeroDuration_ReturnsZero()
        {
            // Arrange
            var start = new DateTime(2025, 11, 1, 14, 0, 0);
            var end = new DateTime(2025, 11, 1, 14, 0, 0);
            decimal price = 150m;

            // Act
            decimal result = CostCalculator.Calculate(start, end, price);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void Calculate_EndBeforeStart_ThrowsArgumentException()
        {
            // Arrange
            var start = new DateTime(2025, 11, 1, 15, 0, 0);
            var end = new DateTime(2025, 11, 1, 14, 0, 0);
            decimal price = 150m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CostCalculator.Calculate(start, end, price));
        }
    }
}