using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents;
using Tasks.Runtime.Domain.Services;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.ProcessObserver_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.ProcessObserver_Test
{
    public class ObserveEvent_tests
    {
        private readonly Mock<IExpressionEvaluationService> _evaluationServiceMock;

        public ObserveEvent_tests()
        {
            _evaluationServiceMock = new Mock<IExpressionEvaluationService>();
        }

        [Fact]
        public void ObserveEvent_with_null_event_throws_exception()
        {
            // Arrange
            var sut = GenerateProcessObserver();

            // Act
            var observeEvent = new Action(() => sut.ObserveEvent(null, null));

            // Assert
            Assert.Throws<ArgumentNullException>(observeEvent).ParamName.Should().Be("event");
        }

        [Fact]
        public void ObserveEvent_with_null_expression_evaluator_service_throws_exception()
        {
            // Arrange
            var sut = GenerateProcessObserver();
            var eventDefinitionId = new EventDefinitionName(string.Empty);
            var @event = new DynamicEvent(eventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);

            // Act
            var observeEvent = new Action(() => sut.ObserveEvent(@event, null));

            // Assert
            Assert.Throws<ArgumentNullException>(observeEvent).ParamName.Should().Be("expressionEvaluationService");
        }

        [Fact]
        public void ObserveEvent_emits_ProcessEventReceived_event()
        {
            // Arrange
            var sut = GenerateProcessObserver();
            var eventDefinitionId = new EventDefinitionName("I am a bad Event!");
            var payload = new Dictionary<string, object> { ["badProparty"] = "bad Value" }.ToImmutableDictionary();
            var @event = new DynamicEvent(eventDefinitionId, payload, Guid.Empty, string.Empty);
            sut.MarkChangesAsCommitted();

            // Act
            sut.ObserveEvent(@event, _evaluationServiceMock.Object);

            // Assert
            var firedEvents = sut.GetUncommittedChanges().ToList();
            firedEvents.Should().HaveCount(1);
            firedEvents.Should().AllBeOfType<ProcessEventReceived>();
        }

        [Fact]
        public void ObserveEvent_ExpressionIsEvaluatedToFalse_DoesnotEmitProcessEvents()
        {
            // Arrange
            var goodEventDefinition = new EventDefinitionName("GenericEvent");
            var badEventDefinition = new EventDefinitionName(string.Empty);
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", goodEventDefinition, badEventDefinition, badEventDefinition, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", goodEventDefinition, badEventDefinition, badEventDefinition, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", goodEventDefinition, badEventDefinition, badEventDefinition, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", badEventDefinition, badEventDefinition, badEventDefinition, null, null, null,null,null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();

            var goodEvent = new DynamicEvent(goodEventDefinition, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);

            // Act
            sut.ObserveEvent(goodEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().BeEmpty();
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(goodEvent, null), Times.Exactly(3));
        }

        [Fact]
        public void ObserveEvent_DuplicatedEvents_IsIgnored()
        {
            // Arrange
            var sut = GenerateProcessObserver();
            var eventDefinitionId = new EventDefinitionName("GenericEvent");
            var @event = new DynamicEvent(eventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            sut.MarkChangesAsCommitted();

            // Act
            sut.ObserveEvent(@event, _evaluationServiceMock.Object);
            sut.ObserveEvent(@event, _evaluationServiceMock.Object);

            // Assert
            var firedEvents = sut.GetUncommittedChanges().ToList();
            firedEvents.Should().HaveCount(1);
            firedEvents.Should().AllBeOfType<ProcessEventReceived>();
        }

        [Fact]
        public void ObserveEvent_ExpressionIsEvaluatedToTrue_And_StartEventIsEqualWithEventDefinitionName_DoesEmitTaskInitiatedEvent()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", good, null, null, null, null, null,null,null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(goodEvent, null)).Returns(true);

            // Act
            sut.ObserveEvent(goodEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().HaveCount(1);
            firedEvent.Should().AllBeOfType<TaskInitiated>();
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(goodEvent, null), Times.Exactly(1));
        }
        [Fact]
        public void ObserveEvent_ExpressionIsEvaluatedToTrue_And_StartEventIsNotEqualWithEventDefinitionName_DoesEmitTaskInitiatedEvent()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", start, null, null, null, null, null,null,null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(badEvent, null)).Returns(true);

            // Act
            sut.ObserveEvent(badEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().HaveCount(0);
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(badEvent, null), Times.Never);
        }

        [Fact]
        public void ObserveEvent_does_emit_process_started_events_for_each_definition_that_starts_with_the_correct_event_when_expresion_evaluates_to_true()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", good, bad, bad, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", good, bad, bad, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", good, bad, bad, null, null, null,null,null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", bad, bad, bad, null, null, null,null,null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(goodEvent, null)).Returns(true);

            // Act
            sut.ObserveEvent(goodEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().HaveCount(3);
            firedEvent.Should().AllBeOfType<TaskInitiated>();
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(goodEvent, null), Times.Exactly(3));
        }

        [Fact]
        public void ObserveEvent_doesnot_change_state_for_started_tasks_when_expression_evaluates_to_false()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", start, good, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", start, good, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", start, bad, good, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);

            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(startEvent, null)).Returns(true);

            // Act
            sut.ObserveEvent(startEvent, _evaluationServiceMock.Object);
            sut.MarkChangesAsCommitted();
            sut.ObserveEvent(goodEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().BeEmpty();
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(goodEvent, null), Times.Exactly(6));
        }

        [Fact]
        public void ObserveEvent_does_change_state_for_started_tasks_when_expression_evaluates_to_true()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", start, good, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", start, good, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", start, bad, good, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", bad, bad, good, null, null, null, null, null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(startEvent, null)).Returns(true);
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(goodEvent, null)).Returns(true);

            // Act
            sut.ObserveEvent(startEvent, _evaluationServiceMock.Object);
            sut.MarkChangesAsCommitted();
            sut.ObserveEvent(goodEvent, _evaluationServiceMock.Object);

            // Assert
            var firedEvent = sut.GetUncommittedChanges().Skip(1).ToList();
            firedEvent.Should().HaveCount(3);
            firedEvent[0].Should().BeOfType<TaskFinished>();
            firedEvent[1].Should().BeOfType<TaskFinished>();
            firedEvent[2].Should().BeOfType<TaskCancelled>();
            _evaluationServiceMock.Verify(e => e.EvaluateEventExpression(goodEvent, null), Times.Exactly(6));
        }

        [Fact]
        public void ObserveEvent_starts_multiple_events_with_same_type_of_event_received()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", good, bad, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", good, bad, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", good, bad, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", bad, bad, bad, null, null, null, null, null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            var firstEvent = new DynamicEvent(good, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            var secondEvent = new DynamicEvent(good, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(It.IsAny<DynamicEvent>(), null)).Returns(true);

            // Act
            sut.ObserveEvent(firstEvent, _evaluationServiceMock.Object);
            sut.ObserveEvent(secondEvent, _evaluationServiceMock.Object);

            // Assert
            var taskInitiatedEvents = sut.GetUncommittedChanges().Where(e => e is TaskInitiated).ToList();
            var processEventReceivedEvents = sut.GetUncommittedChanges().Where(e => e is ProcessEventReceived).ToList();
            var otherEvents = sut.GetUncommittedChanges().Except(taskInitiatedEvents).Except(processEventReceivedEvents);

            taskInitiatedEvents.Should().HaveCount(6);
            processEventReceivedEvents.Should().HaveCount(2);
            otherEvents.Should().BeEmpty();
        }

        [Fact]
        public void ObserveEvent_EventsWithCloseEventInDefinitions_CloseAllTasks()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", start, stop, bad, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", start, stop, stop, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", start, bad, stop, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition004", bad, bad, bad, null, null, null, null, null,new AutomaticStart(true))
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            var firstEvent = new DynamicEvent(start, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            var secondEvent = new DynamicEvent(start, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(It.IsAny<DynamicEvent>(), null)).Returns(true);
            var stopEvent = new DynamicEvent(stop, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);

            // Act
            sut.ObserveEvent(firstEvent, _evaluationServiceMock.Object);
            sut.MarkChangesAsCommitted();

            sut.ObserveEvent(stopEvent, _evaluationServiceMock.Object);

            // Assert
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskFinished).Should().HaveCount(2);
        }

        [Fact]
        public void ObserveEvent_EventsWithCancelEventInDefinitions_CancelAllTasks()
        {
            // Arrange
            var definitions = new[]
            {
                new TaskDefinition("TaskDefinition001", start, stop, cancel, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition002", start, cancel, stop, null, null, null, null, null,new AutomaticStart(true)),
                new TaskDefinition("TaskDefinition003", start, cancel, stop, null, null, null, null, null,new AutomaticStart(true)),
            };
            var sut = GenerateProcessObserver(definitions);
            sut.MarkChangesAsCommitted();
            var firstEvent = new DynamicEvent(start, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            _evaluationServiceMock.Setup(e => e.EvaluateEventExpression(It.IsAny<DynamicEvent>(), null)).Returns(true);
            //var cancelEvent = new DynamicEvent(cancel, string.Empty, Guid.NewGuid(), string.Empty);

            // Act
            sut.ObserveEvent(firstEvent, _evaluationServiceMock.Object);
            sut.MarkChangesAsCommitted();

            sut.ObserveEvent(cancelEvent, _evaluationServiceMock.Object);

            // Assert
            var emittedEvents = sut.GetUncommittedChanges().ToList();
            emittedEvents.Where(e => e is TaskFinished).Should().HaveCount(2);
        }
    }
}
