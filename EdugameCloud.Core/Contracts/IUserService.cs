namespace EdugameCloud.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IUserService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SocialUserTokensDTO> GetSocialUserTokens(string key);

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserDTO> GetAll();

        /// <summary>
        /// Gets all users by company Id
        /// </summary>
        /// <param name="companyId">
        /// The id of company to search for
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{UserDTO}"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserWithLoginHistoryDTO> GetAllForCompany(int companyId);

        /// <summary>
        /// Gets all login history for user
        /// </summary>
        /// <param name="userId">
        /// The id of company to search for
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{ContactLoginHistoryDTO}"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryForUser(int userId);

        /// <summary>
        /// The get login history for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryForCompany(int companyId);

        /// <summary>
        /// The get login history paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse ForgotPassword(string email);

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="oldPasswordHash">
        /// The old password.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse UpdatePassword(string email, string oldPasswordHash, string newPassword);

        /// <summary>
        /// The upload batch users.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserDTO> UploadBatchUsers(BatchUsersDTO batch);

        /// <summary>
        /// The get company id by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> GetCompanyIdByEmail(string email);

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserWithSplashScreenDTO> Login(LoginWithHistoryDTO dto);

        /// <summary>
        /// The request session token.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SessionDTO> RequestSessionToken(int userId);

        /// <summary>
        /// Deletes user by id.
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
        /// Deletes user by id.
        /// </summary>
        /// <param name="userIds">
        /// The user Ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteByIds(List<int> userIds);

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse ActivateById(int userId);

        /// <summary>
        /// The update logo.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="logoId">
        /// The logo id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse UpdateLogo(int userId, Guid logoId);

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse DeactivateById(int userId);

        /// <summary>
        /// The activate by ids.
        /// </summary>
        /// <param name="userIds">
        /// The user ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse ActivateByIds(List<int> userIds);

        /// <summary>
        /// The deactivate by ids.
        /// </summary>
        /// <param name="userIds">
        /// The user ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse DeactivateByIds(List<int> userIds);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserDTO> Save(UserDTO user);
        
        /// <summary>
        /// The send activation.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<string> SendActivation(string email);

        /// <summary>
        /// The update password by code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse UpdatePasswordByCode(string code, string newPassword);

        #endregion
    }
}