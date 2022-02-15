using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tasks.Migrations
{
    public class Scripts : IEnumerable<Script>
    {
        private readonly ConcurrentDictionary<string, string> _scripts = new ConcurrentDictionary<string, string>();
        private readonly int _minimumScriptVersion;
        private const string ManifestResourcePrefix = "Tasks.Migrations.SqlScripts.";
        private const string ScriptExtension = ".sql";

        private IEnumerable<Script> All
        {
            get
            {
                var all = typeof(Scripts).GetTypeInfo().Assembly.GetManifestResourceNames();
                var scripts = all
                    .Where(x => x.StartsWith(ManifestResourcePrefix) && x.EndsWith(ScriptExtension))
                    .Select(x => x.Replace(ManifestResourcePrefix, string.Empty).Replace(ScriptExtension, string.Empty))
                    .Where(x => x.Split(".").Length > 1)
                    .Select(x => new Script(x, x.Split(".")[0]))
                    .Where(x => x.ScriptNumber >= _minimumScriptVersion)
                    .OrderBy(x => x.ScriptNumber);

                return scripts;
            }
        }

        public Scripts(int minimumScriptVersion = 0)
        {
            this._minimumScriptVersion = minimumScriptVersion;
        }

        public string GetScriptContent(string name)
        {
            return _scripts.GetOrAdd(name,
                key =>
                {
                    var manifestResourceNames = typeof(Scripts).GetTypeInfo().Assembly.GetManifestResourceNames()
                        .First(x => x == $"{ManifestResourcePrefix}{key}{ScriptExtension}");

                    using var stream = typeof(Scripts).GetTypeInfo().Assembly.GetManifestResourceStream(manifestResourceNames);
                    if (stream == null)
                        throw new FileNotFoundException(name);

                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                });
        }

        public IEnumerator<Script> GetEnumerator()
        {
            return All.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return All.GetEnumerator();
        }
    }

    public class Script
    {
        public string ScriptFileName { get; }
        public int ScriptNumber { get; }

        public Script(string scriptFileName, string scriptNumber)
        {
            if (!int.TryParse(scriptNumber, out var value))
                throw new ArgumentException($"Script name ${scriptFileName} is invalid");

            ScriptNumber = value;
            ScriptFileName = scriptFileName;
        }
    }
}
