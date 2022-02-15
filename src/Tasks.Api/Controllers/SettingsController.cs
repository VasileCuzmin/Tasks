using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Queries;
using Tasks.PublishedLanguage.Commands;

namespace Tasks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves info about API (version number and build date)
        /// </summary>
        [HttpGet("api-version")]
        public async Task<IActionResult> GetApiVersion([FromQuery] GetApiVersion.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("publishlanguage")]
        public async Task<IActionResult> PublishLanguage([FromBody] PublishLanguage command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPost("restartTasksWorker")]
        public async Task<IActionResult> RestartTasksWorker([FromBody] RestartTasksWorker.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}