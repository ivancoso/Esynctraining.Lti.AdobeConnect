namespace EdugameCloud.Lti.OAuth
{
    using System;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using System.Web;

    using DotNetOpenAuth.AspNet;
    using DotNetOpenAuth.AspNet.Clients;

    /// <summary>
    /// The LTI open AUTH security manager.
    /// </summary>
    internal sealed class LtiOpenAuthSecurityManager : OpenAuthSecurityManager
    {
        #region Constants

        /// <summary>
        ///     The provider query string name.
        /// </summary>
        private const string ProviderQueryStringName = "__provider__";

        /// <summary>
        ///     The query string name for session id.
        /// </summary>
        private const string SessionIdQueryStringName = "__sid__";

        #endregion

        #region Fields

        /// <summary>
        /// The authentication provider.
        /// </summary>
        private readonly IAuthenticationClient authenticationProvider;

        /// <summary>
        /// The request context.
        /// </summary>
        private readonly HttpContextBase requestContext;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LtiOpenAuthSecurityManager"/> class.
        /// </summary>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        public LtiOpenAuthSecurityManager(
            HttpContextBase requestContext, 
            IAuthenticationClient provider, 
            IOpenAuthDataProvider dataProvider)
            : base(requestContext, provider, dataProvider)
        {
            this.requestContext = requestContext;
            this.authenticationProvider = provider;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The attach query string parameter.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name. This value should not be provided by an end user; the caller should
        ///     ensure that this value comes only from a literal string.
        /// </param>
        /// <param name="parameterValue">
        /// The parameter value.
        /// </param>
        /// <returns>
        /// An absolute URI.
        /// </returns>
        public static Uri AttachQueryStringParameter(Uri url, string parameterName, string parameterValue)
        {
            var builder = new UriBuilder(url);
            string query = builder.Query;
            if (query.Length > 1)
            {
                // remove the '?' character in front of the query string
                query = query.Substring(1);
            }

            string parameterPrefix = parameterName + "=";

            string encodedParameterValue = Uri.EscapeDataString(parameterValue);

            string newQuery = Regex.Replace(query, parameterPrefix + "[^\\&]*", parameterPrefix + encodedParameterValue);
            if (newQuery == query)
            {
                if (newQuery.Length > 0)
                {
                    newQuery += "&";
                }

                newQuery = newQuery + parameterPrefix + encodedParameterValue;
            }

            builder.Query = newQuery;

            return builder.Uri;
        }

        /// <summary>
        /// Converts an app-relative url, e.g. ~/Content/Return.cshtml, to a full-blown url, e.g.
        ///     http://mysite.com/Content/Return.cshtml
        /// </summary>
        /// <param name="returnUrl">
        /// The return URL.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// An absolute URI.
        /// </returns>
        public static Uri ConvertToAbsoluteUri(string returnUrl, HttpContextBase context)
        {
            if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
            {
                return new Uri(returnUrl, UriKind.Absolute);
            }

            if (!VirtualPathUtility.IsAbsolute(returnUrl))
            {
                returnUrl = VirtualPathUtility.ToAbsolute(returnUrl);
            }

            Uri publicUrl = GetPublicFacingUrl(context.Request);
            return new Uri(publicUrl, returnUrl);
        }

        /// <summary>
        /// The verify authentication.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="AuthenticationResult"/>.
        /// </returns>
        public new AuthenticationResult VerifyAuthentication(string returnUrl)
        {
            // Only OAuth2 requires the return url value for the verify authenticaiton step
            var oauth2Client = this.authenticationProvider as OAuth2Client;
            if (oauth2Client != null)
            {
                string sessionId = this.requestContext.Request.QueryString[SessionIdQueryStringName];

                // convert returnUrl to an absolute path
                Uri uri;
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    uri = ConvertToAbsoluteUri(returnUrl, this.requestContext);
                }
                else
                {
                    uri = GetPublicFacingUrl(this.requestContext.Request);
                }

                // attach the provider parameter so that we know which provider initiated
                // the login when user is redirected back to this page
                uri = AttachQueryStringParameter(uri, ProviderQueryStringName, this.authenticationProvider.ProviderName);

                // When we called RequestAuthentication(), we put the sessionId in the returnUrl query string.
                // Hence, we need to put it in the VerifyAuthentication url again to please FB/Microsoft account providers.
                uri = AttachQueryStringParameter(uri, SessionIdQueryStringName, sessionId);

                try
                {
                    AuthenticationResult result = oauth2Client.VerifyAuthentication(this.requestContext, uri);
                    if (!result.IsSuccessful)
                    {
                        // if the result is a Failed result, creates a new Failed response which has providerName info.
                        result = new AuthenticationResult(
                            false, 
                            this.authenticationProvider.ProviderName, 
                            null, 
                            null, 
                            null);
                    }

                    return result;
                }
                catch (HttpException exception)
                {
                    return new AuthenticationResult(
                        exception.GetBaseException(), 
                        this.authenticationProvider.ProviderName);
                }
            }

            return this.authenticationProvider.VerifyAuthentication(this.requestContext);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the public facing URL for the given incoming HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The URI that the outside world used to create this request.
        /// </returns>
        internal static Uri GetPublicFacingUrl(HttpRequestBase request)
        {
            return GetPublicFacingUrl(request, request.ServerVariables);
        }

        /// <summary>
        /// Gets the public facing URL for the given incoming HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="serverVariables">
        /// The server variables to consider part of the request.
        /// </param>
        /// <returns>
        /// The URI that the outside world used to create this request.
        /// </returns>
        /// <remarks>
        /// Although the <paramref name="serverVariables"/> value can be obtained from
        ///     <see cref="HttpRequest.ServerVariables"/>, it's useful to be able to pass them
        ///     in so we can simulate injected values from our unit tests since the actual property
        ///     is a read-only kind of <see cref="NameValueCollection"/>.
        /// </remarks>
        internal static Uri GetPublicFacingUrl(HttpRequestBase request, NameValueCollection serverVariables)
        {
            // Due to URL rewriting, cloud computing (i.e. Azure)
            // and web farms, etc., we have to be VERY careful about what
            // we consider the incoming URL.  We want to see the URL as it would
            // appear on the public-facing side of the hosting web site.
            // HttpRequest.Url gives us the internal URL in a cloud environment,
            // So we use a variable that (at least from what I can tell) gives us
            // the public URL:
            if (serverVariables["HTTP_HOST"] != null)
            {
                string scheme = serverVariables["HTTP_X_FORWARDED_PROTO"] ?? request.Url.Scheme;
                var hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + serverVariables["HTTP_HOST"]);
                var publicRequestUri = new UriBuilder(request.Url);
                publicRequestUri.Scheme = scheme;
                publicRequestUri.Host = hostAndPort.Host;
                publicRequestUri.Port = hostAndPort.Port; // CC missing Uri.Port contract that's on UriBuilder.Port
                return publicRequestUri.Uri;
            }

            // Failover to the method that works for non-web farm enviroments.
            // We use Request.Url for the full path to the server, and modify it
            // with Request.RawUrl to capture both the cookieless session "directory" if it exists
            // and the original path in case URL rewriting is going on.  We don't want to be
            // fooled by URL rewriting because we're comparing the actual URL with what's in
            // the return_to parameter in some cases.
            // Response.ApplyAppPathModifier(builder.Path) would have worked for the cookieless
            // session, but not the URL rewriting problem.
            return new Uri(request.Url, request.RawUrl);
        }

        #endregion
    }
}