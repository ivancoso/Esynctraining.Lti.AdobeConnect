namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class State : Entity
    {
        public virtual string StateName { get; set; }

        public virtual string StateCode { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual Country Country { get; set; }

    }

}