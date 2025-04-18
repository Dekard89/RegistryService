using FluentValidation;
using MediatR;

namespace Spotify.Identity.Mediatr.Pipeline;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
    IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(validators.Select(v => 
            v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

        var failures = validationResults
            .Where(v => v.Errors.Count > 0)
            .SelectMany(v => v.Errors)
            .ToList();
            
            
        
        if(failures.Any())
            throw new ValidationException(failures);
        
        return await next();
    }
}