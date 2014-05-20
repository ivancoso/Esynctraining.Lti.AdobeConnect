// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNProfileService : BaseService, ISNProfileService
    {
        #region Properties

        /// <summary>
        /// Gets the SN map provider model.
        /// </summary>
        private SNMapProviderModel SNMapProviderModel
        {
            get
            {
                return IoC.Resolve<SNMapProviderModel>();
            }
        }

        /// <summary>
        /// Gets the SN map settings model.
        /// </summary>
        private SNMapSettingsModel SNMapSettingsModel
        {
            get
            {
                return IoC.Resolve<SNMapSettingsModel>();
            }
        }

        /// <summary>
        ///     Gets the SN profile Model.
        /// </summary>
        private SNProfileModel SNProfileModel
        {
            get
            {
                return IoC.Resolve<SNProfileModel>();
            }
        }

        /// <summary>
        ///     Gets the SN service model.
        /// </summary>
        private SNServiceModel SNServiceModel
        {
            get
            {
                return IoC.Resolve<SNServiceModel>();
            }
        }

        /// <summary>
        ///     Gets the SN profile SN service model.
        /// </summary>
        private SNProfileSNServiceModel SNProfileSNServiceModel
        {
            get
            {
                return IoC.Resolve<SNProfileSNServiceModel>();
            }
        }

        /// <summary>
        ///     Gets the SN link model.
        /// </summary>
        private SNLinkModel SNLinkModel
        {
            get
            {
                return IoC.Resolve<SNLinkModel>();
            }
        }

        /// <summary>
        /// Gets the address model
        /// </summary>
        private AddressModel AddressModel
        {
            get
            {
                return IoC.Resolve<AddressModel>();
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileDTO> GetById(int id)
        {
            var result = new ServiceResponse<SNProfileDTO>();
            SNProfile profile;
            if ((profile = this.SNProfileModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.EntityGetError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SNProfileDTO(profile);
            }

            return result;
        }

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileExtraDTO> GetAllByUserId(int userId)
        {
            return new ServiceResponse<SNProfileExtraDTO>
            {
                objects = this.SNProfileModel.GetAllByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileExtraDTO> GetAllSharedByUserId(int userId)
        {
            return new ServiceResponse<SNProfileExtraDTO>
            {
                objects = this.SNProfileModel.GetAllSharedByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileExtraDTO> GetSharedProfilesByUserId(int userId)
        {
            return new ServiceResponse<SNProfileExtraDTO>
            {
                objects = this.SNProfileModel.GetAllSharedByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// Gets one by SMI.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileDTO> GetBySMIId(int smiId)
        {
            var result = new ServiceResponse<SNProfileDTO>();
            SNProfile profile;
            if ((profile = this.SNProfileModel.GetOneBySMIId(smiId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION,
                        ErrorsTexts.EntityGetError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SNProfileDTO(profile);
            }

            return result;
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="profile">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNProfileDTO> Save(SNProfileDTO profile)
        {
            var result = new ServiceResponse<SNProfileDTO>();
            ValidationResult validationResult;
            if (this.IsValid(profile, out validationResult))
            {
                var profileModel = this.SNProfileModel;
                bool isTransient = profile.snProfileId == 0;
                var instance = isTransient ? null : profileModel.GetOneById(profile.snProfileId).Value;
                List<SNLink> links;
                List<SNProfileSNService> services;
                instance = this.ConvertDto(profile, instance, out links, out services);
                profileModel.RegisterSave(instance, true);
                this.SaveLinks(links, instance);
                this.SaveServices(services, instance);
                profileModel.Refresh(ref instance);
                this.UpdateCache();
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable(NotificationType.Update, this.CurrentUser.With(x => x.Company.Id), instance);
                result.@object = new SNProfileDTO(instance);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private void SaveServices(List<SNProfileSNService> services, SNProfile instance)
        {
            if (services.Any())
            {
                var servicesModel = this.SNProfileSNServiceModel;
                foreach (var service in services)
                {
                    service.Profile = instance;
                    servicesModel.RegisterSave(service);
                }

                servicesModel.Flush();
            }
        }

        /// <summary>
        /// The save links.
        /// </summary>
        /// <param name="links">
        /// The links.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private void SaveLinks(List<SNLink> links, SNProfile instance)
        {
            if (links.Any())
            {
                var linksModel = this.SNLinkModel;
                foreach (var link in links)
                {
                    link.Profile = instance;
                    linksModel.RegisterSave(link);
                }

                linksModel.Flush();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The update cache.
        /// </summary>
        private void UpdateCache()
        {
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="profileDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="links">
        /// The links.
        /// </param>
        /// <param name="services">
        /// The services.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfile"/>.
        /// </returns>
        private SNProfile ConvertDto(SNProfileDTO profileDTO, SNProfile instance, out List<SNLink> links, out List<SNProfileSNService> services)
        {
            instance = instance ?? new SNProfile();
            instance.ProfileName = profileDTO.profileName;
            instance.UserName = profileDTO.userName;
            instance.JobTitle = profileDTO.jobTitle;
            instance.Email = profileDTO.email;
            instance.Phone = profileDTO.phone;
            instance.About = profileDTO.about;
            instance.SubModuleItem = this.SubModuleItemModel.GetOneById(profileDTO.subModuleItemId).Value;
            instance.MapSettings = this.UpdateMapSettings(instance, profileDTO);
            if (profileDTO.addressVO != null)
            {
                instance.Address = instance.Address ?? new Address();
                var addressVo = profileDTO.addressVO;
                instance.Address.Address1 = addressVo.address1;
                instance.Address.Address2 = addressVo.address2;
                instance.Address.City = addressVo.city;
                instance.Address.DateCreated = addressVo.dateCreated.HasValue ? addressVo.dateCreated.Value : DateTime.Now;
                instance.Address.DateModified = addressVo.dateModified.HasValue ? addressVo.dateModified.Value : DateTime.Now;
                instance.Address.Latitude = addressVo.latitude;
                instance.Address.Longitude = addressVo.longitude;
                instance.Address.Country = addressVo.countryId.HasValue
                                               ? this.CountryModel.GetOneById(addressVo.countryId.Value).Value
                                               : null;
                instance.Address.State = addressVo.stateId.HasValue
                                               ? this.StateModel.GetOneById(addressVo.stateId.Value).Value
                                               : null;
                instance.Address.Zip = addressVo.zip;
                this.AddressModel.RegisterSave(instance.Address);
            }

            links = new List<SNLink>();
            if (profileDTO.links != null)
            {
                this.UpdateLinks(instance, profileDTO, out links);
            }

            services = new List<SNProfileSNService>();
            if (profileDTO.services != null)
            {
                this.UpdateServices(instance, profileDTO, out services);
            }

            return instance;
        }

        /// <summary>
        /// The update services.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="profileDTO">
        /// The profile DTO.
        /// </param>
        /// <param name="outServices">
        /// The out Services.
        /// </param>
        private void UpdateServices(SNProfile instance, SNProfileDTO profileDTO, out List<SNProfileSNService> outServices)
        {
            outServices = new List<SNProfileSNService>();
            var model = this.SNProfileSNServiceModel;
            var serviceModel = this.SNServiceModel;
            var entities = profileDTO.services.Any(x => x.snProfileSNServiceId != 0) ? model.GetAllByIds(profileDTO.services.Where(x => x.snProfileSNServiceId != 0).Select(x => x.snProfileSNServiceId).ToList()).ToList() : new List<SNProfileSNService>();
            var services = serviceModel.GetAllByIds(profileDTO.services.Where(x => x.snServiceId != 0).Select(x => x.snServiceId).ToList()).ToList();
            var listToDelete = instance.Services.ToList();
            foreach (var service in profileDTO.services)
            {
                SNService serviceInstance;
                if ((serviceInstance = services.FirstOrDefault(x => x.Id == service.snServiceId)) != null)
                {
                    var entity = entities.FirstOrDefault(x => x.Id == service.snProfileSNServiceId) ?? new SNProfileSNService();
                    entity.IsEnabled = service.isEnabled;
                    entity.Service = serviceInstance;
                    entity.ServiceUrl = service.serviceUrl;
                    if (!entity.IsTransient() && listToDelete.Any(x => x.Id == entity.Id))
                    {
                        listToDelete.Remove(listToDelete.First(x => x.Id == entity.Id));
                    }

                    outServices.Add(entity);
                }
            }

            foreach (var service in listToDelete)
            {
                model.RegisterDelete(service);
            }
        }

        /// <summary>
        /// The update links.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="profileDTO">
        /// The profile DTO.
        /// </param>
        /// <param name="outLinks">
        /// The out Links.
        /// </param>
        private void UpdateLinks(SNProfile instance, SNProfileDTO profileDTO, out List<SNLink> outLinks)
        {
            outLinks = new List<SNLink>();
            var model = this.SNLinkModel;
            var listToDelete = instance.Links.ToList();
            var links = profileDTO.links.Any(x => x.snLinkId != 0) ? model.GetAllByIds(profileDTO.links.Where(x => x.snLinkId != 0).Select(x => x.snLinkId).ToList()).ToList() : new List<SNLink>();
            foreach (var link in profileDTO.links)
            {
                var linkInstance = links.FirstOrDefault(x => x.Id == link.snLinkId) ?? new SNLink();
                linkInstance.LinkName = link.linkName; 
                linkInstance.LinkValue = link.linkValue;
                if (!linkInstance.IsTransient() && listToDelete.Any(x => x.Id == linkInstance.Id))
                {
                    listToDelete.Remove(listToDelete.First(x => x.Id == linkInstance.Id));
                }

                outLinks.Add(linkInstance);
            }

            foreach (var link in listToDelete)
            {
                model.RegisterDelete(link);
            }
        }

        /// <summary>
        /// The update map settings.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="sessionDTO">
        /// The session DTO.
        /// </param>
        /// <returns>
        /// The <see cref="SNMapSettings"/>.
        /// </returns>
        private SNMapSettings UpdateMapSettings(SNProfile instance, SNProfileDTO sessionDTO)
        {
            if (sessionDTO.mapSettingsVO != null)
            {
                var mapSettingsVo = sessionDTO.mapSettingsVO;
                var mapSettings = instance.MapSettings ?? new SNMapSettings();
                mapSettings.Country = mapSettingsVo.countryId.HasValue ? this.CountryModel.GetOneById(mapSettingsVo.countryId.Value).Value : null;
                mapSettings.MapProvider = mapSettingsVo.snMapProviderId.HasValue ? this.SNMapProviderModel.GetOneById(mapSettingsVo.snMapProviderId.Value).Value : null;
                mapSettings.ZoomLevel = mapSettingsVo.zoomLevel;

                this.SNMapSettingsModel.RegisterSave(mapSettings);
                return mapSettings;
            }

            return null;
        }

        #endregion
    }
}