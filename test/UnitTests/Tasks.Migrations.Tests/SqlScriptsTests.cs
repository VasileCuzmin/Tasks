using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Tasks.Migrations.Tests
{
    public class SqlScriptsTests
    {
        private readonly Scripts _sut;

        public SqlScriptsTests()
        {
            _sut = new Scripts();
        }

        [Fact]
        public void Scripts_are_all_embedded()
        {
            //Arrange
            var expected = new List<string>(GetType().GetTypeInfo().Assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith("Tasks.Migrations.Tests.SqlScripts."))
                .Select(x => x.Replace("Tasks.Migrations.Tests.SqlScripts.", string.Empty)));

            //Act
            var scripts = _sut.ToList();
            var result = new List<string>(scripts.Select(x => x.ScriptFileName));

            //Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}