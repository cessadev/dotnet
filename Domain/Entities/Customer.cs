using CarCredit.Domain.Enums;

namespace CarCredit.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public EDocumentType DocumentType { get; set; }
    public int DocumentNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}