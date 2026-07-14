using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record CreateCustomerRequest(
    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    string Name,

    [Required]
    [StringLength(100, ErrorMessage = "Lastname cannot exceed 100 characters.")]
    string Lastname,

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120.")]
    int Age,

    [Required]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    string Address
);