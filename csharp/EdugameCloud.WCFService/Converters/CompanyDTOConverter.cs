namespace EdugameCloud.WCFService.Converters
{
    using System;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.DTO;

    public sealed class CompanyDTOConverter : BaseConverter<CompanyDTO, Company>
    {
        #region Fields

        /// <summary>
        /// The user model.
        /// </summary>
        private readonly UserModel userModel;

        /// <summary>
        /// The address DTO converter.
        /// </summary>
        private readonly BaseConverter<AddressDTO, Address> addressDTOConverter;

        /// <summary>
        /// The company theme DTO converter.
        /// </summary>
        private readonly BaseConverter<CompanyThemeDTO, CompanyTheme> companyThemeDTOConverter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyDTOConverter"/> class.
        /// </summary>
        /// <param name="userModel">
        /// The user Model.
        /// </param>
        /// <param name="addressDTOConverter">
        /// The address DTO Converter.
        /// </param>
        /// <param name="companyThemeDTOConverter">
        /// The company Theme DTO Converter.
        /// </param>
        public CompanyDTOConverter(
            UserModel userModel,
            BaseConverter<AddressDTO, Address> addressDTOConverter,
            BaseConverter<CompanyThemeDTO, CompanyTheme> companyThemeDTOConverter)
        {
            this.userModel = userModel;
            this.addressDTOConverter = addressDTOConverter;
            this.companyThemeDTOConverter = companyThemeDTOConverter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO and save.
        /// </summary>
        /// <param name="companyDto">
        /// The company DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="flushUpdates">
        /// The flush Updates.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        public override Company Convert(CompanyDTO companyDto, Company instance, bool flushUpdates = false)
        {
            instance = instance ?? new Company();
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.UseEventMapping = companyDto.useEventMapping;
            instance.CompanyName = companyDto.companyName;
            instance.Status = companyDto.isActive ? CompanyStatus.Active : CompanyStatus.Inactive;
            instance.DateModified = DateTime.Now;
            if (companyDto.addressVO != null)
            {
                instance.Address = this.addressDTOConverter.Convert(companyDto.addressVO, instance.Address, true);
            }

            if (companyDto.themeVO != null)
            {
                instance.Theme = this.companyThemeDTOConverter.Convert(companyDto.themeVO, instance.Theme, true);
            }

            instance.PrimaryContact = (companyDto.primaryContactId.HasValue && companyDto.primaryContactId != default(int)) ? this.userModel.GetOneById(companyDto.primaryContactId.Value).Value : instance.PrimaryContact;
            return instance;
        }

        #endregion
    }
}