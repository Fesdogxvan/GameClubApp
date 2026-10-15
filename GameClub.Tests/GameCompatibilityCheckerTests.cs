using GameClub.Logic;
using Xunit;

namespace GameClub.Tests
{
    public class GameCompatibilityCheckerTests
    {
        [Fact]
        public void CanRun_Level_Above_Min_Returns_True()
        {
            // Arrange: суммарный уровень ПК выше минимума игры
            int totalPerf = 70;
            int minLevel = 9;

            // Act
            bool result = GameCompatibilityChecker.CanRun(totalPerf, minLevel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanRun_Level_Below_Min_Returns_False()
        {
            // Arrange: суммарный уровень ПК ниже минимума игры
            int totalPerf = 5;
            int minLevel = 9;

            // Act
            bool result = GameCompatibilityChecker.CanRun(totalPerf, minLevel);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanRun_Level_Equals_Min_Returns_True()
        {
            // Arrange: граничный случай, равенство
            int totalPerf = 9;
            int minLevel = 9;

            // Act
            bool result = GameCompatibilityChecker.CanRun(totalPerf, minLevel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanRun_Zero_Level_Returns_False()
        {
            // Arrange: у ПК нет комплектующих
            int totalPerf = 0;
            int minLevel = 1;

            // Act
            bool result = GameCompatibilityChecker.CanRun(totalPerf, minLevel);

            // Assert
            Assert.False(result);
        }
    }
}