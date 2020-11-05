using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Persistence.Mappings.CompanyContactMapping
{
    /// <summary>
    /// The password activation map.
    /// </summary>
    public class PasswordActivationMap : BaseClassMap<PasswordActivation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordActivationMap"/> class.
        /// </summary>
        public PasswordActivationMap()
        {
            this.Map(x => x.PasswordActivationCode).Not.Nullable();
            this.Map(x => x.ActivationDateTime).Not.Nullable();
            this.References(x => x.Contact).Not.Nullable();
        }

    }

}