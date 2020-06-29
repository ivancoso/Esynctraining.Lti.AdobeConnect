using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.Lti.Lms.Common.Dto.OAuth.Desire2Learn
{
    public class PagedResultSet<T> where T : new()
    {
        public PagingInfo PagingInfo { get; set; }
        public List<T> Items { get; set; }

    }
}
