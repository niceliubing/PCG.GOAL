using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using PCG.GOAL.Common.Interface;

namespace PCG.GOAL.WebService.Security
{
    public class GoalOAuthProvider : OAuthAuthorizationServerProvider
    {
        [Inject]
        public IOAuthValidator Validator { get; set; }

        public GoalOAuthProvider()
        {
            Validator = WebApiApplication.Kernel.Get<IOAuthValidator>();
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                string clientId, clientSecret;
                if (context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret))
                {
                    if (Validator.ValidateClient(clientId, clientSecret))
                    {
                        context.Validated();
                    }
                }
                else
                {
                    context.SetError("Invalid credentials");
                    context.Rejected();
                }
            }
            catch (Exception e)
            {
                context.SetError("Server error");
                context.Rejected();
            }

        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            if (!Validator.ValidateUser(context.UserName, context.Password))
            {
                context.Rejected();
                return;
            }

            // create identity
            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim("sub", context.UserName));

            // create metadata to pass on to refresh token provider
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {"oauth:client_id", context.ClientId}
                });

            var ticket = new AuthenticationTicket(id, props);
            context.Validated(ticket);
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["oauth:client_id"];
            var currentClient = context.ClientId;

            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
                return;
            }

            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
        }
    }
}
