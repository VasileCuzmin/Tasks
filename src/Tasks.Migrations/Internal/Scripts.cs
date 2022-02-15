using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace Tasks.Migrations.Internal
{
    public class Scripts
    {
        private const string ManifestResourcePrefix = "Tasks.Migrations.Internal.SqlScripts.";
        private const string ScriptExtension = ".sql";
        private readonly ConcurrentDictionary<string, string> _scripts
            = new ConcurrentDictionary<string, string>();

        public string EnsureMigrationsTable => GetScript(nameof(EnsureMigrationsTable));
        public string GetScriptsVersion => GetScript(nameof(GetScriptsVersion));
        public string UpdateScriptsVersion => GetScript(nameof(UpdateScriptsVersion));

        private string GetScript(string name)
        {
            return _scripts.GetOrAdd(name,
                key =>
                {
                    using var stream = typeof(Scripts).GetTypeInfo().Assembly.GetManifestResourceStream($"{ManifestResourcePrefix}{key}{ScriptExtension}");
                    if (stream == null)
                        throw new FileNotFoundException($"Embedded resource, {name}-{ManifestResourcePrefix}{key}{ScriptExtension}, not found.");

                    using var reader = new StreamReader(stream);
                    return reader
                        .ReadToEnd();
                });
        }
    }
}