using System.Collections.Generic;

namespace EdugameCloud.Lti.DTO
{
    public sealed class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public long Total { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
