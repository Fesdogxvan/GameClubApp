using GameClub.Logic;
using Xunit;

namespace GameClub.Tests
{
    public class SessionValidatorTests
    {
        [Fact]
        public void CanStart_FreeWorkstation_NoActiveSessions_Returns_True()
        {
            // Arrange
            string status = "Свободен";

            // Act
            bool result = SessionValidator.CanStart(
                status,
                customerHasActiveSession: false,
                workstationHasActiveSession: false,
                out string error);

            // Assert
            Assert.True(result);
            Assert.Null(error);
        }

        [Fact]
        public void CanStart_BusyWorkstation_Returns_False()
        {
            // Arrange
            string status = "Занят";

            // Act
            bool result = SessionValidator.CanStart(status, false, false, out string error);

            // Assert
            Assert.False(result);
            Assert.NotNull(error);
            Assert.Contains("занято", error);
        }

        [Fact]
        public void CanStart_ServiceWorkstation_Returns_False()
        {
            // Arrange
            string status = "Обслуживание";

            // Act
            bool result = SessionValidator.CanStart(status, false, false, out string error);

            // Assert
            Assert.False(result);
            Assert.NotNull(error);
            Assert.Contains("обслуживании", error);
        }

        [Fact]
        public void CanStart_CustomerWithActiveSession_Returns_False()
        {
            // Arrange
            string status = "Свободен";

            // Act
            bool result = SessionValidator.CanStart(status, true, false, out string error);

            // Assert
            Assert.False(result);
            Assert.NotNull(error);
            Assert.Contains("клиента", error);
        }

        [Fact]
        public void CanStart_WorkstationWithActiveSession_Returns_False()
        {
            // Arrange
            string status = "Свободен";

            // Act
            bool result = SessionValidator.CanStart(status, false, true, out string error);

            // Assert
            Assert.False(result);
            Assert.NotNull(error);
            Assert.Contains("рабочего места", error);
        }
    }
}