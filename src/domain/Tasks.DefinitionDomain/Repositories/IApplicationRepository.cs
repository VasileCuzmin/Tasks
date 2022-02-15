using System.Threading.Tasks;
using NBB.Data.Abstractions;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Domain.Repositories
{
    public interface IApplicationRepository : ICrudRepository<Application>
    {
        Task<Application> GetByNameAsync(string applicationName);        
    }
}
