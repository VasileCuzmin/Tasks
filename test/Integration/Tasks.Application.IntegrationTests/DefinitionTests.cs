using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBB.Application.DataContracts.Schema;
using NBB.Correlation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Queries;
using Tasks.Migrations;
using Tasks.PublishedLanguage.Events.Definition;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Xunit;
using Xunit.Abstractions;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tasks.Application.IntegrationTests
{
    //[Collection("Sequential")]
    public class DefinitionTests : IClassFixture<DefinitionFixture>, IDisposable
    {
        private readonly DefinitionFixture _fixture;
        private readonly IMediator _mediator;
        private readonly IServiceScope _scope;
        private bool _isDisposed;

        public DefinitionTests(DefinitionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;

            _scope = fixture.Container.CreateScope();
            _mediator = _scope.ServiceProvider.GetService<IMediator>();
            var logger = _scope.ServiceProvider.GetService<ILogger<DefinitionTests>>();

            logger.LogInformation($"{testOutputHelper.GetTestName()}");
            Task.Run(PrepareDbAsync).Wait();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // free managed resources
                _scope.Dispose();
            }

            // free native resources if there are any.

            _isDisposed = true;
        }

        #region private
        private async Task PrepareDbAsync()
        {
            var configuration = _scope.ServiceProvider.GetService<IConfiguration>();

            if (!_fixture.ShouldCleanDb(configuration))
                return;

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            using (var cnx = new SqlConnection(connectionString))
            {
                cnx.Open();

                List<string> tbls = new List<string>()
                    {"TaskDefinition", "ProcessEventDefinition", "ProcessDefinition", "EventDefinition", "Application"};

                foreach (var tbl in tbls)
                {
                    var cmd = new SqlCommand($"DROP TABLE [dbo].[{tbl}]", cnx);
                    await cmd.ExecuteNonQueryAsync();
                }

                var resetCommand = new SqlCommand("UPDATE [dbo].[__TasksMigration] SET ScriptsVersion = 0", cnx);
                await resetCommand.ExecuteNonQueryAsync();
            }

            var migrator = new DatabaseMigrator("DefaultConnection");
            await migrator.MigrateToLatestVersion();
        }

        private readonly BaseEntity _app1 = new BaseEntity("App1");

        private readonly BaseEntity _event1 = new BaseEntity("Event 1");
        private readonly BaseEntity _event2 = new BaseEntity("Event 2");
        private readonly BaseEntity _event3 = new BaseEntity("Event 3");

        private readonly BaseEntity _process1 = new BaseEntity("Process 1");
        private readonly BaseEntity _process2 = new BaseEntity("Process 2");
        private readonly BaseEntity _process3 = new BaseEntity("Process 3");

        private readonly BaseEntity _task1 = new BaseEntity("Task 1");
        private readonly BaseEntity _task2 = new BaseEntity("Task 2");

        private async Task CreateApplication(BaseEntity entity)
        {
            var createCommand = new CreateApplication.Command(entity.Value);
            await _mediator.Send(createCommand);

            var application = await _mediator.Send(new GetApplicationByName.Query { Name = entity.Value });
            entity.Key = application.ApplicationId;
        }

        private async Task UpdateApplication(BaseEntity entity, string name)
        {
            var updateCommand = new UpdateApplication.Command(entity.Key, name);
            await _mediator.Send(updateCommand);
        }

        private async Task CreateEventDefinition(BaseEntity entity, int applicationId, string topic = "Topic")
        {
            var createEventDefinitionCommand = new CreateEventDefinition.Command(entity.Value, topic, applicationId, string.Empty);
            await _mediator.Send(createEventDefinitionCommand);

            var eventDefinition = await _mediator.Send(new GetEventDefinitionByName.Query
            { ApplicationId = applicationId, Name = entity.Value });
            entity.Key = eventDefinition.EventDefinitionId;
        }

        private async Task UpdateEventDefinition(BaseEntity entity, int applicationId, string name, string topic = "Topic")
        {
            var updateEventDefinitionCommand = new UpdateEventDefinition.Command(entity.Key, name, topic, applicationId, string.Empty);
            await _mediator.Send(updateEventDefinitionCommand);
        }

        private async Task CreateProcessDefinition(BaseEntity entity, int applicationId, bool enabled = false, string props = "DocumentId;SiteId")
        {
            var createProcessDefinitionCommand = new CreateProcessDefinition.Command(entity.Value, props, applicationId, enabled);
            await _mediator.Send(createProcessDefinitionCommand);

            var processDefinition = await _mediator.Send(new GetProcessDefinitionByName.Query
            { ApplicationId = applicationId, Name = entity.Value });
            entity.Key = processDefinition.ProcessDefinitionId;
        }

        private async Task UpdateProcessDefinition(BaseEntity entity, int applicationId, string name, bool enabled, string props = "DocumentId;SiteId")
        {
            var updateProcessDefinitionCommand = new UpdateProcessDefinition.Command(entity.Key, name, props, applicationId, enabled);
            await _mediator.Send(updateProcessDefinitionCommand);
        }

        private async Task EnableProcessDefinition(int processDefinitionId)
        {
            var enableProcessDefinitionCommand = new EnableProcessDefinition.Command(processDefinitionId);
            await _mediator.Send(enableProcessDefinitionCommand);
        }

        private async Task DisableProcessDefinition(int processDefinitionId)
        {
            var disableProcessDefinitionCommand = new DisableProcessDefinition.Command(processDefinitionId);
            await _mediator.Send(disableProcessDefinitionCommand);
        }

        private async Task AddProcessEventDefinition(int processDefinitionId, int eventDefinitionId, string props = "InvoiceId;SiteId")
        {
            var addProcessEventDefinitionCommand = new AddProcessEventDefinition.Command(processDefinitionId, eventDefinitionId, props);
            await _mediator.Send(addProcessEventDefinitionCommand);
        }

        private async Task UpdateProcessEventDefinition(int processDefinitionId, int eventDefinitionId, string props = "InvoiceId;SiteIdUpdated")
        {
            var updateProcessEventDefinitionCommand = new UpdateProcessEventDefinition.Command(processDefinitionId, eventDefinitionId, props);
            await _mediator.Send(updateProcessEventDefinitionCommand);
        }

        private async Task RemoveProcessEventDefinition(int processDefinitionId, int eventDefinitionId)
        {
            var disableProcessEventDefinitionCommand = new RemoveProcessEventDefinition.Command(processDefinitionId, eventDefinitionId);
            await _mediator.Send(disableProcessEventDefinitionCommand);
        }

        private async Task AddTaskDefinition(BaseEntity entity, int processDefinitionId, int startEventId, int endEventId, int cancelEventId,
            string startExpr = "startExpr", string endExpr = "endExpr", string cancelExpr = "cancelExpr", string groupallocexpr = "groupallocexpr", string userallocexpr = "userallocexpr")
        {
            var addTaskDefinitionCommand = new AddTaskDefinition.Command(
                processDefinitionId, entity.Value,
                startEventId, endEventId, cancelEventId,
                startExpr, endExpr, cancelExpr, groupallocexpr, userallocexpr);
            await _mediator.Send(addTaskDefinitionCommand);

            var taskDefinitions = await _mediator.Send(new GetTaskDefinitionsByName.Query { Name = entity.Value });
            if (taskDefinitions.Count == 1)
                entity.Key = taskDefinitions.First().TaskDefinitionId;
        }

        private async Task RemoveTaskDefinition(int taskDefinitionId)
        {
            var disableTaskDefinitionCommand = new RemoveTaskDefinition.Command(taskDefinitionId);
            await _mediator.Send(disableTaskDefinitionCommand);
        }
        #endregion private

        #region tests
        [Fact]
        public async void CreateApplicationTest()
        {
            //act
            await CreateApplication(_app1);
            var application = await _mediator.Send(new GetApplicationById.Query { ApplicationId = _app1.Key });

            //assert 
            Assert.NotEqual(0, _app1.Key);
            Assert.Equal(_app1.Value, application.Name);
        }

        [Fact]
        public async void UpdateApplicationTest()
        {
            //prepare
            await CreateApplication(_app1);

            //act
            await UpdateApplication(_app1, _app1.Value.Update());
            var updatedApplication = await _mediator.Send(new GetApplicationById.Query { ApplicationId = _app1.Key });

            //assert
            Assert.Equal(_app1.Key, updatedApplication.ApplicationId);
            Assert.Equal(_app1.Value.Update(), updatedApplication.Name);
        }

        [Fact]
        public async void CreateEventDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            var topic = $"Topic_{Guid.NewGuid()}";

            //act
            await CreateEventDefinition(_event1, _app1.Key, topic);
            var createdEvent = await _mediator.Send(new GetEventDefinitionById.Query { EventDefinitionId = _event1.Key });

            //assert
            Assert.NotEqual(0, _event1.Key);
            Assert.Equal(_app1.Key, createdEvent.ApplicationId);
            Assert.Equal(_event1.Value, createdEvent.Name);
            Assert.Equal(topic, createdEvent.Topic);
        }

        [Fact]
        public async void UpdateEventDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            var topicUpdated = "Topic_updated";

            //act
            await UpdateEventDefinition(_event1, _app1.Key, _event1.Value.Update(), topicUpdated);
            var updatedEvent = await _mediator.Send(new GetEventDefinitionById.Query { EventDefinitionId = _event1.Key });

            //assert
            Assert.Equal(_event1.Key, updatedEvent.EventDefinitionId);
            Assert.Equal(_event1.Value.Update(), updatedEvent.Name);
            Assert.Equal(topicUpdated, updatedEvent.Topic);
        }

        [Fact]
        public async void CreateProcessDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            string props = $"DocumentId;SiteId_{Guid.NewGuid()}";

            //act            
            await CreateProcessDefinition(_process1, _app1.Key, false, props);
            var createdProcess = await _mediator.Send(new GetProcessDefinitionById.Query
            { ProcessDefinitionId = _process1.Key });

            //assert
            Assert.NotEqual(0, _process1.Key);
            Assert.Equal(_process1.Value, createdProcess.Name);
            Assert.Equal(props, createdProcess.ProcessIdentifierEventProps);
            Assert.Equal(_app1.Key, createdProcess.ApplicationId);
            Assert.False(createdProcess.Enabled);
        }

        [Fact]
        public async void UpdateProcessDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            string propsUpdated = $"DocumentId;SiteId;Key_{Guid.NewGuid()}";

            //act                        
            await UpdateProcessDefinition(_process1, _app1.Key, _process1.Value.Update(), false, propsUpdated);
            var updatedProcess = await _mediator.Send(new GetProcessDefinitionById.Query
            { ProcessDefinitionId = _process1.Key });

            //assert
            Assert.Equal(_process1.Key, updatedProcess.Id);
            Assert.Equal(_process1.Value.Update(), updatedProcess.Name);
            Assert.Equal(_app1.Key, updatedProcess.ApplicationId);
            Assert.Equal(propsUpdated, updatedProcess.ProcessIdentifierEventProps);
            Assert.False(updatedProcess.Enabled);
        }

        [Fact]
        public async void EnableProcessDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key, true);

            //act            
            await EnableProcessDefinition(_process1.Key);
            var enabledProcess = await _mediator.Send(new GetProcessDefinitionById.Query
            { ProcessDefinitionId = _process1.Key });

            //assert
            enabledProcess.Enabled.Should().BeTrue();
        }

        [Fact]
        public async void DisableProcessDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key, false);
            await EnableProcessDefinition(_process1.Key);

            //act            
            await DisableProcessDefinition(_process1.Key);
            var disabledProcess = await _mediator.Send(new GetProcessDefinitionById.Query
            { ProcessDefinitionId = _process1.Key });

            //assert
            disabledProcess.Enabled.Should().BeFalse();
        }

        [Fact]
        public async void AddProcessEventDefinitionTest()
        {
            //prepare
            string props = "InvoiceId;SiteId" + Guid.NewGuid();
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);

            //act
            await AddProcessEventDefinition(_process1.Key, _event1.Key, props);
            var processEventDefinition = await _mediator.Send(new GetProcessEventDefinitionsById.Query
            { ProcessDefinitionId = _process1.Key, EventDefinitionId = _event1.Key });

            //assert
            processEventDefinition.Should().NotBeNull();
            processEventDefinition.ProcessIdentifierProps.Should().Be(props);
        }

        [Fact]
        public async void UpdateProcessEventDefinitionTest()
        {
            //prepare
            string props = "InvoiceId;SiteId" + Guid.NewGuid();
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);

            //act
            await UpdateProcessEventDefinition(_process1.Key, _event1.Key, props.Update());
            var updatedProcessEventDefinition = await _mediator.Send(new GetProcessEventDefinitionsById.Query
            { ProcessDefinitionId = _process1.Key, EventDefinitionId = _event1.Key });

            //assert
            updatedProcessEventDefinition.Should().NotBeNull();
            updatedProcessEventDefinition.ProcessIdentifierProps.Should().Be(props.Update());
        }

        [Fact]
        public async void RemoveProcessEventDefinitionTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);

            //act
            await RemoveProcessEventDefinition(_process1.Key, _event1.Key);
            var removedProcessEventDefinition = await _mediator.Send(new GetProcessEventDefinitionsById.Query
            { ProcessDefinitionId = _process1.Key, EventDefinitionId = _event1.Key });

            //assert
            removedProcessEventDefinition.Should().BeNull();
        }

        [Fact]
        public async void RemoveProcessEventDefinitionWithDependentTaskTest()
        {
            //prepare
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateEventDefinition(_event2, _app1.Key);
            await CreateEventDefinition(_event3, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);
            await AddProcessEventDefinition(_process1.Key, _event2.Key);
            await AddProcessEventDefinition(_process1.Key, _event3.Key);
            await AddTaskDefinition(_task1, _process1.Key, _event1.Key, _event2.Key, _event3.Key);

            //act            
            Task Act() => RemoveProcessEventDefinition(_process1.Key, _event1.Key);

            //assert
            var ex = await Assert.ThrowsAsync<ValidationException>(Act);
            Assert.Contains(ValidationMessages.RemoveProcessEventWithDependentTasks, ex.Message);
        }

        [Fact]
        public async void AddTaskDefinitionTest()
        {
            //prepare
            string startExpr = "startExpr", endExpr = "endExpr", cancelExpr = "cancelExpr";
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateEventDefinition(_event2, _app1.Key);
            await CreateEventDefinition(_event3, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);
            await AddProcessEventDefinition(_process1.Key, _event2.Key);
            await AddProcessEventDefinition(_process1.Key, _event3.Key);

            //act            
            await AddTaskDefinition(_task1, _process1.Key, _event1.Key, _event2.Key, _event3.Key, startExpr, endExpr, cancelExpr);
            var createdTaskDefinition =
                await _mediator.Send(new GetTaskDefinitionsById.Query { TaskDefinitionId = _task1.Key });

            //assert
            createdTaskDefinition.TaskDefinitionId.Should().NotBe(0);
            createdTaskDefinition.Name.Should().Be(_task1.Value);
            createdTaskDefinition.ProcessDefinitionId.Should().Be(_process1.Key);
            createdTaskDefinition.StartEventDefinitionId.Should().Be(_event1.Key);
            createdTaskDefinition.EndEventDefinitionId.Should().Be(_event2.Key);
            createdTaskDefinition.CancelEventDefinitionId.Should().Be(_event3.Key);
            createdTaskDefinition.StartExpression.Should().Be(startExpr);
            createdTaskDefinition.EndExpression.Should().Be(endExpr);
            createdTaskDefinition.CancelExpression.Should().Be(cancelExpr);
        }

        [Fact]
        public async void RemoveTaskDefinitionTest()
        {
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateEventDefinition(_event2, _app1.Key);
            await CreateEventDefinition(_event3, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);
            await AddProcessEventDefinition(_process1.Key, _event2.Key);
            await AddProcessEventDefinition(_process1.Key, _event3.Key);
            await AddTaskDefinition(_task1, _process1.Key, _event1.Key, _event2.Key, _event3.Key);

            //act                        
            await RemoveTaskDefinition(_task1.Key);
            var removedTaskDefinition =
                await _mediator.Send(new GetTaskDefinitionsById.Query { TaskDefinitionId = _task1.Key });

            //assert
            removedTaskDefinition.Should().BeNull();
        }

        [Fact]
        public async void ImportTest()
        {
            var createCommand = new CreateApplication.Command("App 1");
            await _mediator.Send(createCommand);

            var createEventDefinitionCommand1 = new CreateEventDefinition.Command("Event 1", "Topic 0", 1, string.Empty);
            await _mediator.Send(createEventDefinitionCommand1);

            var createEventDefinitionCommand2 = new CreateEventDefinition.Command("Event 5", "Topic 0", 1, string.Empty);
            await _mediator.Send(createEventDefinitionCommand2);

            var languagePublishedEvent = new LanguagePublished
            {
                ApplicationName = "App 1",
                SchemaDefinitions = new List<SchemaDefinition>
                {
                    new SchemaDefinition("Event 1", string.Empty,string.Empty,"Topic 1",string.Empty),
                    new SchemaDefinition("Event 2", string.Empty,string.Empty,"Topic 1", string.Empty),
                    new SchemaDefinition("Event 3", string.Empty, string.Empty,"Topic 1",string.Empty),
                    new SchemaDefinition("Event 4", string.Empty,string.Empty,"Topic 1",string.Empty)
                }
            };
            await _mediator.Publish(languagePublishedEvent);
        }

        [Fact]
        public async void GetAllTopicsTest()
        {
            string topic1 = "Topic1", topic2 = "Topic2";
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key, topic1);
            await CreateEventDefinition(_event2, _app1.Key, topic2);
            await CreateEventDefinition(_event3, _app1.Key, topic2);

            var topics = await _mediator.Send(new GetAllTopics.Query());

            topics.Should().Contain(topic1);
            topics.Should().Contain(topic2);
        }

        [Fact]
        public async void GetByNameAndApplication()
        {
            //prepare
            string prop1 = "prop1;prop3", prop2 = "prop2";
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await CreateProcessDefinition(_process2, _app1.Key);
            await CreateProcessDefinition(_process3, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key, prop1);
            await AddProcessEventDefinition(_process2.Key, _event1.Key, prop2);
            var repo = _fixture.Container.GetService<Runtime.Domain.EventDefinitionAggregate.IEventDefinitionRepository>();

            //act
            var eventDefinitions = await repo.GetByNameAndApplication(new EventDefinitionName(_event1.Value), _app1.Value, CancellationToken.None);

            //assert
            eventDefinitions.ProcessDefinitionsDictionary.Should().Equal(
                new Dictionary<ProcessDefinitionId, IdentifierPropsMap>
                {
                    [new ProcessDefinitionId(_process1.Key)] = IdentifierPropsMap.From("DocumentId;SiteId", "prop1;prop3"),
                    [new ProcessDefinitionId(_process2.Key)] = IdentifierPropsMap.From("DocumentId", "prop2")
                });
        }

        [Fact]
        public async void GetByProcessDefinitionId()
        {
            //prepare
            string startExpr = "startExpr", cancelExpr = "cancelExpr", endExpr = "endExpr", userExpr = "userExpr", groupExpr = "groupExpr";
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateEventDefinition(_event2, _app1.Key);
            await CreateEventDefinition(_event3, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);
            await AddProcessEventDefinition(_process1.Key, _event2.Key);
            await AddProcessEventDefinition(_process1.Key, _event3.Key);
            await AddTaskDefinition(_task1, _process1.Key, _event1.Key, _event2.Key, _event3.Key, startExpr, endExpr, cancelExpr, groupExpr, userExpr);
            await AddTaskDefinition(_task2, _process1.Key, _event3.Key, _event2.Key, _event1.Key, startExpr, endExpr, cancelExpr, groupExpr, userExpr);
            var repo = _fixture.Container.GetService<Runtime.Domain.ProcessDefinitionAggregate.IProcessDefinitionRepository>();

            //act
            var processDefinition = await repo.GetById(new ProcessDefinitionId(_process1.Key), CancellationToken.None);

            //assert
            processDefinition.TaskDefinitions.Should().Equal(
                new List<TaskDefinition>
                {
                    new TaskDefinition(_task1.Value,
                        new EventDefinitionName(_event1.Value),
                        new EventDefinitionName(_event2.Value),
                        new EventDefinitionName(_event3.Value),
                        new DynamicExpression(startExpr), new DynamicExpression(endExpr), new DynamicExpression(cancelExpr),
                        new DynamicExpression(userExpr), new DynamicExpression(groupExpr), null), //new AutomaticStart(true)),
                    new TaskDefinition(_task2.Value,
                        new EventDefinitionName(_event3.Value),
                        new EventDefinitionName(_event2.Value),
                        new EventDefinitionName(_event1.Value),
                        new DynamicExpression(startExpr), new DynamicExpression(endExpr), new DynamicExpression(cancelExpr),
                        new DynamicExpression(userExpr), new DynamicExpression(groupExpr),
                        null) //new AutomaticStart(true));
                });
        }

        [Fact]
        public async void GetAllApplications()
        {
            await CreateApplication(new BaseEntity("App-"));
            await CreateApplication(new BaseEntity("App-"));
            await CreateApplication(new BaseEntity("App-"));
            await CreateApplication(new BaseEntity("App-"));
            await CreateApplication(new BaseEntity("App-"));

            var applications = await _mediator.Send(new GetAllApplications.Query());

            applications.TotalCount.Should().BeGreaterOrEqualTo(5);
        }

        [Fact]
        public async void GetProcessEventDefinitionsByProcessDefinitionId()
        {
            //prepare
            //string startExpr = "startExpr", endExpr = "endExpr", cancelExpr = "cancelExpr";
            await CreateApplication(_app1);
            await CreateEventDefinition(_event1, _app1.Key);
            await CreateEventDefinition(_event2, _app1.Key);
            await CreateEventDefinition(_event3, _app1.Key);
            await CreateProcessDefinition(_process1, _app1.Key);
            await AddProcessEventDefinition(_process1.Key, _event1.Key);
            await AddProcessEventDefinition(_process1.Key, _event2.Key);
            await AddProcessEventDefinition(_process1.Key, _event3.Key);

            //act
            var eventsDefinitions = await _mediator.Send(new GetProcessEventDefinitionsByProcessDefinitionId.Query
            { ProcessDefinitionId = _process1.Key });

            //assert
            eventsDefinitions.Count().Should().Be(3);
        }

        #endregion tests
    }

    internal class BaseEntity
    {
        public int Key { get; set; }


        public string Value
        {
            get;
        }

        public BaseEntity(string name)
        {
            Value = name + Guid.NewGuid();
        }
    }
}
