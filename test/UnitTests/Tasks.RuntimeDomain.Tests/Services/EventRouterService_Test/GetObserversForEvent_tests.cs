using Moq;
using NBB.Data.Abstractions;
using System;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents;
using Tasks.Runtime.Domain.Services;
using Xunit;
using static Tasks.RuntimeDomain.Tests.Services.EventRouterService_Test.Setup;

namespace Tasks.RuntimeDomain.Tests.Services.EventRouterService_Test
{
    public class GetObserversForEvent_tests
    {
        public readonly Mock<IEventDefinitionRepository> _eventDefinitionRepositoryMock;
        public readonly Mock<IProcessDefinitionRepository> _processDefinitionRepositoryMock;
        public readonly Mock<IEventSourcedRepository<ProcessObserver>> _processObserverRepositoryMock;
        public readonly EventRouterService sut;

        public GetObserversForEvent_tests()
        {
            _eventDefinitionRepositoryMock = new Mock<IEventDefinitionRepository>();
            _processDefinitionRepositoryMock = new Mock<IProcessDefinitionRepository>();
            _processObserverRepositoryMock = new Mock<IEventSourcedRepository<ProcessObserver>>();

            sut = new EventRouterService(
               _eventDefinitionRepositoryMock.Object,
              _processObserverRepositoryMock.Object,
              _processDefinitionRepositoryMock.Object);
        }

        [Fact]
        public async void GetObserversForEvent_IfProcessObserverDoesNotExists_ThenCreateOne()
        {
            //Arrange
            var offerCreated = new EventDefinitionName("OfferCreated");
            var OfferCreatedEvent = new DynamicEvent(offerCreated, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
            _eventDefinitionRepositoryMock.Setup(e => e.GetByNameAndApplication(OfferCreatedEvent.EventDefinitionName, OfferCreatedEvent.ApplicationName, CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new EventDefinition(OfferCreatedEvent.EventDefinitionName as EventDefinitionName, dict)));

            _processDefinitionRepositoryMock.Setup(e => e.GetById(It.IsAny<ProcessDefinitionId>(), CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new ProcessDefinition(It.IsAny<ProcessDefinitionId>(), runtimeTaskDefinitions)));

            //Act
            var observers = await sut.GetObserversForEvent(OfferCreatedEvent, CancellationToken.None);

            //Assert
            Assert.Single(observers);
            var firedEvent = observers[0].GetUncommittedChanges().ToList();
            Assert.True(firedEvent.Where(e => e is ProcessStarted).Count() == 1);
        }

        [Fact]
        public async void GetObserversForEvent_IfProcessObserverDoesExists_ThenProcessObserverIsLoaded()
        {
            //Arrange
            var offerCreated = new EventDefinitionName("OfferCreated");
            var OfferCreatedEvent = new DynamicEvent(offerCreated, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);

            _eventDefinitionRepositoryMock.Setup(e => e.GetByNameAndApplication(OfferCreatedEvent.EventDefinitionName, OfferCreatedEvent.ApplicationName, CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new EventDefinition(OfferCreatedEvent.EventDefinitionName as EventDefinitionName, dict)));

            _processObserverRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<ProcessObserverId>(), CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new ProcessObserver()));

            _processDefinitionRepositoryMock.Setup(e => e.GetById(It.IsAny<ProcessDefinitionId>(), CancellationToken.None))
                           .Returns(System.Threading.Tasks.Task.FromResult(new ProcessDefinition(It.IsAny<ProcessDefinitionId>(), runtimeTaskDefinitions)));
            //Act
            var observers = await sut.GetObserversForEvent(OfferCreatedEvent, CancellationToken.None);

            //Assert
            Assert.Single(observers);
            var firedEvent = observers[0].GetUncommittedChanges().ToList();
            Assert.True(firedEvent.Where(e => e is ProcessStarted).Count() == 0);
        }

        [Fact]
        public void GetObserversForEvent_IfEventJsonDoesntContainMapProperties_ThrowsArgumentOtOfRangeException()
        {
            //Arrange
            var offerCreated = new EventDefinitionName("OfferCreated");
            var OfferCreatedEvent = new DynamicEvent(offerCreated, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);

            _eventDefinitionRepositoryMock.Setup(e => e.GetByNameAndApplication(OfferCreatedEvent.EventDefinitionName, OfferCreatedEvent.ApplicationName, CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new EventDefinition(OfferCreatedEvent.EventDefinitionName as EventDefinitionName, dict)));

            _processObserverRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<ProcessObserverId>(), CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new ProcessObserver()));

            _processDefinitionRepositoryMock.Setup(e => e.GetById(It.IsAny<ProcessDefinitionId>(), CancellationToken.None))
                           .Returns(System.Threading.Tasks.Task.FromResult(new ProcessDefinition(It.IsAny<ProcessDefinitionId>(), runtimeTaskDefinitions)));

            //Assert
            _ = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.GetObserversForEvent(OfferCreatedEvent, CancellationToken.None));
        }
    }
}