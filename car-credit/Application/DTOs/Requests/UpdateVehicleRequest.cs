using System.ComponentModel.DataAnnotations;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Requests;

public record UpdateVehicleRequest(
    [Required] EVehicleBrand Brand,

    [Required]
    [StringLength(50, ErrorMessage = "El modelo del vehículo no debe exceder los 50 caracteres.")]
    string Model,

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "El valor de mercado debe ser mayor a cero")]
    decimal MarketValue,

    [Range(1980, 2100, ErrorMessage = "El año debe corresponder a la realidad.")]
    int Year
);