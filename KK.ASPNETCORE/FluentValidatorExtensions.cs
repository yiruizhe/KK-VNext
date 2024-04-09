using FluentValidation.Results;

namespace KK.ASPNETCORE;

public static class FluentValidatorExtensions
{
    public static string SumErrors(this ValidationResult validationResult)
    {
        string[] errors = validationResult.Errors.Select(e => $"Code={e.ErrorCode},Message={e.ErrorMessage}").ToArray();
        return string.Join('\n', errors);
    }

}
