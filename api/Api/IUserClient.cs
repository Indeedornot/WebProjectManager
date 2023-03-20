using Refit;

using shared.Models;

namespace api.Api;

public interface IUserClient {
    [Get("/api/Hello")]
    Task<string?> GetHello();

    [Get("/api/UserData")]
    Task<IEnumerable<ApplicationUserDTO>> GetUserData([Query(CollectionFormat.Multi)] IEnumerable<string> ids);
}