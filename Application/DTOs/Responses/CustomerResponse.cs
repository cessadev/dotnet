namespace CarCredit.Application.DTOs.Responses;

public record CustomerResponse(
    int Id,
    string Name,
    string Lastname,
    int Age,
    string Address
);