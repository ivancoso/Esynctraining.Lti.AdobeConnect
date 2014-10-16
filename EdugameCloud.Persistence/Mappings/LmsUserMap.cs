namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    public class LmsUserMap : BaseClassMap<LmsUser>
    {
         #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserMap"/> class.
        /// </summary>
        public LmsUserMap()
        {
            this.Map(x => x.UserId).Not.Nullable();
            this.Map(x => x.Username).Nullable();
            this.Map(x => x.Password).Nullable();
            this.Map(x => x.Token).Nullable();
            this.Map(x => x.CompanyLmsId).Nullable();
        }

        #endregion
    }
}
