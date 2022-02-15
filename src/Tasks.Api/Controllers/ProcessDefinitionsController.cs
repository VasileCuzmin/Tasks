using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.Api.Services;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Queries;

namespace Tasks.Api.Controllers
{
    [Route("api/processDefinitions")]
    [ApiController]
    public class ProcessDefinitionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEventHub _eventHub;

        public ProcessDefinitionsController(IMediator mediator, IEventHub eventHub)
        {
            _mediator = mediator;
            _eventHub = eventHub;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProcessDefinitions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{processDefinitionId}")]
        public async Task<IActionResult> Get([FromRoute] GetProcessDefinitionById.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> Get([FromRoute] GetApplicationProcessDefinitions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{processDefinitionId}/eventDefinitions")]
        public async Task<IActionResult> Get([FromRoute] GetProcessEventDefinitionsByProcessDefinitionId.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{processDefinitionId}/processEventDefinitions/{eventDefinitionId}")]
        public async Task<IActionResult> Get([FromRoute] GetProcessEventDefinitionsById.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("{processDefinitionId}/taskDefinitions/{taskDefinitionId}")]
        public async Task<IActionResult> Get([FromRoute] GetTaskDefinitionsById.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }


        [HttpPost("{processDefinitionId}/processEventDefinitions/")]
        public async Task<IActionResult> AddEventDefinition([FromBody] AddProcessEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPatch("{processDefinitionId}/disable")]
        public async Task<IActionResult> Disable([FromBody] DisableProcessDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPatch("{processDefinitionId}/enable")]
        public async Task<IActionResult> Enable([FromBody] EnableProcessDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPut("{processDefinitionId}/processEventDefinitions/{eventDefinitionId}")]
        public async Task<IActionResult> Update([FromBody] UpdateProcessEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProcessDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(SyncCommandResult.From(command, _eventHub.GetEvents()));
        }

        [HttpPut("{processDefinitionId}/taskDefinitions")]
        public async Task<IActionResult> Update([FromBody] UpdateTaskDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPut("{processDefinitionId}/eventDefinitions")]
        public async Task<IActionResult> Persist([FromBody] PersistProcessEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpDelete("{processDefinitionId}/taskDefinitions/{taskDefinitionId}")]
        public async Task<IActionResult> Remove([FromBody] RemoveTaskDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpDelete("{processDefinitionId}/processEventDefinitions/{eventDefinitionId}")]
        public async Task<IActionResult> Remove([FromBody] RemoveProcessEventDefinition.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}