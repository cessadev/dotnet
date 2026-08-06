using CarCredit.Domain.Enums;

namespace CarCredit.Application.Converters;

public static class DocumentTypeCodes
{
    public static readonly IReadOnlyDictionary<EDocumentType, string> ToCode = new Dictionary<EDocumentType, string>
    {
        [EDocumentType.CedulaCiudadania] = "CC",
        [EDocumentType.CedulaExtranjeria] = "CE",
        [EDocumentType.TarjetaIdentidad] = "TI",
        [EDocumentType.Pasaporte] = "PA",
        [EDocumentType.PermisoProteccionTemporal] = "PPT",
        [EDocumentType.Nit] = "NIT"
    };

    public static readonly IReadOnlyDictionary<string, EDocumentType> FromCode =
        ToCode.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static bool TryParse(string? code, out EDocumentType value)
        => FromCode.TryGetValue((code ?? string.Empty).ToUpperInvariant(), out value);
}