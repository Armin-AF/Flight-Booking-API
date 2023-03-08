using FlightBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

public class UserController : ControllerBase
{
    readonly List<User> _users = new();

    public UserController(){
        _users.Add(new User{
            user_id = "1",
            name = "John Doe",
            email = "JohnD@mail.com",
            password = "1234",
            role = "admin"
        });
        _users.Add(new User{
            user_id = "2",
            name = "Jane Doe",
            email = "JaneD@mail.com",
            password = "1234",
            role = "user"
        });
    }
    
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_users);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetUser(string id)
    {
        var user = _users.FirstOrDefault(user => user.user_id == id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpPost]
    public IActionResult PostUser(User user)
    {
        _users.Add(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, user);
    }
    
    [HttpPut("{id}")]
    public IActionResult PutUser(string id, User user)
    {
        var existingUser = _users.FirstOrDefault(user => user.user_id == id);
        if (existingUser == null) return NotFound();
        existingUser.name = user.name;
        existingUser.email = user.email;
        existingUser.password = user.password;
        existingUser.role = user.role;
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(string id)
    {
        var user = _users.FirstOrDefault(user => user.user_id == id);
        if (user == null) return NotFound();
        _users.Remove(user);
        return NoContent();
    }
    
}