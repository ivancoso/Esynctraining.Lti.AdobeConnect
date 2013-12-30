namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    using Weborb.Service;

    /// <summary>
    ///     The Build Version interface.
    /// </summary>
    [ServiceContract]
    public interface IBuildVersionService //: ICacheInvalidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<BuildVersionDTO> GetAll();

        /// <summary>
        /// Deletes build by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        /// <summary>
        /// Get build by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<BuildVersionDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<BuildVersionDTO> Save(BuildVersionDTO build);

        #endregion
    }
}