using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Queries;

namespace Tasks.Api.Controllers
{
    [Route("api/eventDefinitions")]
    [ApiController]
    public class EventDefinitionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventDefinitionsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllEventDefinitions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("event/{eventDefinitionId}")]
        public async Task<IActionResult> Get([FromRoute] GetEventDefinitionById.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("process/{processDefinitionId}")]
        public async Task<IActionResult> Get([FromRoute] GetProcessEventDefinitions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> Get([FromRoute] GetEventDefinitionsByApplicationId.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("schemas")]
        public async Task<IActionResult> Get([FromQuery] GetSchemas.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPut("{eventDefinitionId}")]
        public async Task<IActionResult> Update([FromBody] UpdateEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}