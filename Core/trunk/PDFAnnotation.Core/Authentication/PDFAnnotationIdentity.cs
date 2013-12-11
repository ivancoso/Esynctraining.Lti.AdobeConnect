namespace PDFAnnotation.Core.Authentication
{
    using System.Collections.Generic;

    using Esynctraining.Core.Authentication;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The PDF Annotation identity.
    /// </summary>
    public class PDFAnnotationIdentity : IWebOrbIdentity
    {
        #region Fields

        /// <summary>
        ///     The roles.
        /// </summary>
        private readonly List<string> roles = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFAnnotationIdentity"/> class.
        /// </summary>
        /// <param name="userEntity">
        /// The user entity.
        /// </param>
        public PDFAnnotationIdentity(Contact userEntity)
        {
            this.Name = userEntity.Email;
            this.InternalId = userEntity.Id;
            this.InternalEntity = userEntity;
            this.roles.Add(((ContactTypeEnum)userEntity.ContactType.Id).ToString().ToLower());
            this.roles.Add(ContactTypeEnum.Any.ToString().ToLower());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFAnnotationIdentity"/> class.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        public PDFAnnotationIdentity(string email)
        {
            this.Name = email;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PDFAnnotationIdentity" /> class.
        /// </summary>
        protected PDFAnnotationIdentity()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the authentication type.
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "Forms";
            }
        }

        /// <summary>
        ///     Gets or sets the entity.
        /// </summary>
        public Contact InternalEntity { get; protected set; }

        /// <summary>
        ///     Gets or sets the internal id.
        /// </summary>
        public int? InternalId { get; set; }

        /// <summary>
        ///     Gets a value indicating whether is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        public List<string> Roles
        {
            get
            {
                return this.roles;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        /// <returns>
        /// The <see cref="PDFAnnotationIdentity"/>.
        /// </returns>
        public static PDFAnnotationIdentity Parse(string identity)
        {
            return new PDFAnnotationIdentity { Name = identity };
        }

        #endregion
    }
}