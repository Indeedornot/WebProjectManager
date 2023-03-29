using System.Security.Claims;

using api.Api;
using api.Database;

using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

using shared.Models;

[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IUserClient _userClient;
    private readonly DataContext _dbContext;
    private readonly EntityHandler _entityHandler;

    public ProjectController(IUserClient userClient, DataContext dbContext, EntityHandler entityHandler)
    {
        _userClient = userClient;
        _dbContext = dbContext;
        _entityHandler = entityHandler;
    }

    [HttpPost("Project/Create")]
    public async Task CreateProject(ProjectCreateDTO projectDto)
    {
        var project = new Project
        {
            Name = projectDto.Name,
            Status = projectDto.Status,
            Description = projectDto.Description,
            DueDate = projectDto.DueDate,
            Assignees = new List<ProjectUser>()
        };

        var existingUsers = _entityHandler.GetExistingUsers(projectDto.Assignees).ToList();

        var newUsers = _entityHandler.GetNewUsers(projectDto.Assignees).ToList();
        if (newUsers.Count > 0)
        {
            await _dbContext.Users.AddRangeAsync(newUsers);
            await _dbContext.SaveChangesAsync();
        }

        var projectUsers = existingUsers.Concat(newUsers).ToList();
        project.Assignees = projectUsers;

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();
    }

    [HttpDelete("Project/Delete/{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        //if user is not a part of the project, return 403
        if (!_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            return Forbid();
        }

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("Project/Update")]
    public async Task<IActionResult> UpdateProject(ProjectUpdateDTO projectDto)
    {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == projectDto.Id);

        if (project == null)
        {
            return NotFound();
        }

        //if user is not a part of the project, return 403
        if (!_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            return Forbid();
        }

        project.Name = projectDto.Name ?? project.Name;
        project.Description = projectDto.Description ?? project.Description;
        project.Status = projectDto.Status ?? project.Status;
        if (projectDto.DueDate.HasValue)
        {
            project.DueDate = projectDto.DueDate.Value;
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("Project/{id:int}")]
    public async Task<ProjectDTO?> GetProject(int id)
    {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
        {
            return null;
        }

        ProjectDTO projectDto = await _entityHandler.GetProjectWithUsers(project);
        return projectDto;
    }

    [HttpGet("Projects")]
    public async Task<IEnumerable<ProjectDTO>> GetProjects()
    {
        IEnumerable<Project> projects = _dbContext.Projects
            .Include(x => x.Assignees);

        IEnumerable<ProjectDTO> projectsDto = await _entityHandler.GetProjectsWithUsers(projects);
        return projectsDto;
    }

    [HttpDelete("Project/Leave/{id:int}")]
    public async Task<IActionResult> LeaveProject(int id)
    {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        ProjectUser? user = project.Assignees.FirstOrDefault(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            return NotFound();
        }

        project.Assignees = project.Assignees.Where(x => x.UserId != user.UserId).ToList();
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("Project/Join/{id:int}")]
    public async Task<IActionResult> JoinProject(int id)
    {
        Project? project = _dbContext.Projects
            .Include(x => x.Assignees)
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        //if user is already a part of the project, return 403
        if (_entityHandler.IsUserInProject(project, User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            return Forbid();
        }

        ProjectUser? user = _dbContext.Users
            .FirstOrDefault(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user != null)
        {
            user.Projects = user.Projects.Append(project);
        }
        else
        {
            user = new ProjectUser { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), Projects = new List<Project>() { project } };
            _dbContext.Users.Add(user);
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
