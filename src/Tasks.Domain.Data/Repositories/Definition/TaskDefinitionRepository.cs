using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NBB.Core.Abstractions;
using NBB.Data.EntityFramework;
using NBB.Data.EntityFramework.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Domain.Data.Repositories.Definition
{
    public class TaskDefinitionRepository : EfCrudRepository<TaskDefinition, TasksDefinitionDbContext>, ITaskDefinitionRepository
    {
        private readonly TasksDefinitionDbContext _dbContext;

        public TaskDefinitionRepository(TasksDefinitionDbContext dbContext, IExpressionBuilder expressionBuilder, IUow<TaskDefinition> uow,
            ILogger<EfCrudRepository<TaskDefinition, TasksDefinitionDbContext>> logger) : base(dbContext, expressionBuilder, uow, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<TaskDefinition>> GetByProcessId(int processDefinitionId)
        {
            var items = _dbContext.TaskDefinitions
                .Include(x => x.StartEventDefinition)
                .Include(x => x.EndEventDefinition)
                .Include(x => x.CancelEventDefinition)
                .Where(a => a.ProcessDefinitionId == processDefinitionId);
            var result = await items.ToListAsync();
            return result;
        }

        public async Task<bool> Any(int processDefinitionId, int eventDefinitionId)
        {
            var any = await _dbContext.TaskDefinitions.Where(a => a.ProcessDefinitionId == processDefinitionId)
                .AnyAsync(a =>
                    a.StartEventDefinitionId == eventDefinitionId ||
                    a.EndEventDefinitionId == eventDefinitionId ||
                    a.CancelEventDefinitionId == eventDefinitionId);

            return any;
        }

        public async Task<IList<TaskDefinition>> GetByNameAsync(string requestName)
        {
            var items = _dbContext.TaskDefinitions.Where(a => a.Name.ToLower() == requestName.ToLowerInvariant());
            return await items.ToListAsync();
        }

        public async Task PersistAllAsync(List<TaskDefinition> taskDefinitions, List<TaskDefinition> deleteTaskData)
        {
            await _dbContext.BulkInsertOrUpdateAsync(taskDefinitions);
            await _dbContext.BulkDeleteAsync(deleteTaskData);
        }

        public async Task UpdateAll(IList<TaskDefinition> taskDefinitions)
        {
            await _dbContext.BulkInsertOrUpdateAsync(taskDefinitions);
        }
    }
}