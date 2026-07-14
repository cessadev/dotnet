using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record CreateCreditRequest(
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than zero.")]
    int CustomerId,

    [Required]
    [StringLength(100, ErrorMessage = "Vehicle cannot exceed 100 characters.")]
    string Vehicle,

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "ValueCredit must be greater than zero.")]
    decimal ValueCredit,

    [Range(1, 36, ErrorMessage = "Fee must be between 1 and 36.")]
    int Fee
);