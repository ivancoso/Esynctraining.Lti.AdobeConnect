using System;
using System.Web;
using EdugameCloud.PublicApi.Identity;
using EdugameCloud.PublicApi.Security.OAuth;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace EdugameCloud.PublicApi.Host
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }


        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<EdugameCloudUserManager>(() => new EdugameCloudUserManager());

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            /////app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, () => new EdugameCloudUserManager()),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                // TODO: config??
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(15),
                AllowInsecureHttp = HttpContext.Current.IsDebuggingEnabled,
            };

            // NOTE: on prod environment we use Access-Control-Allow- headers from the root website
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                app.Use(async (context, next) =>
                {
                    IOwinRequest req = context.Request;
                    IOwinResponse res = context.Response;
                    // for auth2 token requests
                    if (req.Path.StartsWithSegments(new PathString("/Token")))
                    {
                        // if there is an origin header
                        var origin = req.Headers.Get("Origin");
                        if (!string.IsNullOrEmpty(origin))
                        {
                            // allow the cross-site request
                            res.Headers.Set("Access-Control-Allow-Origin", origin);
                        }

                        // if this is pre-flight request
                        if (req.Method == "OPTIONS")
                        {
                            // respond immediately with allowed request methods and headers
                            res.StatusCode = 200;
                            res.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Methods", "GET", "POST");
                            res.Headers.AppendCommaSeparatedValues("Access-Control-Allow-Headers", "authorization", "content-type");
                            // no further processing
                            return;
                        }
                    }
                // continue executing pipeline
                await next();
                });
            }

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }

    }

}