using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.OAuth
{
    internal sealed class QueryParameterComparer : IComparer<QueryParameter>
    {
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.CompareOrdinal(x.Value, y.Value);
            }

            return string.CompareOrdinal(x.Name, y.Name);
        }

    }

}
