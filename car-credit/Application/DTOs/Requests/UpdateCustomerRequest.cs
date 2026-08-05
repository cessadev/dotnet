using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record UpdateCustomerRequest(
    [Required]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    string Name,

    [Required]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres.")]
    string Lastname,

    [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120.")]
    int Age,

    [Required]
    [StringLength(200, ErrorMessage = "La dirección de residencia no puede exceder los 200 caracteres.")]
    string Address
);