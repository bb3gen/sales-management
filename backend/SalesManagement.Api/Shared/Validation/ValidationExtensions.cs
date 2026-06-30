using FluentValidation.Results;
using System.Text.Json;

namespace SalesManagement.Api.Shared.Validation;

public static class ValidationExtensions
{
    public static IDictionary<string, string[]> ToCamelCaseDictionary(
        this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => ToCamelCasePath(x.PropertyName))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }

    private static string ToCamelCasePath(string propertyName)
    {
        return string.Join(
            ".",
            propertyName
                .Split('.')
                .Select(JsonNamingPolicy.CamelCase.ConvertName));
    }
}
