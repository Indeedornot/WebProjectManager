using Refit;

using shared.Common;
using shared.Models;

namespace blazor.Api;

public interface IUserClient
{
    [Get(Routes.User.Get)]
    Task<UserDTO?> GetUser(string id);

    [Get(Routes.User.GetAll)]
    Task<IEnumerable<UserDTO>> GetAllUsers();

    [Get(Routes.User.GetProjects)]
    Task<IEnumerable<ProjectDTO>> GetUserProjects(string id);
}
