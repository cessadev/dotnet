using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Responses;

public record CustomerResponse(
    EDocumentType DocumentType,
    int DocumentNumber,
    string Name,
    string Lastname,
    int Age,
    string Address
);