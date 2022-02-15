using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.Definition.Application.Queries;
using Tasks.PublishedLanguage.Events.Runtime;
using Tasks.Runtime.Application.Commands;

namespace Tasks.Api.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("process/{processId}")]
        public async Task<IActionResult> Get([FromRoute] GetTaskDefinitionsByProcessId.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("startTask")]
        public async Task<IActionResult> StartTask([FromBody] StartTask command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPost("pauseTask")]
        public async Task<IActionResult> PauseTask([FromBody] PauseTask command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPost("updateUserTaskAllocation")]
        public async Task<IActionResult> UpdateUserTaskAllocation([FromBody] UpdateUserTaskAllocation command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}