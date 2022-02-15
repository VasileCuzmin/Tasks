using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class CreateApplication
    {
        public record Command(string Name) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IApplicationRepository _repository;

            public Validator(IApplicationRepository repository)
            {
                _repository = repository;
                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName); ;
            }

            private async Task<bool> ValidateName(Command command, string applicationName, CancellationToken cancellationToken)
            {
                var application = await _repository.GetByNameAsync(applicationName);
                return application == null;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IApplicationRepository _repository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(IApplicationRepository repository, IMapper mapper, IMediator mediator)
            {
                _repository = repository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var application = _mapper.Map<Domain.Entities.Application>(request);
                await _repository.AddAsync(application, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                await _mediator.Publish(new ApplicationUpdated(request.Name, application.ApplicationId), cancellationToken);

                return Unit.Value;
            }
        }
    }
}