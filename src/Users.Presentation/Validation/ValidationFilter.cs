using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Users.Presentation.Validation;

internal sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is null)
        {
            return ValueTask.FromResult<object?>(Results.BadRequest());
        }

        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(model, new ValidationContext(model), results, true))
        {
            return next(context);
        }

        var errors = results
            .SelectMany(result =>
                result.MemberNames.DefaultIfEmpty(string.Empty)
                    .Select(member => new { Member = member, Message = result.ErrorMessage ?? "Invalid value." }))
            .GroupBy(error => error.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());

        return ValueTask.FromResult<object?>(Results.ValidationProblem(errors));
    }
}
