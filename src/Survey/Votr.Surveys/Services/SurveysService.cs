using Votr.Core.DataTransferObjects;
using Votr.Surveys.Abstractions;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Details;
using Votr.Surveys.Mappings;

namespace Votr.Surveys.Services;

public class SurveysService(ISurveysRepository surveysRepository) : ISurveysService
{
    public async Task<VotrResponse<List<SurveyDetailsResponse>>> List(CancellationToken cancellationToken)
    {
        try
        {
            var surveys = await surveysRepository.List(cancellationToken);
            return VotrResponse<List<SurveyDetailsResponse>>.Success(surveys);
        }
        catch (Exception ex)
        {
            return VotrResponse<List<SurveyDetailsResponse>>.Failure(ex.Message);
        }
    }

    public async Task<VotrResponse<SurveyDetailsResponse>> Create(
        SurveyCreateRequest requestData,
        CancellationToken cancellationToken)
    {
        var survey = requestData.FromCreateModel();
        if (await surveysRepository.Save(survey, cancellationToken))
        {
            return VotrResponse<SurveyDetailsResponse>.Success(survey.ToDetailsResponse());
        }
        return VotrResponse<SurveyDetailsResponse>.Failure("Failed to save survey");
    }
}