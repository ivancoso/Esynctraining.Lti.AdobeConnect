namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNProfileDTO> GetBySMIId(int smiId);

        /// <summary>
        /// Get profile by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNProfileDTO> GetById(int id);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNProfileExtraDTO> GetAllByUserId(int userId);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNProfileExtraDTO> GetAllSharedByUserId(int userId);

        /// <summary>
        /// Get profile by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNProfileExtraDTO> GetSharedProfilesByUserId(int userId);

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
        ServiceResponse<SNProfileDTO> Save(SNProfileDTO profile);

        #endregion
    }
}