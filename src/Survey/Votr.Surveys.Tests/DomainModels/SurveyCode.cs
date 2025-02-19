using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Tests.DomainModels
{
    public class RoomCodeTest
    {
        [Fact]
        public void RoomCode_ShouldBeGenerated_WhenRoomIsCreated()
        {
            // Arrange
            var room = Survey.Create("Name");

            // Act
            var roomCode = room.Code;

            // Assert
            Assert.False(string.IsNullOrEmpty(roomCode));
        }

        [Fact]
        public void RoomCode_ShouldBeRandom_WhenRoomIsCreated()
        {
            // Arrange
            var room1 = Survey.Create("Name");
            var room2 = Survey.Create("Name");

            // Act
            var roomCode1 = room1.Code;
            var roomCode2 = room2.Code;

            // Assert
            Assert.NotEqual(roomCode1, roomCode2);
        }
    }

}