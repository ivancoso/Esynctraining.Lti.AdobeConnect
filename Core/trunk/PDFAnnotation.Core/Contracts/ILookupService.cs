namespace PDFAnnotation.Core.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using global::Weborb.Service;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The Company Service interface.
    /// </summary>
    [ServiceContract]
    public interface ILookupService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get contact types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<ContactTypeDTO> GetContactTypes();

        /// <summary>
        /// The get company types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<CompanyTypeDTO> GetFirmTypes();

        /// <summary>
        /// The get countries.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<CountryDTO> GetCountries();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<StateDTO> GetStates();

        /// <summary>
        /// The get company contact types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<CompanyContactTypeDTO> GetFirmContactTypes();

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<LookupDTO> GetAll();

        /// <summary>
        /// The is EST in daylight zone.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<bool> IsESTInDaylightZone();

        #endregion
    }
}