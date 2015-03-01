namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The contact login history DTO.
    /// </summary>
    [DataContract]
    public class UserLoginHistoryDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginHistoryDTO"/> class.
        /// </summary>
        public UserLoginHistoryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginHistoryDTO"/> class.
        /// </summary>
        /// <param name="clh">
        /// The contact login history.
        /// </param>
        /// <param name="company">
        /// The company.
        /// </param>
        public UserLoginHistoryDTO(UserLoginHistory clh, Company company)
        {
            this.userLoginHistoryId = clh.Id;
            this.dateCreated = clh.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.fromIP = clh.FromIP;
            this.application = clh.Application;
            this.userId = clh.User.With(x => x.Id);
            this.userName = clh.User.With(x => x.FullName);
            this.companyName = company.With(x => x.CompanyName);
            this.companyId = company.With(x => x.Id);
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember]
        public string userName { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string companyName { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the user login history id.
        /// </summary>
        [DataMember]
        public int userLoginHistoryId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the document title.
        /// </summary>
        [DataMember]
        public string fromIP { get; set; }

        /// <summary>
        /// Gets or sets the document name.
        /// </summary>
        [DataMember]
        public string application { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        #endregion
    }
}