using System.ComponentModel.DataAnnotations;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Requests;

public record CreateLoanRequest(
    [Range(1, int.MaxValue, ErrorMessage = "CustomerDocumentNumber must be greater than zero.")]
    int CustomerDocumentNumber,

    [Required]
    [StringLength(20, ErrorMessage = "VehicleIdentifier cannot exceed 20 characters.")]
    string VehicleIdentifier,

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Amount must be greater than zero.")]
    decimal Amount,

    [Required] EInstallmentsTerm Installments
);