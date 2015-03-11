using System.Collections.Generic;

namespace EdugameCloud.Lti.OAuth.Desire2Learn
{
    public class PagedResultSet<T> where T: new ()
    {
        public PagingInfo PagingInfo { get; set; }
        public List<T> Items { get; set; }

    }
}