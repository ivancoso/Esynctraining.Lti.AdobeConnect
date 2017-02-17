using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public class ScoContentsFilter
    {
        public string ScoId { get; set; }
        public SortOptions SortOptions { get; set; }
        public PageOptions PageOptions { get; set; }
        public string NameLikeFilter { get; set; }
        public IEnumerable<ScoType> Types { get; set; }
    }
}