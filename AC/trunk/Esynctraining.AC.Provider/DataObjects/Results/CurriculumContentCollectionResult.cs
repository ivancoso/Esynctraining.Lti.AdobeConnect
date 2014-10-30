using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class CurriculumContentCollectionResult : GenericCollectionResultBase<CurriculumContent>
    {
        public CurriculumContentCollectionResult(StatusInfo status) : base(status)
        {
        }

        public CurriculumContentCollectionResult(StatusInfo status, IEnumerable<CurriculumContent> values)
            : base(status, values)
        {
        }

        public CurriculumContentCollectionResult(StatusInfo status, IEnumerable<CurriculumContent> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        public string ScoId { get; set; }
    }
}
