using System.ComponentModel.DataAnnotations;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Requests;

public record CreateCustomerRequest(
    [Required] EDocumentType DocumentType,

    [Range(1, int.MaxValue, ErrorMessage = "DocumentNumber must be greater than zero.")]
    int DocumentNumber,

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