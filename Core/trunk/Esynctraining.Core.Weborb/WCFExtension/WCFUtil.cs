namespace Esynctraining.Core.Weborb.WCFExtension
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.ServiceModel.Configuration;
    using System.Web.Configuration;

    using Esynctraining.Core.Extensions;

    using global::Weborb.Service;
    using global::Weborb.Util.Logging;

    /// <summary>
    /// The WCF util.
    /// </summary>
    public class WCFUtil
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get services.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ServiceModel.Configuration.ServiceElementCollection"/>.
        /// </returns>
        public static ServiceElementCollection GetServices()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Configuration config;
            try
            {
                config = WebConfigurationManager.OpenWebConfiguration(baseDirectory);
            }
            catch (Exception ex1)
            {
                try
                {
                    config = WebConfigurationManager.OpenWebConfiguration("~");
                }
                catch (Exception ex2)
                {
                    if (Log.isLogging(LoggingConstants.ERROR))
                    {
                        Log.log(LoggingConstants.ERROR, "Unable to load web.config for WCF services inspection. If you run WebORB using ASP.NET Development Server, make sure to start Visual Studio 'as Administrator'");
                        Log.log(LoggingConstants.ERROR, ex1);
                        Log.log(LoggingConstants.ERROR, ex2);
                    }

                    return null;
                }
            }

            Log.log(LoggingConstants.INFO, "CONFIG PATH - " + config.FilePath);
            return ServiceModelSectionGroup.GetSectionGroup(config).With(x => x.Services).Services;
        }

        /// <summary>
        /// The invalidate cache.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void InvalidateCache(object target, MethodInfo method, object[] args)
        {
            try
            {
                Assembly webOrbAssembly = Assembly.GetAssembly(typeof(ICacheInvalidator));
                Type cacheType = webOrbAssembly.GetType("Weborb.Util.Cache.Cache");
                MethodInfo invalidationMethod = cacheType.GetMethod(
                    "InvalidateCache", BindingFlags.Static | BindingFlags.NonPublic);
                invalidationMethod.Invoke(null, new[] { target, method, args });
            }
            catch (Exception ex)
            {
                Log.log(LoggingConstants.ERROR, "Exception during cache invalidation", ex);
            }
        }

        #endregion
    }
}