using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase {
    [HttpGet("Hello")]
    public string Hello() {
        return "Hello from IdentityServer";
    }
}