using GameClub.Logic;
using Xunit;

namespace GameClub.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_Returns_Expected_SHA256_For_Admin()
        {
            // Arrange
            string password = "admin";

            // Act
            string hash = PasswordHasher.Hash(password);

            // Assert
            Assert.Equal("8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", hash);
        }

        [Fact]
        public void Hash_Same_Password_Returns_Same_Hash()
        {
            // Arrange
            string password = "1234";

            // Act
            string hash1 = PasswordHasher.Hash(password);
            string hash2 = PasswordHasher.Hash(password);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Verify_Correct_Password_Returns_True()
        {
            // Arrange
            string password = "admin";
            string hash = PasswordHasher.Hash(password);

            // Act
            bool result = PasswordHasher.Verify(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Verify_Wrong_Password_Returns_False()
        {
            // Arrange
            string hash = PasswordHasher.Hash("admin");

            // Act
            bool result = PasswordHasher.Verify("wrong", hash);

            // Assert
            Assert.False(result);
        }
    }
}