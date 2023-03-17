using api.Api;
using api.Database;

using Microsoft.AspNetCore.Mvc;

using shared.Common;
using shared.Models;

using System.Diagnostics;
using System.Security.Claims;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase {
    private readonly UserClient _userClient;

    public ProjectController(HttpClient httpClient, UserClient userClient) {
        _userClient = userClient;
    }

    [HttpGet("Hello")]
    public async Task<string> Hello() {
        string? response = await _userClient.GetHello();
        return response ?? "No response";
    }
}