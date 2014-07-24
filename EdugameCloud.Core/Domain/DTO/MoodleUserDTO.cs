namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

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
                this.userId = user.UserId;
                this.userName = user.UserName;
                this.password = user.Password;
                this.token = user.Token;
                this.domain = user.Domain;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets password
        /// </summary>
        [DataMember]
        public virtual string password { get; set; }

        /// <summary>
        /// Gets or sets user name
        /// </summary>
        [DataMember]
        public virtual string userName { get; set; }

        /// <summary>
        /// Gets or sets token
        /// </summary>
        [DataMember]
        public virtual string token { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public virtual string domain { get; set; }


        #endregion
    }
}
