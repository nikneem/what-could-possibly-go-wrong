using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Tests.DomainModels
{
    public class SurveyExpiryDate
    {
        [Fact]
        public void UpdateExpiry_ShouldUpdateExpiryDate_WhenValidDateIsProvided()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Test Room", "12345", DateTimeOffset.UtcNow.AddDays(10), new List<Question>());
            var newExpiryDate = DateTimeOffset.UtcNow.AddDays(20);

            // Act
            room.UpdateExpiry(newExpiryDate);

            // Assert
            Assert.Equal(newExpiryDate, room.ExpiresOn);
        }

        [Fact]
        public void UpdateExpiry_ShouldNotUpdateExpiryDate_WhenDateIsInThePast()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Test Room", "12345", DateTimeOffset.UtcNow.AddDays(10), new List<Question>());
            var pastDate = DateTimeOffset.UtcNow.AddDays(-1);
            var originalExpiryDate = room.ExpiresOn;

            // Act
            room.UpdateExpiry(pastDate);

            // Assert
            Assert.Equal(originalExpiryDate, room.ExpiresOn);
        }

        [Fact]
        public void UpdateExpiry_ShouldAddValidationError_WhenDateIsInThePast()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Test Room", "12345", DateTimeOffset.UtcNow.AddDays(10), new List<Question>());
            var pastDate = DateTimeOffset.UtcNow.AddDays(-1);

            // Act
            room.UpdateExpiry(pastDate);

            // Assert
            Assert.Contains(room.ValidationErrors, e => e.PropertyName == nameof(room.ExpiresOn) && e.ErrorMessage == "The expiry date must be in the future");
        }

        [Fact]
        public void UpdateExpiry_ShouldNotUpdateExpiryDate_WhenDateIsSameAsCurrent()
        {
            // Arrange
            var expiryDate = DateTimeOffset.UtcNow.AddDays(10);
            var room = new Survey(Guid.NewGuid(), "Test Room", "12345", expiryDate, new List<Question>());

            // Act
            room.UpdateExpiry(expiryDate);

            // Assert
            Assert.Equal(expiryDate, room.ExpiresOn);
        }
    }
}