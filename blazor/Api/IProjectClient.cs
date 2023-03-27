using Refit;

using shared.Common;
using shared.Models;

namespace blazor.Api;

public interface IProjectClient {
    [Get(Routes.Hello)]
    Task<string> Hello();

    [Post(Routes.Project.Create)]
    Task CreateProject(ProjectCreateDTO project);

    [Delete(Routes.Project.Delete)]
    Task DeleteProject(int id);

    [Put(Routes.Project.Update)]
    Task UpdateProject(ProjectDTO project);

    [Get(Routes.Project.Get)]
    Task<ProjectDTO?> GetProject(int id);

    [Get(Routes.Project.GetAll)]
    Task<IEnumerable<ProjectDTO>> GetAllProjects();

    [Put(Routes.Project.Leave)]
    Task LeaveProject(int id);

    [Put(Routes.Project.Join)]
    Task JoinProject(int id);
}