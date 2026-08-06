using System.Text.Json;
using System.Text.Json.Serialization;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Converters;

public class DocumentTypeJsonConverter : JsonConverter<EDocumentType>
{
    public override EDocumentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? code = reader.GetString();

        if (!DocumentTypeCodes.TryParse(code, out EDocumentType value))
            throw new JsonException(
                $"'{code}' no es un tipo de documento válido. Se espera: {string.Join(", ", DocumentTypeCodes.FromCode.Keys)}.");

        return value;
    }

    public override void Write(Utf8JsonWriter writer, EDocumentType value, JsonSerializerOptions options)
        => writer.WriteStringValue(DocumentTypeCodes.ToCode[value]);
}