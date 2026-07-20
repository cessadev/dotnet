using System.ComponentModel.DataAnnotations;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Requests;

public record CreateVehicleRequest(
    [Required]
    [StringLength(20, ErrorMessage = "Identifier cannot exceed 20 characters.")]
    string Identifier,

    [Required] EVehicleBrand Brand,

    [Required]
    [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
    string Model,

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "MarketValue must be greater than zero.")]
    decimal MarketValue,

    [Range(1980, 2100, ErrorMessage = "Year must be a realistic value.")]
    int Year
);