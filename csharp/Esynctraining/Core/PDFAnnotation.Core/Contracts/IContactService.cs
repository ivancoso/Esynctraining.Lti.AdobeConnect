namespace PDFAnnotation.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IContactService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="maxRows">
        /// The max Rows.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> Search(string pattern, int maxRows);

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> GetAllByCompanyId(int companyId);

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> GetAllByIds(List<int> ids);

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
        /// <param name="oldPassword">
        /// The old password.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse UpdatePassword(string email, string oldPassword, string newPassword);

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse UpdatePasswordById(int id, string newPassword);

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> Login(string email, string password);

        /// <summary>
        /// The logout.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse Logout();

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
        /// The notify via RTMP by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse NotifyViaRTMPById(int userId);

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
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactWithDetailsDTO> GetById(int id, int pageSize);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> GetByActivationCode(Guid code);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="contact">
        /// The contact.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ContactDTO> Save(ContactDTO contact);

        #endregion
    }
}