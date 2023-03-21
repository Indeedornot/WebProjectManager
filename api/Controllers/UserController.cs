using api.Api;
using api.Database;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using shared.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
    private readonly IUserClient _userClient;
    private readonly DataContext _dbContext;
    private readonly EntityHandler _entityHandler;

    public UserController(IUserClient userClient, DataContext dbContext, EntityHandler entityHandler) {
        _userClient = userClient;
        _dbContext = dbContext;
        _entityHandler = entityHandler;
    }

    [HttpGet("Users/{page:int}/{pageSize:int}")]
    public async Task<IEnumerable<UserDTO>> GetUsers(int page = 1, int pageSize = 10) {
        IEnumerable<ProjectUser> users = _dbContext.Users
            .Include(x => x.Projects)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        IEnumerable<string> userIds = users.Select(x => x.UserId);
        IEnumerable<ApplicationUserDTO> userData = await _userClient.GetUserData(userIds);

        IEnumerable<UserDTO> usersDto = userData.Select(x => new UserDTO {
            Id = x.Id, Name = x.Name, Avatar = x.Avatar, Projects = _entityHandler.GetProjectIdsFromUsers(users, x.Id)
        });

        return usersDto;
    }

    [HttpGet("User/{id}")]
    public async Task<UserDTO?> GetUser(string id) {
        ProjectUser? user = _dbContext.Users
            .Include(x => x.Projects)
            .FirstOrDefault(x => x.UserId == id);
        if (user == null) {
            return null;
        }

        return await _entityHandler.GetUser(user);
    }

    [HttpGet("UserProjects/{id}")]
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