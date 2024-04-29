using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pannella.Helpers;

public class StringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32().ToString(),
            JsonTokenType.String => reader.GetString(),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
