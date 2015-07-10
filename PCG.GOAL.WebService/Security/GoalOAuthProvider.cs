using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace PCG.GOAL.WebService.Security
{
    public class GoalOAuthProvider: OAuthAuthorizationServerProvider
    {
       
            //protected IStoriUow Uow { get; set; }

            //private readonly IRepository<ApiUser> _apiUsers;
            //private readonly IRepository<ApiAppRegistration> _apiAppRegistrations;

            public GoalOAuthProvider()
            {
                
                //Uow = WebApiApplication.Kernel.Get<IStoriUow>();
                //_apiUsers = Uow.GetRepository<ApiUser>();
                //_apiAppRegistrations = Uow.GetRepository<ApiAppRegistration>();
            }

            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                var c = context.OwinContext;
                // validate client credentials
                // should be stored securely (salted, hashed, iterated)

                try
                {
                    string id, secret;
                    // TryGetBasicCredentials -- for Basic authentication credentials
                    // TryGetFormCredentials -- for Form encoded POST from the http request body
                    if (context.TryGetBasicCredentials(out id, out secret)
                        || context.TryGetFormCredentials(out id, out secret))
                    {
                        //DONE - check DB to see if the Application Id is regitered and the Application secret is valid
                        //var app = await _apiAppRegistrations.FirstAsync(x => x.AppCode == id);
                        //if (app != null && System.Web.Helpers.Crypto.VerifyHashedPassword(app.AppSecret, secret))
                        if(id=="goalview" &&secret=="goalview")
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
                    //LogHelper.GetLogger().Error("ValidateClientAuthentication", e);
                }

            }

            /// <summary>
            /// Called when a request to the Token endpoint arrives with a "grant_type" of "password".
            /// if the request is validated, the middleware will issue a access token
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                // validate user credentials (demo mode)
                // should be stored securely (salted, hashed, iterated)

                /*
                var user = _apiUsers.First(x => x.UserName == context.UserName);
                if (user == null || !System.Web.Helpers.Crypto.VerifyHashedPassword(user.Password, context.Password))
                {
                    context.Rejected();
                    //return;
                }
                */
                // todo: validate with credentials against database
                if (context.UserName != "admin" || context.Password != "admin")
                {
                    context.Rejected();
                    return;
                }

                // create identity
                var id = new ClaimsIdentity(context.Options.AuthenticationType);
                id.AddClaim(new Claim("sub", context.UserName));
                //id.AddClaim(new Claim("role", user.Role));




                // create metadata to pass on to refresh token provider
                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {"goal:client_id", context.ClientId}
                });

                var ticket = new AuthenticationTicket(id, props);
                context.Validated(ticket);
            }

            /// <summary>
            /// Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token"
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
            {
                var originalClient = context.Ticket.Properties.Dictionary["goal:client_id"];
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
