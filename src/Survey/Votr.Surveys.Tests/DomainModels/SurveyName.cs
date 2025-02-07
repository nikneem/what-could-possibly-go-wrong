using Votr.Core.DDD.Enums;
using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Tests.DomainModels
{
    public class SurveyName
    {
        [Fact]
        public void UpdateName_ShouldUpdateName_WhenValidName()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            var newName = "Updated Room Name";

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal(newName, room.Name);
            Assert.Equal(TrackingState.Modified, room.TrackingState);
        }

        [Fact]
        public void UpdateName_ShouldNotUpdateName_WhenNameIsNull()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            string newName = null;

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal("Initial Name", room.Name);
            Assert.NotEqual(TrackingState.Modified, room.TrackingState);
        }

        [Fact]
        public void UpdateName_ShouldNotUpdateName_WhenNameIsWhitespace()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            var newName = "   ";

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal("Initial Name", room.Name);
            Assert.NotEqual(TrackingState.Modified, room.TrackingState);
        }

        [Fact]
        public void UpdateName_ShouldNotUpdateName_WhenNameIsTooShort()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            var newName = "abc";

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal("Initial Name", room.Name);
            Assert.NotEqual(TrackingState.Modified, room.TrackingState);
        }

        [Fact]
        public void UpdateName_ShouldNotUpdateName_WhenNameIsTooLong()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            var newName = new string('a', 101);

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal("Initial Name", room.Name);
            Assert.NotEqual(TrackingState.Modified, room.TrackingState);
        }

        [Fact]
        public void UpdateName_ShouldNotUpdateName_WhenNameIsSame()
        {
            // Arrange
            var room = new Survey(Guid.NewGuid(), "Initial Name", "12345", DateTimeOffset.UtcNow.AddDays(1), new List<Question>());
            var newName = "Initial Name";

            // Act
            room.UpdateName(newName);

            // Assert
            Assert.Equal("Initial Name", room.Name);
            Assert.NotEqual(TrackingState.Modified, room.TrackingState);
        }
    }
}