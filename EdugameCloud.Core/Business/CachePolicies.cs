﻿using System;
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

            public static string States()
            {
                return "States";
            }

            public static string Schools()
            {
                return "Schools";
            }

            public static string Languages()
            {
                return "Languages";
            }

            public static string TimeZones()
            {
                return "TimeZones";
            }

            public static string ScoreTypes()
            {
                return "ScoreTypes";
            }

            public static string UserRoles()
            {
                return "UserRoles";
            }

            public static string QuizFormats()
            {
                return "QuizFormats";
            }

            public static string QuestionTypes()
            {
                return "QuestionTypes";
            }

            public static string SurveyGroupingTypes()
            {
                return "SurveyGroupingTypes";
            }

            public static string SNServices()
            {
                return "SNServices";
            }

            public static string SNMapProviders()
            {
                return "SNMapProviders";
            }

            public static string VersionInfo()
            {
                return "VersionInfo";
            }


            public static string AcDetails(string apiUrl)
            {
                return apiUrl;
            }

            public static string SharedMeetingTemplates(string apiUrl)
            {
                return string.Concat("SMT_", apiUrl);
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
