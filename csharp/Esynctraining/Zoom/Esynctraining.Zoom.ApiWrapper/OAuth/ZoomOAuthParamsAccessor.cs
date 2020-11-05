using System.Threading.Tasks;

namespace Esynctraining.Zoom.ApiWrapper.OAuth
{
    public class ZoomOAuthParamsAccessor : IZoomAuthParamsAccessor
    {
        private ZoomOAuthOptions _options;
        private readonly IZoomOAuthOptionsAccessor _optionsAccessor;

        public ZoomOAuthParamsAccessor(IZoomOAuthOptionsAccessor optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;
        }

        public async Task<string> GetAuthToken()
        {
            //TODO: Investigate Question
            //if (_options == null)
                await Init();

            var token = _options.AccessToken;
            return token;
        }

        public async Task<string> GetApiUrl()
        {
            if (_options == null)
                await Init();

            return _options.ApiBaseUrl;
        }

        private async Task<ZoomOAuthOptions> Init()
        {
            _options = await _optionsAccessor.GetOptions();
            if (string.IsNullOrWhiteSpace(_options.ApiBaseUrl))
                _options.ApiBaseUrl = "https://api.zoom.us/v2";
            return _options;
        }
    }
}