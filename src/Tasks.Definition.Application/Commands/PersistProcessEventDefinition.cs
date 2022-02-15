using AutoMapper;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class PersistProcessEventDefinition
    {
        public record Command(List<Command.ProcessEventDefinitionModel> ProcessEventDefinitions, List<Command.DeleteProcessEventDefinitionModel> DeletedProcessEventDefinitions) : IRequest
        {
            public record ProcessEventDefinitionModel
            {
                public int ProcessDefinitionId { get; init; }
                public string ProcessIdentifierProps { get; init; }
                public EventDefinitionModel EventDefinition { get; init; }
            }

            public record DeleteProcessEventDefinitionModel
            {
                public int ProcessDefinitionId { get; init; }
                public int EventDefinitionId { get; init; }
            }

            public record EventDefinitionModel
            {
                public int Id { get; init; }
                public string Name { get; init; }
                public string Topic { get; init; }
                public int ApplicationId { get; init; }
                public string Schema { get; init; }
            }
        }

        public class Validator : AbstractValidator<Command>
        {

            public Validator(IValidator<Command.ProcessEventDefinitionModel> processEventDefinitionModelValidator)
            {
                RuleForEach(p => p.ProcessEventDefinitions).SetValidator(processEventDefinitionModelValidator);
            }
        }

        public class ProcessEventDefinitionModelValidator : AbstractValidator<Command.ProcessEventDefinitionModel>
        {
            public ProcessEventDefinitionModelValidator(IValidator<Command.EventDefinitionModel> eventDefinitionValidator)
            {
                RuleFor(p => p.ProcessDefinitionId)
                    .NotEmpty();
                RuleFor(p => p.ProcessIdentifierProps)
                    .NotEmpty();
                RuleFor(p => p.EventDefinition).SetValidator(eventDefinitionValidator);
            }
        }

        public class EventDefinitionValidator : AbstractValidator<Command.EventDefinitionModel>
        {
            public EventDefinitionValidator()
            {
                RuleFor(e => e.Id).NotEmpty();
                RuleFor(e => e.Name).NotEmpty();
                RuleFor(e => e.Topic).NotEmpty();
                RuleFor(e => e.ApplicationId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(IProcessEventDefinitionRepository processEventDefinitionRepository, IMapper mapper, IMediator mediator)
            {
                _processEventDefinitionRepository = processEventDefinitionRepository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var processEventDefinitions = _mapper.Map<List<ProcessEventDefinition>>(request.ProcessEventDefinitions);
                var deleteProcessEventDefinitions = _mapper.Map<List<ProcessEventDefinition>>(request.DeletedProcessEventDefinitions);

                await _processEventDefinitionRepository.PersistAllAsync(processEventDefinitions, deleteProcessEventDefinitions);

                var eventData = _mapper.Map<List<EventDefinitionUpdated.ProcessEventDefinitionModel>>(request.ProcessEventDefinitions);
                var deleteEventData = _mapper.Map<List<EventDefinitionUpdated.DeleteProcessEventDefinitionModel>>(request.DeletedProcessEventDefinitions);
                await _mediator.Publish(new EventDefinitionUpdated(eventData, deleteEventData), cancellationToken);

                return Unit.Value;
            }
        }
    }
}