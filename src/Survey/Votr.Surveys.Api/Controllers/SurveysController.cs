using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

}