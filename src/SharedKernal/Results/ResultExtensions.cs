using Microsoft.AspNetCore.Http;
using SharedKernal.Results;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace SharedKernal.Extensions;

public static class ResultExtensions
{
    public static IResult Match<TValue>(
        this Result<TValue> result,
        Func<TValue, IResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : result.ToHttpResult();

    public static IResult Match(
        this Result result,
        Func<IResult> onSuccess) =>
        result.IsSuccess ? onSuccess() : result.ToHttpResult();

    public static IResult Match(
        this Result result,
        IResult onSuccess) =>
        result.IsSuccess ? onSuccess : result.ToHttpResult();

    private static IResult ToHttpResult(this Result result)
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

            return HttpResults.ValidationProblem(errors);
        }

        return result.Error.ToHttpResult();
    }

    private static IResult ToHttpResult(this Error error) =>
        error.Type switch
        {
            ErrorType.Validation => HttpResults.BadRequest(new { error.Code, error.Message }),
            ErrorType.NotFound => HttpResults.NotFound(new { error.Code, error.Message }),
            ErrorType.Conflict => HttpResults.Conflict(new { error.Code, error.Message }),
            ErrorType.Unauthorized => HttpResults.Unauthorized(),
            _ => HttpResults.Problem(error.Message)
        };
}
