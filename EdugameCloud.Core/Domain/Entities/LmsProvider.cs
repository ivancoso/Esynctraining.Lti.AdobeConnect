namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class LmsProvider : Entity
    {
        public virtual int Id { get; set; }

        public virtual string LmsProviderName { get; set; }
    }
}
