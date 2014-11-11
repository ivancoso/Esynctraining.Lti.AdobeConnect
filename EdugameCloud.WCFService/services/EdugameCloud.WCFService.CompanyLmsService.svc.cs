﻿// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.Core.Domain.Contracts;
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
                if (isTransient && 
                    (entity.LmsProvider.Id == (int)LmsProviderEnum.BrainHoney 
                    || entity.LmsProvider.Id == (int)LmsProviderEnum.Moodle
                    || entity.LmsProvider.Id == (int)LmsProviderEnum.Blackboard))
                {
                    var lmsUser = new LmsUser
                    {
                        CompanyLms = entity,
                        Username = resultDto.lmsAdmin,
                        Password = resultDto.lmsAdminPassword,
                        Token = resultDto.lmsAdminToken,
                        UserId = "0"
                    };
                    LmsUserModel.RegisterSave(lmsUser, true);
                    entity.AdminUser = lmsUser;
                    CompanyLmsModel.RegisterSave(entity);
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
            switch (test.type.ToLowerInvariant())
            {
                case "ac":
                    success = this.TestACConnection(test, out info);
                    break;
                case "brainhoney":
                    success = this.TestBrainHoneyConnection(test, out info);
                    break;
                case "moodle":
                    success = this.TestMoodleConnection(test, out info);
                    break;
            }

            result.@object = new ConnectionInfoDTO { status = success ? "Connected successfully" : "Failed to connect", info = info };

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
            //// todo
            info = "Moodle connection test is not yet implemented";
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
            var session = this.DlapAPI.LoginAndCreateASession(out info, test.domain, test.login, test.password);
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
            if (!domainUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !domainUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                info = "Adobe Connect Domain url should start with http or https";
                return false;
            }

            var fixedUrl = domainUrl.EndsWith("/") ? domainUrl : string.Format("{0}/", domainUrl);
            fixedUrl = fixedUrl.EndsWith("api/xml/") ? fixedUrl : string.Format("{0}api/xml/", fixedUrl);

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
            instance.Company = this.CompanyModel.GetOneById(dto.companyId).Value;
            instance.ConsumerKey = dto.consumerKey;
            instance.CreatedBy = this.UserModel.GetOneById(dto.createdBy).Value;
            instance.DateCreated = dto.dateCreated;
            instance.DateModified = dto.dateModified;
            instance.LmsProvider = this.LmsProviderModel.GetOneByName(dto.lmsProvider).Value;
            instance.ModifiedBy = this.UserModel.GetOneById(dto.modifiedBy).Value;
            instance.SharedSecret = dto.sharedSecret;
            instance.PrimaryColor = dto.primaryColor;
            instance.Layout = dto.layout;
            instance.Title = dto.title;
            if (instance.IsTransient() && !string.IsNullOrWhiteSpace(dto.lmsDomain))
            {
                instance.LmsDomain = dto.lmsDomain;
            }
            
            return instance;
        }

        #endregion
    }
}