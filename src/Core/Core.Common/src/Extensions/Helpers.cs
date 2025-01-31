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

    public static string ConvertToCsv<T>(IEnumerable<T> data)
    {
        var properties = typeof(T).GetProperties();
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
