namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public abstract class PageList
    {
        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }
    }
}