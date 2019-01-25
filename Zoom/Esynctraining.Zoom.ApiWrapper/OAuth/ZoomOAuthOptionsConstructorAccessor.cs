using System;
using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper.OAuth
{
    public class ZoomOAuthOptionsConstructorAccessor : IZoomOAuthOptionsAccessor
    {
        private readonly ZoomOAuthOptions _oAuthOptions;

        public ZoomOAuthOptionsConstructorAccessor(ZoomOAuthOptions zoomOAuthOptions)
        {
            _oAuthOptions = zoomOAuthOptions ?? throw new ArgumentNullException(nameof(zoomOAuthOptions));
        }

        public async Task<ZoomOAuthOptions> GetOptions()
        {
            return _oAuthOptions;
        }
    }
}