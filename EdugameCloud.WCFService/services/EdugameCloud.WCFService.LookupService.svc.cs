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
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

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
        /// Gets the vcf model.
        /// </summary>
        private VCFModel VCFModel
        {
            get
            {
                return IoC.Resolve<VCFModel>();
            }
        }

        /// <summary>
        /// Gets the twitter model.
        /// </summary>
        private TwitterModel TwitterModel
        {
            get
            {
                return IoC.Resolve<TwitterModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get version info.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<EGCVersionsDTO> GetVersionInfo()
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
            return new ServiceResponse<EGCVersionsDTO>
            {
                @object = new EGCVersionsDTO
                              {
                                  adminVersion = admin.Return(x => new VersionDTO(admin), null),
                                  publicVersion = @public.Return(x => new VersionDTO(@public), null)
                              }
            };
        }

       /// <summary>
        /// The get countries.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CountryDTO> GetCountries()
        {
            return new ServiceResponse<CountryDTO>
            {
                objects = this.CountryModel.GetAll().Select(x => new CountryDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get location.
        /// </summary>
        /// <param name="geoDTO">
        /// The geo dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<GeoResultDTO> GetLocation(GeoDTO geoDTO)
        {
            var response = new ServiceResponse<GeoResultDTO>();
            string error;
            var result = this.GeoModel.GetLocation(geoDTO, out error);
            if (result != null)
            {
                response.@object = result;
            }
            else if (!string.IsNullOrWhiteSpace(geoDTO.country))
            {
                result = this.GeoModel.GetLocation(new GeoDTO { country = geoDTO.country }, out error);    
            }

            if (result == null)
            {
                response.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Location Server Error", error));
            }
            else
            {
                response.@object = result;
            }

            return response;
        }

        /// <summary>
        /// The get twitter profiles.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TwitterProfileDTO> GetTwitterProfiles(string name)
        {
            return new ServiceResponse<TwitterProfileDTO>
            {
                @objects = this.TwitterModel.SearchForUsers(name).ToList()
            };
        }

        /// <summary>
        /// The get google social profiles.
        /// </summary>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<GoogleSearchDTO> SearchSocialLinksUsingGoogleAPI(string fullName)
        {
            var result = this.GoogleSearchAPIModel.Search(fullName);
            if (result != null)
            {
                return new ServiceResponse<GoogleSearchDTO> { @objects = result.ToList() };
            }

            var faultResult = new ServiceResponse<GoogleSearchDTO>();
            faultResult.SetError(
                new Error(Errors.TOO_MANY_DEPOSITIONS, "GoogleReachedTheLimit", "Daily request limit reached"));
            return faultResult;
        }

        /// <summary>
        /// The get google social profiles.
        /// </summary>
        /// <param name="vcfProfile">
        /// The vcf Profile.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<string> ConvertFromVCF(string vcfProfile)
        {
            var result = this.VCFModel.ConvertFromVCF(vcfProfile);
            return new ServiceResponse<string> { @object = result };
        }

        /// <summary>
        /// Convert to VCF.
        /// </summary>
        /// <param name="xmlProfile">
        /// The xml Profile.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<byte> ConvertToVCF(string xmlProfile)
        {
            var model = new VCFProfileDTO { xmlProfile = xmlProfile };

            var result = new ServiceResponse<byte>();
            ValidationResult validationResult;
            if (this.IsValid(model, out validationResult))
            {
                string fileName, exception;
                var resultBytes = this.VCFModel.ConvertToVCF(model.xmlProfile, out fileName, out exception);
                if (resultBytes != null)
                {
                    result.objects = resultBytes;
                }
                else if (exception != null)
                {
                    result.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "ConversionFailed", exception));
                }
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityGetError_Subject, result, (string)null);
            return result;
        }

        /// <summary>
        /// The get tweets.
        /// </summary>
        /// <param name="screenname">
        /// The screenname.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TwitterStatusDTO> GetTweets(string screenname)
        {
            return new ServiceResponse<TwitterStatusDTO>
            {
                @objects = this.TwitterModel.SearchForTweets(screenname).ToList()
            };
        }

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<StateDTO> GetStates()
        {
            return new ServiceResponse<StateDTO>
            {
                objects = this.StateModel.GetAll().Select(x => new StateDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNServiceDTO> GetServices()
        {
            return new ServiceResponse<SNServiceDTO>
            {
                objects = this.SNServiceModel.GetAll().Select(x => new SNServiceDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNMapProviderDTO> GetMapProviders()
        {
            return new ServiceResponse<SNMapProviderDTO>
            {
                objects = this.SNMapProviderModel.GetAll().Select(x => new SNMapProviderDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get build version types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<BuildVersionTypeDTO> GetBuildVersionTypes()
        {
            return new ServiceResponse<BuildVersionTypeDTO>
            {
                objects = this.BuildVersionTypeModel.GetAll().Select(x => new BuildVersionTypeDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LanguageDTO> GetLanguages()
        {
            return new ServiceResponse<LanguageDTO>
            {
                objects = this.LanguageModel.GetAll().Select(x => new LanguageDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get question types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionTypeDTO> GetQuestionTypes()
        {
            return new ServiceResponse<QuestionTypeDTO>
            {
                objects = this.QuestionTypeModel.GetAll().Select(x => new QuestionTypeDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get time zones.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TimeZoneDTO> GetTimeZones()
        {
            return new ServiceResponse<TimeZoneDTO>
            {
                objects = this.TimeZoneModel.GetAll().Select(x => new TimeZoneDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get score types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ScoreTypeDTO> GetScoreTypes()
        {
            return new ServiceResponse<ScoreTypeDTO>
            {
                objects = this.ScoreTypeModel.GetAll().Select(x => new ScoreTypeDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LookupAllDTO> GetAll()
        {
            return new ServiceResponse<LookupAllDTO>
            {
                @object = new LookupAllDTO
                              {
                                  buildVersionTypes = this.GetBuildVersionTypes().objects.ToList(),
                                  countries = this.GetCountries().objects.ToList(),
                                  languages = this.GetLanguages().objects.ToList(),
                                  mapProviders = this.GetMapProviders().objects.ToList(),
                                  questionTypes = this.GetQuestionTypes().objects.ToList(),
                                  quizFormats = this.GetQuizFormats().objects.ToList(),
                                  scoreTypes = this.GetScoreTypes().objects.ToList(),
                                  services = this.GetServices().objects.ToList(),
                                  states = this.GetStates().objects.ToList(),
                                  surveyGroupingTypes = this.GetSurveyGroupingTypes().objects.ToList(),
                                  timeZones = this.GetTimeZones().objects.ToList(),
                                  userRoles = this.GetUserRoles().objects.ToList(),
                              }
            };
        }

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserRoleDTO> GetUserRoles()
        {
            return new ServiceResponse<UserRoleDTO>
            {
                objects = this.UserRoleModel.GetAll().Select(x => new UserRoleDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The get quiz formats.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizFormatDTO> GetQuizFormats()
        {
            return new ServiceResponse<QuizFormatDTO>
            {
                objects = this.QuizFormatModel.GetAll().Select(x => new QuizFormatDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The survey grouping types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyGroupingTypeDTO> GetSurveyGroupingTypes()
        {
            return new ServiceResponse<SurveyGroupingTypeDTO>
            {
                objects = this.SurveyGroupingTypeModel.GetAll().Select(x => new SurveyGroupingTypeDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="countryDTO">
        /// The country DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<GeoCountryDTO> SaveCountry(GeoCountryDTO countryDTO)
        {
            var result = new ServiceResponse<GeoCountryDTO>();
            ValidationResult validationResult;
            if (this.IsValid(countryDTO, out validationResult))
            {
                var countryModel = this.CountryModel;
                var country = countryModel.GetOneById(countryDTO.countryId).Value;
                country = this.ConvertDto(countryDTO, country);
                countryModel.RegisterSave(country);
                this.UpdateCache<LookupService>(x => x.GetCountries());
                this.UpdateCache<LookupService>(x => x.GetAll());
                result.@object = new GeoCountryDTO(country);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="stateDTO">
        /// The state DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<GeoStateDTO> SaveState(GeoStateDTO stateDTO)
        {
            var result = new ServiceResponse<GeoStateDTO>();
            ValidationResult validationResult;
            if (this.IsValid(stateDTO, out validationResult))
            {
                var stateModel = this.StateModel;
                var state = stateModel.GetOneById(stateDTO.stateId).Value;
                state = this.ConvertDto(stateDTO, state);
                stateModel.RegisterSave(state);
                this.UpdateCache<LookupService>(x => x.GetStates());
                this.UpdateCache<LookupService>(x => x.GetAll());
                result.@object = new GeoStateDTO(state);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
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