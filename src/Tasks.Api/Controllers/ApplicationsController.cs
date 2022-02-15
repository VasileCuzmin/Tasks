using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Queries;

namespace Tasks.Api.Controllers
{
    [Route("api/applications")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllApplications.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> Get([FromRoute] GetApplicationById.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplication.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }

        [HttpPut("{applicationId}")]
        public async Task<IActionResult> Update([FromBody] UpdateApplication.Command command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}