using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Application.Commands;
using Tasks.Runtime.Domain.TaskAllocationAggregate;

namespace Tasks.Runtime.Application.CommandHandlers
{
    public class PauseTaskCommandHandler : IRequestHandler<PauseTask>
    {
        private readonly IEventSourcedRepository<TaskAllocation> _taskAllocationRepository;

        public PauseTaskCommandHandler(IEventSourcedRepository<TaskAllocation> taskAllocationRepository)
        {
            _taskAllocationRepository = taskAllocationRepository;
        }

        public async Task<Unit> Handle(PauseTask request, CancellationToken cancellationToken)
        {
            var task = await _taskAllocationRepository.GetByIdAsync(request.TaskId, cancellationToken);
            task.Pause(request.ProcessId);
            await _taskAllocationRepository.SaveAsync(task, cancellationToken);

            return Unit.Value;
        }
    }
}