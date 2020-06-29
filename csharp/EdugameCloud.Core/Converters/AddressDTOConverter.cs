namespace EdugameCloud.Core.Converters
{
    using System;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The company theme DTO converter.
    /// </summary>
    public sealed class AddressDTOConverter : BaseConverter<AddressDTO, Address>
    {
        #region Fields

        /// <summary>
        /// The address model.
        /// </summary>
        private readonly AddressModel addressModel;

        /// <summary>
        /// The country model.
        /// </summary>
        private readonly CountryModel countryModel;

        /// <summary>
        /// The state model.
        /// </summary>
        private readonly StateModel stateModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressDTOConverter"/> class.
        /// </summary>
        /// <param name="addressModel">
        /// The address Model.
        /// </param>
        /// <param name="countryModel">
        /// The country Model.
        /// </param>
        /// <param name="stateModel">
        /// The state Model.
        /// </param>
        public AddressDTOConverter(
            AddressModel addressModel, 
            CountryModel countryModel,
            StateModel stateModel)
        {
            this.addressModel = addressModel;
            this.countryModel = countryModel;
            this.stateModel = stateModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert DTO and save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
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
        public override Address Convert(AddressDTO dto, Address instance, bool flushUpdates = false)
        {
            instance = instance ?? new Address();
            var addressVo = dto;
            instance.Address1 = addressVo.address1;
            instance.Address2 = addressVo.address2;
            instance.City = addressVo.city;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.Country = addressVo.countryId.HasValue
                                   ? this.countryModel.GetOneById(addressVo.countryId.Value).Value
                                   : null;
            instance.State = addressVo.stateId.HasValue
                                   ? this.stateModel.GetOneById(addressVo.stateId.Value).Value
                                   : null;
            instance.Zip = addressVo.zip;

            if (flushUpdates)
            {
                this.addressModel.RegisterSave(instance);
            }

            return instance;
        }

        #endregion
    }
}