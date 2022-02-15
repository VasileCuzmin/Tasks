using FluentAssertions;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Xunit;

namespace Tasks.RuntimeDomain.Tests.EventDefinitionAggregate.EventDefinition_Test
{
    public class IdentifierPropsMapTests
    {
        [Fact]
        public void IdentifierPropsMap_IfAPropertyIsNotInEventPropsAtAIndex_TakeTheElementFromProcessPropsAtTheSameIndex()
        {
            //Arrange
            var processProps = "DocumentId;SiteId";
            var eventProps = "DocumentId;";
            var givenMap = IdentifierPropsMap.From("DocumentId;SiteId", "DocumentId;SiteId");

            //Act
            var map = IdentifierPropsMap.From(processProps, eventProps);

            //Assert
            map.Should().Be(givenMap);
        }
    }
}