namespace Esynctraining.Lti.Zoom.Common.Dto
{
    public class ZoomUserDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUrl { get; set; }
    }
}
