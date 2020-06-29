namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The applet item mapping
    /// </summary>
    public class AppletItemMap : BaseClassMap<AppletItem>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemMap"/> class. 
        /// </summary>
        public AppletItemMap()
        {
            this.Map(x => x.AppletName).Length(50).Not.Nullable();
            this.Map(x => x.DocumentXML).CustomType("StringClob").CustomSqlType("nvarchar(max)").Nullable();
            this.HasMany(x => x.Results).ExtraLazyLoad().Cascade.Delete().Inverse();
            this.References(x => x.SubModuleItem).Nullable();
        }

        #endregion
    }
}