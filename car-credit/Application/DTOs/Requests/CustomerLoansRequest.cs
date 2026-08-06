using System.ComponentModel.DataAnnotations;

namespace CarCredit.Application.DTOs.Requests;

public record CustomerLoansRequest(
    [Required(ErrorMessage = "El tipo de documento es requerido.")]
    string DocumentType,

    [Required(ErrorMessage = "El número de documento es requerido.")]
    int DocumentNumber
);