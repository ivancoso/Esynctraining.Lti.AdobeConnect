using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper
{
    public interface IZoomOptionsAccessor
    {
        Task<ZoomApiOptions> GetOptions();

    }

}