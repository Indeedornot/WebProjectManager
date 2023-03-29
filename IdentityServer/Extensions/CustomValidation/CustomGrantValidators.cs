using System.Security.Claims;
using System.Text.Json;

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

using IdentityModel;

namespace IdentityServer.Extensions.CustomValidation;

public class CustomGrantValidators
{
    public class TokenExchangeGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public TokenExchangeGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // defaults
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
            var customResponse =
                new Dictionary<string, object> { { OidcConstants.TokenResponse.IssuedTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken } };

            string subjectToken = context.Request.Raw.Get(OidcConstants.TokenRequest.SubjectToken);
            string subjectTokenType = context.Request.Raw.Get(OidcConstants.TokenRequest.SubjectTokenType);

            // mandatory parameters
            if (string.IsNullOrWhiteSpace(subjectToken))
            {
                return;
            }

            if (!string.Equals(subjectTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken))
            {
                return;
            }

            TokenValidationResult validationResult = await _validator.ValidateAccessTokenAsync(subjectToken);
            if (validationResult.IsError)
            {
                return;
            }

            string sub = validationResult.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value;
            string clientId = validationResult.Claims.First(c => c.Type == JwtClaimTypes.ClientId).Value;

            string style = context.Request.Raw.Get("exchange_style");

            switch (style)
            {
                case "impersonation":
                    {
                        // set token client_id to original id
                        context.Request.ClientId = clientId;

                        context.Result = new GrantValidationResult(
                            sub,
                            GrantType,
                            customResponse: customResponse);
                        break;
                    }
                case "delegation":
                    {
                        // set token client_id to original id
                        context.Request.ClientId = clientId;

                        var actor = new { client_id = context.Request.Client.ClientId };

                        var actClaim = new Claim(JwtClaimTypes.Actor, JsonSerializer.Serialize(actor), IdentityServerConstants.ClaimValueTypes.Json);

                        context.Result = new GrantValidationResult(
                            sub,
                            GrantType,
                            new[] { actClaim },
                            customResponse: customResponse);
                        break;
                    }
                case "custom":
                    {
                        context.Result = new GrantValidationResult(
                            sub,
                            GrantType,
                            customResponse: customResponse);
                        break;
                    }
            }
        }

        public string GrantType => OidcConstants.GrantTypes.TokenExchange;
    }
}
