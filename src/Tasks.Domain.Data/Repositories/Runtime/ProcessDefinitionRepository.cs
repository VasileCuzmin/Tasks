using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Domain.Data.Repositories.Runtime
{
    public class ProcessDefinitionRepository : Tasks.Runtime.Domain.ProcessDefinitionAggregate.IProcessDefinitionRepository
    {
        private readonly ITaskDefinitionRepository _taskDefinitionRepository;

        public ProcessDefinitionRepository(ITaskDefinitionRepository taskDefinitionRepository)
        {
            _taskDefinitionRepository = taskDefinitionRepository;
        }

        public async Task<ProcessDefinition> GetById(ProcessDefinitionId processDefinitionId, CancellationToken cancellationToken)
        {
            //This should be outside.Don't call repo inside another repo
            var taskDefinitions = await _taskDefinitionRepository.GetByProcessId(processDefinitionId.Value);
            var runtimeTaskDefinitions = new HashSet<TaskDefinition>(taskDefinitions.Select(x =>
                new TaskDefinition(
                    x.Name,
                    new EventDefinitionName(x.StartEventDefinition?.Name),
                    new EventDefinitionName(x.EndEventDefinition?.Name),
                    new EventDefinitionName(x.CancelEventDefinition?.Name),
                    x.StartExpression == null ? null : new DynamicExpression(x.StartExpression),
                    x.EndExpression == null ? null : new DynamicExpression(x.EndExpression),
                    x.CancelExpression == null ? null : new DynamicExpression(x.CancelExpression),
                    x.UserAllocationExpression == null ? null : new DynamicExpression(x.UserAllocationExpression),
                    x.GroupAllocationExpression == null ? null : new DynamicExpression(x.GroupAllocationExpression),
                    !x.AutomaticStart.HasValue ? null : new AutomaticStart(x.AutomaticStart.Value)
                )));

            return new ProcessDefinition(processDefinitionId, runtimeTaskDefinitions);
        }
    }
}