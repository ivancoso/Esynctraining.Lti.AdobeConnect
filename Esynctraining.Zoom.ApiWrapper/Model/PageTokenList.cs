namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public abstract class PageTokenList : PageList
    {
        public string NextPageToken { get; set; }
    }
}