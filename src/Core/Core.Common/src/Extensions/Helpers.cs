using Newtonsoft.Json;
using System.Globalization;

namespace Tilray.Integrations.Core.Common.Extensions;

public static class Helpers
{
    /// <summary>
    /// Throw an ArgumentNullException if the object is null
    /// </summary>
    /// <typeparam name="T">The type of the to be tested</typeparam>
    /// <param name="argument">The object to be tested</param>
    /// <param name="paramName">The name of the parameter. Is null, the method will try to use it dynamic</param>
    /// <returns>The object or the exception if the object is null</returns>
    public static T ThrowIfNull<T>([AllowNull()] this T argument, string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName: paramName);

        return argument;
    }

    public static T ToObject<T>(this string jsonString)
    {
        jsonString.ThrowIfNull(jsonString);

        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public static string ToString<T>(T entity)
    {
        entity.ThrowIfNull();

        return JsonConvert.SerializeObject(entity);
    }

    public static StringContent CreateStringContent<T>(T entity)
    {
        return new StringContent(ToString(entity), Encoding.UTF8, "application/json");
    }

    public static DateTime? ParseDate(string date)
    {
        return DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : null;
    }

    public static decimal ParseDecimal(string value)
    {
        return decimal.TryParse(value, out decimal result) ? result : 0.0m;
    }

    public static string GetErrorMessage<T>(IEnumerable<T> items)
    {
        return string.Join(", ", items.Select(item => item?.ToString() ?? string.Empty));
    }

    public static string ConvertToCsv<T>(IEnumerable<T> data, string[] ignoredProperties = null)
    {
        ignoredProperties ??= [];

        var properties = typeof(T).GetProperties()
            .Where(p => !ignoredProperties.Contains(p.Name))
            .ToArray();
        var csvBuilder = new StringBuilder();

        csvBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item, null)?.ToString() ?? string.Empty);
            csvBuilder.AppendLine(string.Join(",", values));
        }

        return csvBuilder.ToString();
    }

    public static string GetErrorFromResponse(HttpResponseMessage response)
    {
        return $"{response.StatusCode} (Details: '{response.Content.AsString()}')";
    }

    public static Result? WithValidationError(this Result? result, string property, string error)
    {
        return result?.WithError(new Error(property).WithMetadata(error, property));
    }

    public static Result<T> WithValidationError<T>(this Result<T> result, string property, string error)
    {
        return result.WithError(new Error(property).WithMetadata(error, property));
    }
}
