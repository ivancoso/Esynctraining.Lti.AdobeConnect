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

        public static class Names
        {
            public const string PersistantCache = "PersistantCache";
        }

        public static class Keys
        {
            public static string Countries() => "Countries";

            public static string States() => "States";

            public static string Schools() => "Schools";

            public static string CompanyAcServers() => "CompanyAcServers";

            public static string Languages() => "Languages";

            public static string TimeZones() => "TimeZones";

            public static string ScoreTypes() => "ScoreTypes";

            public static string UserRoles() => "UserRoles";

            public static string QuizFormats() => "QuizFormats";

            public static string QuestionTypes() => "QuestionTypes";

            public static string LmsQuestionTypes() => "LmsQuestionTypes";

            public static string SurveyGroupingTypes() => "SurveyGroupingTypes";

            public static string SNServices() => "SNServices";

            public static string SNMapProviders() => "SNMapProviders";

            public static string VersionInfo() => "VersionInfo";


            public static string AcDetails(string apiUrl)
            {
                return apiUrl;
            }

            public static string SharedMeetingTemplates(string apiUrl, string type)
            {
                return $"SMT_{apiUrl}_{type}";
            }

            public static string IsActiveCompany(int companyId)
            {
                return string.Concat("C_", companyId.ToString());
            }

            public static string CompanyLmsSettings(int companyLmsId)
            {
                return string.Concat("LMS_", companyLmsId.ToString());
            }

            public static string CompanyLmsAdobeConnectProxy(int companyLmsId)
            {
                return string.Concat("LMS_AC_", companyLmsId.ToString());
            }

            public static string UserAdobeConnectProxy(int companyLmsId, string lmsUserId)
            {
                return $"LMC_{companyLmsId}_{lmsUserId}_AC";
            }

            public static string LmsProviders()
            {
                return "LP_";
            }

            public static string VersionFileName(string pattern)
            {
                return $"Version_{pattern}";
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
