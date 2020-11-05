using Newtonsoft.Json;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class OAuthTokenInfo
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("error")]
        public string Rrror { get; set; }
    }
}
