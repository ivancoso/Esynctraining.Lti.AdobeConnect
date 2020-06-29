using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EdugameCloud.PublicApi.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace EdugameCloud.PublicApi.Security.OAuth
{
    public sealed class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly Func<EdugameCloudUserManager> _userManagerFactory;


        public ApplicationOAuthProvider(string publicClientId, Func<EdugameCloudUserManager> userManagerFactory)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException(nameof(publicClientId));
            _userManagerFactory = userManagerFactory ?? throw new ArgumentNullException(nameof(userManagerFactory));
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (EdugameCloudUserManager userManager = _userManagerFactory())
            {
                EdugameCloudUser user = await userManager.FindAsync(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user,
                    context.Options.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
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
