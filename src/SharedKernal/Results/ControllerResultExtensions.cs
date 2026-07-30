using Microsoft.AspNetCore.Mvc;
using SharedKernal.Results;

namespace SharedKernal.Extensions;

/// <summary>
/// Controller (IActionResult) equivalents of ResultExtensions.
/// Use these in ApiController actions instead of the IResult-based Match overloads.
/// </summary>
public static class ControllerResultExtensions
{
    public static IActionResult Match<TValue>(
        this Result<TValue> result,
        Func<TValue, IActionResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : result.ToActionResult();

    public static IActionResult Match(
        this Result result,
        Func<IActionResult> onSuccess) =>
        result.IsSuccess ? onSuccess() : result.ToActionResult();

    public static IActionResult Match(
        this Result result,
        IActionResult onSuccess) =>
        result.IsSuccess ? onSuccess : result.ToActionResult();

    private static IActionResult ToActionResult(this Result result)
    {
        var validationErrors = result.Errors
            .Where(e => e.Type == ErrorType.Validation)
            .ToList();

        if (validationErrors.Count > 0)
        {
            var errors = validationErrors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Message).ToArray());

            return new BadRequestObjectResult(new ValidationProblemDetails(errors));
        }

        return result.Error.ToActionResult();
    }

    private static IActionResult ToActionResult(this Error error) =>
        error.Type switch
        {
            ErrorType.Validation => new BadRequestObjectResult(new { error.Code, error.Message }),
            ErrorType.NotFound   => new NotFoundObjectResult(new { error.Code, error.Message }),
            ErrorType.Conflict   => new ConflictObjectResult(new { error.Code, error.Message }),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(new { error.Code, error.Message }),
            _                    => new ObjectResult(new { error.Code, error.Message }) { StatusCode = 500 }
        };
}
