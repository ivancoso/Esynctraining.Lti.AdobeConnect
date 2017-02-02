using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Persistence.Mappings
{
    public class SchoolMap : BaseClassMap<School>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolMap"/> class.
        /// </summary>
        public SchoolMap()
        {
            this.Map(x => x.SchoolNumber).Length(20);
            this.Map(x => x.StateId).Not.Nullable();
            this.Map(x => x.OnsiteOperator).Length(500);
            this.Map(x => x.FirstDirector).Length(200);
            this.Map(x => x.MainPhone).Length(20);
            this.Map(x => x.Fax).Length(20);
            this.Map(x => x.SpeedDialNumber).Length(20);
            this.Map(x => x.SchoolEmail).Length(100);
            this.Map(x => x.CorporateName).Length(100);
            this.Map(x => x.FBCRepresentative).Length(200);
            this.Map(x => x.ESSRepresentative).Length(200);
            this.Map(x => x.StandardsRepresentative).Length(200);
            this.Map(x => x.AdvRepresentative).Length(200);
            this.Map(x => x.MktgRepresentative).Length(200);
            this.Map(x => x.AccountName).Length(100);

            this.References(x => x.State).Not.Nullable();

        }

        #endregion
    }
}