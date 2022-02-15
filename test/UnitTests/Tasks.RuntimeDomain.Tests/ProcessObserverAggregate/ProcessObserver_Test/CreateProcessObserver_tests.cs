using FluentAssertions;
using System;
using System.Linq;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.ProcessObserver_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.ProcessObserver_Test
{
    public class CreateProcessObserver_tests
    {
        [Fact]
        public void Creating_process_observer_with_null_identity_throws_exception()
        {
            // Arrange
            var createNewProcessObserver = new Action(() => ProcessObserver.New(null, null));

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(createNewProcessObserver).ParamName.Should().Be("processObserverId");
        }

        [Fact]
        public void Creating_process_observer_with_null_task_definition_throws_exception()
        {
            // Arrange
            var processDefinitionId = new ProcessDefinitionId(default);
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var processObserverId = new ProcessObserverId(processDefinitionId, processId);
            var createNewProcessObserver = new Action(() => ProcessObserver.New(processObserverId, null));

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(createNewProcessObserver).ParamName.Should().Be("taskDefinitions");
        }

        [Fact]
        public void Creating_process_observer_emits_process_started_event()
        {
            // Arrange

            // Act
            var sut = GenerateProcessObserver();
            var changes = sut.GetUncommittedChanges().ToList();

            // Assert
            changes.Should().HaveCount(1);
            changes.Should().AllBeOfType<ProcessStarted>();
        }
    }
}