namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The SN Group discussion.
    /// </summary>
    public class SNGroupDiscussionMap : BaseClassMap<SNGroupDiscussion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNGroupDiscussionMap"/> class. 
        /// </summary>
        public SNGroupDiscussionMap()
        {
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.Map(x => x.GroupDiscussionTitle).Length(255).Not.Nullable();
            this.Map(x => x.GroupDiscussionData).CustomType("StringClob").CustomSqlType("ntext").Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
        }
    }
}