using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Votr.Surveys.Abstractions;

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

}
