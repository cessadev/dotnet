using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Responses;

public record LoanResponse(
    string Reference,
    int CustomerDocumentNumber,
    string VehicleIdentifier,
    decimal Amount,
    EInstallmentsTerm Installments,
    DateTime DateCreation
);