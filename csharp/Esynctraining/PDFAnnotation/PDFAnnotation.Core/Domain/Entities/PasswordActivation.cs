namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Domain.Entities;

    [DataContract]
    public class PasswordActivation : Entity
    {
        public virtual Guid PasswordActivationCode { get; set; }

        public virtual DateTime ActivationDateTime { get; set; }

        public virtual Contact Contact { get; set; }

    }

}