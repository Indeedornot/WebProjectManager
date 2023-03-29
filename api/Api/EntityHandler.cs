using api.Database;

using shared.Models;

namespace api.Api;

public class EntityHandler
{
    private readonly IUserClient _userClient;
    private readonly DataContext _dbContext;

    public EntityHandler(IUserClient userClient, DataContext dbContext)
    {
        _userClient = userClient;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets users connected to the project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserDTO>> GetUsersByProject(Project project)
    {
        IEnumerable<string> assignees = project.Assignees.Select(x => x.UserId);
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetUsersByIds(assignees);
        return users.Select(u => new UserDTO { Id = u.Id, Name = u.Name, Avatar = u.Avatar });
    }

    /// <summary>
    /// Gets all distinct users connected to the projects
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserDTO>> GetUsersByProjects(IEnumerable<Project> projects)
    {
        // Get all distinct user ids
        IEnumerable<string> assignees = projects
            .SelectMany(p => p.Assignees)
            .Select(x => x.UserId)
            .Distinct();

        // Get their data
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetUsersByIds(assignees);
        return users.Select(u => new UserDTO { Id = u.Id, Name = u.Name, Avatar = u.Avatar });
    }

    /// <summary>
    /// Gets users connected to the project from the list of all users
    /// </summary>
    /// <param name="project"></param>
    /// <param name="appUsers">
    /// Previously fetched users <see cref="GetUsersByProjects" />
    /// </param>
    /// <returns></returns>
    public IEnumerable<UserDTO> GetProjectUsers(Project project, IEnumerable<UserDTO> appUsers)
    {
        return appUsers.Where(u => project.Assignees.Any(a => a.UserId == u.Id));
    }

    /// <summary>
    /// Connects previously fetched users to the projects
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="users"></param>
    /// <returns></returns>
    public IEnumerable<ProjectDTO> ConnectProjectsWithUsers(IEnumerable<Project> projects, IEnumerable<UserDTO> users)
    {
        return projects.Select(p => new ProjectDTO
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            DueDate = p.DueDate,
            Status = p.Status,
            Assignees = GetProjectUsers(p, users)
        });
    }

    /// <summary>
    /// Fetches users and connects them to the projects
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ProjectDTO>> GetProjectsWithUsers(IEnumerable<Project> projects)
    {
        IEnumerable<UserDTO> users = await GetUsersByProjects(projects);
        return ConnectProjectsWithUsers(projects, users);
    }

    /// <summary>
    /// Fetches users and connects them to the project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task<ProjectDTO> GetProjectWithUsers(Project project)
    {
        IEnumerable<UserDTO> users = await GetUsersByProject(project);
        return new ProjectDTO
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            DueDate = project.DueDate,
            Status = project.Status,
            Assignees = users
        };
    }

    /// <summary>
    /// Returns ids of projects that the user is connected to
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IEnumerable<int> GetProjectIdsForUser(string userId)
    {
        IQueryable<int> projectIds = _dbContext.Users
            .Where(pu => pu.UserId == userId)
            .SelectMany(pu => pu.Projects)
            .Select(p => p.Id);

        return projectIds;
    }

    public bool IsUserInProject(Project project, string userId)
    {
        return project.Assignees.Any(a => a.UserId == userId);
    }

    /// <summary>
    /// Gets Users that are already connected to some project
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public IEnumerable<ProjectUser> GetExistingUsers(IEnumerable<string> userIds)
    {
        return _dbContext.Users.Where(u => userIds.Contains(u.UserId));
    }

    /// <summary>
    /// Gets Users that are not connected to any project
    /// </summary>
    /// <param name="users"></param>
    /// <returns></returns>
    public IEnumerable<ProjectUser> GetNewUsers(IEnumerable<string> users)
    {
        return users.Where(u => !_dbContext.Users.Any(x => x.UserId == u))
            .Select(x => new ProjectUser() { UserId = x });
    }
}
