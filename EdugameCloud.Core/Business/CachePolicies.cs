using System;
using System.IO;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Core.Business
{
    // TODO: find better place\name
    public static class CachePolicies
    {
        private static readonly string _cacheDependencyFolderPath;


        static CachePolicies()
        {
            _cacheDependencyFolderPath = (string)((dynamic)IoC.Resolve<ApplicationSettingsProvider>()).Cache_CacheDependencyFolder;
            _cacheDependencyFolderPath = _cacheDependencyFolderPath.TrimEnd('\\');
        }


        public static class Keys
        {
            public static string Countries()
            {
                return "Countries";
            }

            public static string Languages()
            {
                return "Languages";
            }
            
            public static string PasswordPolicies(string apiUrl)
            {
                return apiUrl;
            }

            public static string IsActiveCompany(int companyId)
            {
                return string.Concat("C_", companyId.ToString());
            }

            public static string CompanyLmsSettings(int companyLmsId)
            {
                return string.Concat("LMS_", companyLmsId.ToString());
            }

            public static string LmsProviders()
            {
                return "LP_";
            }

        }

        public static class Dependencies
        {
            public static string IsActiveCompany(int companyId)
            {
                return string.Concat(_cacheDependencyFolderPath, @"\C_", companyId.ToString(), ".cache");
            }

            public static string CompanyLmsSettings(int companyLmsId)
            {
                return string.Concat(_cacheDependencyFolderPath, @"\LMS_", companyLmsId.ToString(), ".cache");
            }
            
        }

        public static void InvalidateCache(string dependencyPath)
        {
            //File.Delete(dependencyPath);
            File.WriteAllText(dependencyPath, Guid.NewGuid().ToString());
        }

        public static void InvalidateCache()
        {
            Array.ForEach(Directory.GetFiles(_cacheDependencyFolderPath), (path) => File.WriteAllText(path, Guid.NewGuid().ToString()));
        }

    }

}
