namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The MoodleService interface.
    /// </summary>
    [ServiceContract]
    public interface IMoodleService
    {
        #region Public Methods and Operators
        
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        /*
        [OperationContract]
        ServiceResponse<MoodleUserDTO> Save(MoodleUserDTO user);
        */
        /// <summary>
        /// The get quizes.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="username">
        /// The token.
        /// </param>
        /// <param name="password">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<MoodleQuizInfoDTO> GetQuizesForUser(MoodleUserInfoDTO userInfo);

        /// <summary>
        /// The get quizes.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        bool ConvertQuizes(MoodleQuizConvertDTO quiz);

        #endregion
    }
}
