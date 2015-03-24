// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    /// <summary>
    /// The company license service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class LookupService : BaseService, ILookupService
    {
        #region Properties

        /// <summary>
        /// Gets the language model.
        /// </summary>
        private LanguageModel LanguageModel
        {
            get
            {
                return IoC.Resolve<LanguageModel>();
            }
        }

        /// <summary>
        /// Gets the question type model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        /// Gets the geo model.
        /// </summary>
        private GeoModel GeoModel
        {
            get
            {
                return IoC.Resolve<GeoModel>();
            }
        }

        /// <summary>
        /// Gets the quiz format model.
        /// </summary>
        private QuizFormatModel QuizFormatModel
        {
            get
            {
                return IoC.Resolve<QuizFormatModel>();
            }
        }

        /// <summary>
        /// Gets the survey grouping type model.
        /// </summary>
        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get
            {
                return IoC.Resolve<SurveyGroupingTypeModel>();
            }
        }

        /// <summary>
        /// Gets the user role model.
        /// </summary>
        private UserRoleModel UserRoleModel
        {
            get
            {
                return IoC.Resolve<UserRoleModel>();
            }
        }

        /// <summary>
        /// Gets the score type model.
        /// </summary>
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        /// <summary>
        /// Gets the time zone model.
        /// </summary>
        private TimeZoneModel TimeZoneModel
        {
            get
            {
                return IoC.Resolve<TimeZoneModel>();
            }
        }

        /// <summary>
        /// Gets the build version type model.
        /// </summary>
        private BuildVersionTypeModel BuildVersionTypeModel
        {
            get
            {
                return IoC.Resolve<BuildVersionTypeModel>();
            }
        }

        /// <summary>
        /// Gets the state model
        /// </summary>
        private StateModel StateModel
        {
            get
            {
                return IoC.Resolve<StateModel>();
            }
        }

        /// <summary>
        /// Gets the country model
        /// </summary>
        private CountryModel CountryModel
        {
            get
            {
                return IoC.Resolve<CountryModel>();
            }
        }

        /// <summary>
        /// Gets the SN service model.
        /// </summary>
        private SNServiceModel SNServiceModel
        {
            get
            {
                return IoC.Resolve<SNServiceModel>();
            }
        }

        /// <summary>
        /// Gets the SN map provider.
        /// </summary>
        private SNMapProviderModel SNMapProviderModel
        {
            get
            {
                return IoC.Resolve<SNMapProviderModel>();
            }
        }

        /// <summary>
        /// Gets the facebook model.
        /// </summary>
        private GoogleSearchAPIModel GoogleSearchAPIModel
        {
            get
            {
                return IoC.Resolve<GoogleSearchAPIModel>();
            }
        }

        /// <summary>
        /// Gets the VCF model.
        /// </summary>
        private VCFModel VCFModel
        {
            get
            {
                return IoC.Resolve<VCFModel>();
            }
        }

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get version info.
        /// </summary>
        /// <returns>
        /// The <see cref="EGCVersionsDTO"/>.
        /// </returns>
        public EGCVersionsDTO GetVersionInfo()
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
                           publicVersion = @public.Return(x => new VersionDTO(@public), null)
                       };
        }

       /// <summary>
       /// The get countries.
       /// </summary>
       /// <returns>
       /// The <see cref="CountryDTO"/>.
       /// </returns>
       public CountryDTO[] GetCountries()
       {
           return this.CountryModel.GetAll().Select(x => new CountryDTO(x)).ToArray();
       }

        /// <summary>
        /// The get location.
        /// </summary>
        /// <param name="geoDTO">
        /// The geo DTO.
        /// </param>
        /// <returns>
       /// The <see cref="GeoResultDTO"/>.
        /// </returns>
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

        /// <summary>
        /// The get google social profiles.
        /// </summary>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <returns>
       /// The <see cref="GoogleSearchDTO"/>.
        /// </returns>
       public GoogleSearchDTO[] SearchSocialLinksUsingGoogleAPI(string fullName)
        {
            var result = this.GoogleSearchAPIModel.Search(fullName);
            if (result != null)
            {
                return result.ToArray();
            }

            var error = new Error(Errors.TOO_MANY_DEPOSITIONS, "GoogleReachedTheLimit", "Daily request limit reached");
            this.LogError("Lookup.SearchSocialLinksUsingGoogleAPI", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get google social profiles.
        /// </summary>
        /// <param name="vcfProfile">
        /// The VCF Profile.
        /// </param>
        /// <returns>
       /// The <see cref="string"/>.
        /// </returns>
        public string ConvertFromVCF(string vcfProfile)
        {
            var result = this.VCFModel.ConvertFromVCF(vcfProfile);
            return result;
        }

        /// <summary>
        /// Convert to VCF.
        /// </summary>
        /// <param name="xmlProfile">
        /// The xml Profile.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
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

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="StateDTO"/>.
        /// </returns>
        public StateDTO[] GetStates()
        {
            return this.StateModel.GetAll().Select(x => new StateDTO(x)).ToArray();
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <returns>
        /// The <see cref="SNServiceDTO"/>.
        /// </returns>
        public SNServiceDTO[] GetServices()
        {
            return this.SNServiceModel.GetAll().Select(x => new SNServiceDTO(x)).ToArray();
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <returns>
        /// The <see cref="SNMapProviderDTO"/>.
        /// </returns>
        public SNMapProviderDTO[] GetMapProviders()
        {
            return this.SNMapProviderModel.GetAll().Select(x => new SNMapProviderDTO(x)).ToArray();
        }

        /// <summary>
        /// The get build version types.
        /// </summary>
        /// <returns>
        /// The <see cref="BuildVersionTypeDTO"/>.
        /// </returns>
        public BuildVersionTypeDTO[] GetBuildVersionTypes()
        {
            return this.BuildVersionTypeModel.GetAll().Select(x => new BuildVersionTypeDTO(x)).ToArray();
        }

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="LanguageDTO"/>.
        /// </returns>
        public LanguageDTO[] GetLanguages()
        {
            return this.LanguageModel.GetAll().Select(x => new LanguageDTO(x)).ToArray();
        }

        /// <summary>
        /// The get question types.
        /// </summary>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        public QuestionTypeDTO[] GetQuestionTypes()
        {
            return this.QuestionTypeModel.GetAll().Select(x => new QuestionTypeDTO(x)).ToArray();
        }

        /// <summary>
        /// The get time zones.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeZoneDTO"/>.
        /// </returns>
        public TimeZoneDTO[] GetTimeZones()
        {
            return this.TimeZoneModel.GetAll().Select(x => new TimeZoneDTO(x)).ToArray();
        }

        /// <summary>
        /// The get score types.
        /// </summary>
        /// <returns>
        /// The <see cref="ScoreTypeDTO"/>.
        /// </returns>
        public ScoreTypeDTO[] GetScoreTypes()
        {
            return this.ScoreTypeModel.GetAll().Select(x => new ScoreTypeDTO(x)).ToArray();
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
                           surveyGroupingTypes = this.GetSurveyGroupingTypes(),
                           timeZones = this.GetTimeZones(),
                           userRoles = this.GetUserRoles(),
                       };
        }

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="UserRoleDTO"/>.
        /// </returns>
        public UserRoleDTO[] GetUserRoles()
        {
            return this.UserRoleModel.GetAll().Select(x => new UserRoleDTO(x)).ToArray();
        }

        /// <summary>
        /// The get quiz formats.
        /// </summary>
        /// <returns>
        /// The <see cref="QuizFormatDTO"/>.
        /// </returns>
        public QuizFormatDTO[] GetQuizFormats()
        {
            return this.QuizFormatModel.GetAll().Select(x => new QuizFormatDTO(x)).ToArray();
        }

        /// <summary>
        /// The survey grouping types.
        /// </summary>
        /// <returns>
        /// The <see cref="SurveyGroupingTypeDTO"/>.
        /// </returns>
        public SurveyGroupingTypeDTO[] GetSurveyGroupingTypes()
        {
            return this.SurveyGroupingTypeModel.GetAll().Select(x => new SurveyGroupingTypeDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="countryDTO">
        /// The country DTO.
        /// </param>
        /// <returns>
        /// The <see cref="GeoCountryDTO"/>.
        /// </returns>
        public GeoCountryDTO SaveCountry(GeoCountryDTO countryDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(countryDTO, out validationResult))
            {
                var countryModel = this.CountryModel;
                var country = countryModel.GetOneById(countryDTO.countryId).Value;
                country = this.ConvertDto(countryDTO, country);
                countryModel.RegisterSave(country);
                this.UpdateCache<LookupService>(x => x.GetCountries());
                this.UpdateCache<LookupService>(x => x.GetAll());
                return new GeoCountryDTO(country);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Lookup.SaveCountry", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="stateDTO">
        /// The state DTO.
        /// </param>
        /// <returns>
        /// The <see cref="GeoStateDTO"/>.
        /// </returns>
        public GeoStateDTO SaveState(GeoStateDTO stateDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(stateDTO, out validationResult))
            {
                var stateModel = this.StateModel;
                var state = stateModel.GetOneById(stateDTO.stateId).Value;
                state = this.ConvertDto(stateDTO, state);
                stateModel.RegisterSave(state);
                this.UpdateCache<LookupService>(x => x.GetStates());
                this.UpdateCache<LookupService>(x => x.GetAll());
                return new GeoStateDTO(state);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Lookup.SaveState", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="countryDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Country"/>.
        /// </returns>
        private Country ConvertDto(GeoCountryDTO countryDTO, Country instance)
        {
            instance.Latitude = countryDTO.latitude;
            instance.Longitude = countryDTO.longitude;
            instance.ZoomLevel = countryDTO.zoomLevel;
            return instance;
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="stateDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="State"/>.
        /// </returns>
        private State ConvertDto(GeoStateDTO stateDTO, State instance)
        {
            instance.Latitude = stateDTO.latitude;
            instance.Longitude = stateDTO.longitude;
            instance.ZoomLevel = stateDTO.zoomLevel;
            return instance;
        }

        #endregion
    }
}