using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper.JWT
{
    public interface IZoomApiJwtOptionsAccessor
    {
        Task<ZoomApiJwtOptions> GetOptions();
    }
}