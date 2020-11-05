using System;
using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper.JWT
{
    public class ZoomApiJwtOptionsConstructorAccessor : IZoomApiJwtOptionsAccessor
    {
        private readonly ZoomApiJwtOptions _apiJwtOptions;

        public ZoomApiJwtOptionsConstructorAccessor(ZoomApiJwtOptions zoomApiJwtOptions)
        {
            _apiJwtOptions = zoomApiJwtOptions ?? throw new ArgumentNullException(nameof(zoomApiJwtOptions));
        }

        public async Task<ZoomApiJwtOptions> GetOptions()
        {
            return _apiJwtOptions;
        }
    }
}