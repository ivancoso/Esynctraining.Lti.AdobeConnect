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
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly ILogger _logger;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        private static readonly SemaphoreSlim _sem = new SemaphoreSlim(1, 1);

        public ZoomOAuthOptionsFromLicenseAccessor(ILmsLicenseAccessor licenseAccessor, ILmsLicenseService lmsLicenseService, ILogger logger)
        {
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
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
                _logger.Debug($"Check token for license {license.ConsumerKey}");
                if (!(await IsAccessTokenValid(license.ZoomUserDto.AccessToken)))
                {
                    _logger.Warn($"[Failed] Check token for license {license.ConsumerKey}");
                    var response = await UpdateAccessToken(license.ZoomUserDto);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Info($"Update access token for license key {license.ConsumerKey}");
                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        license.ZoomUserDto = (await _lmsLicenseService.UpdateOAuthTokensForLicense(license.ConsumerKey,
                            tokenResponse.access_token, tokenResponse.refresh_token)).ZoomUserDto;
                    }
                    else
                    {
                        _logger.Error($"Error while updating token for license {license.ConsumerKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, $"GetOptions for license {license.ConsumerKey}");
            }
            finally
            {
                _logger.Debug($"End Check token for license {license.ConsumerKey}");
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
            //---------POPOV----------
            await Task.Delay(180_000);
            //-------------------------

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.zoom.us/v2/users/me");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer  {accessToken}");
            var response = await _httpClient.SendAsync(request);
            return response;
        }

        private async Task<HttpResponseMessage> UpdateAccessToken(Dto.ZoomUserDto zoomUser)
        {
            var url =
                $"https://zoom.us/oauth/token?grant_type=refresh_token&refresh_token={zoomUser.RefreshToken}&&redirect_uri={zoomUser.RedirectUrl}";
            _logger.Info($"[RefreshToken] {url}");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var token = Convert.ToBase64String(
                ASCIIEncoding.ASCII.GetBytes($"{zoomUser.ClientId}:{zoomUser.ClientSecret}"));
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Basic {token}");
            var response = await _httpClient.SendAsync(request);
            return response;
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