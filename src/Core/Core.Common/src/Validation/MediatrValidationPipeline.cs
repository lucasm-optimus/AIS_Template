using MediatR;
using FluentValidation;
using FluentValidation.Results;

namespace Optimus.Core.Common.Validation;

/// <summary>
/// This class is responsible for validate the Commands and Queries before executing the Handler
/// </summary>
/// <typeparam name="TRequest">The IRequest object to be validated</typeparam>
/// <typeparam name="TResponse">The expected result of the operation with the generic type</typeparam>
public class MediatrValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly bool _hasValidators;
    private readonly ILogger<MediatrValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public MediatrValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<MediatrValidationBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _validators = validators;
        _hasValidators = _validators.Any();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogDebug("[MediatR][Validator][Request {RequestType}]", request.GetType().Name);
        if (!_hasValidators)
        {
            _logger.LogDebug("[MediatR][Validator][Request {RequestType}][Validation ignored]", typeof(TRequest).FullName);
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults =
                    await Task.WhenAll(_validators.Select(v =>
                           v.ValidateAsync(context, cancellationToken)));

        List<ValidationFailure> failures =
                    validationResults.SelectMany(r => r.Errors)
                          .Where(f => f != null).ToList();

        var result = new TResponse();

        if (failures.Any())
        {
            _logger.LogWarning("[MediatR][Validator][Request {RequestType}][Validation failed]", typeof(TRequest).Name);

            result.Reasons.AddRange(ConvertFailuresToErrors(failures));

            return result;
        }

        _logger.LogDebug("[MediatR][Validator][Request {RequestType}][Validation passed]", typeof(TRequest).Name);

        var pipelineResult = await next();

        if (pipelineResult.IsFailed)
        {
            _logger.LogWarning("[MediatR][Validator][Request {RequestType}][Pipeline failed]", typeof(TRequest).Name);

            result.Reasons.AddRange(pipelineResult.Errors);

            return result;
        }

        return pipelineResult;
    }

    private IEnumerable<Error> ConvertFailuresToErrors(IEnumerable<ValidationFailure> failures)
    {
        return failures
            .GroupBy(f => f.PropertyName)
            .Select(g => new Error(g.Key).WithMetadata(g.ToDictionary(f => f.ErrorMessage, z => z.PropertyName as object)));
    }

}