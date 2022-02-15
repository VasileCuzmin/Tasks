using System.Threading.Tasks;

namespace Tasks.Migrations
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var migrator = new DatabaseMigrator();
            await migrator.MigrateToLatestVersion();
        }
    }
}
