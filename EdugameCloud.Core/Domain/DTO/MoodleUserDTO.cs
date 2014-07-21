namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using NHibernate.Hql.Ast.ANTLR;

    [DataContract]
    public class MoodleUserDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserDTO"/> class.
        /// </summary>
        public MoodleUserDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserDTO"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public MoodleUserDTO(MoodleUser user)
        {
            if (user != null)
            {
                this.id = user.Id;
                this.userName = user.UserName;
                this.firstName = user.FirstName;
                this.lastName = user.LastName;
                this.password = user.Password;
                this.companyId = user.CompanyId;
                this.moodleUserId = user.MoodleUserId;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [DataMember]
        public virtual int id { get; set; }

        /// <summary>
        /// Gets or sets user name
        /// </summary>
        [DataMember]
        public virtual string userName { get; set; }

        /// <summary>
        /// Gets or sets first name
        /// </summary>
        [DataMember]
        public virtual string firstName { get; set; }

        /// <summary>
        /// Gets or sets last name
        /// </summary>
        [DataMember]
        public virtual string lastName { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        [DataMember]
        public virtual string password { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public virtual int companyId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public virtual int moodleUserId { get; set; }

        #endregion
    }
}
