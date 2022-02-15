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
    public class ProcessDefinitionRepository : EfCrudRepository<ProcessDefinition, TasksDefinitionDbContext>, IProcessDefinitionRepository
    {
        private readonly TasksDefinitionDbContext _dbContext;

        public ProcessDefinitionRepository(TasksDefinitionDbContext dbContext, IExpressionBuilder expressionBuilder, IUow<ProcessDefinition> uow,
            ILogger<EfCrudRepository<ProcessDefinition, TasksDefinitionDbContext>> logger) : base(dbContext, expressionBuilder, uow, logger)
        {
            _dbContext = dbContext;
        }

        public void AddOrUpdate(ProcessDefinition processDefinition)
        {
            var entry = _dbContext.Entry(processDefinition);
            if (entry != null) entry.State = EntityState.Detached;

            _dbContext.Update(processDefinition);
        }

        public async Task<ProcessDefinition> GetAsync(int applicationId, string processDefinitionName)
        {
            var processDefinition = await _dbContext.ProcessDefinitions.SingleOrDefaultAsync(a =>
                a.ApplicationId == applicationId && a.Name.ToLower() == processDefinitionName.ToLower());

            return processDefinition;
        }

        public async Task UpdateEnabled(int id, bool enabled)
        {
            var entity = await GetByIdAsync(id, default);
            entity.Enabled = enabled;
            _dbContext.Update(entity);
        }

        public async Task<List<string>> GetAllTopicsForEnabledProcesses()
        {
            var taskDefinitions = _dbContext.ProcessDefinitions
                .Where(x => x.Enabled)
                .SelectMany(x => x.TaskDefinitions);

            var startEventTopics = await taskDefinitions.Select(x => x.StartEventDefinition.Topic).ToListAsync();
            var closeEventTopics = await taskDefinitions.Select(x => x.EndEventDefinition.Topic).ToListAsync();
            var cancelEventTopics = await taskDefinitions.Where(x => x.CancelEventDefinitionId != null).Select(x => x.CancelEventDefinition.Topic).ToListAsync();

            var topics = startEventTopics.Union(closeEventTopics).Union(cancelEventTopics).Where(x => x != null).Distinct().ToList();
            return topics;
        }

        public async Task<IEnumerable<ProcessDefinition>> GetAllAsync(int applicationId)
        {
            var processDefinitions = await _dbContext.ProcessDefinitions.Where(x => x.ApplicationId == applicationId)
                .IncludePaths(new[] { "Application" }).ToListAsync();
            return processDefinitions;
        }
    }
}