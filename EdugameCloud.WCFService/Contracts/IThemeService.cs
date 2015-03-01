namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///  The theme service interface.
    /// </summary>
    [ServiceContract]
    public interface IThemeService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ThemeDTO[] GetAll();

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        PagedThemeDTO GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ThemeDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ThemeDTO Save(ThemeDTO resultDto);

        #endregion
    }
}