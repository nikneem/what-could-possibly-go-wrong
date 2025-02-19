using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Votr.Core;
using Votr.Surveys.Abstractions;
using Votr.Surveys.DataTransferObjects.Create;
using Votr.Surveys.DataTransferObjects.Update;

namespace Votr.Surveys.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SurveysController(ISurveysService service) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var response = await service.List(cancellationToken);
        return Ok(response);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Single(string code, CancellationToken cancellationToken)
    {
        var response = await service.Get(code, cancellationToken);
        return Ok(response);
    }
    [HttpGet("{code}/questions/{questionId}/activate")]
    public async Task<IActionResult> ActivateQuestion(string code, Guid questionId, CancellationToken cancellationToken)
    {
        var response = await service.ActivateQuestion(code, questionId, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SurveyCreateRequest requestPayload,
        CancellationToken cancellationToken)
    {
        var response = await service.Create(requestPayload, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{code}")]

    public async Task<IActionResult> Update(string code, [FromBody] SurveyUpdateRequest requestPayload,
        CancellationToken cancellationToken)
    {
        var response = await service.Update(code, requestPayload, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{code}/connect")]
    public async Task<IActionResult> ConnectRealtime(string code, CancellationToken cancellationToken)
    {
        // Find the voter ID
        var voterId = GetVoterId() ?? Guid.NewGuid();
        //if (!voterId.HasValue)
        //{
        //    return BadRequest();
        //}

        var response = await service.CreateWebPubSubConnectionString(code, voterId, cancellationToken);
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