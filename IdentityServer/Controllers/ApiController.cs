using System.Linq;

using IdentityServer.Data;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

namespace IdentityServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ApiController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("Hello")]
    public string Hello()
    {
        return "Hello from IdentityServer";
    }

    [HttpGet("Users/{ids}")]
    public IEnumerable<ApplicationUserDTO> GetUsersById([FromQuery] string[] ids)
    {
        Console.WriteLine("hi from identity server");
        var users = _dbContext.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new ApplicationUserDTO { Id = u.Id, Name = u.UserName, Avatar = u.Avatar })
            .ToList();
        return users;
    }

    [HttpGet("Users")]
    public IEnumerable<ApplicationUserDTO> GetAllUsers()
    {
        Console.WriteLine("hi from identity server");
        var users = _dbContext.Users
            .Select(u => new ApplicationUserDTO { Id = u.Id, Name = u.UserName, Avatar = u.Avatar })
            .ToList();
        return users;
    }

    [HttpGet("User/{id}")]
    public ApplicationUserDTO? GetUserById(string id)
    {
        Console.WriteLine("hi from identity server");
        ApplicationUserDTO? user = _dbContext.Users
            .Where(u => u.Id == id)
            .Select(u => new ApplicationUserDTO { Id = u.Id, Name = u.UserName, Avatar = u.Avatar })
            .FirstOrDefault();
        return user;
    }
}
