using IdentityServer.Data;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

using System.Linq;

namespace IdentityServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase {
    private readonly ApplicationDbContext _dbContext;

    public ApiController(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    [HttpGet("Hello")]
    public string Hello() {
        return "Hello from IdentityServer";
    }

    [HttpGet("UserData")]
    public IEnumerable<ApplicationUserDTO> GetUserData([FromQuery] string[] ids) {
        Console.WriteLine("hi from identity server");
        var users = _dbContext.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new ApplicationUserDTO { Id = u.Id, Name = u.UserName, Avatar = u.Avatar })
            .ToList();
        return users;
    }
}