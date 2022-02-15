using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NBB.Core.Abstractions;
using NBB.Data.EntityFramework;
using NBB.Data.EntityFramework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Domain.Data.Repositories.Definition
{
    public class EventDefinitionRepository : EfCrudRepository<EventDefinition, TasksDefinitionDbContext>, IEventDefinitionRepository
    {
        private readonly TasksDefinitionDbContext _dbContext;

        public EventDefinitionRepository(TasksDefinitionDbContext dbContext, IExpressionBuilder expressionBuilder, IUow<EventDefinition> uow,
            ILogger<EfCrudRepository<EventDefinition, TasksDefinitionDbContext>> logger) : base(dbContext, expressionBuilder, uow, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<EventDefinition> GetAsync(int applicationId, string eventDefinitionName)
        {
            var eventDefinition = await _dbContext.EventDefinitions.SingleOrDefaultAsync(a =>
                a.ApplicationId == applicationId && a.Name.ToLower() == eventDefinitionName.ToLower());

            return eventDefinition;
        }

        public async Task<List<string>> GetAllTopicsAsync()
        {
            var result = await _dbContext.EventDefinitions.Select(a => a.Topic).Distinct().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<EventDefinition>> GetAllByApplicationIdAsync(int applicationId)
        {
            var eventDefinitions = await _dbContext.EventDefinitions.Where(e => e.ApplicationId == applicationId).ToListAsync();
            return eventDefinitions;
        }

        public async Task<IEnumerable<(string , string )>> GetSchemasAsync(int applicationId, params int[] ids)
        {
            var eventDefinition = await _dbContext.EventDefinitions.Where(e => e.ApplicationId == applicationId && ids.Contains(e.EventDefinitionId)).ToListAsync();
            return eventDefinition.Select(e => (e.Schema, e.Name));
        }

        public async Task<IEnumerable<ProcessEventDefinition>> GetAllByProcessIdAsync(int processDefinitionId)
        {
            var eventDefinitionsByProcessId = await _dbContext.ProcessEventDefinitions.Where(e => e.ProcessDefinitionId == processDefinitionId)
                .IncludePaths(new[] { "EventDefinition", "ProcessDefinition" })
                .ToListAsync();
            return eventDefinitionsByProcessId;
        }

    }
}