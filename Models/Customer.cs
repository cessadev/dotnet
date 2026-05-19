using System.ComponentModel.DataAnnotations.Schema;

namespace CarCredit.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}

public record CreateCustomerRequest(
    string Name,
    string Lastname,
    int Age,
    string Address
);