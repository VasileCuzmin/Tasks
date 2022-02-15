using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class CreateProcessDefinition
    {
        public record Command(string Name, string ProcessIdentifierEventProps, int ApplicationId, bool Enabled) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;
            private readonly IApplicationRepository _applicationRepository;

            public Validator(IProcessDefinitionRepository processDefinitionRepository, IApplicationRepository applicationRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;
                _applicationRepository = applicationRepository;

                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName).WithMessage("Process definition already exists");

                RuleFor(a => a.ApplicationId)
                    .NotEmpty()
                    .MustAsync(ValidateApplicationId).WithMessage("Application is missing"); ;
            }

            private async Task<bool> ValidateApplicationId(int applicationId, CancellationToken cancellationToken)
            {
                var application = await _applicationRepository.GetByIdAsync(applicationId, cancellationToken);
                return application != null;
            }

            private async Task<bool> ValidateName(Command command, string processDefinitionName, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetAsync(command.ApplicationId, processDefinitionName);
                return processDefinition == null;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(IProcessDefinitionRepository processDefinitionRepository, IMapper mapper, IMediator mediator)
            {
                _processDefinitionRepository = processDefinitionRepository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var processDefinition = _mapper.Map<ProcessDefinition>(request);
                await _processDefinitionRepository.AddAsync(processDefinition, cancellationToken);
                await _processDefinitionRepository.SaveChangesAsync(cancellationToken);

                var newProcess = await _processDefinitionRepository.GetAsync(request.ApplicationId, request.Name);
                var eventData = _mapper.Map<ProcessUpdated.Model>(newProcess);
                await _mediator.Publish(new ProcessUpdated(eventData), cancellationToken);

                return Unit.Value;
            }
        }
    }
}