namespace EdugameCloud.WCFService.Contracts
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserDTO[] GetAll();

        /// <summary>
        /// Gets all users by company Id
        /// </summary>
        /// <param name="companyId">
        /// The id of company to search for
        /// </param>
        /// <returns>
        /// The <see cref="UserWithLoginHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAllForCompany?companyId={companyId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserWithLoginHistoryDTO[] GetAllForCompany(int companyId);

        /// <summary>
        /// Gets all login history for user
        /// </summary>
        /// <param name="userId">
        /// The id of company to search for
        /// </param>
        /// <returns>
        /// The <see cref="UserLoginHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLoginHistoryForUser?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserLoginHistoryDTO[] GetLoginHistoryForUser(int userId);

        /// <summary>
        /// The get login history for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="UserLoginHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLoginHistoryForCompany?companyId={companyId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserLoginHistoryDTO[] GetLoginHistoryForCompany(int companyId);

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
        /// The <see cref="PagedUserLoginHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLoginHistoryPaged?pageIndex={pageIndex}&pageSize={pageSize}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PagedUserLoginHistoryDTO GetLoginHistoryPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "ForgotPassword?email={email}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void ForgotPassword(string email);

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
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "UpdatePassword?email={email}&oldPasswordHash={oldPasswordHash}&newPassword={newPassword}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void UpdatePassword(string email, string oldPasswordHash, string newPassword);

        /// <summary>
        /// The upload batch users.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "UploadBatchUsers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        UserDTO[] UploadBatchUsers(BatchUsersDTO batch);

        /// <summary>
        /// The get company id by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetCompanyIdByEmail?email={email}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int GetCompanyIdByEmail(string email);

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="UserWithSplashScreenDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        UserWithSplashScreenDTO Login(LoginWithHistoryDTO dto);

        /// <summary>
        /// The request session token.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "RequestSessionToken?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SessionDTO RequestSessionToken(int userId);

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
        [WebGet(UriTemplate = "DeleteById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int DeleteById(int id);

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="userIds">
        /// The user Ids.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "DeleteByIds", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        int[] DeleteByIds(int[] userIds);

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "ActivateById?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void ActivateById(int userId);

        /// <summary>
        /// The update logo.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="logoId">
        /// The logo id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "UpdateLogo?userId={userId}&logoId={logoId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void UpdateLogo(int userId, Guid logoId);

        /// <summary>
        /// The get social user tokens.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="SocialUserTokensDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSocialUserTokens?key={key}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SocialUserTokensDTO GetSocialUserTokens(string key);

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "DeactivateById?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void DeactivateById(int userId);

        /// <summary>
        /// The activate by ids.
        /// </summary>
        /// <param name="userIds">
        /// The user ids.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "ActivateByIds", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        void ActivateByIds(int[] userIds);

        /// <summary>
        /// The deactivate by ids.
        /// </summary>
        /// <param name="userIds">
        /// The user ids.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "DeactivateByIds", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        void DeactivateByIds(int[] userIds);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        UserDTO Save(UserDTO user);
        
        /// <summary>
        /// The send activation.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "SendActivation?email={email}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string SendActivation(string email);

        /// <summary>
        /// The update password by code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "UpdatePasswordByCode?code={code}&newPassword={newPassword}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void UpdatePasswordByCode(string code, string newPassword);

        /// <summary>
        /// The activate by code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "ActivateByCode?code={code}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        bool ActivateByCode(string code);

        #endregion
    }
}