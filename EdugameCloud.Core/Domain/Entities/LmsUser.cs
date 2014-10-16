namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class LmsUser : Entity
    {
        public virtual int CompanyLmsId { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Token { get; set; }
    }
}
