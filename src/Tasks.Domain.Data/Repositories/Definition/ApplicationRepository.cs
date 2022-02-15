using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NBB.Core.Abstractions;
using NBB.Data.EntityFramework;
using NBB.Data.EntityFramework.Internal;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Domain.Data.Repositories.Definition
{
    public class ApplicationRepository : EfCrudRepository<Application, TasksDefinitionDbContext>, IApplicationRepository
    {
        private readonly TasksDefinitionDbContext _dbContext;

        public ApplicationRepository(TasksDefinitionDbContext dbContext, IExpressionBuilder expressionBuilder, IUow<Application> uow, 
            ILogger<EfCrudRepository<Application, TasksDefinitionDbContext>> logger) : base(dbContext, expressionBuilder, uow, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<Application> GetByNameAsync(string applicationName)
        {
            var app = await _dbContext.Applications.SingleOrDefaultAsync(a => a.Name.ToLower() == applicationName.ToLower());
            return app;
        }
    }
}