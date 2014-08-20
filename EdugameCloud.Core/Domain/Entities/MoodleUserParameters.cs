namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class MoodleUserParameters : Entity
    {
        public virtual string AcId { get; set; }
        public virtual string Provider { get; set; }
        public virtual string Session { get; set; }
        public virtual string Wstoken { get; set; }
        public virtual int Course { get; set; }
        public virtual string Domain { get; set; }
    }
}
