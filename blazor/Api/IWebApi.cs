using Refit;

using shared.Common;

namespace blazor.Api;

public interface IWebApi {
    [Get(Routes.Hello)]
    Task<string> Hello();
}