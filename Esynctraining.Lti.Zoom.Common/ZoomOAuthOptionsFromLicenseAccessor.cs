using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Zoom.ApiWrapper.OAuth;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;

namespace Esynctraining.Lti.Zoom.Common
{
    public class ZoomOAuthOptionsFromLicenseAccessor : IZoomOAuthOptionsAccessor
    {
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomOAuthConfig _zoomOAuthConfig;
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly ILogger _logger;

        private static readonly SemaphoreSlim _sem = new SemaphoreSlim(1, 1);

        public ZoomOAuthOptionsFromLicenseAccessor(ILmsLicenseAccessor licenseAccessor, ZoomOAuthConfig zoomOAuthConfig, ILmsLicenseService lmsLicenseService, ILogger logger)
        {
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _zoomOAuthConfig = zoomOAuthConfig ?? throw new ArgumentNullException(nameof(zoomOAuthConfig));
            _lmsLicenseService = lmsLicenseService ?? throw new ArgumentNullException(nameof(lmsLicenseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ZoomOAuthOptions> GetOptions()
        {
            Dto.LmsLicenseDto license = null;
            await _sem.WaitAsync();
            try
            {
                license = await _licenseAccessor.GetLicense();
                _logger.Info($"Check token for license {license.ConsumerKey}");
                if (!(await IsAccessTokenValid(license.ZoomUserDto.AccessToken)))
                {
                    var response = await UpdateAccessToken(license.ZoomUserDto.RefreshToken);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Info($"Update acees token for license key {license.ConsumerKey}");
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        license.ZoomUserDto = (await _lmsLicenseService.UpdateOAuthTokensForLicense(license.ConsumerKey, tokenResponse.access_token, tokenResponse.refresh_token)).ZoomUserDto;
                    }
                }
            }
            finally
            {
                _logger.Info($"End Check token for license {license.ConsumerKey}");
                _sem.Release();
            }

            return new ZoomOAuthOptions
            {
                AccessToken = license.ZoomUserDto.AccessToken,
            };
        }

        private async Task<bool> IsAccessTokenValid(string accessToken)
        {
            var response = await GetZoomUserInfo(accessToken);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> GetZoomUserInfo(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/v2/users/me");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer  {accessToken}");
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                return response;
            }
        }

        private async Task<HttpResponseMessage> UpdateAccessToken(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://zoom.us/oauth/token?grant_type=refresh_token&refresh_token={refreshToken}&&redirect_uri={_zoomOAuthConfig.RedirectURL}");
            var token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{_zoomOAuthConfig.ClientID}:{_zoomOAuthConfig.ClientSecret}"));
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Basic {token}");

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                return response;
            }
        }

        private class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public string expires_in { get; set; }
            public string scope { get; set; }
            public string reason { get; set; }
            public string error { get; set; }
        }
    }
}