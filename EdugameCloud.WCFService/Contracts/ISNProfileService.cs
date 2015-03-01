namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN profile interface.
    /// </summary>
    [ServiceContract]
    public interface ISNProfileService 
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get service by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfileDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileDTO GetBySMIId(int smiId);

        /// <summary>
        /// Get profile by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfileDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileDTO GetById(int id);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfileExtraDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileExtraDTO[] GetAllByUserId(int userId);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfileExtraDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileExtraDTO[] GetAllSharedByUserId(int userId);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SNProfileExtraDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileExtraDTO[] GetSharedProfilesByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="profile">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNProfileDTO Save(SNProfileDTO profile);

        #endregion
    }
}