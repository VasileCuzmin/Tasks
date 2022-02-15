using MediatR;
using Microsoft.AspNetCore.Mvc;
using NBB.Correlation;
using System.Threading.Tasks;
using Tasks.Api.Models;
using Tasks.PublishedLanguage.Commands;

namespace Tasks.Api.Controllers
{
    [Route("api/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OperationsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("shutdown")]
        public async Task<IActionResult> Create([FromBody] Shutdown command)
        {
            await _mediator.Send(command);
            return Ok(new AsyncCommandResult(CorrelationManager.GetCorrelationId()));
        }
    }
}