namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using ClosedXML.Excel;
    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    //using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Esynctraining.NHibernate;
    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    /// <summary>
    /// The user model.
    /// </summary>
    public class UserModel : BaseModel<User, int>
    {
        #region Fields

        /// <summary>
        /// The email validator.
        /// </summary>
        private static readonly Regex emailValidator =
            new Regex(
                @"^[a-z0-9_\+-]+(\.[a-z0-9_\+-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.([a-z]{2,4})$", 
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The language model.
        /// </summary>
        private readonly LanguageModel languageModel;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        /// <summary>
        /// The time zone model.
        /// </summary>
        private readonly TimeZoneModel timeZoneModel;

        private readonly FileModel fileModel;

        /// <summary>
        /// The user role model.
        /// </summary>
        private readonly UserRoleModel userRoleModel;

        #endregion

        #region Constructors and Destructors
        
        public UserModel(
            UserRoleModel userRoleModel, 
            LanguageModel languageModel, 
            TimeZoneModel timeZoneModel, 
            FileModel fileModel,
            ApplicationSettingsProvider settings, 
            IRepository<User, int> repository)
            : base(repository)
        {
            this.userRoleModel = userRoleModel;
            this.languageModel = languageModel;
            this.timeZoneModel = timeZoneModel;
            this.fileModel = fileModel;
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        //public IEnumerable<string> GetAdministratorEmails()
        //{
        //    const int AdminRole = (int)UserRoleEnum.Admin;
        //    QueryOver<User, User> queryOver =
        //        new QueryOverUser().GetQueryOver().And(x => x.UserRole.Id == AdminRole).Select(x => x.Email);
        //    return this.Repository.FindAll<string>(queryOver);
        //}

        /// <summary>
        ///     The get all for company.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable{User}" />.
        /// </returns>
        public override IEnumerable<User> GetAll()
        {
            QueryOver<User, User> defaultQuery = new QueryOverUser().GetQueryOver().Fetch(x => x.UserRole).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by emails.
        /// </summary>
        /// <param name="emails">
        /// The emails.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Contact}"/>.
        /// </returns>
        public virtual IEnumerable<User> GetAllByEmails(List<string> emails)
        {
            var queryOver = new QueryOverUser().GetQueryOver();
            var disjunction = new Disjunction();
            foreach (var email in emails)
            {
                disjunction.Add(Restrictions.On<User>(x => x.Email).IsInsensitiveLike(email));
            }

            queryOver.Where(disjunction);

            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{User}"/>.
        /// </returns>
        public override IEnumerable<User> GetAllByIds(List<int> ids)
        {
            QueryOver<User, User> queryOver = new QueryOverUser().GetQueryOver().AndRestrictionOn(x => x.Id).IsIn(ids);
            return this.Repository.FindAll(queryOver);
        }

        //public IEnumerable<int> GetAllDependantUserIdsIncludingCurrent(User user)
        //{
        //    List<int> result =
        //        this.Repository.StoreProcedureForMany<int>(
        //            "GetChildUserIds", 
        //            new StoreProcedureParam<int>("UserId", user.Id)).ToList();
        //    result.Add(user.Id);
        //    return result;
        //}

        /// <summary>
        /// The get all for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{User}"/>.
        /// </returns>
        public IEnumerable<UserWithLoginHistoryDTO> GetAllForCompany(int companyId)
        {
            var queryOver = new QueryOverUser().GetQueryOver().Where(x => x.Company.Id == companyId).Fetch(x => x.UserRole).Eager;
            var users = this.Repository.FindAll(queryOver).ToList();
            var usersIds = users.Select(x => x.Id).ToList();
            User u = null;
            UserLoginHistory h = null;
            UserLastLoginFromStoredProcedureDTO dto = null;
            var queryOver2 = new QueryOverUser().GetQueryOver(() => u).WhereRestrictionOn(() => u.Id).IsIn(usersIds)
                .JoinQueryOver(() => u.LoginHistory, () => h).OrderBy(() => h.DateCreated).Desc
                .SelectList(l => l.Select(() => u.Id).WithAlias(() => dto.userId).Select(() => h.DateCreated).WithAlias(() => dto.loginDate))
                .TransformUsing(Transformers.AliasToBean<UserLastLoginFromStoredProcedureDTO>()).Take(1);
            var logindatesForUsers = this.Repository.FindAll<UserLastLoginFromStoredProcedureDTO>(queryOver2).ToList();

            return users.Select(x => new UserWithLoginHistoryDTO(x, logindatesForUsers.FirstOrDefault(hl => hl.userId == x.Id).Return(hl => hl.loginDate, (DateTime?)null)));
        }
        
        //public IEnumerable<User> GetAllForUsersPaged(
        //    IEnumerable<int> userIds, 
        //    int pageIndex, 
        //    int pageSize, 
        //    out int totalCount)
        //{
        //    QueryOver<User, User> queryOver =
        //        new DefaultQueryOver<User, int>().GetQueryOver()
        //            .WhereRestrictionOn(x => x.Id)
        //            .IsInG(userIds)
        //            .OrderBy(x => x.FirstName)
        //            .Asc;
        //    QueryOver<User, User> rowCountQuery = queryOver.ToRowCountQuery();
        //    totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
        //    QueryOver<User> pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
        //    return this.Repository.FindAll(pagedQuery);
        //}
        
        //public virtual IEnumerable<User> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        //{
        //    QueryOver<User, User> queryOver =
        //        new DefaultQueryOver<User, int>().GetQueryOver().OrderBy(x => x.FirstName).Asc;
        //    QueryOver<User, User> rowCountQuery = queryOver.ToRowCountQuery();
        //    totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
        //    QueryOver<User> pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
        //    return this.Repository.FindAll(pagedQuery);
        //}

        //public IEnumerable<int> GetAllUsersThatCanApproveThisPin(int pinId)
        //{
        //    return
        //        this.Repository.StoreProcedureForMany<int>(
        //            "GetAllUsersThatCanApproveThisPin", 
        //            new StoreProcedureParam<int>("PinId", pinId)).ToList();
        //}

        /// <summary>
        /// The get count for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Int32}"/>.
        /// </returns>
        public IFutureValue<int> GetCountForCompany(int companyId)
        {
            QueryOver<User, User> queryOver =
                new QueryOverUser().GetQueryOver().And(x => x.Company.Id == companyId).ToRowCountQuery();
            return this.Repository.FindOne<int>(queryOver);
        }

        public IEnumerable<int> GetCompanyIdsByUsersProperties(string firstName, string lastName, string email)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(email))
            {
                return new List<int>();
            }

            QueryOver<User, User> queryOver = new QueryOverUser().GetQueryOver();

            if (!string.IsNullOrEmpty(firstName))
            {
                queryOver.And(Restrictions.On<User>(u => u.FirstName).IsInsensitiveLike(firstName, MatchMode.Anywhere));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                queryOver.And(Restrictions.On<User>(u => u.LastName).IsInsensitiveLike(lastName, MatchMode.Anywhere));
            }

            if (!string.IsNullOrEmpty(email))
            {
                queryOver.And(Restrictions.On<User>(u => u.Email).IsInsensitiveLike(email, MatchMode.Anywhere));
            }

            queryOver.Select(u => u.Company.Id).TransformUsing(Transformers.DistinctRootEntity);

            return this.Repository.FindAll<int>(queryOver);
        }


        /// <summary>
        /// The get one by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{User}"/>.
        /// </returns>
        public virtual IFutureValue<User> GetOneByEmail(string email)
        {
            string emailToLower = email.ToLower();
            QueryOver<User, User> queryOver =
                new QueryOverUser().GetQueryOver().WhereRestrictionOn(x => x.Email).IsInsensitiveLike(emailToLower);
            return this.Repository.FindOne(queryOver);
        }

        public virtual User GetByEmailWithRole(string email)
        {
            string emailToLower = email.ToLower();
            QueryOver<User, User> queryOver =
                new QueryOverUser().GetQueryOver()
                .Fetch(x => x.UserRole).Eager
                .WhereRestrictionOn(x => x.Email).IsInsensitiveLike(emailToLower);
            return this.Repository.FindOne(queryOver).Value;
        }

        //public virtual IFutureValue<User> GetOneByUserNameOrEmailAndPassword(
        //    string emailOrUserName, 
        //    byte[] passwordHash)
        //{
        //    string passwordHashString = BitConverter.ToString(passwordHash);
        //    return
        //        this.Repository.Session.Query<User>()
        //            .Where(x => (x.Email.ToLower() == emailOrUserName.ToLower()) && x.Password == passwordHashString)
        //            .ToFutureValue();
        //}

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public void RealDelete(User entity, bool flush = false)
        {
            try
            {
                var company = entity.Company;

                if (entity.Logo != null)
                {
                    this.fileModel.RegisterDelete(entity.Logo);
                }

                foreach (var file in entity.Files)
                {
                    if (file.Equals(company.Theme.With(x => x.Logo)))
                    {
                        company.Theme.Logo = null;
                        IoC.Resolve<CompanyThemeModel>().RegisterSave(company.Theme);
                    }

                    this.fileModel.RegisterDelete(file);
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("UserModel.RealDelete", ex);
            }

            this.fileModel.Flush();

            this.Refresh(ref entity);

            base.RegisterDelete(entity, flush);
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public override void RegisterDelete(User entity, bool flush)
        {
            entity.Status = UserStatus.Deleted;
            this.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The upload batch of users.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="fileStream">
        /// The file stream.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="failed">
        /// The failed.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="sendActivation">
        /// The send Activation.
        /// </param>
        /// <param name="notifyViaRTMP">
        /// The notify via RTMP.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{User}"/>.
        /// </returns>
        public IEnumerable<User> UploadBatchOfUsers(
            Company company, 
            Stream fileStream, 
            string type,
            out List<string> failed, 
            out string error, 
            Action<User> sendActivation = null,
            // ReSharper disable once InconsistentNaming
            bool notifyViaRTMP = true)
        {
            var result = new List<User>();
            failed = new List<string>();
            error = null;
            try
            {
                var bookToRead = new XLWorkbook(fileStream);
                IXLWorksheet sheet = bookToRead.Worksheets.FirstOrDefault();
                if (sheet != null)
                {
                    var license = company.Licenses.FirstOrDefault();
                    var currentUsersCount = this.GetCountForCompany(company.Id).Value;
                    IXLRows rows = sheet.RowsUsed();
                    foreach (IXLRow row in rows)
                    {
                        IXLCell nameCell = row.FirstCellUsed();
                        string nameValue = nameCell.With(x => x.Value.ToString());
                        IXLCell familyNameCell = nameCell.With(x => x.CellRight());
                        string familyNameValue = familyNameCell.With(x => x.Value.ToString());
                        IXLCell emailCell = familyNameCell.With(x => x.CellRight());
                        string emailValue = emailCell.With(x => x.Value.ToString());
                        IXLCell passwordCell = emailCell.With(x => x.CellRight());
                        string passwordCellValue = passwordCell.With(x => x.Value.ToString());
                        if (!string.IsNullOrWhiteSpace(emailValue) && emailValidator.Match(emailValue).Success)
                        {
                            if (this.GetOneByEmail(emailValue).Value != null)
                            {
                                failed.Add(string.Format("{0}{1}{2}{3}", nameValue, familyNameValue, emailValue, ": Email exist"));
                                continue;
                            }

                            if (license != null && license.TotalLicensesCount < currentUsersCount + result.Count + 1)
                            {
                                error = "Max users amount is reached";
                                return result;
                            }

                            try
                            {
                                var instance = new User
                                                   {
                                                       Email = emailValue, 
                                                       FirstName = nameValue, 
                                                       LastName = familyNameValue, 
                                                       Company = company, 
                                                       DateCreated = DateTime.Now, 
                                                       DateModified = DateTime.Now, 
                                                       Status = UserStatus.Inactive, 
                                                       Language =
                                                           this.languageModel.GetOneById(
                                                               this.GetLanguageIdBatchImport(emailCell)).Value, 
                                                       TimeZone =
                                                           this.timeZoneModel.GetOneById(
                                                               this.GetTimeZoneIdForBatchImport(emailCell)).Value, 
                                                       UserRole =
                                                           this.userRoleModel.GetOneById((int)UserRoleEnum.User)
                                                           .Value, 
                                                       CreatedBy = company.PrimaryContact, 
                                                       ModifiedBy = null
                                                   };
                                if (string.IsNullOrWhiteSpace(passwordCellValue))
                                {
                                    instance.SetPassword(Password.CreateAlphaNumericRandomPassword(8));
                                    if (sendActivation != null)
                                    {
                                        sendActivation(instance);
                                    }
                                }
                                else
                                {
                                    instance.Status = UserStatus.Active;
                                    instance.SetPassword(passwordCellValue);
                                }

                                this.RegisterSave(instance, true);
                                //if (notifyViaRTMP)
                                //{
                                //    IoC.Resolve<RealTimeNotificationModel>()
                                //        .NotifyClientsAboutChangesInTable<User>(
                                //            NotificationType.Update, 
                                //            instance.Company.Id, 
                                //            instance.Id);
                                //}

                                result.Add(instance);
                            }
                            catch (Exception ex)
                            {
                                failed.Add(
                                    string.Format(
                                        "{0}{1}{2}{3}", 
                                        nameValue, 
                                        familyNameValue, 
                                        emailValue, 
                                        ": Creation error + " + ex.With(x => x.Message)));
                            }
                        }
                        else
                        {
                            failed.Add(
                                string.Format("{0}{1}{2}{3}", nameValue, familyNameValue, emailValue, ": Invalid email"));
                        }
                    }
                }
                else
                {
                    error = "Wrong excel file";
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// The upload batch of users.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="fileContent">
        /// The file content.
        /// </param>
        /// <param name="fileType">
        /// The file Type.
        /// </param>
        /// <param name="failed">
        /// The failed.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="sendActivation">
        /// The send Activation.
        /// </param>
        /// <param name="notifyViaRTMP">
        /// The notify via RTMP.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{User}"/>.
        /// </returns>
        public IEnumerable<User> UploadBatchOfUsers(
            Company company, 
            string fileContent, 
            string fileType,
            out List<string> failed, 
            out string error,
            Action<User> sendActivation = null,
            // ReSharper disable once InconsistentNaming
            bool notifyViaRTMP = true)
        {
            return this.UploadBatchOfUsers(
                company, 
                new MemoryStream(Encoding.UTF8.GetBytes(fileContent)), 
                fileType,
                out failed, 
                out error,
                sendActivation,
                notifyViaRTMP);
        }

        /// <summary>
        /// The get one by token.
        /// </summary>
        /// <param name="sessionToken">
        /// The session token.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{User}"/>.
        /// </returns>
        public IFutureValue<User> GetOneByToken(string sessionToken)
        {
            var queryOver = new QueryOverUser().GetQueryOver()
                .Fetch(x => x.UserRole).Eager
                .WhereRestrictionOn(x => x.SessionToken).IsLike(sessionToken).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get language batch import.
        /// </summary>
        /// <param name="emailCell">
        /// The email cell.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetLanguageIdBatchImport(IXLCell emailCell)
        {
            int result;
            if (
                !int.TryParse(
                    emailCell.With(x => x.CellRight().With(y => y.CellRight())).With(x => x.Value.ToString()), 
                    NumberStyles.Any, 
                    CultureInfo.InvariantCulture, 
                    out result))
            {
                return int.Parse(this.settings.BatchImportDefaultLanguageId);
            }

            return result;
        }

        /// <summary>
        /// The get time zone for batch import.
        /// </summary>
        /// <param name="emailCell">
        /// The email cell.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetTimeZoneIdForBatchImport(IXLCell emailCell)
        {
            int result;
            if (
                !int.TryParse(
                    emailCell.With(x => x.CellRight()).With(x => x.Value.ToString()), 
                    NumberStyles.Any, 
                    CultureInfo.InvariantCulture, 
                    out result))
            {
                return int.Parse(this.settings.BatchImportDefaultTimeZoneId);
            }

            return result;
        }

        #endregion
    }
}