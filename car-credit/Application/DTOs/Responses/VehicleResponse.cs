using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Responses;

public record VehicleResponse(
    string Identifier,
    EVehicleBrand Brand,
    string Model,
    decimal MarketValue,
    int Year
);