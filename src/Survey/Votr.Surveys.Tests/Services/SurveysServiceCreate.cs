using Moq;
using Votr.Surveys.Abstractions;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DomainModels;
using Votr.Surveys.Mappings;
using Votr.Surveys.Services;

namespace Votr.Surveys.Tests.Services
{
    public class SurveysServiceTest
    {
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly SurveysService _surveysService;

        public SurveysServiceTest()
        {
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _surveysService = new SurveysService(_surveysRepositoryMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenSurveyIsSaved()
        {
            // Arrange
            var requestData = new SurveyCreateRequest("Test", DateTimeOffset.UtcNow, [
                new SurveyCreateQuestion("Question 1", [
                    new SurveyCreateAnswerOption("Option 1"),
                    new SurveyCreateAnswerOption("Option 2")
                ]),
                new SurveyCreateQuestion("Question 2", [
                    new SurveyCreateAnswerOption("Option 1"),
                    new SurveyCreateAnswerOption("Option 2") ])
            ]);
            _surveysRepositoryMock.Setup(repo => repo.Save(It.IsAny<Survey>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _surveysService.Create(requestData, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            _surveysRepositoryMock.Verify(repo => repo.Save(It.IsAny<Survey>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenSurveyIsNotSaved()
        {
            // Arrange
            var requestData = new SurveyCreateRequest("Test", DateTimeOffset.UtcNow, [
                new SurveyCreateQuestion("Question 1", [
                    new SurveyCreateAnswerOption("Option 1"),
                    new SurveyCreateAnswerOption("Option 2")
                ]),
                new SurveyCreateQuestion("Question 2", [
                    new SurveyCreateAnswerOption("Option 1"),
                    new SurveyCreateAnswerOption("Option 2") ])
            ]);
            _surveysRepositoryMock.Setup(repo => repo.Save(It.IsAny<Survey>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _surveysService.Create(requestData, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to save survey", result.ErrorMessage);
            _surveysRepositoryMock.Verify(repo => repo.Save(It.IsAny<Survey>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}