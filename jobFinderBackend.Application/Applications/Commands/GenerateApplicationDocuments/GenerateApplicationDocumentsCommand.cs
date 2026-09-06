using FluentValidation;
using jobFinderBackend.Application.Applications.DTOs;
using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Applications.Commands.GenerateApplicationDocuments;

public record GenerateApplicationDocumentsCommand(int JobId)
    : IRequest<GeneratedDocumentsResponse>;

public class GenerateApplicationDocumentsValidator
    : AbstractValidator<GenerateApplicationDocumentsCommand>
{
    public GenerateApplicationDocumentsValidator()
    {
        RuleFor(request => request.JobId)
            .GreaterThan(0)
            .WithMessage("Job ID must be greater than zero.");
    }
}

public class GenerateApplicationDocumentsCommandHandler
    : IRequestHandler<GenerateApplicationDocumentsCommand, GeneratedDocumentsResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IGeneratedDocumentRepository _repository;
    private readonly IGenerativeDocumentService _generator;
    private readonly IValidator<GenerateApplicationDocumentsCommand> _validator;

    public GenerateApplicationDocumentsCommandHandler(
        ICurrentUserService currentUser,
        IUserRepository userRepository,
        IGeneratedDocumentRepository repository,
        IGenerativeDocumentService generator,
        IValidator<GenerateApplicationDocumentsCommand> validator)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _repository = repository;
        _generator = generator;
        _validator = validator;
    }

    public async Task<GeneratedDocumentsResponse> Handle(
        GenerateApplicationDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        if (_currentUser.UserId is not int userId)
        {
            throw new UnauthorizedAccessException();
        }

        var plan = await _userRepository.GetActiveSubscriptionPlanAsync(userId);
        if (plan is null || !plan.CanGenerateCV || !plan.CanGenerateCoverLetter)
        {
            throw new ForbiddenAccessException(
                "Your subscription does not allow document generation.");
        }

        var data = await _repository.GetGenerationDataAsync(
            userId, request.JobId, cancellationToken);
        if (data is null)
        {
            throw new KeyNotFoundException("Job or user was not found.");
        }

        var generated = await _generator.GenerateAsync(data, cancellationToken);
        return await _repository.SaveAsync(
            userId,
            request.JobId,
            generated.Cv,
            generated.CoverLetter,
            cancellationToken);
    }
}

public sealed class ForbiddenAccessException(string message) : Exception(message);