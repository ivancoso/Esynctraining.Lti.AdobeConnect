// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using Core.Business;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Comparers;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class LookupService : BaseService, ILookupService
    {
        private readonly Func<StateModel> StateModel;

        public LookupService(Func<StateModel> stateModel)
        {
            StateModel = stateModel;
        }

        #region Properties

        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();
        
        private QuestionTypeModel QuestionTypeModel => IoC.Resolve<QuestionTypeModel>();
        
        private GeoModel GeoModel => IoC.Resolve<GeoModel>();
        
        private QuizFormatModel QuizFormatModel => IoC.Resolve<QuizFormatModel>();
        
        private SurveyGroupingTypeModel SurveyGroupingTypeModel => IoC.Resolve<SurveyGroupingTypeModel>();
        
        private UserRoleModel UserRoleModel => IoC.Resolve<UserRoleModel>();
        
        private ScoreTypeModel ScoreTypeModel => IoC.Resolve<ScoreTypeModel>();
        
        private TimeZoneModel TimeZoneModel => IoC.Resolve<TimeZoneModel>();
        
        private BuildVersionTypeModel BuildVersionTypeModel => IoC.Resolve<BuildVersionTypeModel>();
        
        private CountryModel CountryModel => IoC.Resolve<CountryModel>();
        
        private SNServiceModel SNServiceModel => IoC.Resolve<SNServiceModel>();
        
        private SNMapProviderModel SNMapProviderModel => IoC.Resolve<SNMapProviderModel>();

        ///// <summary>
        ///// Gets the facebook model.
        ///// </summary>
        //private GoogleSearchAPIModel GoogleSearchAPIModel
        //{
        //    get
        //    {
        //        return IoC.Resolve<GoogleSearchAPIModel>();
        //    }
        //}
        
        private VCFModel VCFModel => IoC.Resolve<VCFModel>();

        ///// <summary>
        ///// Gets the twitter model.
        ///// </summary>
        //private TwitterModel TwitterModel
        //{
        //    get
        //    {
        //        return IoC.Resolve<TwitterModel>();
        //    }
        //}

        private ICache Cache => IoC.Resolve<ICache>();

        #endregion

        #region Public Methods and Operators

        public EGCVersionsDTO GetVersionInfo()
        {
            try
            {
                return CacheUtility.GetCachedItem<EGCVersionsDTO>(Cache, CachePolicies.Keys.VersionInfo(), () =>
                {
                    string @base = string.Empty;
                    DirectoryInfo parent;
                    if ((parent = Directory.GetParent(HttpContext.Current.Server.MapPath("~"))) != null)
                    {
                        @base = parent.FullName;
                        if (parent.EnumerateDirectories("EdugameCloud.Web").Any())
                        {
                            @base = Path.Combine(@base, "EdugameCloud.Web");
                        }
                    }

                    var adminPath = @base + @"\Content\swf\admin";
                    var publicPath = @base + @"\Content\swf\pub";
                    var admin = this.ProcessVersion(adminPath, (string)this.Settings.BuildSelector);
                    var @public = this.ProcessVersion(publicPath, (string)this.Settings.PublicBuildSelector);
                    return new EGCVersionsDTO
                    {
                        adminVersion = admin.Return(x => new VersionDTO(admin), null),
                        publicVersion = @public.Return(x => new VersionDTO(@public), null),
                    };

                });
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetVersionInfo", ex);
                throw;
            }
        }

        public CountryDTO[] GetCountries()
        {
            try
            {
                return this.CountryModel.GetAll().Select(x => new CountryDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetCountries", ex);
                throw;
            }
        }

        public GeoResultDTO GetLocation(GeoDTO geoDTO)
        {
            string errorString;
            var result = this.GeoModel.GetLocation(geoDTO, out errorString);
            if (result != null)
            {
                return result;
            }

            if (!string.IsNullOrWhiteSpace(geoDTO.country))
            {
                result = this.GeoModel.GetLocation(new GeoDTO { country = geoDTO.country }, out errorString);
            }

            if (result == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Location Server Error", errorString);
                this.LogError("Lookup.GetLocation", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return result;
        }

        // /// <summary>
        // /// The get twitter profiles.
        // /// </summary>
        // /// <param name="name">
        // /// The name.
        // /// </param>
        // /// <returns>
        ///// The <see cref="TwitterProfileDTO"/>.
        // /// </returns>
        //public TwitterProfileDTO[] GetTwitterProfiles(string name)
        //{
        //    return this.TwitterModel.SearchForUsers(name).ToArray();
        //}

        // /// <summary>
        // /// The get google social profiles.
        // /// </summary>
        // /// <param name="fullName">
        // /// The full name.
        // /// </param>
        // /// <returns>
        ///// The <see cref="GoogleSearchDTO"/>.
        // /// </returns>
        //public GoogleSearchDTO[] SearchSocialLinksUsingGoogleAPI(string fullName)
        // {
        //     var result = this.GoogleSearchAPIModel.Search(fullName);
        //     if (result != null)
        //     {
        //         return result.ToArray();
        //     }

        //     var error = new Error(Errors.TOO_MANY_DEPOSITIONS, "GoogleReachedTheLimit", "Daily request limit reached");
        //     this.LogError("Lookup.SearchSocialLinksUsingGoogleAPI", error);
        //     throw new FaultException<Error>(error, error.errorMessage);
        // }

        public string ConvertFromVCF(string vcfProfile)
        {
            try
            {
                var result = this.VCFModel.ConvertFromVCF(vcfProfile);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.ConvertFromVCF", ex);
                throw;
            }
        }

        public byte[] ConvertToVCF(string xmlProfile)
        {
            var model = new VCFProfileDTO { xmlProfile = xmlProfile };
            Error error;
            ValidationResult validationResult;
            if (this.IsValid(model, out validationResult))
            {
                string fileName, exception;
                var resultBytes = this.VCFModel.ConvertToVCF(model.xmlProfile, out fileName, out exception);
                if (resultBytes != null)
                {
                    return resultBytes;
                }

                error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "ConversionFailed", exception);
            }
            else
            {
                error = this.GenerateValidationError(validationResult);
            }

            this.LogError("Lookup.ConvertToVCF", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        ///// <summary>
        ///// The get tweets.
        ///// </summary>
        ///// <param name="screenname">
        ///// The screen name.
        ///// </param>
        ///// <returns>
        ///// The <see cref="TwitterStatusDTO"/>.
        ///// </returns>
        //public TwitterStatusDTO[] GetTweets(string screenname)
        //{
        //    return this.TwitterModel.SearchForTweets(screenname).ToArray();
        //}

        public StateDTO[] GetStates()
        {
            try
            {
                return this.StateModel().GetAll().Select(x => new StateDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetStates", ex);
                throw;
            }
        }
        
        //public SchoolDTO[] GetSchools()
        //{
        //    try
        //    {
        //        return this.SchoolModel.GetAll().Select(x => new SchoolDTO(x)).ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Lookup.GetSchools", ex);
        //        throw;
        //    }
        //}

        public SNServiceDTO[] GetServices()
        {
            try
            {
                return this.SNServiceModel.GetAll().Select(x => new SNServiceDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetServices", ex);
                throw;
            }
        }

        public SNMapProviderDTO[] GetMapProviders()
        {
            try
            {
                return this.SNMapProviderModel.GetAll().Select(x => new SNMapProviderDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetMapProviders", ex);
                throw;
            }
        }

        public BuildVersionTypeDTO[] GetBuildVersionTypes()
        {
            try
            {
                return this.BuildVersionTypeModel.GetAll().Select(x => new BuildVersionTypeDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetBuildVersionTypes", ex);
                throw;
            }
        }

        public LanguageDTO[] GetLanguages()
        {
            try
            {
                return this.LanguageModel.GetAll().Select(x => new LanguageDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetLanguages", ex);
                throw;
            }
        }

        public QuestionTypeDTO[] GetQuestionTypes()
        {
            try
            {
                return this.QuestionTypeModel.GetAll().Select(x => new QuestionTypeDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetQuestionTypes", ex);
                throw;
            }
        }

        public TimeZoneDTO[] GetTimeZones()
        {
            try
            {
                return this.TimeZoneModel.GetAll().Select(x => new TimeZoneDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetTimeZones", ex);
                throw;
            }
        }

        /// <summary>
        /// The get score types.
        /// </summary>
        /// <returns>
        /// The <see cref="ScoreTypeDTO"/>.
        /// </returns>
        public ScoreTypeDTO[] GetScoreTypes()
        {
            try
            { 
            return this.ScoreTypeModel.GetAll().Select(x => new ScoreTypeDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetScoreTypes", ex);
                throw;
            }
        }

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="LookupAllDTO"/>.
        /// </returns>
        public LookupAllDTO GetAll()
        {
            return new LookupAllDTO
            {
                buildVersionTypes = this.GetBuildVersionTypes(),
                countries = this.GetCountries(),
                languages = this.GetLanguages(),
                mapProviders = this.GetMapProviders(),
                questionTypes = this.GetQuestionTypes(),
                quizFormats = this.GetQuizFormats(),
                scoreTypes = this.GetScoreTypes(),
                services = this.GetServices(),
                states = this.GetStates(),
                //schools = this.GetSchools(),
                surveyGroupingTypes = this.GetSurveyGroupingTypes(),
                timeZones = this.GetTimeZones(),
                userRoles = this.GetUserRoles(),
            };
        }

        public UserRoleDTO[] GetUserRoles()
        {
            try
            {
                return this.UserRoleModel.GetAll().Select(x => new UserRoleDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetUserRoles", ex);
                throw;
            }
        }

        public QuizFormatDTO[] GetQuizFormats()
        {
            try
            {
                return this.QuizFormatModel.GetAll().Select(x => new QuizFormatDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetQuizFormats", ex);
                throw;
            }
        }

        public SurveyGroupingTypeDTO[] GetSurveyGroupingTypes()
        {
            try
            {
                return this.SurveyGroupingTypeModel.GetAll().Select(x => new SurveyGroupingTypeDTO(x)).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("Lookup.GetSurveyGroupingTypes", ex);
                throw;
            }
        }

        #endregion

        protected Version ProcessVersion(string swfFolder, string buildSelector)
        {
            if (string.IsNullOrWhiteSpace(swfFolder))
                throw new ArgumentNullException("swfFolder");
            if (string.IsNullOrWhiteSpace(buildSelector))
                throw new ArgumentNullException("buildSelector");

            if (Directory.Exists(swfFolder))
            {
                var versions = new List<KeyValuePair<Version, string>>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var file in Directory.GetFiles(swfFolder, buildSelector))
                {
                    var fileName = Path.GetFileName(file);
                    var version = fileName.GetBuildVersion();
                    versions.Add(new KeyValuePair<Version, string>(version, fileName));
                }

                versions.Sort(new BuildVersionComparer());
                return versions.FirstOrDefault().With(x => x.Key);
            }

            return null;
        }

    }

}