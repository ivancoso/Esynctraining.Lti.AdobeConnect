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
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNProfileService : BaseService, ISNProfileService
    {
        #region Properties

        private SNMapProviderModel SNMapProviderModel => IoC.Resolve<SNMapProviderModel>();

        private SNMapSettingsModel SNMapSettingsModel => IoC.Resolve<SNMapSettingsModel>();

        private SNProfileModel SNProfileModel => IoC.Resolve<SNProfileModel>();

        private SNServiceModel SNServiceModel => IoC.Resolve<SNServiceModel>();

        private SNProfileSNServiceModel SNProfileSNServiceModel => IoC.Resolve<SNProfileSNServiceModel>();

        private SNLinkModel SNLinkModel => IoC.Resolve<SNLinkModel>();

        private AddressModel AddressModel => IoC.Resolve<AddressModel>();

        private StateModel StateModel => IoC.Resolve<StateModel>();

        private CountryModel CountryModel => IoC.Resolve<CountryModel>();

        #endregion

        #region Public Methods and Operators

        public SNProfileDTO GetById(int id)
        {
            SNProfile profile;
            if ((profile = this.SNProfileModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNProfile.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SNProfileDTO(profile);
        }

        public SNProfileExtraDTO[] GetAllByUserId(int userId)
        {
            return this.SNProfileModel.GetAllByUserId(userId).ToArray();
        }

        public SNProfileExtraDTO[] GetAllSharedByUserId(int userId)
        {
            return this.SNProfileModel.GetAllSharedByUserId(userId).ToArray();
        }

        public SNProfileExtraDTO[] GetSharedProfilesByUserId(int userId)
        {
            return this.SNProfileModel.GetAllSharedByUserId(userId).ToArray();
        }

        public SNProfileDTO GetBySMIId(int smiId)
        {
            SNProfile profile;
            if ((profile = this.SNProfileModel.GetOneBySMIId(smiId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNProfile.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SNProfileDTO(profile);
        }

        public SNProfileDTO Save(SNProfileDTO profile)
        {
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
                //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable(NotificationType.Update, this.CurrentUser.With(x => x.Company.Id), instance);
                return new SNProfileDTO(instance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SNProfile.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }


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

        private void UpdateCache()
        {
        }

        private SNProfile ConvertDto(SNProfileDTO profileDTO, SNProfile instance, 
            out List<SNLink> links, out List<SNProfileSNService> services)
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
                if (instance.Address.IsTransient())
                {
                    instance.Address.DateCreated = DateTime.Now;
                }

                instance.Address.DateModified = DateTime.Now;
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