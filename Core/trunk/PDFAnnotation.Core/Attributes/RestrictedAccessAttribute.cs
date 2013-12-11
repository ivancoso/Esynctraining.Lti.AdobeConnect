namespace PDFAnnotation.Core.Attributes
{
    using System.Linq;

    using Esynctraining.Core.Attributes;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The DC restricted access attribute.
    /// </summary>
    public class RestrictedAccessAttribute : WebOrbRestrictedAccessAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictedAccessAttribute"/> class.
        /// </summary>
        public RestrictedAccessAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictedAccessAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public RestrictedAccessAttribute(params ContactTypeEnum[] roles)
        {
            this.Roles = roles.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictedAccessAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public RestrictedAccessAttribute(params string[] roles)
            : base(roles)
        {
        }
    }
}
