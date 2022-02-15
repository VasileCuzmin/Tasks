using System.Reflection;
using Xunit.Abstractions;

namespace Tasks.Application.IntegrationTests
{
    public static class XUnitExtensions
    {
        public static string GetTestName(this ITestOutputHelper testOutputHelper)
        {
            var type = testOutputHelper.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(testOutputHelper);

            return test.DisplayName;
        }

        public static string Update(this string val)
        {
             return $"{val}_updated";
        }
        
    }

}
