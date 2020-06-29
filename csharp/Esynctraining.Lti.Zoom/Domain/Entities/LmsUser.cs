namespace Esynctraining.Lti.Zoom.Domain.Entities
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string global_id { get; set; }
    }

    public class ResponseToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public User user { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }
}
