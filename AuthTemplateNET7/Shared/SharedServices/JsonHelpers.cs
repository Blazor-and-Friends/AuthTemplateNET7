using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthTemplateNET7.Shared.SharedServices;

//added
public static class JsonHelpers
{
    /// <summary>
    /// Serializes any object with JsonSerializerOptions set to PropertyNameCaseInsensitive = true and ReferenceHandler = ReferenceHandler.IgnoreCycles. Optionally adds WriteIndented = true
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="writeIndented">Formats the JSON</param>
    /// <returns>stringified JSON</returns>
    public static string ToJson(this object obj, bool writeIndented = false)
    {
        if (obj is null) return null;

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = writeIndented,
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
    }

    /// <summary>
    /// Hydrates an object from a JSON string
    /// </summary>
    /// <typeparam name="T">The Type of object to instantiate</typeparam>
    /// <param name="json">The JSON representation of the object</param>
    /// <returns></returns>
    public static T FromJson<T>(this string json)
    {
        if (string.IsNullOrEmpty(json)) return default;

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}
