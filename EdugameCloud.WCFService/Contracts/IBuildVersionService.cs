namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The Build Version interface.
    /// </summary>
    [ServiceContract]
    public interface IBuildVersionService 
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        BuildVersionDTO[] GetAll();

        /// <summary>
        /// Deletes build by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get build by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        BuildVersionDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="BuildVersionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        BuildVersionDTO Save(BuildVersionDTO build);

        #endregion
    }
}