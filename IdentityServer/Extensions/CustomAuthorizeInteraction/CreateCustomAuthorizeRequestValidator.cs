using Duende.IdentityServer.Validation;

namespace IdentityServer.Extensions.CustomAuthorizeInteraction;

public class CreateCustomAuthorizeRequestValidator : ICustomAuthorizeRequestValidator {
    public Task ValidateAsync(CustomAuthorizeRequestValidationContext context) {
        string prompt = context.Result.ValidatedRequest.Raw.Get("prompt");
        if (!string.IsNullOrWhiteSpace(prompt) &&
            prompt.Equals("create", StringComparison.OrdinalIgnoreCase)) {
            context.Result.ValidatedRequest.PromptModes = new[] { "create" };
        }

        return Task.CompletedTask;
    }
}