using Moq;
using NBB.Data.Abstractions;
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
    public class GetObserversForProcess_tests
    {
        public readonly Mock<IEventDefinitionRepository> _eventDefinitionRepositoryMock;
        public readonly Mock<IProcessDefinitionRepository> _processDefinitionRepositoryMock;
        public readonly Mock<IEventSourcedRepository<ProcessObserver>> _processObserverRepositoryMock;
        public readonly EventRouterService sut;

        public GetObserversForProcess_tests()
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
        public async void GetObserversForProcess_IfProcessObserverDoesNotExists_ThenCreateOne()
        {
            //Arrange 
            _processDefinitionRepositoryMock.Setup(e => e.GetById(It.IsAny<ProcessDefinitionId>(), CancellationToken.None))
              .Returns(System.Threading.Tasks.Task.FromResult(new ProcessDefinition(It.IsAny<ProcessDefinitionId>(), runtimeTaskDefinitions)));

            //Act
            var observers = await sut.GetObserversForProcess(It.IsAny<int>(), It.IsAny<ProcessId>(), CancellationToken.None);

            //Assert
            Assert.Single(observers);
            var firedEvent = observers[0].GetUncommittedChanges().ToList();
            Assert.True(firedEvent.Where(e => e is ProcessStarted).Count() == 1);
        }

        [Fact]
        public async void GetObserversForProcess_IfProcessObserverDoesExists_ThenProcessObserverIsLoaded()
        {
            //Arrange
            var processDefinitionId = new ProcessDefinitionId(1);
            var processId = new ProcessId(new ImmutableKeyValue(string.Empty, string.Empty));
            var processObserverId = new ProcessObserverId(processDefinitionId, processId);
            _processObserverRepositoryMock.Setup(e => e.GetByIdAsync(It.IsAny<ProcessObserverId>(), CancellationToken.None))
                .Returns(System.Threading.Tasks.Task.FromResult(new ProcessObserver()));

            //Act
            var observers = await sut.GetObserversForProcess(It.IsAny<int>(), It.IsAny<ProcessId>(), CancellationToken.None);

            //Assert
            Assert.Single(observers);
            var firedEvent = observers[0].GetUncommittedChanges().ToList();
            Assert.True(firedEvent.Where(e => e is ProcessStarted).Count() == 0);
        }
    }
}