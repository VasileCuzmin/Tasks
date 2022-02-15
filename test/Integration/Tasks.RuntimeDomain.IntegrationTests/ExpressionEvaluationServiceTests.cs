using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NBB.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.Services;
using Xunit;

namespace Tasks.RuntimeDomain.IntegrationTests
{
    public class ExpressionEvaluationServiceTests : IClassFixture<Fixture>, IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly IExpressionEvaluationService _sut;
        private readonly IMessageSerDes _messageSerDes;

        public ExpressionEvaluationServiceTests(Fixture fixture)
        {
            _scope = fixture.Container.CreateScope();
            _sut = _scope.ServiceProvider.GetService<IExpressionEvaluationService>();
            _messageSerDes = _scope.ServiceProvider.GetService<IMessageSerDes>();
        }

        [Fact]
        public void EvaluateEventExpression_should_work_with_messaging_serializer_and_expresso_evaluator()
        {
            //Arrange
            const string stateName = "Prescoring";
            var businessEvent = new OfferStateUpdated(1, 1, 2, stateName);
            var envelope = new MessagingEnvelope<OfferStateUpdated>(new Dictionary<string, string>(), businessEvent);
            var envelopeJson = _messageSerDes.SerializeMessageEnvelope(envelope);
            var deserEnvelope = _messageSerDes.DeserializeMessageEnvelope(envelopeJson);
            var payload = (deserEnvelope.Payload as ExpandoObject).ToImmutableDictionary();
            var domainEvent = new Runtime.Domain.ProcessObserverAggregate.DynamicEvent(new EventDefinitionName("OfferStateUpdated"), payload, Guid.NewGuid(), "lsng");

            //Act
            var actual = _sut.EvaluateEventExpression(domainEvent, new DynamicExpression($"StateName==\"{stateName}\""));

            //Assert
            actual.Should().BeTrue();
        }

        public void Dispose()
        {
            _scope.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public record OfferStateUpdated(int DocumentId, int SiteId, int StateId, string StateName) : INotification;
}
