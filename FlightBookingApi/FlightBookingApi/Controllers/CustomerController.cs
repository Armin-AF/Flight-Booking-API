using FlightBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

public class CustomerController : ControllerBase
{
    
    readonly List<Customer> _customers = new();

    public CustomerController(){
        _customers.Add(new Customer{
            customer_id = "1",
            first_name = "John",
            last_name = "Doe",
            email = "   ",
            phone = "   ",
            address = "   ",
            city = "   ",
            country = "   ",
            zip = "   "
        });
        _customers.Add(new Customer{
            customer_id = "2",
            first_name = "Jane",
            last_name = "Doe",
            email = "   ",
            phone = "   ",
            address = "   ",
            city = "   ",
            country = "   ",
            zip = "   "
        });
        _customers.Add(new Customer{
            customer_id = "3",
            first_name = "John",
            last_name = "Smith",
            email = "   ",
            phone = "   ",
            address = "   ",
            city = "   ",
            country = "   ",
            zip = "   "
        });
        _customers.Add(new Customer{
            customer_id = "4",
            first_name = "Jane",
            last_name = "Smith",
            email = "   ",
            phone = "   ",
            address = "   ",
            city = "   ",
            country = "   ",
            zip = "   "
        });
    }
    
    [HttpGet]
    public IActionResult GetCustomers()
    {
        return Ok(_customers);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetCustomer(string id)
    {
        var customer = _customers.FirstOrDefault(customer => customer.customer_id == id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
    
    [HttpPost]
    public IActionResult PostCustomer(Customer customer)
    {
        _customers.Add(customer);
        return CreatedAtAction(nameof(GetCustomer), new {id = customer.customer_id}, customer);
    }
    
    [HttpPut("{id}")]
    public IActionResult PutCustomer(string id, Customer customer)
    {
        var existingCustomer = _customers.FirstOrDefault(c => c.customer_id == id);
        if (existingCustomer == null) return NotFound();
        existingCustomer.first_name = customer.first_name;
        existingCustomer.last_name = customer.last_name;
        existingCustomer.email = customer.email;
        existingCustomer.phone = customer.phone;
        existingCustomer.address = customer.address;
        existingCustomer.city = customer.city;
        existingCustomer.country = customer.country;
        existingCustomer.zip = customer.zip;
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(string id)
    {
        var existingCustomer = _customers.FirstOrDefault(c => c.customer_id == id);
        if (existingCustomer == null) return NotFound();
        _customers.Remove(existingCustomer);
        return NoContent();
    }
}