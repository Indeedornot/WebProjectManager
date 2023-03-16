using api.Database;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase {
    [HttpGet("Hello")]
    public string Hello() {
        return "Hello World";
    }
}