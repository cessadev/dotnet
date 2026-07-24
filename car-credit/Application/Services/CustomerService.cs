using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerResponse>> GetAll()
    {
        IEnumerable<Customer> customers = await _customerRepository.GetAll();
        return customers.Select(ToResponse);
    }

    public async Task<CustomerResponse?> GetByDocumentNumber(int documentNumber)
    {
        Customer? customer = await _customerRepository.GetByDocumentNumber(documentNumber);
        return customer is null ? null : ToResponse(customer);
    }

    public async Task<CustomerResponse> Create(CreateCustomerRequest request)
    {
        Customer customer = new Customer
        {
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            Name = request.Name,
            Lastname = request.Lastname,
            Age = request.Age,
            Address = request.Address
        };

        await _customerRepository.Add(customer);
        await _customerRepository.SaveChanges();

        return ToResponse(customer);
    }

    public async Task<bool> Delete(int documentNumber)
    {
        Customer? customer = await _customerRepository.GetByDocumentNumber(documentNumber);
        if (customer is null) return false;

        if (await _customerRepository.HasUnpaidInstallments(documentNumber))
            throw new InvalidOperationException(
                "The customer cannot be deleted because at least one of their loans has outstanding installments.");

        await _customerRepository.Remove(customer);
        await _customerRepository.SaveChanges();

        return true;
    }

    private static CustomerResponse ToResponse(Customer c) => new(
        c.DocumentType, c.DocumentNumber, c.Name, c.Lastname, c.Age, c.Address
    );
}