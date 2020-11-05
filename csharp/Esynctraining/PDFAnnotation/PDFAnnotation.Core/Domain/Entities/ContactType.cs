namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class ContactType : Entity
    {
        public virtual string ContactTypeName { get; set; }

        public virtual string ACMappedType { get; set; }

    }

}