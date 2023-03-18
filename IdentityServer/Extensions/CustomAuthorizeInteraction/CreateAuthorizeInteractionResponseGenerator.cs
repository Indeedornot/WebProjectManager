using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.ResponseHandling;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;

using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.Extensions.CustomAuthorizeInteraction;

public class CreateAuthorizeInteractionResponseGenerator : AuthorizeInteractionResponseGenerator {
    public CreateAuthorizeInteractionResponseGenerator(IdentityServerOptions options,
        ISystemClock clock,
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consent,
        IProfileService profile)
        : base(options, clock, logger, consent, profile) {
    }

    public override async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request, ConsentResponse consent = null) {
        if (!request.PromptModes.Contains("create")) {
            return await base.ProcessInteractionAsync(request, consent);
        }

        request.Raw.Remove("prompt");
        var response = new InteractionResponse { RedirectUrl = "/account/register" };

        return response;
    }
}