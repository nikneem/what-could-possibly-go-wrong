using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Votr.Core;
using Votr.Votes.DataTransferObjects;
using Votr.Votes.Abstractions;

namespace Votr.Votes.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VotesController(IVotesService votesService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VoteCreateRequest requestData,
        CancellationToken cancellationToken)
    {
        var voterId = GetVoterId();
        if (!voterId.HasValue)
        {
            return BadRequest("No Voter ID present in request");
        }
        var response = await votesService.StoreVote(voterId.Value, requestData, cancellationToken);
        return Ok(response);
    }




    private Guid? GetVoterId()
    {
        if (HttpContext.Request.Headers.TryGetValue(HttpHeaders.VoterId, out var reviewerId))
        {
            if (Regex.IsMatch(reviewerId.ToString(), RegularExpression.Guid))
            {
                return Guid.Parse(reviewerId.ToString());
            }
        }

        return null;
    }
}
