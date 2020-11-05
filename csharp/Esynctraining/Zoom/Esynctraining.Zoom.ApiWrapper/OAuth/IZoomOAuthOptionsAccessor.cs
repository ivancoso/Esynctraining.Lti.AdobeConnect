using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper.OAuth
{
    public interface IZoomOAuthOptionsAccessor
    {
        Task<ZoomOAuthOptions> GetOptions();
    }
}