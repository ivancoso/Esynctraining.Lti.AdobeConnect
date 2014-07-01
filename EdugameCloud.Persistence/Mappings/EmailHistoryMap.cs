namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    
    /// <summary>
    /// The user map.
    /// </summary>
    public class EmailHistoryMap : BaseClassMap<EmailHistory>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHistory"/> class.
        /// </summary>
        public EmailHistoryMap()
        {
            this.Map(x => x.SentTo).Length(50).Not.Nullable();
            this.Map(x => x.SentFrom).Length(50).Not.Nullable();
            this.Map(x => x.SentToName).Length(100).Nullable();
            this.Map(x => x.SentFromName).Length(100).Nullable();
            this.Map(x => x.SentCc).Length(200).Nullable();
            this.Map(x => x.SentBcc).Length(200).Nullable();
            this.Map(x => x.Subject).Length(500).Nullable();
            this.Map(x => x.Message).Nullable();
            this.Map(x => x.Body).CustomType("StringClob").CustomSqlType("nvarchar(max)").Nullable();
            this.Map(x => x.Date).Not.Nullable();
            this.References(x => x.User).Nullable();
        }


        #endregion
    }
}