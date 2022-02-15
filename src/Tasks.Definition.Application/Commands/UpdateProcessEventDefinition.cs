using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class UpdateProcessEventDefinition
    {
        public record Command(int ProcessDefinitionId, int EventDefinitionId, string ProcessIdentifierProps) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;

            public Validator(IProcessEventDefinitionRepository processEventDefinitionRepository)
            {
                _processEventDefinitionRepository = processEventDefinitionRepository;

                RuleFor(a => a.ProcessDefinitionId)
                    .NotEmpty();
                RuleFor(a => a.EventDefinitionId)
                    .NotEmpty();
                RuleFor(a => a)
                    .MustAsync(ValidateProcessEventDefinition).WithMessage("Missing process event definition");
                RuleFor(a => a.ProcessIdentifierProps).NotEmpty();
            }

            private async Task<bool> ValidateProcessEventDefinition(Command command, CancellationToken cancellationToken)
            {
                var processDefinition = await _processEventDefinitionRepository.GetByIdAsync(new object[] { command.ProcessDefinitionId, command.EventDefinitionId }, cancellationToken);
                return processDefinition != null;
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly IProcessEventDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IProcessEventDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var processEventDefinition = await _repository.GetByIdAsync(new object[]
                    {request.ProcessDefinitionId, request.EventDefinitionId}, cancellationToken);
                _mapper.Map(request, processEventDefinition);
                await _repository.Update(processEventDefinition, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}