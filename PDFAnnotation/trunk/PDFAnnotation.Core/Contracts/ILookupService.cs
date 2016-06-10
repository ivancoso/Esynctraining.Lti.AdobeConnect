using Esynctraining.Core.Domain.Entities;

namespace PDFAnnotation.Core.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

 /// <summary>
    ///     The Company Service interface.
    /// </summary>
    [ServiceContract]
    public interface ILookupService
    {
        #region Public Methods and Operators
        /*
        /// <summary>
        /// The get contact types.
        /// </summary>
        /// <returns>
        /// The array of ContactTypeDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ContactTypeDTO[] GetContactTypes();

        /// <summary>
        /// The get countries.
        /// </summary>
        /// <returns>
        /// The array of CountryDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CountryDTO[] GetCountries();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The array of StateDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        StateDTO[] GetStates();


        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The array of LookupDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        LookupDTO GetAll();

    */


        /// <summary>
        /// The get config.
        /// </summary>
        /// <returns>
        /// The ContactDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ConfigDTO GetConfig();

        /// <summary>
        /// The is EST in daylight zone.
        /// </summary>
        /// <returns>
        /// Boolean
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        bool IsESTInDaylightZone();

        #endregion
    }
}