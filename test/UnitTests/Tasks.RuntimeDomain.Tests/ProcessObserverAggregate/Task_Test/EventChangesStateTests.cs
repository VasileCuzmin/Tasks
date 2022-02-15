using FluentAssertions;
using Moq;
using System;
using System.Collections.Immutable;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.Services;
using Xunit;
using static Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test
{
    public class EventChangesStateTests
    {
        private readonly Mock<IExpressionEvaluationService> _expressionEvaluationServiceMock;
        public EventChangesStateTests()
        {
            _expressionEvaluationServiceMock = new Mock<IExpressionEvaluationService>();
        }

        [Fact]
        public void EventChangesState_EventIsNull_ThrowArgumentNullException()
        {
            var sut = CreateTaskWithExpressions();
            Assert.Throws<ArgumentNullException>(() => sut.EventChangesState(null, _expressionEvaluationServiceMock.Object)).ParamName.Should().Be("event");
        }

        [Fact]
        public void EventChangesState_ExpressionEvaluationServiceIsNull_ThrowArgumentNullException()
        {
            var sut = CreateTaskWithExpressions();
            var closeEventDefinitionId = sut.TaskDefinition.CloseEvent;
            var closeEvent = new DynamicEvent(closeEventDefinitionId, ImmutableDictionary.Create<string, object>(), Guid.Empty, string.Empty);
            Assert.Throws<ArgumentNullException>(() => sut.EventChangesState(closeEvent, null)).ParamName.Should().Be("expressionEvaluationService");
        }
    }
}