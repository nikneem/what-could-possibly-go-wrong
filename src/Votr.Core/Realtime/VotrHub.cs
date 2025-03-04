using Microsoft.AspNetCore.SignalR;

namespace Votr.Core.Realtime;

public class VotrHub : Hub
{

    public Task BroadcastQuestionChanged(string surveyCode, string payload)
    {
        return BroadcastGroupMessage(surveyCode, "QuestionChanged", payload);
    }
    public Task BroadcastVotesChangedChanged(string surveyCode, string payload)
    {
        return BroadcastGroupMessage(surveyCode, "VotesChanged", payload);
    }
    private Task BroadcastGroupMessage(string group, string message, string payload)
    {
        return Clients.Group(group).SendAsync(message, payload);
    }

}