using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Queries;

public record LoanSimulation(
    decimal Amount,
    EInstallmentsTerm Installments,
    decimal InstallmentValue,
    decimal TotalToPay,
    IEnumerable<SimulatedInstallment> Schedule
);

public record SimulatedInstallment(
    int Number,
    decimal Amount,
    DateTime DateExpiration
);