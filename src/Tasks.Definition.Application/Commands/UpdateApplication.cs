using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class UpdateApplication
    {
        public record Command(int ApplicationId, string Name) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IApplicationRepository _repository;

            public Validator(IApplicationRepository repository)
            {
                _repository = repository;

                RuleFor(a => a.ApplicationId)
                    .NotEmpty()
                    .MustAsync(ValidateApplicationId).WithMessage("Application is missing"); ;
                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName).WithMessage("Duplicate application");
            }

            private async Task<bool> ValidateApplicationId(int applicationId, CancellationToken cancellationToken)
            {
                var application = await _repository.GetByIdAsync(applicationId, cancellationToken);
                return application != null;
            }

            private async Task<bool> ValidateName(Command command, string applicationName, CancellationToken cancellationToken)
            {
                var application = await _repository.GetByNameAsync(applicationName);
                return application == null || application.ApplicationId == command.ApplicationId;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IApplicationRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IApplicationRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);
                _mapper.Map(request, application);
                await _repository.Update(application, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}