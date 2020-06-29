namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public class RefreshTokenParamsDto
    {
        public string OAuthId { get; set; }
        public string OAuthKey { get; set; }
        public string RefreshToken { get; set; }
    }
}