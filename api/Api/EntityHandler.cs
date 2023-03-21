using shared.Models;

namespace api.Api;

public class EntityHandler {
    private readonly IUserClient _userClient;

    public EntityHandler(IUserClient userClient) {
        _userClient = userClient;
    }

    /// <summary>
    /// Gets users connected to the project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserDTO>> GetUsersByProject(Project project) {
        IEnumerable<string> assignees = project.Assignees.Select(x => x.UserId);
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetUserData(assignees);
        return users.Select(u => new UserDTO { Id = u.Id, Name = u.Name, Avatar = u.Avatar });
    }

    /// <summary>
    /// Gets all distinct users connected to the projects
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserDTO>> GetUsersByProjects(IEnumerable<Project> projects) {
        IEnumerable<string> assignees = projects.SelectMany(p => p.Assignees).Select(x => x.UserId).Distinct();
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetUserData(assignees);
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
    public IEnumerable<UserDTO> GetProjectUsers(Project project, IEnumerable<UserDTO> appUsers) {
        return appUsers.Where(u => project.Assignees.Any(a => a.UserId == u.Id));
    }

    /// <summary>
    /// Connects previously fetched users to the projects
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="users"></param>
    /// <returns></returns>
    public IEnumerable<ProjectDTO> ConnectProjectsWithUsers(IEnumerable<Project> projects, IEnumerable<UserDTO> users) {
        return projects.Select(p => new ProjectDTO {
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
    public async Task<IEnumerable<ProjectDTO>> GetProjectsWithUsers(IEnumerable<Project> projects) {
        IEnumerable<UserDTO> users = await GetUsersByProjects(projects);
        return ConnectProjectsWithUsers(projects, users);
    }

    /// <summary>
    /// Fetches users and connects them to the project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public async Task<ProjectDTO> GetProjectWithUsers(Project project) {
        IEnumerable<UserDTO> users = await GetUsersByProject(project);
        return new ProjectDTO {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            DueDate = project.DueDate,
            Status = project.Status,
            Assignees = users
        };
    }

    public async Task<UserDTO?> GetUser(ProjectUser projectUser) {
        IEnumerable<ApplicationUserDTO> users = await _userClient.GetUserData(new[] { projectUser.UserId });
        ApplicationUserDTO user = users.First();
        return new UserDTO() { Avatar = user.Avatar, Id = user.Id, Name = user.Name, Projects = projectUser.Projects.Select(x => x.Id) };
    }

    public IEnumerable<int> GetProjectIdsFromUsers(IEnumerable<ProjectUser> users, string userId) {
        ProjectUser? projectUser = users.FirstOrDefault(u => u.UserId == userId);
        if (projectUser == null) {
            return Enumerable.Empty<int>();
        }

        return projectUser.Projects.Select(p => p.Id);
    }

    public bool IsUserInProject(Project project, string userId) {
        return project.Assignees.Any(a => a.UserId == userId);
    }
}