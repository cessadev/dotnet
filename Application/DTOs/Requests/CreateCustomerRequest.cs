using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record CreateCustomerRequest(
    [Required]
    [StringLength(100)]
    string Name,

    [Required]
    [StringLength(100)]
    string Lastname,

    [Range(18, 120)]
    int Age,

    [Required]
    [StringLength(200)]
    string Address
);