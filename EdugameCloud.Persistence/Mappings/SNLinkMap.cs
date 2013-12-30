namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN Link discussion message.
    /// </summary>
    public class SNLinkMap : BaseClassMap<SNLink>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNLinkMap"/> class.
        /// </summary>
        public SNLinkMap()
        {
            this.Map(x => x.LinkName).Length(255).Not.Nullable();
            this.Map(x => x.LinkValue).Length(2000).Nullable();
            this.References(x => x.Profile).Not.Nullable();
        }
    }
}