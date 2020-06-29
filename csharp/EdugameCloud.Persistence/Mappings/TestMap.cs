namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The test mapping
    /// </summary>
    public class TestMap : BaseClassMap<Test>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMap"/> class. 
        /// </summary>
        public TestMap()
        {
            this.Map(x => x.TestName).Length(50).Not.Nullable();
            this.Map(x => x.Description).Nullable();
            this.Map(x => x.PassingScore).Nullable();
            this.Map(x => x.TimeLimit).Nullable();
            this.Map(x => x.InstructionTitle).Nullable();
            this.Map(x => x.InstructionDescription).Nullable();
            this.Map(x => x.ScoreFormat).Length(50).Nullable();

            this.HasMany(x => x.Results).ExtraLazyLoad().Cascade.Delete().Inverse();

            this.References(x => x.SubModuleItem).Nullable();
            this.References(x => x.ScoreType).Nullable();
        }

        #endregion
    }
}