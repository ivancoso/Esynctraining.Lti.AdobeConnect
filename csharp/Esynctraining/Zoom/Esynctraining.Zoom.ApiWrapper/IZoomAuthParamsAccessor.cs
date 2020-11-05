using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper
{
    public interface IZoomAuthParamsAccessor
    {
        Task<string> GetApiUrl();
        Task<string> GetAuthToken();
    }
}