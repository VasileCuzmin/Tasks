using Microsoft.Extensions.Configuration;
using Subtext.Scripting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Tasks.Migrations
{
    public class DatabaseMigrator
    {
        private readonly string _connectionString;
        private Scripts _scripts;
        private readonly Internal.Scripts _internalScripts;
        private readonly IConfigurationRoot _configuration;

        public DatabaseMigrator(string connectionStringConfig = "Tasks_Database")
        {
            _configuration = GetConfigurationRoot();
            _connectionString = _configuration.GetConnectionString(connectionStringConfig);
            _scripts = new Scripts();
            _internalScripts = new Internal.Scripts();
        }

        private static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public async Task MigrateToLatestVersion()
        {
            Console.WriteLine("Migrating Database");

            using (var cnx = new SqlConnection(_connectionString))
            {
                await cnx.OpenAsync();

                await Initialize(cnx);
                await ExecuteScripts(cnx);
            }

            Console.WriteLine("Database Migration completed");
        }

        private async Task Initialize(SqlConnection cnx)
        {
            await EnsureVersionTable(cnx);
            var scriptsVersion = await GetScriptsVersion(cnx);
            Console.WriteLine($"Current scripts version: {scriptsVersion}");
            _scripts = new Scripts(scriptsVersion + 1);
        }

        private async Task ExecuteScripts(SqlConnection cnx)
        {
            Console.WriteLine("Migrating database");
            foreach (var script in _scripts)
            {
                Console.WriteLine($" * {script.ScriptFileName}");
                await ExecuteScript(cnx, script);
            }
        }

        private Task ExecuteScript(SqlConnection cnx, Script script)
        {
            var scriptContent = _scripts.GetScriptContent(script.ScriptFileName);
            var updateVersionScriptContent = _internalScripts.UpdateScriptsVersion.Replace("@NewScriptsVersion", $"{script.ScriptNumber}");

            return ExecuteNonQuery(cnx, $"{scriptContent}{Environment.NewLine}{updateVersionScriptContent}");
        }

        private Task EnsureVersionTable(SqlConnection cnx)
        {
            var sql = _internalScripts.EnsureMigrationsTable;

            return ExecuteNonQuery(cnx, sql);
        }

        private async Task<int> GetScriptsVersion(SqlConnection cnx)
        {
            var sql = _internalScripts.GetScriptsVersion;

            await using var command = new SqlCommand(sql, cnx);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private static Task ExecuteNonQuery(SqlConnection cnx, string sql)
        {
            var runner = new SqlScriptRunner(sql);
            using (var transaction = cnx.BeginTransaction())
            {
                runner.Execute(transaction);
                transaction.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
