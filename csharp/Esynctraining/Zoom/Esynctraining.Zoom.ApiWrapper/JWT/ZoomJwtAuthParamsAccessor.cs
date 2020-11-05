using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Esynctraining.Zoom.ApiWrapper.JWT
{
    public class ZoomJwtAuthParamsAccessor : IZoomAuthParamsAccessor
    {
        private ZoomApiJwtOptions _options;
        private readonly IZoomApiJwtOptionsAccessor _optionsAccessor;

        public ZoomJwtAuthParamsAccessor(IZoomApiJwtOptionsAccessor optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;
        }

        public async Task<string> GetAuthToken()
        {
            if(_options == null)
                await Init();

            var token = JwtEncode(_options.ApiKey, _options.ApiSecret);
            return token;
        }

        public async Task<string> GetApiUrl()
        {
            if (_options == null)
                await Init();

            return _options.ApiBaseUrl;
        }

        private async Task<ZoomApiJwtOptions> Init()
        {
            _options = await _optionsAccessor.GetOptions();
            if (string.IsNullOrWhiteSpace(_options.ApiBaseUrl))
                _options.ApiBaseUrl = "https://api.zoom.us/v2";
            return _options;
        }

        private static string JwtEncode(string key, string secret)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var expires = DateTime.UtcNow.AddMinutes(5);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = key,
                Expires = expires,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler { SetDefaultTimesOnTokenCreation = false };
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);
            return signedAndEncodedToken;
        }
    }
}