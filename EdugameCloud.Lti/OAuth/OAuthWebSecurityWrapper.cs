﻿namespace EdugameCloud.Lti.OAuth
{
    using System;
    using System.Reflection;
    using System.Web;

    using DotNetOpenAuth.AspNet;

    using EdugameCloud.Lti.OAuth.Canvas;

    using Esynctraining.Core.Providers;

    using Microsoft.Web.WebPages.OAuth;

    /// <summary>
    /// The OAUTH web security wrapper.
    /// </summary>
    internal static class OAuthWebSecurityWrapper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The verify authentication.
        /// </summary>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="AuthenticationResult"/>.
        /// </returns>
        public static AuthenticationResult VerifyAuthentication(string providerName, ApplicationSettingsProvider settings)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return AuthenticationResult.Failed;
            }

            var context = (HttpContextBase)new HttpContextWrapper(HttpContext.Current);
            providerName = providerName.ToLower();
            return providerName.Contains("canvas") ? VerifyLtiAuthentication(context, settings) : OAuthWebSecurity.VerifyAuthentication();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get provider.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IOpenAuthDataProvider"/>.
        /// </returns>
        private static IOpenAuthDataProvider GetProvider(Type type)
        {
            var field = type.GetField("OAuthDataProvider", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                return (IOpenAuthDataProvider)field.GetValue(null);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The verify lti authentication.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="AuthenticationResult"/>.
        /// </returns>
        private static AuthenticationResult VerifyLtiAuthentication(HttpContextBase context, dynamic settings)
        {
            var canvasClient = new CanvasClient((string)settings.CanvasClientId, (string)settings.CanvasClientSecret);

            return new LtiOpenAuthSecurityManager(context, canvasClient, GetProvider(typeof(OAuthWebSecurity))).VerifyAuthentication(null);
        }

        #endregion
    }
}