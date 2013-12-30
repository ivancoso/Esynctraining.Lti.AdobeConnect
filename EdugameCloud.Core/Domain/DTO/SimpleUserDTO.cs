namespace EdugameCloud.Core.Domain.DTO
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The user DTO.
    /// </summary>
    [DataContract]
    public class SimpleUserDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleUserDTO"/> class.
        /// </summary>
        public SimpleUserDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleUserDTO"/> class. 
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public SimpleUserDTO(User user)
        {
            if (user != null)
            {
                this.userId = user.Id;
                this.firstName = user.FirstName;
                this.lastName = user.LastName;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        // ReSharper disable ValueParameterNotUsed
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1504:AllAccessorsMustBeMultiLineOrSingleLine", Justification = "Reviewed. Suppression is OK here."), DataMember]
        public string fullName
        {
            get
            {
                return this.firstName + ' ' + this.lastName;
            }
            set { }
        }
        // ReSharper restore ValueParameterNotUsed

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        #endregion
    }
}