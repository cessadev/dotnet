namespace CarCredit.Application.DTOs.Responses;

public record CustomerResponse (
    int Id,
    string Name,
    string LastName,
    int Age,
    string Address
);