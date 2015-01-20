// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    /// The company LMS service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    public class CompanyLmsService : BaseService, ICompanyLmsService
    {
        #region Properties

        /// <summary>
        ///     Gets the company license model.
        /// </summary>
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        /// <summary>
        ///     Gets the DLAP API.
        /// </summary>
        private DlapAPI DlapAPI
        {
            get
            {
                return IoC.Resolve<DlapAPI>();
            }
        }

        /// <summary>
        ///     Gets the Moodle API.
        /// </summary>
        private MoodleAPI MoodleAPI
        {
            get
            {
                return IoC.Resolve<MoodleAPI>();
            }
        }

        /// <summary>
        ///     Gets the SOAP API.
        /// </summary>
        private SoapAPI SoapAPI
        {
            get
            {
                return IoC.Resolve<SoapAPI>();
            }
        }

        /// <summary>
        ///     Gets the LMS User model.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return IoC.Resolve<LmsUserModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private CompanyModel CompanyModel
        {
            get
            {
                return IoC.Resolve<CompanyModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{T}"/>.
        /// </returns>
        public ServiceResponse<CompanyLmsDTO> Save(CompanyLmsDTO resultDto)
        {
            var result = new ServiceResponse<CompanyLmsDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                bool isTransient = resultDto.id == 0;
                CompanyLms entity = isTransient ? null : this.CompanyLmsModel.GetOneById(resultDto.id).Value;
                entity = this.ConvertDto(resultDto, entity);
                if (isTransient)
                {
                    entity.ConsumerKey = Guid.NewGuid().ToString();
                    entity.SharedSecret = Guid.NewGuid().ToString();
                }

                this.CompanyLmsModel.RegisterSave(entity);
                if (entity.LmsProvider.Id != (int)LmsProviderEnum.Canvas)
                {
                    var lmsUser = entity.AdminUser ?? new LmsUser
                    {
                        CompanyLms = entity,
                        UserId = "0"
                    };

                    lmsUser.Username = resultDto.lmsAdmin;
                    if (!string.IsNullOrEmpty(resultDto.lmsAdminPassword))
                    {
                        lmsUser.Password = resultDto.lmsAdminPassword;
                    }
                    lmsUser.Token = resultDto.lmsAdminToken;

                    LmsUserModel.RegisterSave(lmsUser, true);
                    if (isTransient)
                    {
                        entity.AdminUser = lmsUser;
                        CompanyLmsModel.RegisterSave(entity);
                    }
                }

                result.@object = new CompanyLmsDTO(entity);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{ConnectionInfoDTO}"/>.
        /// </returns>
        public ServiceResponse<ConnectionInfoDTO> TestConnection(ConnectionTestDTO test)
        {
            var result = new ServiceResponse<ConnectionInfoDTO>();
            bool success = false;
            string info = string.Empty;

            if (!test.domain.StartsWithProtocol())
            {
                info = "Domain url should start with http:// or https://";
            }
            else
            {
                switch (test.type.ToLowerInvariant())
                {
                    case "ac":
                        success = this.TestACConnection(test, out info);
                        break;
                    case "brainhoney":
                        success = this.TestBrainHoneyConnection(test, out info);
                        break;
                    case "blackboard":
                        success = this.TestBlackBoardConnection(test, out info);
                        break;
                    case "moodle":
                        success = this.TestMoodleConnection(test, out info);
                        break;
                    case "sakai":
                        success = this.TestSakaiConnection(test, out info);
                        break;
                }    
            }
            
            result.@object = new ConnectionInfoDTO { status = success ? "Connected successfully" : "Failed to connect", info = info };
            if (!success)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Failed to connect", info));
            }

            return result;
        }

        /// <summary>
        /// The test MOODLE connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestMoodleConnection(ConnectionTestDTO test, out string info)
        {
            var session = this.MoodleAPI.LoginAndCreateAClient(out info, test.domain.IsSSL(), test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password);
            return session != null;
        }

        /// <summary>
        /// The test SAKAI connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        // ReSharper disable once UnusedParameter.Local
        private bool TestSakaiConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not yet implemented";
            return false;
        }

        /// <summary>
        /// The test brain honey connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestBrainHoneyConnection(ConnectionTestDTO test, out string info)
        {
            var session = this.DlapAPI.LoginAndCreateASession(out info, test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password);
            return session != null;
        }

        /// <summary>
        /// The test brain honey connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestBlackBoardConnection(ConnectionTestDTO test, out string info)
        {
            var session = this.SoapAPI.LoginAndCreateAClient(out info, test.domain.IsSSL(), test.domain, test.login, test.password);
            return session != null;
        }

        /// <summary>
        /// The test AC connection.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TestACConnection(ConnectionTestDTO test, out string info)
        {
            info = string.Empty;
            var domainUrl = test.domain.ToLowerInvariant();
            if (!domainUrl.StartsWithProtocol())
            {
                info = "Adobe Connect Domain url should start with http or https";
                return false;
            }

            var fixedUrl = domainUrl.EndsWith("/") ? domainUrl : string.Format("{0}/", domainUrl);
            fixedUrl = fixedUrl.EndsWith("api/xml/") ? fixedUrl : string.Format("{0}api/xml", fixedUrl);

            var provider = new AdobeConnectProvider(new ConnectionDetails { ServiceUrl = fixedUrl });

            var result = provider.Login(new UserCredentials(test.login, test.password));

            if (!result.Success)
            {
                var error = FormatErrorFromAC(result).With(x => x.error);
                if (error != null)
                {
                    info = error.errorMessage;
                }
            }

            return result.Success;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLms"/>.
        /// </returns>
        private CompanyLms ConvertDto(CompanyLmsDTO dto, CompanyLms instance)
        {
            instance = instance ?? new CompanyLms();
            if (!string.IsNullOrEmpty(dto.acPassword)) 
            {
                instance.AcPassword = dto.acPassword;
            }

            instance.AcServer = dto.acServer;
            instance.AcUsername = dto.acUsername;
            instance.CompanyId = dto.companyId;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = dto.createdBy;
            instance.DateCreated = dto.dateCreated;
            instance.DateModified = dto.dateModified;
            instance.LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsProvider).Value;
            instance.ModifiedBy = this.UserModel.GetOneById(dto.modifiedBy).Value.Return(x => x.Id, dto.createdBy);
            instance.SharedSecret = dto.sharedSecret;
            instance.PrimaryColor = dto.primaryColor;
            instance.Title = dto.title;
            instance.UseUserFolder = dto.useUserFolder;
            instance.CanRemoveMeeting = dto.canRemoveMeeting;
            instance.CanEditMeeting = dto.canEditMeeting;
            instance.IsSettingsVisible = dto.isSettingsVisible;
            instance.EnableOfficeHours = dto.enableOfficeHours;
            instance.EnableStudyGroups = dto.enableStudyGroups;
            instance.UserFolderName = dto.userFolderName;
            if (!string.IsNullOrWhiteSpace(dto.lmsDomain))
            {
                instance.LmsDomain = dto.lmsDomain.RemoveHttpProtocolAndTrailingSlash();
                instance.UseSSL = dto.lmsDomain.IsSSL();
            }
            
            return instance;
        }

        #endregion
    }
}