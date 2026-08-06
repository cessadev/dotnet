using System.ComponentModel.DataAnnotations;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Requests;

public record SimulateLoanRequest(
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "El monto debe ser mayor a cero.")]
    decimal Amount,

    [Required] EInstallmentsTerm Installments,

    [StringLength(20, ErrorMessage = "El identificador del vehículo no debe exceder los 20 caracteres.")]
    string? VehicleIdentifier
);