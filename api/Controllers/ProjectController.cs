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
    private readonly IUserClient _userClient;

    public ProjectController(HttpClient httpClient, IUserClient userClient) {
        _userClient = userClient;
    }

    [HttpGet("Hello")]
    public async Task<string> Hello() {
        string? response = await _userClient.GetHello();
        return response ?? "No response";
    }

    [HttpGet("UserData")]
    public async Task<ApplicationUserDTO?> GetUserData(string id) {
        IEnumerable<ApplicationUserDTO>? data = await _userClient.GetUserData(new[] { id });
        return data.FirstOrDefault();
    }
}