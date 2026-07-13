using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository _r)
    {
        _repository = _r;
    }

    public async Task<IEnumerable<CustomerResponse>> GetAll()
    {
        IEnumerable<Customer> customers = await _repository.GetAll();

        return customers.Select(c => new CustomerResponse(
            c.Id,
            c.Name,
            c.LastName,
            c.Age,
            c.Address
        ));
    }

    public async Task<CustomerResponse?> GetById(int customerId)
    {
        CustomerResponse customer = await _repository.GetById(customerId);
        
        return customer is null ? null : new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.LastName,
            customer.Age,
            customer.Address
        );
    }

    public async Task<CustomerResponse> Create(CreateCustomerRequest request)
    {
        Customer customer = new Customer
        {
            Name = request.Name,
            Lastname = request.Lastname,
            Age = request.Age,
            Address = request.Address
        };

        _repository.Add(customer);
        await _repository.SaveChanges();

        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Lastname,
            customer.Age,
            customer.Address
        );
    }

    public async Task<bool> Delete(int customerId)
    {
        Customer customer = await _repository.GetById(customerId);
        if (customer is null) return false;

        if (await _repository.HasUnpaidFees(customerId))
        {
            throw new InvalidOperationException(
                "The customer cannot be deleted because at least one of their loans has outstanding payments.");
        }

        _repository.Remove(customer);
        await _repository.SaveChanges();

        return true;
    }
}