using api.Api;
using api.Database;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using shared.Models;

namespace api.Controllers;

[ApiController]
public class UserController : ControllerBase {
    private readonly IUserClient _userClient;
    private readonly DataContext _dbContext;
    private readonly EntityHandler _entityHandler;

    public UserController(IUserClient userClient, DataContext dbContext, EntityHandler entityHandler) {
        _userClient = userClient;
        _dbContext = dbContext;
        _entityHandler = entityHandler;
    }

    [HttpGet("Users")]
    public async Task<IEnumerable<UserDTO>> GetUsers() {
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetAllUsers();

        IEnumerable<UserDTO> usersDto = users.Select(x => new UserDTO {
            Id = x.Id,
            Name = x.Name,
            Avatar = x.Avatar,
            Projects = _entityHandler.GetProjectIdsForUser(x.Id)
        });

        return usersDto;
    }

    [HttpGet("User/{id}")]
    public async Task<UserDTO?> GetUser(string id) {
        ApplicationUserDTO? user = await _userClient.GetUserById(id);
        if (user == null) {
            return null;
        }

        IEnumerable<int>? projects = _entityHandler.GetProjectIdsForUser(id);
        var userDto = new UserDTO { Id = user.Id, Name = user.Name, Avatar = user.Avatar, Projects = projects };
        return userDto;
    }

    [HttpGet("User/Projects/{id}")]
    public async Task<IEnumerable<ProjectDTO>> GetUserProjects(string id) {
        ProjectUser? user = _dbContext.Users
            .Include(x => x.Projects)
            .FirstOrDefault(x => x.UserId == id);
        if (user == null) {
            throw new Exception("User not found");
        }

        IEnumerable<ProjectDTO> projectsDto = await _entityHandler.GetProjectsWithUsers(user.Projects);
        return projectsDto;
    }
}