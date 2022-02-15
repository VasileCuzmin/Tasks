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
    public class ProcessEventDefinitionRepository : EfCrudRepository<ProcessEventDefinition, TasksDefinitionDbContext>, IProcessEventDefinitionRepository
    {
        private readonly TasksDefinitionDbContext _dbContext;

        public ProcessEventDefinitionRepository(TasksDefinitionDbContext dbContext, IExpressionBuilder expressionBuilder, IUow<ProcessEventDefinition> uow,
            ILogger<EfCrudRepository<ProcessEventDefinition, TasksDefinitionDbContext>> logger) : base(dbContext, expressionBuilder, uow, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<EventDefinition>> GetEventDefinitionsByProcessDefinitionIdAsync(int processDefinitionId)
        {
            var eventDefinitions = await _dbContext.ProcessEventDefinitions.Where(a => a.ProcessDefinitionId == processDefinitionId).Select(a => a.EventDefinition).ToListAsync();
            return eventDefinitions;
        }

        public async Task<IEnumerable<ProcessEventDefinition>> GetAsync(string eventName, string applicationName)
        {
            var processEventDefinitions = await _dbContext.ProcessEventDefinitions
                .Include(a => a.ProcessDefinition)
                .Where(a =>
                    a.EventDefinition.Name.ToLower() == eventName.ToLower() &&
                    a.EventDefinition.Application.Name.ToLower() == applicationName.ToLower())
                .ToListAsync();

            return processEventDefinitions;
        }

        public async Task PersistAllAsync(List<ProcessEventDefinition> processEventDefinitions, List<ProcessEventDefinition> deleteProcessEventDefinitions)
        {
            await _dbContext.BulkInsertOrUpdateAsync(processEventDefinitions);
            await _dbContext.BulkDeleteAsync(deleteProcessEventDefinitions);
        }

        public async Task<IEnumerable<ProcessEventDefinition>> GetProcessEventDefinitionsByProcesIdAsync(int processDefinitionId)
        {
            var processEventDefinitions = await _dbContext.ProcessEventDefinitions.Where(a => a.ProcessDefinitionId == processDefinitionId)
                .IncludePaths(new[] { "EventDefinition", "ProcessDefinition" }).ToListAsync();
            return processEventDefinitions;
        }
    }
}
