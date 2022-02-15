using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.PublishedLanguage.Events.Runtime;
using Tasks.Runtime.Domain.TaskAllocationAggregate;

namespace Tasks.Runtime.Application.CommandHandlers
{
    public class UpdateUserTaskAllocationCommandHandler : IRequestHandler<UpdateUserTaskAllocation>
    {
        private readonly IEventSourcedRepository<TaskAllocation> _taskAllocationRepository;

        public UpdateUserTaskAllocationCommandHandler(IEventSourcedRepository<TaskAllocation> taskAllocationRepository)
        {
            _taskAllocationRepository = taskAllocationRepository;
        }

        public async Task<Unit> Handle(UpdateUserTaskAllocation request, CancellationToken cancellationToken)
        {
            var task = await _taskAllocationRepository.GetByIdAsync(request.TaskId, cancellationToken);
            task.AllocateUser(request.ProcessId, request.UserId);
            await _taskAllocationRepository.SaveAsync(task, cancellationToken);

            return Unit.Value;
        }
    }
}