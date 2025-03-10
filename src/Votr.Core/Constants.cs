namespace Votr.Core;

public class Constants
{
    
}

public class ServiceName
{
    public const string SurveysApi = "mainApi";
    public const string VotesApi = "mainApi";
}

public class HttpHeaders
{
    public const string VoterId = "X-VotR-VoterId";
}

public class RegularExpression
{
    public const string Guid = @"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$";
}

public class RealtimeMessage
{
    public const string SurveyQuestionActivated = "SurveyQuestionActivated";
    public const string SurveyQuestionVotesChanged = "SurveyQuestionVotesChanged";
}