using System.Text.Json;
using System.Text.Json.Serialization;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Converters;

public class DocumentTypeJsonConverter : JsonConverter<EDocumentType>
{
    private static readonly Dictionary<EDocumentType, string> ToCode = new()
    {
        [EDocumentType.CedulaCiudadania] = "CC",
        [EDocumentType.CedulaExtranjeria] = "CE",
        [EDocumentType.TarjetaIdentidad] = "TI",
        [EDocumentType.Pasaporte] = "PA",
        [EDocumentType.PermisoProteccionTemporal] = "PPT",
        [EDocumentType.Nit] = "NIT"
    };

    private static readonly Dictionary<string, EDocumentType> FromCode =
        ToCode.ToDictionary(kv => kv.Value, kv => kv.Key);

    public override EDocumentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? code = reader.GetString();

        if (code is null || !FromCode.TryGetValue(code.ToUpperInvariant(), out var value))
            throw new JsonException(
                $"'{code}' is not a valid document type. Expected one of: {string.Join(", ", FromCode.Keys)}.");

        return value;
    }

    public override void Write(Utf8JsonWriter writer, EDocumentType value, JsonSerializerOptions options)
        => writer.WriteStringValue(ToCode[value]);
}