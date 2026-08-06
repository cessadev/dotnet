namespace CarCredit.Application.DTOs.Responses;

public record VehicleEligibilityResponse(
    string VehicleIdentifier,
    bool IsEligible,
    string? ActiveLoanReference,
    string? Reason
);