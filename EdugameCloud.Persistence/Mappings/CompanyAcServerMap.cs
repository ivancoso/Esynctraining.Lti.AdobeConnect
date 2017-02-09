using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Persistence.Mappings
{
    public class CompanyAcServerMap : BaseClassMap<CompanyAcServer>
    {
        #region Constructors and Destructors
       
        public CompanyAcServerMap()
        {
            Table("CompanyAcDomains");
            this.Map(x => x.AcServer).Length(100).Not.Nullable();
            this.Map(x => x.Username).Length(50);
            this.Map(x => x.Password).Length(50);
            this.Map(x => x.IsDefault);
           
            this.References(x => x.Company).Nullable();
        }

        #endregion
    }
}