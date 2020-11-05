namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class Country : Entity
    {
        public virtual string CountryCode { get; set; }

        public virtual string CountryCode3 { get; set; }

        public virtual string CountryName { get; set; }

    }

}