using Refit;

using shared.Models;

namespace api.Api;

public interface IUserClient {
    [Get("/api/Hello")]
    Task<string?> GetHello();

    [Get("/api/Users")]
    Task<IEnumerable<ApplicationUserDTO>> GetUsersByIds([Query(CollectionFormat.Multi)] IEnumerable<string> ids);

    [Get("/api/Users")]
    Task<IEnumerable<ApplicationUserDTO>> GetAllUsers();

    [Get("/api/User/{id}")]
    Task<ApplicationUserDTO?> GetUserById(string id);
}