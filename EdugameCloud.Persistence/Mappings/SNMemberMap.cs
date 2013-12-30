namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The SN session member map.
    /// </summary>
    public class SNMemberMap : BaseClassMap<SNMember>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMemberMap"/> class.
        /// </summary>
        public SNMemberMap()
        {
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.Map(x => x.Participant).Length(255).Not.Nullable();
            this.Map(x => x.ParticipantProfile).Not.Nullable();
            this.Map(x => x.DateCreated).Nullable();
            this.Map(x => x.IsBlocked).Not.Nullable();
        }

        #endregion
    }
}