using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs;

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