using api.Api;
using api.Database;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

[ApiController]
public class ProjectController : ControllerBase {
    private readonly IUserClient _userClient;
    private readonly DataContext _dbContext;
    private readonly EntityHandler _entityHandler;

    public ProjectController(IUserClient userClient, DataContext dbContext, EntityHandler entityHandler) {
        _userClient = userClient;
        _dbContext = dbContext;
        _entityHandler = entityHandler;
    }

    [HttpPost("Project/Create")]
    public async Task<IActionResult> CreateProject(ProjectDTO projectDto) {
        var project = new Project { Name = projectDto.Name, Description = projectDto.Description, Assignees = new List<ProjectUser>() };
        IEnumerable<ProjectUser> projectUsers = projectDto.Assignees.Select(x => new ProjectUser { UserId = x.Id, Projects = new List<Project> { project } });
        project.Assignees = projectUsers.ToList();

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("Project/Delete/{id:int}")]
    public async Task<IActionResult> DeleteProject(int id) {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null) {
            return NotFound();
        }

        //if user is not a part of the project, return 403
        if (!_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier))) {
            return Forbid();
        }

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("Project/Update")]
    public async Task<IActionResult> UpdateProject(ProjectUpdateDTO projectDto) {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == projectDto.Id);

        if (project == null) {
            return NotFound();
        }

        //if user is not a part of the project, return 403
        if (!_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier))) {
            return Forbid();
        }

        project.Name = projectDto.Name ?? project.Name;
        project.Description = projectDto.Description ?? project.Description;
        project.Status = projectDto.Status ?? project.Status;
        if (projectDto.DueDate.HasValue) {
            project.DueDate = projectDto.DueDate.Value;
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("Project/{id:int}")]
    public async Task<ProjectDTO?> GetProject(int id) {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null) {
            return null;
        }

        ProjectDTO projectDto = await _entityHandler.GetProjectWithUsers(project);
        return projectDto;
    }

    [HttpGet("Projects/{page:int}/{pageSize:int}")] //create a method getting projects from database with pagination
    public async Task<IEnumerable<ProjectDTO>> GetProjects(int page = 1, int pageSize = 10) {
        IEnumerable<Project> projects = _dbContext.Projects
            .Include(x => x.Assignees)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        IEnumerable<ProjectDTO> projectsDto = await _entityHandler.GetProjectsWithUsers(projects);
        return projectsDto;
    }

    [HttpDelete("Project/Leave/{id:int}")]
    public async Task<IActionResult> LeaveProject(int id) {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null) {
            return NotFound();
        }

        ProjectUser? user = project.Assignees.FirstOrDefault(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null) {
            return NotFound();
        }

        project.Assignees = project.Assignees.Where(x => x.UserId != user.UserId).ToList();
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("Project/Join/{id:int}")]
    public async Task<IActionResult> JoinProject(int id) {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null) {
            return NotFound();
        }

        //if user is already a part of the project, return 403
        if (_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier))) {
            return Forbid();
        }

        ProjectUser? user = _dbContext.Users
            .FirstOrDefault(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user != null) {
            user.Projects = user.Projects.Append(project);
        }
        else {
            user = new ProjectUser {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Projects = new List<Project>() { project }
            };
            _dbContext.Users.Add(user);
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}