using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.OwinSecurity.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Esynctraining.AdobeConnect.OwinSecurity.Security.OAuth
{
    public sealed class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly Func<AdobeConnectUserManager> _userManagerFactory;


        public ApplicationOAuthProvider(string publicClientId, Func<AdobeConnectUserManager> userManagerFactory)
        {
            if (publicClientId == null)
                throw new ArgumentNullException(nameof(publicClientId));

            if (userManagerFactory == null)
                throw new ArgumentNullException(nameof(userManagerFactory));

            _publicClientId = publicClientId;
            _userManagerFactory = userManagerFactory;
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (AdobeConnectUserManager userManager = _userManagerFactory())
            {
                AdobeConnectUser user = null;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch (Exception ex)
                {
                    // TODO: production-ready exceptions
                    context.SetError("server_error", ex.Message);
                    return;
                }

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user,
                    context.Options.AuthenticationType);

                oAuthIdentity.AddClaim(new Claim("c_token", user.CompanyToken));
                oAuthIdentity.AddClaim(new Claim("ac_domain", user.AcDomain));
                oAuthIdentity.AddClaim(new Claim("ac_session", user.AcSessionToken));
                if (user.Roles != null)
                {
                    foreach (var role in user.Roles)
                    {
                        var roleClaim = new Claim(ClaimTypes.Role, role);
                        oAuthIdentity.AddClaim(roleClaim);
                    }
                }

                // cookie: ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,
                // cookie: CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
//                _logger.Info($"[GrantResourceOwnerCredentials] PrincipalId={user.Id}, ACSession={context.Ticket.Identity.FindFirst("ac_session")}");
                // cookie: context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var identity = context.Ticket.Identity;
            var id = identity.GetUserId();
            var domain = identity.FindFirst("ac_domain");
            var companyToken = identity.FindFirst("c_token");
            AdobeConnectUser user = null;
            using (AdobeConnectUserManager userManager = _userManagerFactory())
            {
                try
                {
                    user = await userManager.RefreshSession(id, companyToken.Value, domain.Value, identity.Name);
//                    _logger?.Info($"[GrantRefreshToken.AfterLogin] ACSession={user.AcSessionToken}");
                }
                catch (Exception ex)
                {
                    // TODO: production-ready exceptions
                    context.SetError("server_error", ex.Message);
                    return;
                }
            }

            if (user == null)
            {
                context.SetError("token_refresh_error", "User session has not been updated successfully.");
                return;
            }
            // check for existing claim and remove it
//            var existingClaim = identity.FindFirst("ac_session");
//            if (existingClaim != null)
            var existingClaims = identity.Claims.Where(x => x.Type == "ac_session" || x.Type == ClaimTypes.Role);
            foreach (var existingClaim in existingClaims)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim("ac_session", user.AcSessionToken));

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    var roleClaim = new Claim(ClaimTypes.Role, role);
                    identity.AddClaim(roleClaim);
                }
            }
            context.Validated(context.Ticket);
//            _logger?.Info($"[GrantRefreshToken] PrincipalId={user.Id}, ACSession={context.Ticket.Identity.FindFirst("ac_session")}");
            //            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        //{
        //    if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
        //    {
        //        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        //        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "authorization", "content-type" });
        //        context.RequestCompleted();
        //        return Task.FromResult(0);
        //    }

        //    return base.MatchEndpoint(context);
        //}

    }
}
