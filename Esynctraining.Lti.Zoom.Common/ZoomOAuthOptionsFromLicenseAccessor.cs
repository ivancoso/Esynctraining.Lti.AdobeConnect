using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Zoom.ApiWrapper.OAuth;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Common
{
    public class ZoomOAuthOptionsFromLicenseAccessor : IZoomOAuthOptionsAccessor
    {
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomOAuthConfig _zoomOAuthConfig;
        private readonly ILmsLicenseService _lmsLicenseService;

        public ZoomOAuthOptionsFromLicenseAccessor(ILmsLicenseAccessor licenseAccessor, ZoomOAuthConfig zoomOAuthConfig, ILmsLicenseService lmsLicenseService)
        {
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _zoomOAuthConfig = zoomOAuthConfig ?? throw new ArgumentNullException(nameof(zoomOAuthConfig));
            _lmsLicenseService = lmsLicenseService ?? throw new ArgumentNullException(nameof(lmsLicenseService));
         }

        public async Task<ZoomOAuthOptions> GetOptions()
        {
            var license = await _licenseAccessor.GetLicense();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/v2/users/me");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer  {license.ZoomUserDto.AccessToken}");
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var refreshToken = license.ZoomUserDto.RefreshToken;
                    request = new HttpRequestMessage(HttpMethod.Post, $"https://zoom.us/oauth/token?grant_type=refresh_token&refresh_token={refreshToken}&&redirect_uri={_zoomOAuthConfig.RedirectURL}");

                    var token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{_zoomOAuthConfig.ClientID}:{_zoomOAuthConfig.ClientSecret}"));
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("Authorization", $"Basic {token}");
                    response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        license = await _lmsLicenseService.UpdateOAuthTokensForLicense(license.ConsumerKey, tokenResponse.access_token, tokenResponse.refresh_token);
                    }
                }
            }

            return new ZoomOAuthOptions
            {
                AccessToken = license.ZoomUserDto.AccessToken,
            };
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