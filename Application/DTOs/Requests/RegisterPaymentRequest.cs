using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record RegisterPaymentRequest(
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Amount must be greater than zero.")]
    decimal Amount
);